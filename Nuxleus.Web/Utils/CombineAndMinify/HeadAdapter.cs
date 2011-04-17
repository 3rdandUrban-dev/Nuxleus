/*  
	Developed by Matt Perdeck and published at http://www.codeproject.com/KB/aspnet/CombineAndMinify.aspx 
	As specified at the above URI this code has been licensed under The Code Project Open License (CPOL)
	A copy of this license has been provided in the ~/license folder of this project and can be viewed online at http://www.codeproject.com/info/cpol10.aspx 
*/
using System;
using System.Web;
using System.Web.UI;
using System.Text;
using System.Web.UI.WebControls.Adapters;
using System.Web.UI.WebControls;
using System.Web.UI.Adapters;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using System.Web.Caching;
using System.Web.UI.HtmlControls;
using System.Linq;


namespace Nuxleus.Web.Utils
{
    public class HeadAdapter : ControlAdapter
    {
        protected override void Render(HtmlTextWriter writer)
        {
            ConfigSection cs = ConfigSection.CurrentConfigSection();

            // If we are not active, render the head section to the writer as is.
            if (!ConfigSection.OptionIsActive(cs.Active))
            {
                base.Render(writer);
                return;
            }

            // --------------

            UrlProcessor urlProcessor =
                new UrlProcessor(
                    cs.CookielessDomains, cs.MakeImageUrlsLowercase, cs.InsertVersionIdInImageUrls,
                    ConfigSection.OptionIsActive(cs.EnableCookielessDomains), cs.PreloadAllImages,
                    ConfigSection.OptionIsActive(cs.ExceptionOnMissingFile));

            // --------------

            // headHtml holds the html on the page making up the 
            // head element, including the <head> tag itself.
            StringBuilder headHtmlSb = new StringBuilder();
            base.Render(new HtmlTextWriter(new StringWriter(headHtmlSb)));

            // ------------

            HeadAnalysis headAnalysis = null;
            if (cs.HeadCaching == ConfigSection.HeadCachingOption.None)
            {
                headAnalysis = new HeadAnalysis(
                            headHtmlSb.ToString(), null, cs.CombineCSSFiles, cs.CombineJavaScriptFiles,
                            cs.MinifyCSS, cs.MinifyJavaScript,
                            urlProcessor);
            }
            else
            {
                string headCacheKey = HeadCacheKey(HttpContext.Current.Request.Url, cs.HeadCaching);
                headAnalysis = (HeadAnalysis)HttpContext.Current.Cache[headCacheKey];
                if (headAnalysis == null)
                {
                    // The urls of the combined CSS and JavaScript files in the new head
                    // are dependent on the versions of the actual files (because they contain the version
                    // ids).

                    // totalFileNames will be filled with a list of the names of all
                    // CSS and JavaScript files loaded in the head (that is, those
                    // that get combined and/or minified).
                    List<string> totalFileNames = new List<string>();
                    headAnalysis = new HeadAnalysis(
                            headHtmlSb.ToString(), totalFileNames, cs.CombineCSSFiles, cs.CombineJavaScriptFiles,
                            cs.MinifyCSS, cs.MinifyJavaScript,
                            urlProcessor);

                    AddPageFilePaths(totalFileNames);

                    CacheDependency cd = new CacheDependency(totalFileNames.ToArray());
                    HttpContext.Current.Cache.Insert(headCacheKey, headAnalysis, cd);
                }
            }

            // ------------
            // Do all replacements in the head specified in headAnalysis

            foreach(HeadAnalysis.Replacement r in headAnalysis.Replacements)
            {
                headHtmlSb.Replace(r.original, r.replacement);
            }

            // ------------
            // Process all images in the page if needed. 

            if (cs.RemoveWhitespace || urlProcessor.ImagesNeedProcessing())
            {
                ProcessAllImages(Control.Page.Controls, urlProcessor, cs.RemoveWhitespace);
            }

            // ------------

            string headHtml = headHtmlSb.ToString();

            // At this point, urlProcessor and headAnalysis contains all image urls.
            // Build the JavaScript to preload any images that need to be preloaded,
            // and insert it at the start of the head, just after the initial head tag.

            string preloadJS1 = PreloadJS(cs.PreloadAllImages, cs.PrioritizedImages, headAnalysis.ProcessedImageUrls);
            string preloadJS2 = PreloadJS(cs.PreloadAllImages, cs.PrioritizedImages, urlProcessor.ProcessedImageUrls);
            string preloadJS = preloadJS1 + preloadJS2;

            // If any urls need to be preloaded, insert the JavaScript block after the first > (that is, after the
            // head tag).
            if (!string.IsNullOrEmpty(preloadJS))
            {
                headHtml = InsertedAfterFirstTag(headHtml, preloadJS);
            }

            writer.Write(headHtml);
        }

