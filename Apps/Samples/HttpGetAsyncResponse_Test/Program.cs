using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuxleus.Web;
using System.IO;
using System.Threading;
using System.Diagnostics;
using Nuxleus.Asynchronous;
using System.Collections;

namespace HttpGetAsyncResponse_Test {

    class Program {

        static bool m_complete = false;

        static void Main ( string[] args ) {

            string testURIBase = "http://m.david.s3.amazonaws.com/photos/asynctest/";
            string[] testPhotoBaseArray = new string[]{
                "2203239268_17915bbbc8",
                "2205458193_7f8ff3797b",
                "2206244498_3b645a4929",
            };
            string[] testPhotoItemArray = new string[]{
                "",
                "_b",
                "_m",
                "_s",
                "_t",
            };

            bool pipelineRequests = bool.Parse(args[0]);
            bool runSynchronously = bool.Parse(args[1]);

            string[] requestList = new string[testPhotoBaseArray.Length * testPhotoItemArray.Length];

            int p = 0;

            foreach (string b in testPhotoBaseArray) {
                foreach (string i in testPhotoItemArray) {
                    requestList[p] = String.Format("{0}{1}{2}.jpg", testURIBase, b, i);
                    p++;
                }
            }


            Console.WriteLine("Current thread id: {0}", Thread.CurrentThread.ManagedThreadId);
            HttpGetAsyncResponse response = new HttpGetAsyncResponse(Console.Out, true, pipelineRequests, runSynchronously, requestList);
            response.Stopwatch.Start();

            Console.WriteLine("Invoking Process");
            IAsyncResult asyncResult = response.BeginProcessRequests(new AsyncCallback(EndProcess), response);

            Console.ReadLine();
        }

        public static void EndProcess ( IAsyncResult asyncResult ) {

            HttpGetAsyncResponse response = (HttpGetAsyncResponse)asyncResult.AsyncState;

            foreach (KeyValuePair<int, MemoryStream> entry in response.ResponseStreamDictionary) {
                using (MemoryStream memoryStream = entry.Value) {
                    Console.WriteLine("Length of Stream: {0}", memoryStream.Length);
                }
            }

            response.Stopwatch.Stop();

            Console.WriteLine("Request processing is complete.  There are {0} responses in the response dictionary.", response.ResponseStreamDictionary.Count);
            Console.WriteLine("The process completed in: {0}ms.", response.Stopwatch.ElapsedMilliseconds);
        }
    }
}
