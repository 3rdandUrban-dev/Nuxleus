using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Nuxleus.Asynchronous;
using Nuxleus.MetaData;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace Nuxleus.Core
{
    public class CustomPolicyHack : ICertificatePolicy
    {
        public bool CheckValidationResult(ServicePoint srvPoint, X509Certificate certificate, WebRequest request, int certificateProblem)
        {
            return true;
        }
    }

    public enum WebServiceType
    {
        SOAP,
        REST,
        QUERY,
        PROTOBUF
    }

    public struct HttpRequestSettings
    {
        public WebServiceType WebServiceType
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
    public class HttpWebServiceCallbackContainer
    {
        public AsyncCallback Callback { get; set; }
        public HttpWebRequest Request { get; set; }
        public NuxleusAsyncResult AsyncResult { get; set; }
        public ITask Task { get; set; }
    }

    public class HttpWebService<TRequestType, TResponseType>
    {
        static readonly string AWS_PUBLIC_KEY = System.Environment.GetEnvironmentVariable("AWS_PUBLIC_KEY");
        static readonly string AWS_PRIVATE_KEY = System.Environment.GetEnvironmentVariable("AWS_PRIVATE_KEY");
        static readonly string AWS_URI_ENDPOINT = System.Environment.GetEnvironmentVariable("AWS_URI_ENDPOINT");
        static readonly Uri m_AwsUri;
        static readonly XNamespace s = "http://schemas.xmlsoap.org/soap/envelope/";
        static readonly XNamespace i = "http://www.w3.org/2001/XMLSchema-instance";
        static XmlSerializer m_xSerializer = new XmlSerializer(typeof(TRequestType));
        static XmlSerializer m_responseTypeXmlSerializer = new XmlSerializer(typeof(TResponseType));
        static Encoding m_encoding = new UTF8Encoding();
        static XmlWriterSettings xSettings = new XmlWriterSettings();

        static HttpWebService()
        {
            xSettings.Encoding = Encoding.UTF8;
            xSettings.Indent = true;
            xSettings.OmitXmlDeclaration = true;
            m_AwsUri = new Uri(AWS_URI_ENDPOINT);
            ServicePointManager.MaxServicePoints = 20;
            ServicePointManager.MaxServicePointIdleTime = 10000;
            ServicePointManager.DefaultConnectionLimit = 100;
            //Not implemented on Mono
            //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback (ValidateServerCertificate); 
            //We need to use the deprecated CertificatePolicy ICertificatePolicy implementation instead.
            ServicePointManager.CertificatePolicy = new CustomPolicyHack();

        }

        public static HttpRequestSettings GetDefaultSettings()
        {
            return new HttpRequestSettings
            {
                ContentType = "application/soap+xml",
                KeepAlive = false,
                Method = "POST",
                Pipelined = false,
                Timeout = 10000,
                WebServiceType = WebServiceType.SOAP
            };

        }

        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            //Temporary hack
            return true;
        }

        static HttpWebRequest GetHttpWebRequest(ITask task, HttpRequestSettings settings)
        {
            byte[] buffer = null;
            IRequest webServiceRequest = task.Transaction.Request;
            WebServiceType wsType = settings.WebServiceType;
            string requestType = LabelAttribute.FromMember(webServiceRequest.RequestType);

            switch (wsType)
            {
                case WebServiceType.SOAP:
                    using (MemoryStream stream = new MemoryStream())
                    {
                        using (XmlWriter writer = XmlWriter.Create(stream, xSettings))
                        {
                            CreateSoapMessage(task, requestType, writer);
                        }
                        stream.Seek(0, 0);
                        buffer = stream.ToArray();
                    }
                    break;
                case WebServiceType.REST:
                case WebServiceType.QUERY:
                case WebServiceType.PROTOBUF:
                default:
                    throw new NotSupportedException(String.Format("{0} is not supported in this release.", wsType.ToString()));
            }

            ServicePoint servicePoint = ServicePointManager.FindServicePoint(m_AwsUri);
            HttpWebRequest request = null;
            try
            {
                request = (HttpWebRequest)WebRequest.CreateDefault(m_AwsUri);
                ServicePoint n_servicePoint = request.ServicePoint;
                n_servicePoint.ConnectionLimit = 100;
                Log.LogInfo<HttpWebService<TRequestType, TResponseType>>("Maximum # of ServicePoint Connections: {0}", servicePoint.ConnectionLimit);
                request.Timeout = settings.Timeout;
                request.KeepAlive = settings.KeepAlive;
                request.Pipelined = settings.Pipelined;
                request.Method = settings.Method;
                request.ContentType = settings.ContentType;
                request.Headers.Add("SOAPAction", requestType);
            }
            catch (UriFormatException ufe)
            {
                Log.LogInfo<HttpWebService<TRequestType, TResponseType>>("Caught UriFormatException on WebRequest.Create: {0}", ufe.Message);
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
                Log.LogInfo<HttpWebService<TRequestType, TResponseType>>("Caught WebException on GetResponseAsync: {0}", we.Message);
                task.Transaction.Successful = false;
            }
            if (webStream != null)
            {
                using (webStream)
                {
                    webStream.Write(buffer, 0, contentLength);
                    Log.LogInfo<HttpWebService<TRequestType, TResponseType>>("Sending request for task {0} on {1} thread: {2}",
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
                new HttpWebServiceCallbackContainer
                {
                    AsyncResult = asyncResult,
                    Callback = callback,
                    Request = request,
                    Task = task
                });
        }

        public void EndCallWebService(IAsyncResult asyncResult)
        {
            HttpWebServiceCallbackContainer container = (HttpWebServiceCallbackContainer)asyncResult.AsyncState;
            HttpWebResponse response = (HttpWebResponse)container.Request.EndGetResponse(asyncResult);
            ITask task = container.Task;
            ITransaction transaction = task.Transaction;
            Stream responseStream = response.GetResponseStream();
            try
            {
                transaction.Response = (IResponse)SerializeToObject(responseStream);
                transaction.Response.Headers = response.Headers;
                transaction.Successful = true;
            }
            catch (InvalidCastException e)
            {
                transaction.Successful = false;
                Log.LogDebug<HttpWebService<TRequestType, TResponseType>>(e.Message);
            }
            catch (Exception e)
            {
                transaction.Successful = false;
                Log.LogDebug<HttpWebService<TRequestType, TResponseType>>(e.Message);
            }
            task.StatusCode = response.StatusCode;
            task.Transaction.Commit();
        }

        public static IResponse CallWebService(ITask task)
        {
            return CallWebService(task, GetDefaultSettings());
        }

        public static IResponse CallWebService(ITask task, HttpRequestSettings settings)
        {
            IRequest sdbRequest = task.Transaction.Request;

            HttpWebRequest request = GetHttpWebRequest(task, settings);
            Stream responseStream;
            Log.LogInfo<HttpWebService<TRequestType, TResponseType>>("Sending request for task {0} on thread: {1}", task.TaskID, Thread.CurrentThread.ManagedThreadId);
            try
            {
                WebResponse response = request.GetResponse();
                responseStream = response.GetResponseStream();
                task.Transaction.Response = (IResponse)SerializeToObject(responseStream);
                task.Transaction.Response.Headers = response.Headers;
                Log.LogInfo<HttpWebService<TRequestType, TResponseType>>("Received response for task {0} on thread: {1}", task.TaskID, Thread.CurrentThread.ManagedThreadId);
            }
            catch (WebException exception)
            {
                task.Transaction.Response.Headers = exception.Response.Headers;
                responseStream = exception.Response.GetResponseStream();
                Log.LogDebug<TRequestType>("ErrorOutput: {0}", new StreamReader(responseStream).ReadToEnd());
            }

            return task.Transaction.Response;
        }

        public static IEnumerable<IAsync> CallWebServiceAsync(ITask task)
        {
            return CallWebServiceAsync(task, GetDefaultSettings());
        }

        public static IEnumerable<IAsync> CallWebServiceAsync(ITask task, HttpRequestSettings settings)
        {
            HttpWebRequest request = GetHttpWebRequest(task, settings);
            Async<WebResponse> response = request.GetResponseAsync();
            yield return response;
            Log.LogInfo<HttpWebService<TRequestType, TResponseType>>("Receiving response for task {0} on {1} thread: {2}",
                task.TaskID,
                Thread.CurrentThread.IsThreadPoolThread ? "threadpool" : "calling",
                Thread.CurrentThread.ManagedThreadId);
            Stream stream = response.Result.GetResponseStream();
            Async<MemoryStream> resultStream = stream.ReadToEndAsync<MemoryStream>().ExecuteAsync<MemoryStream>();
            yield return resultStream;
            try
            {
                task.Transaction.Response.Result = (IResult)SerializeToObject(resultStream.Result);
                task.Transaction.Response.Headers = response.Result.Headers;
                task.StatusCode = ((HttpWebResponse)response.Result as HttpWebResponse).StatusCode;
                task.Transaction.Successful = ProcessHttpStatusCode(task.StatusCode);
            }
            catch (InvalidCastException e)
            {
                task.Transaction.Successful = false;
                Log.LogDebug<HttpWebService<TRequestType, TResponseType>>(e.Message);
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

        static TResponseType SerializeToObject(Stream stream)
        {
            //using (TextReader reader = new StreamReader(stream))
            //{
            //    Console.WriteLine(reader.ReadToEnd());
            //}
            using (XmlReader reader = XmlReader.Create(stream))
            {
                TResponseType response = default(TResponseType);
                try
                {
                    reader.ReadToDescendant(String.Format("{0}", typeof(TResponseType).Name), "http://sdb.amazonaws.com/doc/2007-11-07/");
                    response = (TResponseType)m_responseTypeXmlSerializer.Deserialize(reader.ReadSubtree());
                }
                catch (InvalidCastException ex)
                {
                    Log.LogDebug<HttpWebService<TRequestType, TResponseType>>(ex.Message);
                }
                catch (Exception e)
                {
                    Log.LogDebug<HttpWebService<TRequestType, TResponseType>>(e.Message);
                }
                return response;
            }
        }

        private static IEnumerable<IAsync> HandleError(WebException exception, ITask task, HttpRequestSettings settings)
        {

            //HttpStatusCode statusCode = ((HttpWebResponse)exception.Response as HttpWebResponse).StatusCode;
            //if (statusCode == HttpStatusCode.InternalServerError || statusCode == HttpStatusCode.ServiceUnavailable)
            //{
            //    //TODO: Check for number of retries
            //    return CallWebServiceAsync(task, settings);
            //}
            //else
            //{
            //    return null;
            //}
            XElement.Load(XmlReader.Create(exception.Response.GetResponseStream())).Save(Console.Out);
            return null;
        }

        private static void CreateSoapMessage(ITask task, String action, XmlWriter writer)
        {
            writer.WriteStartElement("Envelope", s.NamespaceName);
            writer.WriteStartElement("Body", s.NamespaceName);
            m_xSerializer.Serialize(writer, (TRequestType)task);
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndDocument();

            //StringBuilder builder = new StringBuilder();
            //using (TextWriter w = new StringWriter(builder))
            //{
            //    m_xSerializer.Serialize(w, (TRequestType)task);
            //}
            //Console.WriteLine(builder.ToString());
        }
    }
}
