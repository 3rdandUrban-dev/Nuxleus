using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuxleus.Web;
using System.IO;
using System.Threading;
using Nuxleus.Agent;


namespace HttpGetAsyncResponse_Test {
    class Program {
        static void Main ( string[] args ) {
            string flickrPhotoStreamBase = "2182/2211087507_682a43b09";

            if (args.Length > 0) {
                flickrPhotoStreamBase = args[0];
            }

            string[] requestArray = new string[] { 
                String.Format("http://farm3.static.flickr.com/{0}a_o_d.jpg", flickrPhotoStreamBase), 
                String.Format("http://farm3.static.flickr.com/{0}a_d.jpg", flickrPhotoStreamBase),
                String.Format("http://farm3.static.flickr.com/{0}a_m_d.jpg", flickrPhotoStreamBase), 
                String.Format("http://farm3.static.flickr.com/{0}a_t_d.jpg", flickrPhotoStreamBase),
                String.Format("http://farm3.static.flickr.com/{0}a_s_d.jpg", flickrPhotoStreamBase)
            };

            Console.WriteLine("Current thread id: {0}", Thread.CurrentThread.ManagedThreadId);
            HttpGetAsyncResponse response = new HttpGetAsyncResponse(Console.Out, requestArray);

            Console.WriteLine("Invoking Process");
            response.BeginProcessRequests(new AsyncCallback(EndProcess), response);

            Console.ReadLine();
        }

        public static void EndProcess ( IAsyncResult asyncResult ) {
            HttpGetAsyncResponse process = (HttpGetAsyncResponse)asyncResult.AsyncState;
            Dictionary<int, Stream> responseStreamDictionary = process.ResponseStreamDictionary;
            Console.WriteLine("Request processing is complete.  There are {0} responses in the response dictionary.", responseStreamDictionary.Count);
        }
    }
}
