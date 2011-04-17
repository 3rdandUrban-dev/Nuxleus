/*  
	Developed by Matt Perdeck and published at http://www.codeproject.com/KB/aspnet/CombineAndMinify.aspx 
	As specified at the above URI this code has been licensed under The Code Project Open License (CPOL)
	A copy of this license has been provided in the ~/license folder of this project and can be viewed online at http://www.codeproject.com/info/cpol10.aspx 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Nuxleus.Web.Utils
{
    /// <summary>
    /// Deals with inserting a version id into a url, and removing a version id from a url.
    /// </summary>
    public class UrlVersioner
    {
        private const string _versionIdSeparator = "__";

        /// <summary>
        /// Inserts a version id into a url and returns the result.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string InsertVersionId(string url, string versionId)
        {
            string result = url;

            if (!string.IsNullOrEmpty(versionId))
            {
                int idxExtension = result.LastIndexOf(".");
                if (idxExtension == -1)
                {
                    result = url + _versionIdSeparator + versionId;
                }
                else
                {
                    result = url.Insert(idxExtension, _versionIdSeparator + versionId);
                }
            }

            return result;
        }

        /// <summary>
        /// Takes a url, and returns the name of the file without the version id and without the extension.
        /// 
        /// This method assumes that the url has at least a version id, or an extension, or both.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string UnversionedFilename(string url)
        {
            string urlWithoutVersionIdOrExtension = url;

            // Find the index of the version id separator. If there is no separator, find the extension instead.
            int idxSeparator = url.IndexOf(_versionIdSeparator);
            if (idxSeparator == -1)
            {
                idxSeparator = url.LastIndexOf(".");
            }

            if (idxSeparator != -1)
            {
                urlWithoutVersionIdOrExtension = url.Substring(0, idxSeparator);
            }

            // Get rid of the path and domain
            string result = urlWithoutVersionIdOrExtension;
            int idxLastSlash = urlWithoutVersionIdOrExtension.LastIndexOf("/");

            if (idxLastSlash != -1)
            {
                int idxFilename = idxLastSlash + 1;
                if (urlWithoutVersionIdOrExtension.Length > idxFilename)
                {
                    result = urlWithoutVersionIdOrExtension.Substring(idxFilename);
                }
            }

            result = result.Trim(new char[] { '-' });

            return result;
        }

        /// <summary>
        /// Takes an image url (ends in .png, .gif or .jpg) and removes the version id from it.
        /// </summary>
        /// <param name="imageUrl">
        /// The image url.
        /// </param>
        /// <param name="deversioned">
        /// Returns true if the url was an image url, and it contained a version.
        /// False otherwise.
        /// </param>
        /// <returns>
        /// The image url with the version id removed. If the url was not an image url, or if it didn't have 
        /// a version id, the original url is returned.
        /// </returns>
        public static string DeversionedImageUrl(string imageUrl, out bool deversioned)
        {
            deversioned = false;

            // If the url ends in __<versionid>.png, __<versionid>.jpg or __<versionid>.gif,
            // then rewrite the path to get rid of the __<versionid>

            // Should you ever decide to cater for image files with query strings,
            // you need to un-escape the query string before passing it on to RewritePath.
            // Do that with Uri.UnescapeDataString

            const string regexPattern = @"(.*)" + _versionIdSeparator + @".*\.(png|jpg|gif)";

            Match m = Regex.Match(imageUrl, regexPattern, RegexOptions.IgnoreCase);
            if (!m.Success) { return imageUrl; }

            string replaceUrl = m.Groups[1] + "." + m.Groups[2];
            deversioned = true;

            return replaceUrl;
        }

    }
}
