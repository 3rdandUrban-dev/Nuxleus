using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuxleus.Web;
using System.IO;
using System.Threading;


namespace HttpGetAsyncResponse_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] requestArray = new string[] { 
                "http://farm3.static.flickr.com/2182/2211087507_682a43b09a_o_d.jpg", 
                "http://farm3.static.flickr.com/2182/2211087507_682a43b09a_d.jpg",
                "http://farm3.static.flickr.com/2182/2211087507_682a43b09a_m_d.jpg", 
                "http://farm3.static.flickr.com/2182/2211087507_682a43b09a_t_d.jpg",
                "http://farm3.static.flickr.com/2182/2211087507_682a43b09a_s_d.jpg"
            };
            Console.WriteLine("Current thread id: {0}", Thread.CurrentThread.ManagedThreadId);
            HttpGetAsyncResponse response = new HttpGetAsyncResponse(Console.Out, requestArray);
            Dictionary<int, Stream> responseDictionary = response.BeginProcessRequests();
            Console.ReadLine();
        }
    }
}
