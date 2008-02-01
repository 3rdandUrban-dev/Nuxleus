using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuxleus.Web;
using System.IO;
using System.Threading;
using System.Diagnostics;

using EeekSoft.Asynchronous;
using System.Net;
using System.Text.RegularExpressions;

namespace HttpGetAsyncResponse_Test {

    class Program {

        static Dictionary<int, Stream> m_responseStreamDictionary = new Dictionary<int, Stream>();

        static void Main ( string[] args ) {
            // Download the URLs and wait until all of them complete
            DownloadAll().ExecuteAndWait();
        }

        static IEnumerable<IAsync> AsyncMethod ( string url ) {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Timeout = 10000 /*TODO: This should be set dynamically*/;
            request.KeepAlive = true;
            request.Pipelined = true;

            Console.WriteLine("[{0}] starting on thread: {1}", url, Thread.CurrentThread.ManagedThreadId);

            // asynchronously get the response from http server
            Async<WebResponse> response = request.GetResponseAsync();
            yield return response;

            Console.WriteLine("[{0}] got response on thread: {1}", url, Thread.CurrentThread.ManagedThreadId);

            Stream stream = response.Result.GetResponseStream();

            // download response stream using the asynchronous extension method
            // instead of using synchronous StreamReader
            Async<string> responseString = stream.ReadToEndAsync().ExecuteAsync<string>();
            yield return responseString;

            Console.WriteLine("Current thread id: {0}", Thread.CurrentThread.ManagedThreadId);

            //TODO: Similar to the code HttpGetAsyncResponse code base, add in a dictionary for
            //storing the response stream (or a variation of that response stream, e.g. a string,
            //XmlReader, XDocument, XElement, etc.) to then iterate through the entire collection
            //to process further.  This will blend well with web services such as SimpleDB.
            //See: http://nuxleus.com/dev/browser/trunk/nuxleus/Source/CodeSamples/HttpGetAsyncResponse_Test/Program.cs#L63
            //for example.
            //m_responseStreamDictionary.Add(url.GetHashCode(), image);
        }


        static IEnumerable<IAsync> DownloadAll () {

            string testURIBase = "http://m.david.s3.amazonaws.com/photos/asynctest/";

            string[] testPhotoBaseArray = new string[]{
                "2203239268_17915bbbc8",
                "2205458193_7f8ff3797b",
                "2206244498_3b645a4929",
            };
            string[] testPhotoItemArray = new string[]{
                String.Empty,
                "_b",
                "_m",
                "_s",
                "_t",
            };

            IEnumerable<IAsync>[] requestOperations = new IEnumerable<IAsync>[testPhotoBaseArray.Length * testPhotoItemArray.Length];

            int p = 0;

            foreach (string b in testPhotoBaseArray) {
                foreach (string i in testPhotoItemArray) {
                    string url = String.Format("{0}{1}{2}.jpg", testURIBase, b, i);
                    requestOperations[p] = AsyncMethod(url);
                    p++;
                }
            }

            Stopwatch stopwatch = new Stopwatch();

            Console.WriteLine("Current thread id:\t {0}", Thread.CurrentThread.ManagedThreadId);
            stopwatch.Start();

            var methods = Async.Parallel(requestOperations);
            yield return methods;

            stopwatch.Stop();

            Console.WriteLine("Completed all in:\t {0}ms", stopwatch.ElapsedMilliseconds);
        }
    }
}
