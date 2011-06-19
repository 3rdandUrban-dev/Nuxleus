/*  
	Developed by Matt Perdeck and published at http://www.codeproject.com/KB/aspnet/CombineAndMinify.aspx 
	As specified at the above URI this code has been licensed under The Code Project Open License (CPOL)
	A copy of this license has been provided in the ~/license folder of this project and can be viewed online at http://www.codeproject.com/info/cpol10.aspx 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nuxleus.Web.Utils
{
    public class FileTypeUtilities
    {
        public enum FileType
        {
            CSS,
            JavaScript,
            Jpeg,
            Png,
            Gif
        }

        public static bool FileTypeIsImage(FileType fileType)
        {
            return (fileType == FileType.Gif) || (fileType == FileType.Jpeg) || (fileType == FileType.Png);
        }

        /// <summary>
        /// Returns an extension based on the given file type.
        /// </summary>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public static string FileTypeToExtension(FileType fileType)
        {
            string extension = null;

            switch (fileType)
            {
                case FileType.CSS:
                    extension = ".css";
                    break;

                case FileType.JavaScript:
                    extension = ".js";
                    break;

                case FileType.Gif:
                    extension = ".gif";
                    break;

                case FileType.Jpeg:
                    extension = ".jpg";
                    break;

                case FileType.Png:
                    extension = ".png";
                    break;

                default:
                    throw new Exception("FileTypeToExtension - unknown file type: " + fileType.ToString());
            }

            return extension;
        }

        /// <summary>
        /// Returns the file type of a url.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static FileType FileTypeOfUrl(string url)
        {
            int idxExtension = url.LastIndexOf('.');

            if (idxExtension == -1)
            {
                throw new Exception("FileTypeOfUrl - cannot find extension in: " + url);
            }

            string threeLetterExtension = CombinedFile.SafeSubstring(url, idxExtension, 4).ToLower();
            string twoLetterExtension = CombinedFile.SafeSubstring(url, idxExtension, 3).ToLower();

            if (threeLetterExtension == ".css")
                return FileType.CSS;
            else if (twoLetterExtension == ".js")
                return FileType.JavaScript;
            else if (threeLetterExtension == ".gif")
                return FileType.Gif;
            else if (threeLetterExtension == ".jpg")
                return FileType.Jpeg;
            else if (threeLetterExtension == ".png")
                return FileType.Png;

            throw new Exception("FileTypeOfUrl - unknown extension: " + (idxExtension));
        }

        /// <summary>
        /// Converts a file type to a mime.
        /// </summary>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public static string FileTypeToContentType(FileType fileType)
        {
            string mime = null;

            switch (fileType)
            {
                case FileType.CSS:
                    mime = "text/css";
                    break;

                case FileType.JavaScript:
                    mime = "text/javascript";
                    break;

                case FileType.Gif:
                    mime = "image/gif";
                    break;

                case FileType.Jpeg:
                    mime = "image/jpeg";
                    break;

                case FileType.Png:
                    mime = "image/png";
                    break;

                default:
                    throw new Exception("FileTypeToContentType - unknown file type: " + fileType.ToString());
            }

            return mime;
        }
    }
}
