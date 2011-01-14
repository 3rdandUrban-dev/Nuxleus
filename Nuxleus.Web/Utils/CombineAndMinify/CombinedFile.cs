/*  
	Developed by Matt Perdeck and published at http://www.codeproject.com/KB/aspnet/CombineAndMinify.aspx 
	As specified at the above URI this code has been licensed under The Code Project Open License (CPOL)
	A copy of this license has been provided in the ~/license folder of this project and can be viewed online at http://www.codeproject.com/info/cpol10.aspx 
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Caching;
using System.IO;
using Yahoo.Yui.Compressor;
using System.Security.Cryptography;
using System.Web;
using System.Text.RegularExpressions;
using System.Web.UI;

namespace Nuxleus.Web.Utils
{
    public class CombinedFile
    {
        /// <summary>
        /// Takes the url of a combined file, and returns its content,
        /// ready to be sent to the browser.
        /// The url does not relate to an actual file. The combined content
        /// only lives in cache. If it is not in cache, this method
        /// finds out which files are associated with the fileUrl,
        /// reads them, compresses the content and stores that in cache
        /// (as well as returning it).
        /// </summary>
        /// <param name="context"></param>
        /// <param name="combinedFileUrl"></param>
        /// <returns></returns>
        public static string Content(
            HttpContext context, string combinedFileUrl,
            bool minifyCSS, bool minifyJavaScript,
            UrlProcessor urlProcessor,
            out FileTypeUtilities.FileType fileType)
        {
            // Get the urlsHash from the combined file url.
            string urlsHash = UrlVersioner.UnversionedFilename(combinedFileUrl);

            // Based on that hash, get the compressed content of the combined file.
            string combinedFileContent = null;
            string newVersionId = null;
            GetContentVersion(
                context, urlsHash, urlProcessor, null,
                minifyCSS, minifyJavaScript,
                out combinedFileContent, out newVersionId, out fileType);

            if (combinedFileContent == null)
            {
                // combinedFileUrl matches an actual file on the server. Load that file
                // and return its content to the browser. Because this situation normally
                // only happens when a (already minified) library file could not be loaded
                // from a CDN (a rare event), or if we are in debug mode, there is no need 
                // to minify the file.

                combinedFileContent = "";
                string filePath = MapPath(combinedFileUrl, urlProcessor.ThrowExceptionOnMissingFile);
                if (filePath != null)
                {
                    combinedFileContent = File.ReadAllText(filePath);
                }

                fileType = FileTypeUtilities.FileTypeOfUrl(combinedFileUrl);
            }

            return combinedFileContent;
        }

        /// <summary>
        /// Takes the urls of a series of files (taken from the src or href
        /// attribute of their script or link tags), and returns the url
        /// of the combined file. That url will be placed in 
        /// single script or link tag that replaces the individual script or link tags.
        ///
        /// When the browser sends a request for this url, get the content
        /// to return by calling CombinedFileContent.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="fileUrls"></param>
        /// <param name="totalFileNames">
        /// The method adds the physical file names of the files making up the combined
        /// file to this parameter. If this is null, nothing is done.
        /// </param>
        /// <returns></returns>
        public static string Url(
            HttpContext context, 
            List<Uri> fileUrls, 
            FileTypeUtilities.FileType fileType,
            bool minifyCSS, bool minifyJavaScript,
            UrlProcessor urlProcessor, 
            List<string> totalFileNames)
        {
            string urlsHash = UrlsHash(fileUrls);

            // Store the urls of the files, so GetContentVersion can retrieve
            // the urls if needed.
            StoreFileUrls(context, urlsHash, fileUrls, fileType);

            string combinedFileContent = null;
            string versionId = null;
            GetContentVersion(
                context, urlsHash, urlProcessor, totalFileNames, 
                minifyCSS, minifyJavaScript,
                out combinedFileContent, out versionId, out fileType);

            string combinedFileUrl = CombinedFileUrl(urlsHash, versionId, fileType, urlProcessor);
            return combinedFileUrl;
        }

        public static bool UrlStartsWithProtocol(string url)
        {
            string first6 = SafeSubstring(url, 0, 6).TrimStart();
            return
                ((string.Compare(first6, "http:/", StringComparison.OrdinalIgnoreCase) == 0) ||
                 (string.Compare(first6, "https:", StringComparison.OrdinalIgnoreCase) == 0));
        }

        private class FileUrlsElement
        {
            public List<Uri> fileUrls { get; set; }
            public FileTypeUtilities.FileType fileType { get; set; }
        }

        /// <summary>
        /// Stores the file urls list in Application state under the given hash.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="urlsHash"></param>
        /// <param name="fileUrls"></param>
        private static void StoreFileUrls(
            HttpContext context, 
            string urlsHash,
            List<Uri> fileUrls, 
            FileTypeUtilities.FileType fileType)
        {
            FileUrlsElement fileUrlsElement = new FileUrlsElement();
            fileUrlsElement.fileType = fileType;
            fileUrlsElement.fileUrls = fileUrls;

            context.Application[urlsHash] = fileUrlsElement;
        }

        /// <summary>
        /// Retrieves the file urls list in Application state under the given hash.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="urlsHash"></param>
        /// <returns>
        /// List of file urls. Null if the hash was not found.
        /// </returns>
        private static void RetrieveFileUrls(
            HttpContext context, string urlsHash, 
            out List<Uri> fileUrls, out FileTypeUtilities.FileType fileType)
        {
            fileUrls = null;
            fileType = FileTypeUtilities.FileType.JavaScript;

            FileUrlsElement fileUrlsElement = (FileUrlsElement)context.Application[urlsHash];
            if (fileUrlsElement == null)
            {
                return;
            }

            fileUrls = fileUrlsElement.fileUrls;
            fileType = fileUrlsElement.fileType;
        }

        /// <summary>
        /// Takes a list of urls, and returns a hash that should be unique
        /// for each combination of urls. The resulting hash will be valid for
        /// use in a url.
        /// </summary>
        /// <param name="fileUrls"></param>
        /// <returns></returns>
        private static string UrlsHash(List<Uri> fileUrls)
        {
            // Put all urls together in a string
            StringBuilder sb = new StringBuilder();
            foreach(Uri u in fileUrls)
            {
                sb.Append(u.AbsolutePath);
            }

            string concatenatedUrls = sb.ToString();

            // Create hash
            MD5 md5 = MD5.Create();
            byte[] urlsHashBytes = md5.ComputeHash(Encoding.ASCII.GetBytes(concatenatedUrls));

            string urlsHash = BitConverter.ToString(urlsHashBytes).Replace("-","");
            return urlsHash;
        }

        /// <summary>
        /// Returns a combined file url.
        /// </summary>
        /// <param name="urlsHash">
        /// Hash based on the urls of the files that make up the combined file.
        /// </param>
        /// <param name="versionId">
        /// A string that is different for each version of the files that make up 
        /// the combined file. This is used to make sure that a browser doesn't
        /// pick up an outdated version from its internal browser cache.
        /// </param>
        /// <param name="fileType">
        /// </param>
        /// <param name="urlDomain">
        /// Domain to be used for the url.
        /// Make null or empty if you don't want a domain used in the url.
        /// </param>
        /// <returns></returns>
        private static string CombinedFileUrl(
            string urlsHash, string versionId, FileTypeUtilities.FileType fileType, UrlProcessor urlProcessor)
        {
            string url = "/" + urlsHash + FileTypeUtilities.FileTypeToExtension(fileType);

            return urlProcessor.ProcessedUrl(url, false, false, null, versionId);
        }

        private class CacheElement
        {
            public string CombinedFileContent { get; set; }
            public string VersionId { get; set; }
        }

        /// <summary>
        /// Takes the hash identifying the urls of the files that make up a combined file.
        /// Returns the compressed content of the combined files, and the version ID
        /// of the combined files. The version ID is based on the last modified time of the last
        /// modified file file that goes into the combined file.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="urlsHash"></param>
        /// <param name="totalFileNames">
        /// The file names of the files read by this method get added to this list.
        /// If this is null, nothing is done with this parameter.
        /// </param>
        /// <param name="combinedFileContent">
        /// Content to be sent back to the browser. 
        /// Will be null if the content could not be retrieved, because the hash was not found in
        /// the Application object. This means that the file tag that caused the browser to 
        /// request this file was generated in JavaScript or appeared outside the head tag
        /// on the page. This will also happen in debug mode. 
        /// In this case, the name of the requested file matches an actual
        /// file on the server.
        /// </param>
        /// <param name="versionId"></param>
        private static void GetContentVersion(
            HttpContext context, 
            string urlsHash,
            UrlProcessor urlProcessor,
            List<string> totalFileNames,
            bool minifyCSS, bool minifyJavaScript,
            out string combinedFileContent,
            out string versionId, 
            out FileTypeUtilities.FileType fileType)
        {
            combinedFileContent = null;
            versionId = null;

            List<Uri> fileUrls;
            RetrieveFileUrls(context, urlsHash, out fileUrls, out fileType);
            if (fileUrls == null)
            {
                return;
            }

            CacheElement cacheElement = (CacheElement)context.Cache[urlsHash];
            if (cacheElement == null)
            {
                StringBuilder combinedContentSb = new StringBuilder();
                DateTime mostRecentModifiedTime = DateTime.MinValue;
                List<string> fileNames = new List<string>();
                bool fileMissing = false;

                foreach (Uri fileUrl in fileUrls)
                {
                    string filePath = MapPath(fileUrl.AbsolutePath, urlProcessor.ThrowExceptionOnMissingFile);
                    string fileContent = null;

                    if (filePath != null)
                    {
                        fileContent = File.ReadAllText(filePath);
                        if (fileType == FileTypeUtilities.FileType.CSS)
                        {
                            FixUrlProperties(ref fileContent, fileUrl, urlProcessor);
                        }

                        DateTime lastModifiedTime = File.GetLastWriteTime(filePath);

                        mostRecentModifiedTime =
                            (mostRecentModifiedTime > lastModifiedTime) ?
                                mostRecentModifiedTime : lastModifiedTime;

                        fileNames.Add(filePath);
                        if (totalFileNames != null) { totalFileNames.Add(filePath); }
                    }
                    else
                    {
                        // A comment starting with /*! doesn't get removed by the minifier
                        fileContent = string.Format("\n/*!\n** Does not exist: {0}\n*/\n", fileUrl);

                        fileMissing = true;
                    }

                    combinedContentSb.Append(fileContent);
                }

                string combinedContent = combinedContentSb.ToString();
                if (!string.IsNullOrEmpty(combinedContent))
                {
                    cacheElement = new CacheElement();

                    cacheElement.CombinedFileContent = combinedContent;
                    if (fileType == FileTypeUtilities.FileType.JavaScript)
                    {
                        if (minifyJavaScript)
                        {
                            cacheElement.CombinedFileContent = JavaScriptCompressor.Compress(combinedContent);
                        }
                    }
                    else
                    {
                        if (minifyCSS)
                        {
                            cacheElement.CombinedFileContent = CssCompressor.Compress(combinedContent);
                        }
                    }

                    cacheElement.VersionId = VersionId(mostRecentModifiedTime);

                    // Cache the newly created cacheElement
                    // 
                    // Do not cache the cacheElement if one of the files couldn't be found.
                    // That way, the package will keep checking the missing file, and pick it up
                    // when someone puts the file there.
                    if (!fileMissing)
                    {
                        CacheDependency cd = new CacheDependency(fileNames.ToArray());
                        context.Cache.Insert(urlsHash, cacheElement, cd);
                    }
                }
            }

            if (cacheElement == null)
            {
                if (context.IsDebuggingEnabled) { throw new Exception("cacheElement == null"); }

                combinedFileContent = "";
                versionId = "";
            }
            else
            {
                combinedFileContent = cacheElement.CombinedFileContent;
                versionId = cacheElement.VersionId;
            }
        }

        /// <summary>
        /// Takes the content of a CSS file and the original absolute url of that
        /// file, and changes all url() properties to absolute urls.
        /// This way, if the CSS file has been combined with other files, the 
        /// images specified in the url() properties will still show.
        /// </summary>
        /// <param name="fileContent"></param>
        /// <param name="fileUrl"></param>
        private static void FixUrlProperties(
            ref string fileContent, Uri fileUrl, UrlProcessor urlProcessor)
        {
            StringBuilder fileContentSb = new StringBuilder(fileContent);

            const string regexpUrlProperty =
                @"url\([\s\n\r]*(?<url>(?!http://)(?!https://)[^)]*?)[\s\n\r]*\)";

            Regex r = new Regex(regexpUrlProperty, RegexOptions.IgnoreCase);
            Match m = r.Match(fileContent);

            // Visit each url property
            while (m.Success)
            {
                string urlProperty = m.Value;
                CaptureCollection urlProperties = m.Groups["url"].Captures;

                if (urlProperties.Count > 0)
                {
                    string relativeUrl = urlProperties[0].Value;
                    string absoluteUrl = urlProcessor.ProcessedUrl(relativeUrl, true, true, fileUrl, null);

                    fileContentSb.Replace(urlProperty, "url(" + absoluteUrl + ")");
                }

                m = m.NextMatch();
            }

            fileContent = fileContentSb.ToString();
        }
    
        /// <summary>
        /// Returns the last update time of an image.
        /// </summary>
        /// <param name="path">
        /// Relative url of the image.
        /// </param>
        /// <returns>
        /// Number of seconds since the start of the epoch
        /// modulo 40,000,000 that the file was last updated.
        /// 40,000,000 seconds is more than a year. By using modulo,
        /// the size of the long is reduced, while it is still 
        /// extremely unlikely that 2 different update times for the
        /// same file result in the same return value.
        /// 
        /// The long is returned as a string, with hexadecimal characters.
        /// It isn't returned as a base64 string, because base64 is case sensitive,
        /// and a browser cache may do case insensitive compares to urls.
        /// 
        /// If the file doesn't exist, or if the image is inlined, "" (empty string) is returned.
        /// </returns>
        public static string LastUpdateTime(string path, bool throwExceptionOnMissingFile)
        {
            if (IsInlined(path)) { return ""; }

            // File.GetLastWriteTime returns 1/01/1601 11:00:00 AM
            // if the file doesn't exist. That corresponds with these ticks:
            const long ticksFileNotExists = 504911628000000000;

            // Cache key prefix. Used when caching the last modified time to 
            // distinguish last modified time cache entry for a file fron any
            // other cache entries for the file.
            const string cacheKeyPrefix = "lmt_";

            // ------------
            // Try to get last modified time from cache

            string cacheKey = cacheKeyPrefix + path;
            string lastUpdateTimeHex = (string)HttpContext.Current.Cache[cacheKey];

            if (lastUpdateTimeHex == null)
            {
                lastUpdateTimeHex = "";
                string filePath = MapPath(path, throwExceptionOnMissingFile);
                if (filePath != null)
                {
                    // Get last update time in ticks. 
                    DateTime lastUpdateTime = File.GetLastWriteTime(filePath);
                    long lastUpdateTimeTicks = lastUpdateTime.Ticks;

                    if (lastUpdateTimeTicks != ticksFileNotExists)
                    {
                        lastUpdateTimeHex = VersionId(lastUpdateTime);
                    }
                }

                // Cache the newly found last update time
                CacheDependency cd = new CacheDependency(filePath);
                HttpContext.Current.Cache.Insert(cacheKey, lastUpdateTimeHex, cd);
            }

            return lastUpdateTimeHex;
        }

        private static string VersionId(DateTime lastModifiedTime)
        {
            long lastModifiedTimeTicks = lastModifiedTime.Ticks;

            // Shorted lastUpdateTimeSeconds. Make its units seconds rather than ticks
            // 1 second = 10,000,000 seconds. And mod by 40000000 - 40000000 seconds
            // is just over a year, so the same file has to survive over a year on the site
            // before it could possible get a duplicate last modified time .
            long lastModifiedTimeSeconds = (lastModifiedTimeTicks / 10000000) % 40000000;
            string lastModifiedTimeHex = lastModifiedTimeSeconds.ToString("x");

            return lastModifiedTimeHex;
        }

        /// <summary>
        /// Maps a url on a file path on the file system.
        /// Handles absolute urls while running under Cassini.
        /// 
        /// If the file has a domain, disregards the domain.
        /// That way, if you use a cookieless domain outside this library,
        /// you still get the file path.
        /// If the file is actually on another domain, it won't find it on
        /// your server.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// In debug mode, throws an exception if the path could not be generated, or if it doesn't exist.
        /// </exception>
        /// <param name="url"></param>
        /// <returns>
        /// The file path. Null if the file path somehow couldn't be generated, or if the file doesn't exist.
        /// </returns>
        public static string MapPath(string url, bool throwExceptionOnMissingFile)
        {
            HttpContext currentContext = HttpContext.Current;
            HttpServerUtility hsu = currentContext.Server;
            string filePath = null;

            try
            {
                filePath = hsu.MapPath(url);
            }
            catch (InvalidOperationException)
            {
                if (url.StartsWith("/"))
                {
                    try
                    {
                        string urlTilde = "~" + url;
                        string resolvedUrl = ResolveUrl(urlTilde);
                        filePath = hsu.MapPath(resolvedUrl);
                    }
                    catch (InvalidOperationException)
                    {
                    }
                }
            }
            catch (HttpException)
            {
                // You get this exception when the url contains a domain or when it starts with "..".
                string absoluteUrl = new Uri(url).AbsolutePath;
                string urlTilde = "~" + absoluteUrl;
                string resolvedUrl = ResolveUrl(urlTilde);
                filePath = hsu.MapPath(resolvedUrl);
            }

            // Make sure that the file exists
            if ((filePath != null) && (!File.Exists(filePath))) { filePath = null; }

            if (filePath == null)
            {
                if (throwExceptionOnMissingFile) 
                { 
                    throw new ArgumentException("Cannot be found on the file system: " + url); 
                }
            }

            return filePath;
        }

        /// <summary>
        /// Takes a virtual path (starts with ~) and converts it into an
        /// absolute path (starts with / ).
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ResolveUrl(string url)
        {
            string resolvedUrl = VirtualPathUtility.ToAbsolute(url);
            return resolvedUrl;
        }

        /// <summary>
        /// Safe version of Substring. 
        /// If startIndex is outside the string, returns empty string.
        /// If length is too long, returns whatever characters are available from startIndex.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string SafeSubstring(string s, int startIndex, int length)
        {
            try
            {
                return s.Substring(startIndex, length);
            }
            catch(Exception e)
            {
                if ((startIndex < 0) || (length < 0))
                {
                    throw new Exception("SafeSubstring - startIndex: " + startIndex.ToString() + ", length: " + length.ToString(), e);
                }
                
                // If exception happened because startIndex outside the string, return empty string.
                if (startIndex >= s.Length) { return ""; }

                // If the startIndex was inside the string, exception happened because length was too long.
                // Just return whatever characters are available.
                return s.Substring(startIndex);
            }
        }

        /// <summary>
        /// Determines whether an image is inlined (so its contents is not in an external file, but sits in
        /// the CSS or HTML itself).
        /// </summary>
        /// <param name="path">
        /// Path of the image. This would be the src of an img tag, or a url(..) in CSS.
        /// </param>
        /// <returns>
        /// true: image is inlined
        /// false: image is not inlined
        /// </returns>
        public static bool IsInlined(string path)
        {
            return path.StartsWith("data:image");
        }
    }
}
