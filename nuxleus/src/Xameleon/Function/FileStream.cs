using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Web;

namespace Xameleon.Function
{

    public class HttpFileStream
    {

      public static string SaveUploadedImage(HttpRequest request, string fieldName, string fileName) 
      {
	 try
            {
	      if ((request.Files[fieldName].InputStream != null) && (request.Files[fieldName].ContentLength > 0))
		{
		  string filePath = String.Format("{0}{1}", fileName, 
						  Path.GetExtension(request.Files[fieldName].FileName));
		  request.Files[fieldName].SaveAs(filePath);
		  return String.Format("{0}:{1}", Path.GetFileName(filePath), 
				       request.Files[fieldName].ContentType);
		}
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: " + e.Message);
            }
	 return string.Empty;
      }

        public static void SaveExternalImageFile(string externalFile, string fileName)
        {
            try
            {
                using (Stream stream = GetFileStream(externalFile))
                {
                    if (stream != null)
                    {
                        Image image = Image.FromStream(stream);
                        image.Save(fileName, image.RawFormat);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: " + e.Message);
            }
        }

        private static Stream GetFileStream(string fileURL)
        {

            try
            {
                WebRequest fileRequest = WebRequest.Create(fileURL);
                fileRequest.Timeout = 5000;
                ((HttpWebRequest)fileRequest).UserAgent = "XameleonWebCrawler/1.0 (compatible; http://xameleon.org/)";

                WebResponse fileResponse = fileRequest.GetResponse();
                return fileResponse.GetResponseStream();

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }
    }
}


