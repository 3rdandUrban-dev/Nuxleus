/*  
	Developed by Matt Perdeck and published at http://www.codeproject.com/KB/aspnet/CombineAndMinify.aspx 
	As specified at the above URI this code has been licensed under The Code Project Open License (CPOL)
	A copy of this license has been provided in the ~/license folder of this project and can be viewed online at http://www.codeproject.com/info/cpol10.aspx 
*/
using System;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Nuxleus.Web.Utils
{
    public class HttpHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            ConfigSection cs = ConfigSection.CurrentConfigSection();

            // --------------

            Uri url = context.Request.Url;
            string path = url.PathAndQuery;
            FileTypeUtilities.FileType fileType = FileTypeUtilities.FileTypeOfUrl(path);

            // --------------

            context.Response.AddHeader(
                "Content-Type",
                FileTypeUtilities.FileTypeToContentType(fileType));

            const int yearInSeconds = 60 * 60 * 24 * 365;

            int maxAge = yearInSeconds;
            if (context.IsDebuggingEnabled || ((!cs.InsertVersionIdInImageUrls) && FileTypeUtilities.FileTypeIsImage(fileType)))
            {
                maxAge = 0;
            }

            // --------------

            if ((fileType == FileTypeUtilities.FileType.JavaScript) ||
                (fileType == FileTypeUtilities.FileType.CSS))
            {
                UrlProcessor urlProcessor =
                    new UrlProcessor(
                        cs.CookielessDomains, cs.MakeImageUrlsLowercase, cs.InsertVersionIdInImageUrls,
                        ConfigSection.OptionIsActive(cs.EnableCookielessDomains), cs.PreloadAllImages,
                        ConfigSection.OptionIsActive(cs.ExceptionOnMissingFile));

                string content =
                    CombinedFile.Content(
                        context, path,
                        cs.MinifyCSS, cs.MinifyJavaScript,
                        urlProcessor, out fileType);

                context.Response.AddHeader("Cache-Control", "public,max-age=" + maxAge.ToString());
                context.Response.Write(content);
            }
            else
            {
                // The file type is not JavaScript or CSS, so it is an image.
                // In case the image has a version id in it, deversion the path
                string deversionedPath = path;

                if (cs.InsertVersionIdInImageUrls)
                {
                    bool deversioned;
                    deversionedPath = UrlVersioner.DeversionedImageUrl(path, out deversioned);
                }

                string fileName =
                    CombinedFile.MapPath(deversionedPath, ConfigSection.OptionIsActive(cs.ExceptionOnMissingFile));

                if (!String.IsNullOrEmpty(fileName))
                {
                    context.Response.AddHeader("Cache-Control", "public,max-age=" + maxAge.ToString());
                    context.Response.WriteFile(fileName);
                }
            }
        }
    }
}
