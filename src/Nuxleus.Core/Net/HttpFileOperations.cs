using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using Nuxleus.Asynchronous;
using Nuxleus.MetaData;

namespace Nuxleus.Core.Net
{
    public class HttpFileOperations
    {

        public static HttpFileOperations Instance { get { return SingletonProvider<HttpFileOperations>.Instance; } }
        private static readonly HttpFileOperations @this = HttpFileOperations.Instance;

        static HttpFileOperations()
        {

        }


        public enum HttpFileOperationType
        {
            GET,
            PUT,
            POST,
            DELETE,
            HEAD
        }

        public class HttpWebRequestSettings
        {
            public HttpFileOperationType HttpFileOperationType
            {
                get;
                set;
            }
            public int Timeout
            {
                get;
                set;
            }
            public bool KeepAlive
            {
                get;
                set;
            }
            public bool Pipelined
            {
                get;
                set;
            }
            public string Method
            {
                get;
                set;
            }
            public string ContentType
            {
                get;
                set;
            }
        }

        public class HttpWebOperationCallbackContainer
        {
            public AsyncCallback Callback { get; set; }
            public HttpWebRequest Request { get; set; }
            public NuxleusAsyncResult AsyncResult { get; set; }
            public ITask Task { get; set; }
        }

        public static HttpWebRequestSettings GetDefaultSettings()
        {
            return new HttpWebRequestSettings
            {
                ContentType = "application/soap+xml",
                KeepAlive = false,
                Method = "POST",
                Pipelined = false,
                Timeout = 10000,
                HttpFileOperationType = HttpFileOperationType.GET
            };

        }