        private void ProcessAllImages(ControlCollection cc, UrlProcessor urlProcessor, bool removeWhitespace)
        {
            bool imagesNeedProcessing = urlProcessor.ImagesNeedProcessing();

            foreach (Control c in cc)
            {
                if (c is LiteralControl)
                {
                    LiteralControl lit = (LiteralControl)c;
                    string literalContent = lit.Text;
                    string newLiteralContent = literalContent;

                    if (imagesNeedProcessing)
                    {
                        // The "src" group in this regexp doesn't just contain the image url, but also the src= and the quotes.
                        // That allows us to replace the entire src="...", instead of the url. 
                        // If you only replace the old url with the new url, than if you have a tag with url "images/ball3.png" after a tag with "/images/ball3.png"
                        // when the second url ("images/ball3.png") gets replaced, it alsos replace part of the first tag "/images/ball3.png" (because the first tag
                        // contains the second tag). 
                        const string regexpImgGroup =
                            @"<img[^>]*?(?<src>src[^=]*?=[^""']*?(?:""|')(?<url>[^""']*?)(?:""|'))[^>]*?>";

                        Regex r = new Regex(regexpImgGroup, RegexOptions.IgnoreCase);
                        Match m = r.Match(literalContent);

                        while (m.Success)
                        {
                            string oldSrc = m.Groups["src"].Value;

                            string oldUrl = m.Groups["url"].Value;
                            string newUrl = urlProcessor.ProcessedUrl(oldUrl, true, false, Control.Page.Request.Url, null);
                            
                            string newSrc = @"src=""" + newUrl + @"""";
                            
                            newLiteralContent = newLiteralContent.Replace(oldSrc, newSrc);

                            m = m.NextMatch();
                        }
                    }

                    if (removeWhitespace)
                    {
                        newLiteralContent = CollapsedWhitespace(newLiteralContent);
                    }


                    lit.Text = newLiteralContent;
                }
                else if ((c is HtmlImage) && imagesNeedProcessing)
                {
                    HtmlImage hi = (HtmlImage)c;
                    string versionId = LastUpdateTimeImageControl(hi.Src, hi, urlProcessor.ThrowExceptionOnMissingFile);
                    hi.Src = urlProcessor.ProcessedUrl(hi.Src, true, false, Control.Page.Request.Url, versionId);
                }
                else if ((c is HyperLink) && imagesNeedProcessing)
                {
                    HyperLink hl = (HyperLink)c;
                    string versionId = LastUpdateTimeImageControl(hl.ImageUrl, hl, urlProcessor.ThrowExceptionOnMissingFile);
                    hl.ImageUrl = urlProcessor.ProcessedUrl(hl.ImageUrl, true, false, Control.Page.Request.Url, versionId);
                }
                else if ((c is Image) && imagesNeedProcessing)
                {
                    Image i = (Image)c;
                    string versionId = LastUpdateTimeImageControl(i.ImageUrl, i, urlProcessor.ThrowExceptionOnMissingFile);
                    i.ImageUrl = urlProcessor.ProcessedUrl(i.ImageUrl, true, false, Control.Page.Request.Url, versionId);
                }
                else
                {
                    ProcessAllImages(c.Controls, urlProcessor, removeWhitespace);
                }
            }
        }


        /// <summary>
        /// User controls can live in their own folder. 
        /// They can contain Image and Hyperlink controls that have image paths
        /// that are relative to the location of the user control itself.
        /// 
        /// In order to get the last update time of these images, you need to
        /// get their absolute image paths, which take account of both the image path
        /// and the user control path. You can do this by letting the Image or Hyperlink
        /// control resolve the image path.
        /// 
        /// After you've found the resolved path, you can pass that to CombinedFile.LastUpdateTime
        /// to safely get the last update time.
        /// </summary>
        /// <param name="path">
        /// Image path as exposed by the image control.
        /// </param>
        /// <param name="resolver">
        /// The control that will be used to resolve the image path. This would be the 
        /// Image or Hyperlink control itself.
        /// </param>
        /// <returns></returns>
        private string LastUpdateTimeImageControl(string path, Control resolver, bool throwExceptionOnMissingFile)
        {
            string resolvedImageUrl = resolver.ResolveUrl(path);
            return CombinedFile.LastUpdateTime(resolvedImageUrl, throwExceptionOnMissingFile);
        }

        /// <summary>
        /// Inserts a string right after the very first tag in a string with html.
        /// For example, if the html contains a head section, the string is inserted
        /// right after the initial head tag.
        /// </summary>
        /// <param name="html">
        /// Contains the html into which the string will be inserted.
        /// </param>
        /// <param name="toInsert">
        /// String to be inserted.
        /// </param>
        /// <returns>
        /// Resulting html after the insertion.
        /// </returns>
        private string InsertedAfterFirstTag(string html, string toInsert)
        {
            int greaterThanIdx = html.IndexOf('>');

            // If > not found, or it is the very last character in the html,
            // append the string.
            if ((greaterThanIdx == -1) || (html.Length <= (greaterThanIdx + 1)))
            {
                return html + toInsert;
            }

            return html.Insert(greaterThanIdx + 1, toInsert);
        }

        /// <summary>
        /// Generates a string with the JavaScript block that preloads images.
        /// </summary>
        /// <param name="preloadAllImages">
        /// True if the urls in ProcessedImageUrls all need to be preloaded.
        /// </param>
        /// <param name="PrioritizedImages">
        /// Urls that need to be preloaded in any case.
        /// </param>
        /// <param name="ProcessedImageUrls"></param>
        /// <returns>
        /// The JavaScript block. Null or empty if there are no images to preload.
        /// </returns>
        private string PreloadJS(
            bool preloadAllImages, List<string> prioritizedImages, List<string> processedImageUrls)
        {
            // Get the list of urls to be loaded. Make sure prioritised urls come first.
            // Get rid of any duplicates.

            IEnumerable<string> preloadedUrls = null;
            if (preloadAllImages)
            {
                preloadedUrls = prioritizedImages.Union(processedImageUrls);
            }
            else
            {
                preloadedUrls = prioritizedImages.Distinct();
            }

            return PreloadJS(preloadedUrls);
        }

        /// <summary>
        /// Takes a list of image urls, and turns it into JavaScript that loads those images.
        /// </summary>
        /// <param name="preloadedUrls"></param>
        /// <returns></returns>
        private string PreloadJS(IEnumerable<string> preloadedUrls)
        {
            if ((preloadedUrls == null) || (preloadedUrls.Count() == 0))
            {
                return null;
            }

            StringBuilder js = new StringBuilder();
            js.AppendLine(@"<script type=""text/javascript"">");

            // Note that you need to load each image in its own Image object.
            // If you start loading image1 into an Image object, and then
            // start loading image2 into the same object, the loading of 
            // image1 gets cancelled by the browser (at least on Firefox).

            int sequence = 0;
            foreach (string url in preloadedUrls)
            {
                js.AppendFormat("var img{0}=new Image();img{0}.src='{1}';", sequence, url);
                sequence++;
            }

            js.AppendLine("");
            js.Append(@"</script>");

            return js.ToString();
        }

        private string HeadCacheKey(Uri currentUrl, ConfigSection.HeadCachingOption headCachingOption)
        {
            const string keyPrefix = "HeadCacheKey__";

            switch (headCachingOption)
            {
                case ConfigSection.HeadCachingOption.PerSite:
                    // Always return the same key for all pages
                    return keyPrefix;

                case ConfigSection.HeadCachingOption.PerFolder:
                    // Key is based on domain + folder, but not the file
                    //
                    // If no .aspx file is specific, AbsoluteUri will still include
                    // the actual file, such as default.aspx.
                    int idxLastSlash = currentUrl.AbsoluteUri.LastIndexOf('/');
                    if (idxLastSlash == -1)
                    {
                        return keyPrefix + currentUrl.ToString();
                    }

                    return keyPrefix + currentUrl.AbsoluteUri.Substring(0, idxLastSlash);

                case ConfigSection.HeadCachingOption.PerPage:
                    // Key is based on folder + file, but not on query string
                    return keyPrefix + currentUrl.AbsolutePath;

                case ConfigSection.HeadCachingOption.PerUrl:
                    return keyPrefix + currentUrl.ToString();

                default:
                    throw new Exception(
                        "HeadCacheKey - unknown headCachingOption: " + headCachingOption.ToString());
            }
        }

        /// <summary>
        /// Collapses the white space and html comments in a string with HTML.
        /// 
        /// All HTML comments are removed.
        /// Then all runs of white space are replaced with a single space.
        /// If the run of white space contains a newline, it is replaced with a newline instead (so as not to break JavaScript).
        /// </summary>
        /// <param name="html"></param>
        /// <returns>
        /// HTML with white space collapsed and html comments removed.
        /// </returns>
        private string CollapsedWhitespace(string html)
        {
            // Remove html comments.
            // Note that the ? after the * causes the * to do non-greedy matching,
            // so the .*? won't eat the --> and everything else in html.
            // RegexOptions.Singleline causes . to match newlines as well (normally it matches 
            // everything but newlines).
            string result = Regex.Replace(html, "<!--.*?-->", "", RegexOptions.Singleline);

            // Replace all runs of white space that contain a newline with a newline.
            // Such a run of white space starts with zero or more spaces or tabs,
            // which is then followed by one or more newlines, spaces or tabs.

            result = Regex.Replace(result, @"[ \t]*[\r\n][\r\n \t]*", "\r\n");

            // Replace the remaining runs of white space that only contain spaces or tabs
            // (but not newlines!) with spaces.

            result = Regex.Replace(result, @"[ \t]+", " ");

            return result;
        }

        /// <summary>
        /// Adds the paths of the files making up the current page to a string list.
        /// The current page consists of the .aspx file, and maybe the .master file.
        /// </summary>
        /// <param name="filePaths"></param>
        private void AddPageFilePaths(List<string> filePaths)
        {
            Page currentPage = Control.Page;
            Uri currentPageUri = currentPage.Request.Url;
            string currentPageFilePath = currentPage.MapPath(currentPageUri.AbsolutePath);

            string masterPageUrl = currentPage.MasterPageFile;

            filePaths.Add(currentPageFilePath);
            if (masterPageUrl != null) 
            {
                string masterPageFile = currentPage.MapPath(masterPageUrl);
                filePaths.Add(masterPageFile); 
            }
        }
    }
}


