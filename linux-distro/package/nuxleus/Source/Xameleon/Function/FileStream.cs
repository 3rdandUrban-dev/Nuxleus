using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Web;
using Nuxleus.Cryptography;
using System.Collections;
using System.Text;

namespace Xameleon.Function
{
    public class HttpFileStream
    {

        public static string SaveUploadedFileCollection (HttpRequest request, string fieldName, string fileName)
        {
            
            IEnumerator enumerator = request.Files.GetEnumerator();
            StringBuilder filePathStringBuilder = new StringBuilder();
            string path = request.MapPath(fileName);

            try
            {
                if (!Directory.Exists(path))
                {
                    DirectoryInfo directory = Directory.CreateDirectory(path);
                }

                for (int i = 0; enumerator.MoveNext(); i++)
                {
                    string hash = new HashcodeGenerator(request.Files[i].InputStream).GetHashCode().ToString();
                    string filePath = String.Format("{0}/{1}{2}", path, hash, Path.GetExtension(request.Files[i].FileName));
                    request.Files[i].SaveAs(filePath);
                    filePathStringBuilder.Append(String.Format("{0}:{1},", Path.GetFileName(filePath), request.Files[fieldName].ContentType));
                }

                return filePathStringBuilder.ToString();
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }

        public static void SaveExternalImageFile (string externalFile, string fileName)
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

        private static Stream GetFileStream (string fileURL)
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


