/*  
	Developed by Matt Perdeck and published at http://www.codeproject.com/KB/aspnet/CombineAndMinify.aspx 
	As specified at the above URI this code has been licensed under The Code Project Open License (CPOL)
	A copy of this license has been provided in the ~/license folder of this project and can be viewed online at http://www.codeproject.com/info/cpol10.aspx 
*/
using System;
using System.Collections.Generic;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;

namespace Nuxleus.Web.Utils
{
    /// <summary>
    /// Contains the results of an analysis of the head of a page.
    /// </summary>
    public class HeadAnalysis
    {
        // Specifies a string replacement in the head 
        public class Replacement
        {
            public string original { get; set; }
            public string replacement { get; set; }
        }

        // All the replacements that need to be made in the head
        public List<Replacement> Replacements { get; set; }

        // The urls of all images used in the CSS files loaded in the head
        public List<string> ProcessedImageUrls { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="headHtml">
        /// the current contents of the head
        /// </param>
        /// <param name="totalFileNames">
        /// totalFileNames will be filled with a list of the names of all
        /// CSS and JavaScript files loaded in the head (that is, those
        /// that get combined and/or minified).
        /// </param>
        /// <param name="combineCSSFiles">
        /// If true, the CSS files in the group are combined into a single file.
        /// </param>
        /// <param name="combineJavaScriptFiles">
        /// If true, the JavaScript files in the group are combined into a single file.
        /// </param>
        /// <param name="urlProcessor">
        /// Use this UrlProcessor to for example insert version ids.
        /// The ProcessedImageUrls property of this UrlProcessor has already been loaded with the 
        /// urls of the images on the page.
        /// </param>
        /// <returns>
        /// New content of the head
        /// </returns>
        public HeadAnalysis(
            string headHtml, List<string> totalFileNames,
            ConfigSection.CombineOption combineCSSFiles, ConfigSection.CombineOption combineJavaScriptFiles,
            bool minifyCSS, bool minifyJavaScript,
            UrlProcessor urlProcessor)
        {
            // Find groups of script or link tags that load a script or css file from
            // the local site. That is, their source does not start with
            // http:// or https://
            //
            // A script tag that has code between the <script> and </script>
            // has inline script, so we're not interested in that either.
            //
            // The files within each group will be served up as a single file
            // (which only exists in cache).
            // By combining js and css files only within the groups, we make sure
            // that the order of the script files is not changed.
            // 
            // An improvement would be to provide an option to 
            // move all css link tags above the script tags, so you could
            // combine them all. Move them above, in case any javascript depends on the css.

            const string regexpScriptGroup =
                @"(?:<script[^>]*?src=(?:""|')(?<url>(?!http://)(?!https://)[^""']*?)(?:""|')[^>]*?>[\s\n\r]*</script>[\s\n\r]*)+";

            const string tagTemplateSript = "<script type=\"text/javascript\" src=\"{0}\"></script>";

            const string regexpCssGroup =
                @"(?:<link[^>]*?href=(?:""|')(?<url>(?!http://)(?!https://)[^""']*?\.css)(?:""|')[^>]*?>[\s\n\r]*)+";

            const string tagTemplateCss = "<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}\" />";

            // ProcessFileType adds records to the Replacements list

            Replacements = new List<Replacement>();

            ProcessFileType(
                headHtml,
                regexpScriptGroup,
                FileTypeUtilities.FileType.JavaScript,
                tagTemplateSript,
                totalFileNames,
                combineJavaScriptFiles,
                true,
                minifyCSS, minifyJavaScript,
                urlProcessor);

            ProcessFileType(
                headHtml,
                regexpCssGroup,
                FileTypeUtilities.FileType.CSS,
                tagTemplateCss,
                totalFileNames,
                combineCSSFiles,
                false,
                minifyCSS, minifyJavaScript,
                urlProcessor);

            // The urlProcessor now contains all image urls contained in CSS files.
            // Copy those urls to this.ProcessedImageUrls.
            ProcessedImageUrls = new List<string>(urlProcessor.ProcessedImageUrls);
            urlProcessor.ProcessedImageUrls.Clear();
        }

        private class groupInfo
        {
            public string fileGroup = null;
            public List<Uri> fileUrlsList = new List<Uri>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="headHtmlSb"></param>
        /// <param name="groupRegexp"></param>
        /// <param name="fileType"></param>
        /// <param name="tagTemplate"></param>
        /// <param name="totalFileNames"></param>
        /// <param name="combineFiles"></param>
        /// <param name="placeCombinedFilesAtEnd">
        /// This is only relevant if combineFiles equals All.
        /// If placeCombinedFilesAtEnd is true, the tag loading the combined file
        /// replaces the very last file group (important if you're loading js, because it means that if any
        /// js is dependent on a library loaded from a CDN, all the js will load after that library.
        /// 
        /// If placeCombinedFilesAtEnd is false, the tag replaces the very first file group.
        /// You'd use this with CSS, to get it load possibly sooner than the js.
        /// </param>
        /// <param name="urlProcessor"></param>
        private void ProcessFileType(
            string headHtml,
            string groupRegexp,
            FileTypeUtilities.FileType fileType,
            string tagTemplate,
            List<string> totalFileNames,
            ConfigSection.CombineOption combineFiles,
            bool placeCombinedFilesAtEnd,
            bool minifyCSS, bool minifyJavaScript,
            UrlProcessor urlProcessor)
        {
            List<groupInfo> allGroups = new List<groupInfo>();
            List<Uri> totalFileUrlsList = new List<Uri>();

            Regex r = new Regex(groupRegexp, RegexOptions.IgnoreCase);
            Match m = r.Match(headHtml);

            // Visit each group of script or link tags. Record the html of each file group
            // and a list of the urls in the tags in that file group in allGroups.
            while (m.Success)
            {
                string fileGroup = m.Value;
                CaptureCollection fileUrls = m.Groups["url"].Captures;

                // Visit the src or href of each individual script or link tag in the group,
                // and add to a list of urls.

                List<Uri> fileUrlsList = new List<Uri>();
                for (int j = 0; j < fileUrls.Count; j++)
                {
                    Uri fileUrl = new Uri(HttpContext.Current.Request.Url, fileUrls[j].Value);
                    fileUrlsList.Add(fileUrl);
                    totalFileUrlsList.Add(fileUrl);
                }

                allGroups.Add(new groupInfo() { fileGroup = fileGroup, fileUrlsList = fileUrlsList });
                m = m.NextMatch();
            }

            // Process each file group in allGroups
            switch (combineFiles)
            {
                case ConfigSection.CombineOption.None:
                    // In each group, process all URLs individually into tags.
                    // Note that CombinedFile.Url not only has the ability to combine urls, but also
                    // to insert version info - and we still want that to be able to use far future cache expiry,
                    // even if not combining files.
                    // Concatenate the tags and replace the group with the concatenated tags.
                    foreach (groupInfo g in allGroups)
                    {
                        StringBuilder tagsInGroup = new StringBuilder();

                        foreach (Uri u in g.fileUrlsList)
                        {
                            string versionedUrl = CombinedFile.Url(
                                HttpContext.Current, new List<Uri>(new Uri[] { u }), fileType,
                                minifyCSS, minifyJavaScript,
                                urlProcessor, totalFileNames);
                            string versionedFileTag =
                                string.Format(tagTemplate, versionedUrl);
                            tagsInGroup.Append(versionedFileTag);
                        }

                        // Be sure to trim the group before storing it (that is, remove space at the front and end).
                        // If you don't, you may store a group with white space at either end, that then doesn't match
                        // a group in some other file that is exactly the same, except for the white space at either end.
                        Replacements.Add(new Replacement { original = g.fileGroup.Trim(), replacement = tagsInGroup.ToString() });
                    }

                    break;

                case ConfigSection.CombineOption.PerGroup:
                    // In each group, process all URLs together into a combined tag.
                    // Replace the group with that one tag.
                    foreach (groupInfo g in allGroups)
                    {
                        string combinedFileUrl = CombinedFile.Url(
                            HttpContext.Current, g.fileUrlsList, fileType, minifyCSS, minifyJavaScript, urlProcessor, totalFileNames);
                        string combinedFileTag =
                            string.Format(tagTemplate, combinedFileUrl);

                        Replacements.Add(new Replacement { original = g.fileGroup.Trim(), replacement = combinedFileTag });
                    }

                    break;

                case ConfigSection.CombineOption.All:
                    // Combine all urls into a single tag. Then insert that tag in the head.
                    // Also, remove all groups.
                    {
                        string combinedFileUrl = CombinedFile.Url(
                            HttpContext.Current, totalFileUrlsList, fileType, minifyCSS, minifyJavaScript, urlProcessor, totalFileNames);
                        string combinedFileTag =
                            string.Format(tagTemplate, combinedFileUrl);

                        int idxFileGroupToReplace = placeCombinedFilesAtEnd ? (allGroups.Count - 1) : 0;

                        Replacements.Add(
                            new Replacement { original = allGroups[idxFileGroupToReplace].fileGroup.Trim(), replacement = combinedFileTag });

                        // Replace all file groups with empty string, except for the one
                        // we just replaced with the tag.
                        allGroups.RemoveAt(idxFileGroupToReplace);
                        foreach (groupInfo g in allGroups)
                        {
                            Replacements.Add(
                                new Replacement { original = g.fileGroup.Trim(), replacement = "" });
                        }
                    }
                    break;

                default:
                    throw new ArgumentException("ProcessFileType - combineFiles=" + combineFiles.ToString());
            }
        }
    }
}
