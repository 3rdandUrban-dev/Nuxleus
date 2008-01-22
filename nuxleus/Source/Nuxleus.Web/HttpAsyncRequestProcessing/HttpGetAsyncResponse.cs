using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.IO;
using System.Net;
using Nuxleus.Async;
using System.Threading;

namespace Nuxleus.Web
{

    public struct HttpGetAsyncResponse
    {

        public delegate Dictionary<int, Stream> ProcessHttpRequests();

        static Dictionary<int, Stream> m_responseStreamDictionary = new Dictionary<int, Stream>();
        TextWriter m_logWriter;
        string[] m_httpRequestArray;
        int m_httpRequestArrayLength;

        public HttpGetAsyncResponse(TextWriter logWriter, params string[] httpRequestArray)
        {
            m_logWriter = logWriter;
            m_httpRequestArray = httpRequestArray;
            m_httpRequestArrayLength = httpRequestArray.Length;
        }

        public Dictionary<int, Stream> BeginProcessRequests()
        {
            m_logWriter.WriteLine("Beginning async HTTP request process");
            ProcessRequests(new NuxleusAsyncResult(new AsyncCallback(EndProcessRequests), null));
            return m_responseStreamDictionary;
        }

        public void EndProcessRequests(IAsyncResult asyncResult)
        {
            m_logWriter.WriteLine("Request processing is complete.  There are {0} responses in the response dictionary.", m_responseStreamDictionary.Count);
        }

        private void ProcessRequests(NuxleusAsyncResult asyncResult)
        {
            int queryArrayLength = m_httpRequestArrayLength;
            TextWriter logWriter = m_logWriter;
            for (int q = 0; q < m_httpRequestArray.Length; q++)
            {
                string requestString = m_httpRequestArray[q];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestString);
                request.Timeout = 5000;
                request.KeepAlive = true;
                request.Pipelined = true;

                new AsyncHttpRequest(request,
                        delegate(Stream stream)
                        {
                            Console.WriteLine("Current thread id: {0} for current request: {1}", Thread.CurrentThread.ManagedThreadId, requestString);
                            m_responseStreamDictionary.Add(requestString.GetHashCode(), stream);
                            if (m_responseStreamDictionary.Count == queryArrayLength)
                            {
                                logWriter.WriteLine("Completing call");
                                asyncResult.CompleteCall();
                            }
                            else
                            {
                                logWriter.WriteLine("Continuing process...");
                                //queryArrayLength++;
                            }
                        });
            }
        }
    }

    public delegate void HttpResponseStream(Stream responseStream);

    public struct AsyncHttpRequest
    {

        HttpWebRequest m_request;
        HttpResponseStream m_responseStream;

        public AsyncHttpRequest(HttpWebRequest request, HttpResponseStream responseStreamCallback)
        {
            m_request = request;
            m_responseStream = responseStreamCallback;
            Console.WriteLine("Beginning call to {0}", request.RequestUri);
            request.BeginGetResponse(RequestIsComplete, request);
        }

        private void RequestIsComplete(IAsyncResult asyncResult)
        {
            HttpWebRequest request = (HttpWebRequest)asyncResult.AsyncState;
            HttpWebResponse response = (HttpWebResponse)m_request.EndGetResponse(asyncResult);
            Console.WriteLine("Ending call to {0}", request.RequestUri);
            m_responseStream(response.GetResponseStream());
            response.Close();
        }
    }
}
