using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;

namespace Xameleon.Function {

    public class HttpFileStream {

        public static void SaveExternalImageFile(string externalFile, string fileName) {
            try {
                using (Stream stream = GetFileStream(externalFile)) {
                    if (stream != null) {
                        Image image = Image.FromStream(stream);
                        image.Save(fileName, image.RawFormat);
                    }
                }
            } catch (Exception e) {
                Debug.WriteLine("Error: " + e.Message);
            }
        }

        private static Stream GetFileStream(string fileURL) {

            try {
                WebRequest fileRequest = WebRequest.Create(fileURL);
                fileRequest.Timeout = 5000;
                ((HttpWebRequest)fileRequest).UserAgent = "XameleonWebCrawler/1.0 (compatible; http://xameleon.org/)";

                WebResponse fileResponse = fileRequest.GetResponse();
                return fileResponse.GetResponseStream();

            } catch (Exception e) {
                Debug.WriteLine(e.Message);
                return null;
            }
        }
    }
}