        static HttpWebRequest GetHttpWebRequest(ITask task, HttpWebRequestSettings settings)
        {
            byte[] buffer = null;
            IRequest webServiceRequest = task.Transaction.Request;
            HttpFileOperationType httpRequestType = settings.HttpFileOperationType;
            string requestType = LabelAttribute.FromMember(webServiceRequest.RequestType);

            switch (httpRequestType)
            {
                case HttpFileOperationType.GET:
                    using (MemoryStream stream = new MemoryStream())
                    {
                        stream.Seek(0, 0);
                        buffer = stream.ToArray();
                    }
                    break;
                case HttpFileOperationType.PUT:
                case HttpFileOperationType.POST:
                case HttpFileOperationType.DELETE:
                default:
                    throw new NotSupportedException(String.Format("{0} is not supported in this release.", httpRequestType.ToString()));
            }

            Uri fileUri = new Uri("replaceme");
            ServicePoint servicePoint = ServicePointManager.FindServicePoint(fileUri);
            HttpWebRequest request = null;
            try
            {
                request = (HttpWebRequest)WebRequest.CreateDefault(fileUri);
                ServicePoint n_servicePoint = request.ServicePoint;
                servicePoint.ConnectionLimit = 100;
                @this.LogInfo("Maximum # of ServicePoint Connections: {0}", servicePoint.ConnectionLimit);
                request.Timeout = settings.Timeout;
                request.KeepAlive = settings.KeepAlive;
                request.Pipelined = settings.Pipelined;
                request.Method = settings.Method;
                request.ContentType = settings.ContentType;
                request.Headers.Add("SOAPAction", requestType);
            }
            catch (UriFormatException ufe)
            {
                @this.LogInfo("Caught UriFormatException on WebRequest.Create: {0}", ufe.Message);
            }

            foreach (KeyValuePair<string, string> header in webServiceRequest.Headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            int contentLength = buffer.Length;
            request.ContentLength = contentLength;

            Stream webStream = null;

            try
            {
                webStream = request.EndGetRequestStream(request.BeginGetRequestStream(new AsyncCallback(EndGetRequestStream), request));
            }
            catch (WebException we)
            {
                @this.LogInfo("Caught WebException on GetResponseAsync: {0}", we.Message);
                task.Transaction.Successful = false;
            }
            if (webStream != null)
            {
                using (webStream)
                {
                    webStream.Write(buffer, 0, contentLength);
                    @this.LogInfo("Sending request for task {0} on {1} thread: {2}",
                        task.TaskID,
                        Thread.CurrentThread.IsThreadPoolThread ? "threadpool" : "calling",
                        Thread.CurrentThread.ManagedThreadId);
                }
            }

            return request;
        }

        public static void EndGetRequestStream(IAsyncResult asyncResult)
        {

        }

        public IAsyncResult BeginCallWebService(ITask task, NuxleusAsyncResult asyncResult)
        {
            HttpWebRequest request = GetHttpWebRequest(task, GetDefaultSettings());
            AsyncCallback callback = new AsyncCallback(EndCallWebService);
            return request.BeginGetResponse(callback,
                new HttpWebOperationCallbackContainer
                {
                    AsyncResult = asyncResult,
                    Callback = callback,
                    Request = request,
                    Task = task
                });
        }

        public void EndCallWebService(IAsyncResult asyncResult)
        {
            HttpWebOperationCallbackContainer container = (HttpWebOperationCallbackContainer)asyncResult.AsyncState;
            HttpWebResponse response = (HttpWebResponse)container.Request.EndGetResponse(asyncResult);
            ITask task = container.Task;
            ITransaction transaction = task.Transaction;
            Stream responseStream = response.GetResponseStream();
            try
            {
                //transaction.Response = (IResponse)SerializeToObject(responseStream);
                transaction.Response.Headers = response.Headers;
                transaction.Successful = true;
            }
            catch (InvalidCastException e)
            {
                transaction.Successful = false;
                @this.LogDebug(e.Message);
            }
            catch (Exception e)
            {
                transaction.Successful = false;
                @this.LogDebug(e.Message);
            }
            task.StatusCode = response.StatusCode;
            task.Transaction.Commit();
        }

        public static IResponse CallWebService(ITask task)
        {
            return CallWebService(task, GetDefaultSettings());
        }

        public static IResponse CallWebService(ITask task, HttpWebRequestSettings settings)
        {
            IRequest sdbRequest = task.Transaction.Request;

            HttpWebRequest request = GetHttpWebRequest(task, settings);
            Stream responseStream;
            @this.LogInfo("Sending request for task {0} on thread: {1}", task.TaskID, Thread.CurrentThread.ManagedThreadId);
            try
            {
                WebResponse response = request.GetResponse();
                responseStream = response.GetResponseStream();
                //task.Transaction.Response = (IResponse)SerializeToObject(responseStream);
                task.Transaction.Response.Headers = response.Headers;
                @this.LogInfo("Received response for task {0} on thread: {1}", task.TaskID, Thread.CurrentThread.ManagedThreadId);
            }
            catch (WebException exception)
            {
                task.Transaction.Response.Headers = exception.Response.Headers;
                responseStream = exception.Response.GetResponseStream();
                @this.LogError("ErrorOutput: {0}", new StreamReader(responseStream).ReadToEnd());
            }

            return task.Transaction.Response;
        }

        public static IEnumerable<IAsync> CallWebServiceAsync(ITask task)
        {
            return CallWebServiceAsync(task, GetDefaultSettings());
        }

        public static IEnumerable<IAsync> CallWebServiceAsync(ITask task, HttpWebRequestSettings settings)
        {
            HttpWebRequest request = GetHttpWebRequest(task, settings);
            Async<WebResponse> response = request.GetResponseAsync();
            yield return response;
            @this.LogInfo("Receiving response for task {0} on {1} thread: {2}",
                task.TaskID,
                Thread.CurrentThread.IsThreadPoolThread ? "threadpool" : "calling",
                Thread.CurrentThread.ManagedThreadId);
            Stream stream = response.Result.GetResponseStream();
            Async<MemoryStream> resultStream = stream.ReadToEndAsync<MemoryStream>().ExecuteAsync<MemoryStream>();
            yield return resultStream;
            try
            {
                //task.Transaction.Response.Result = (IResult)SerializeToObject(resultStream.Result);
                task.Transaction.Response.Headers = response.Result.Headers;
                task.StatusCode = ((HttpWebResponse)response.Result as HttpWebResponse).StatusCode;
                task.Transaction.Successful = ProcessHttpStatusCode(task.StatusCode);
            }
            catch (InvalidCastException e)
            {
                task.Transaction.Successful = false;
                @this.LogDebug(e.Message);
            }

            try
            {
                task.Transaction.Commit();
            }
            catch
            {

            }
        }

        static bool ProcessHttpStatusCode(HttpStatusCode statusCode)
        {
            // An obvious temp hack
            return true;
        }
    }
}
