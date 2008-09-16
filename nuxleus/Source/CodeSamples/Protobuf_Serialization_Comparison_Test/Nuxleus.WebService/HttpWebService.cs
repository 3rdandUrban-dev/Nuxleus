using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Nuxleus.Asynchronous;
using Nuxleus.Extension;
using Nuxleus.MetaData;

namespace Nuxleus.WebService {

    public enum RequestType {
        PUT,
        POST,
        GET,
        DELETE
    }

    public enum WebServiceType {
        SOAP,
        REST,
        QUERY
    }

    public struct HttpRequestSettings {
        public WebServiceType WebServiceType { get; set; }
        public int Timeout { get; set; }
        public bool KeepAlive { get; set; }
        public bool Pipelined { get; set; }
        public string Method { get; set; }
        public string ContentType { get; set; }
    }

    public struct HttpWebServiceRequest<TRequestType> {

        static readonly string AWS_PUBLIC_KEY = System.Environment.GetEnvironmentVariable("AWS_PUBLIC_KEY");
        static readonly string AWS_PRIVATE_KEY = System.Environment.GetEnvironmentVariable("AWS_PRIVATE_KEY");
        static readonly string WEBSERVICE_URI_ENDPOINT = System.Environment.GetEnvironmentVariable("AWS_URI_ENDPOINT");
        static readonly XNamespace s = "http://schemas.xmlsoap.org/soap/envelope/";
        static readonly XNamespace aws = "http://sdb.amazonaws.com/doc/2007-11-07/";
        static readonly XNamespace i = "http://www.w3.org/2001/XMLSchema-instance";
        static XmlSerializer m_xSerializer = new XmlSerializer(typeof(TRequestType));
        static Encoding m_encoding = new UTF8Encoding();

        public static IEnumerable<IAsync> CallWebServiceAsync(ITask task, HttpRequestSettings settings) {

            WebServiceType type = settings.WebServiceType;

            HttpWebRequest request = null;
            try {
                request = (HttpWebRequest)WebRequest.Create(WEBSERVICE_URI_ENDPOINT);
                request.Timeout = settings.Timeout;
                request.KeepAlive = settings.KeepAlive;
                request.Pipelined = settings.Pipelined;
                request.Method = settings.Method;
                request.ContentType = settings.ContentType;
            } catch (UriFormatException ufe) {
                Log.LogInfo<HttpWebServiceRequest<TRequestType>>("Caught UriFormatException on WebRequest.Create: {0}", ufe.Message);
            }
            IRequest webServiceRequest = task.Request;
            byte[] buffer = null;

            switch (type) {
                case WebServiceType.REST:
                    break;
                case WebServiceType.SOAP:
                    StringBuilder output = new StringBuilder();
                    //output.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                    using (XmlReader xreader = CreateSoapMessage(task, LabelAttribute.FromMember(webServiceRequest.RequestType)).CreateReader()) {
                        while (xreader.Read()) {
                            if (xreader.IsStartElement()) {
                                output.Append(xreader.ReadOuterXml());
                            }
                        }
                    }
                    string soapMessage = output.ToString();
                    webServiceRequest.RequestMessage = soapMessage;
                    buffer = m_encoding.GetBytes(soapMessage);
                    
                    break;
            }

            int contentLength = buffer.Length;
            request.ContentLength = contentLength;

            foreach (KeyValuePair<string, string> header in webServiceRequest.Headers) {
                request.Headers.Add(header.Key, header.Value);
            }

            Stream webStream = null;

            try {
                webStream = request.GetRequestStream();
            } catch (WebException we) {
                Log.LogInfo<HttpWebServiceRequest<TRequestType>>("Caught WebException on GetResponseAsync: {0}", we.Message);
            }
            if (webStream != null) {
                using (webStream) {
                    webStream.Write(buffer, 0, contentLength);
                    Log.LogInfo<HttpWebServiceRequest<TRequestType>>("Sending request for task {0} on thread: {1}", task.TaskID, Thread.CurrentThread.ManagedThreadId);

                    Async<WebResponse> response = null;
                    try {
                        response = request.GetResponseAsync();
                        Log.LogInfo<HttpWebServiceRequest<TRequestType>>("Received response for task {0} on thread: {1}", task.TaskID, Thread.CurrentThread.ManagedThreadId);
                    } catch (WebException we) {
                        Log.LogDebug<HttpWebServiceRequest<TRequestType>>("The call to GetResponseAsync for {0} failed with the error: {1}.", task.TaskID, we.Message);
                        //TODO: Add the failed task to a retry queue.
                    }
                    if (response != null) {
                        yield return response;
                        Stream stream = null;
                        try {
                            stream = response.Result.GetResponseStream();
                        } catch (NotSupportedException nse) {
                            Log.LogDebug<HttpWebServiceRequest<TRequestType>>("Caught NotSupportedException on Result.GetResponseStream(): {0}", nse.Message);
                        } catch (WebException we) {
                            Log.LogDebug<HttpWebServiceRequest<TRequestType>>("Caught WebException on Result.GetResponseStream(): {0}", we.Message);
                        } catch (Exception e) {
                            Log.LogDebug<HttpWebServiceRequest<TRequestType>>("Caught Exception on Result.GetResponseStream(): {0}", e.Message);
                        }
                        if (stream != null) {
                            Async<String> responseObject = null;
                            try {
                                responseObject = stream.ReadToEndAsync<String>().ExecuteAsync<String>();
                            } catch (Exception e) {
                                //TODO: Add the failed task to a retry queue.
                                Log.LogDebug<HttpWebServiceRequest<TRequestType>>("The call to stream.ReadToEndAsync<String>().ExecuteAsync<String>() for {0} failed with the error: {1}.", task.TaskID, e.Message);
                            }

                            string result = String.Empty;

                            if (responseObject != null) {
                                yield return responseObject;
                                result = responseObject.Result;
                            }

                            task.Response.Response = result;
                        }
                    } else {
                        Log.LogDebug<HttpWebServiceRequest<TRequestType>>("Task {0} has failed. Need to add to to new queue to be reprocessed.", task.TaskID);
                        //TODO: Add the failed task to a retry queue.
                    }
                }
            } else {
                Log.LogDebug<HttpWebServiceRequest<TRequestType>>("Task {0} has failed. Need to add to to new queue to be reprocessed.", task.TaskID);
                //TODO: Add the failed task to a retry queue.
            }
        }

