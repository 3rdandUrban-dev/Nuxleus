using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.IO;
using System.Net;
using Nuxleus.Async;
using System.Threading;
using System.Diagnostics;

namespace Nuxleus.Web {

    public delegate void ProcessHttpRequests ();

    public struct HttpGetAsyncResponse {
        static Dictionary<int, Stream> m_responseStreamDictionary = new Dictionary<int, Stream>();
        TextWriter m_logWriter;
        string[] m_httpRequestArray;
        int m_httpRequestArrayLength;
        bool m_DEBUG;
        Stopwatch m_stopwatch;
        List<long> m_elapsedTimeList;
        bool m_pipelineRequests;

        public HttpGetAsyncResponse ( TextWriter logWriter, bool debug, bool pipelineRequests, params string[] httpRequestArray ) {
            m_logWriter = logWriter;
            m_httpRequestArray = httpRequestArray;
            m_httpRequestArrayLength = httpRequestArray.Length;
            m_DEBUG = debug;
            m_stopwatch = new Stopwatch();
            m_elapsedTimeList = new List<long>();
            m_pipelineRequests = pipelineRequests;
        }

        public IAsyncResult BeginProcessRequests ( AsyncCallback callback, object extraData ) {
            if (DEBUG) {
                m_logWriter.WriteLine("Beginning async HTTP request process");
            }
            NuxleusAsyncResult nuxleusAsyncResult = new NuxleusAsyncResult(callback, extraData);
            ProcessRequests(nuxleusAsyncResult);
            return nuxleusAsyncResult;
        }

        public Dictionary<int, Stream> ResponseStreamDictionary { get { return m_responseStreamDictionary; } set { m_responseStreamDictionary = value; } }
        public bool DEBUG { get { return m_DEBUG; } set { m_DEBUG = value; } }
        public Stopwatch Stopwatch { get { return m_stopwatch; } }
        public List<long> ElapsedTimeList { get { return m_elapsedTimeList; } }

        private void ProcessRequests ( NuxleusAsyncResult asyncResult ) {

            int queryArrayLength = m_httpRequestArrayLength;
            TextWriter logWriter = m_logWriter;
            bool DEBUG = m_DEBUG;
            List<long> elaspedTimeList = m_elapsedTimeList;

            for (int q = 0; q < m_httpRequestArray.Length; q++) {
                Stopwatch stopwatch = new Stopwatch();
                if (DEBUG) {
                    stopwatch.Start();
                }
                string requestString = m_httpRequestArray[q];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestString);
                request.Timeout = 5000;
                request.KeepAlive = m_pipelineRequests;
                request.Pipelined = m_pipelineRequests;

                new AsyncHttpRequest(request,
                        delegate( Stream stream ) {
                            stopwatch.Stop();
                            long elapsedTime = stopwatch.ElapsedMilliseconds;
                            elaspedTimeList.Add(elapsedTime);
                            stopwatch.Reset();

                            if (DEBUG) {
                                logWriter.WriteLine("Current thread id: {0} for current request: {1}", Thread.CurrentThread.ManagedThreadId, requestString);
                            }

                            m_responseStreamDictionary.Add(requestString.GetHashCode(), stream);

                            if (m_responseStreamDictionary.Count == queryArrayLength) {
                                if (DEBUG) {
                                    logWriter.WriteLine("Elapsed time of this request:\t {0}ms", elapsedTime);
                                    logWriter.WriteLine("Completing call.");
                                }
                                asyncResult.CompleteCall();
                            } else {
                                if (DEBUG){
                                    logWriter.WriteLine("Elapsed time of this request:\t {0}ms", elapsedTime);
                                    logWriter.WriteLine("KeepAlive connection? {0}", request.KeepAlive);
                                    logWriter.WriteLine("Pipelined request? {0}", request.Pipelined);
                                    logWriter.WriteLine("Continuing process...");
                                }
                            }

                        }, logWriter, DEBUG);
            }
        }
    }

    public delegate void HttpResponseStream ( Stream responseStream );

    public struct AsyncHttpRequest {

        HttpWebRequest m_request;
        HttpResponseStream m_responseStream;
        TextWriter m_logWriter;
        bool m_DEBUG;

        public bool DEBUG { get { return m_DEBUG; } set { m_DEBUG = value; } }

        public AsyncHttpRequest ( HttpWebRequest request, HttpResponseStream responseStreamCallback, TextWriter logWriter, bool debug ) {
            m_request = request;
            m_responseStream = responseStreamCallback;
            m_logWriter = logWriter;
            m_DEBUG = debug;
            if (DEBUG)
                m_logWriter.WriteLine("Beginning call to {0}", request.RequestUri);
            request.BeginGetResponse(RequestIsComplete, request);
        }

        private void RequestIsComplete ( IAsyncResult asyncResult ) {
            HttpWebRequest request = (HttpWebRequest)asyncResult.AsyncState;
            HttpWebResponse response = (HttpWebResponse)m_request.EndGetResponse(asyncResult);
            if (DEBUG)
                m_logWriter.WriteLine("Ending call to {0}", request.RequestUri);
            m_responseStream(response.GetResponseStream());
            response.Close();
        }
    }
}