        private static XElement CreateSoapMessage(ITask task, String action) {

            StringBuilder builder = new StringBuilder();
            using (TextWriter writer = new StringWriter(builder)) {
                m_xSerializer.Serialize(writer, (TRequestType)task);
            }
            XElement body = XElement.Parse(builder.ToString());
            body.Add(GetAuthorizationElements(action));

            return new XElement(s + "Envelope",
                    new XElement(s + "Body",
                        body
                    )
                );
        }

        private static XElement[] GetAuthorizationElements(string action) {
            string timestamp = GetFormattedTimestamp();
            return
                new XElement[] { 
                    new XElement(aws + "AWSAccessKeyId", AWS_PUBLIC_KEY),
                    new XElement(aws + "Timestamp", timestamp),
                    GetSignature(action, timestamp)
                };

        }

        /**
         * Add authentication related and version parameters
         */
        private void AddRequiredParameters(IDictionary<String, String> parameters) {
            parameters.Add("AWSAccessKeyId", AWS_PUBLIC_KEY);
            parameters.Add("Timestamp", GetFormattedTimestamp());
            parameters.Add("Version", "2007-11-07");
            parameters.Add("SignatureVersion", "1");
            parameters.Add("Signature", SignParameters(parameters, AWS_PRIVATE_KEY));
        }

        /**
         * Convert Disctionary of paremeters to Url encoded query string
         */
        private string GetParametersAsString(IDictionary<String, String> parameters) {
            StringBuilder data = new StringBuilder();
            foreach (String key in (IEnumerable<String>)parameters.Keys) {
                String value = parameters[key];
                if (value != null && value.Length > 0) {
                    data.Append(key);
                    data.Append('=');
                    data.Append(HttpUtility.UrlEncodeUnicode(value));
                    data.Append('&');
                }
            }
            String stringData = data.ToString();
            if (stringData.EndsWith("&")) {
                stringData = stringData.Remove(stringData.Length - 1, 1);
            }
            return stringData;
        }

        /**
         * Computes RFC 2104-compliant HMAC signature for request parameters
         * Implements AWS Signature, as per following spec:
         *
         * If Signature Version is 0, it signs concatenated Action and Timestamp
         *
         * If Signature Version is 1, it performs the following:
         *
         * Sorts all  parameters (including SignatureVersion and excluding Signature,
         * the value of which is being created), ignoring case.
         *
         * Iterate over the sorted list and append the parameter name (in original case)
         * and then its value. It will not URL-encode the parameter values before
         * constructing this string. There are no separators.
         */
        private String SignParameters(IDictionary<String, String> parameters, String key) {
            String signatureVersion = parameters["SignatureVersion"];
            StringBuilder data = new StringBuilder();

            if ("0".Equals(signatureVersion)) {
                data.Append(parameters["Action"]).Append(parameters["Timestamp"]);
            } else if ("1".Equals(signatureVersion)) {
                IDictionary<String, String> sorted =
                    new SortedDictionary<String, String>(parameters, StringComparer.InvariantCultureIgnoreCase);
                parameters.Remove("Signature");
                foreach (KeyValuePair<String, String> pair in sorted) {
                    if (pair.Value != null && pair.Value.Length > 0) {
                        data.Append(pair.Key);
                        data.Append(pair.Value);
                    }
                }
            } else {
                throw new Exception("Invalid Signature Version specified");
            }
            return Sign(data.ToString(), key);
        }

        private static XElement GetSignature(string action, string timestamp) {
            return new XElement(aws + "Signature", Sign(System.String.Format("{0}{1}", action, timestamp), AWS_PRIVATE_KEY));
        }

        private static string Sign(string data, string key) {
            HMACSHA1 signature = new HMACSHA1(m_encoding.GetBytes(key));
            return System.Convert.ToBase64String(signature.ComputeHash(m_encoding.GetBytes(data.ToCharArray())));
        }

        private static string GetFormattedTimestamp() {
            System.DateTime dateTime = System.DateTime.Now;
            return
                new System.DateTime(
                        dateTime.Year,
                        dateTime.Month,
                        dateTime.Day,
                        dateTime.Hour,
                        dateTime.Minute,
                        dateTime.Second,
                        dateTime.Millisecond,
                        System.DateTimeKind.Local
                    ).ToUniversalTime().ToString("yyyy-MM-dd\\THH:mm:ss.fff\\Z", CultureInfo.InvariantCulture);
        }
    }
}
