using System;
using Nuxleus.Extension.AWS.SimpleDB;
using System.Xml;
using System.Xml.Linq;
using System.Net;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Web;
using System.Web.Services.Protocols;
using System.Globalization;
using Nuxleus.Extension;
using EeekSoft.Asynchronous;
using System.Collections.Generic;
using System.Threading;
using Nuxleus.MetaData;
using System.Collections;
using System.Xml.Serialization;

namespace Nuxleus.Extension.AWS.SimpleDB {

    public enum RequestType {
        [Label("Query")]
        Query,
        [Label("CreateDomain")]
        CreateDomain,
        [Label("DeleteDomain")]
        DeleteDomain,
        [Label("ListDomains")]
        ListDomains,
        [Label("PutAttributes")]
        PutAttributes,
        [Label("DeleteAttributes")]
        DeleteAttributes,
        [Label("GetAttributes")]
        GetAttributes
    }

    public struct SimpleDBService<TRequestType> {

        static string AWS_PUBLIC_KEY = System.Environment.GetEnvironmentVariable("AWS_PUBLIC_KEY");
        static string AWS_PRIVATE_KEY = System.Environment.GetEnvironmentVariable("AWS_PRIVATE_KEY");
        static XNamespace s = "http://schemas.xmlsoap.org/soap/envelope/";
        static XNamespace aws = "http://sdb.amazonaws.com/doc/2007-11-07/";
        static XNamespace i = "http://www.w3.org/2001/XMLSchema-instance";
        static XmlSerializer m_xSerializer = new XmlSerializer(typeof(TRequestType));

        public static IEnumerable<IAsync> CallWebService<TResultType>(ITask task, IRequest sdbRequest, Dictionary<IRequest, TResultType> responseList) {

            Encoding encoding = new UTF8Encoding();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://sdb.amazonaws.com/");
            request.Timeout = 30000 /*TODO: This should be set dynamically*/;
            request.KeepAlive = true;
            request.Pipelined = false;

            StringBuilder output = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            using (XmlReader xreader = CreateSoapMessage(task, LabelAttribute.FromMember(sdbRequest.RequestType)).CreateReader()) {
                while (xreader.Read()) {
                    if (xreader.IsStartElement()) {
                        output.Append(xreader.ReadOuterXml());
                    }
                }
            }

            string soapMessage = output.ToString();
            sdbRequest.RequestMessage = soapMessage;
            //Console.WriteLine("XML: {0}", soapMessage);

            byte[] buffer = encoding.GetBytes(soapMessage);

            int contentLength = buffer.Length;
            request.ContentLength = contentLength;
            request.Method = "POST";
            request.ContentType = "application/soap+xml";

            foreach (KeyValuePair<string, string> header in sdbRequest.Headers) {
                request.Headers.Add(header.Key, header.Value);
            }

            //System.Console.WriteLine("Start Request: Thread is background: {0}, Thread ID: {1}, Thread is managed: {2}", Thread.CurrentThread.IsBackground, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);

            using (Stream newStream = request.GetRequestStream()) {
                newStream.Write(buffer, 0, contentLength);
                Async<WebResponse> response = request.GetResponseAsync();
                yield return response;
                //System.Console.WriteLine("[] got response on thread: {0}", Thread.CurrentThread.ManagedThreadId);
                Stream stream = response.Result.GetResponseStream();
                Async<TResultType> responseObject = stream.ReadToEndAsync<TResultType>().ExecuteAsync<TResultType>();
                yield return responseObject;
                responseList.Add(sdbRequest, responseObject.Result);
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

        private static XElement GetSignature(string action, string timestamp) {
            return new XElement(aws + "Signature", Sign(System.String.Format("{0}{1}", action, timestamp), AWS_PRIVATE_KEY));
        }

        private static string Sign(string data, string key) {
            Encoding encoding = new UTF8Encoding();
            HMACSHA1 signature = new HMACSHA1(encoding.GetBytes(key));
            return System.Convert.ToBase64String(signature.ComputeHash(
                encoding.GetBytes(data.ToCharArray())));
        }

        private static string GetFormattedTimestamp() {
            System.DateTime dateTime = System.DateTime.Now;
            return
                new System.DateTime(dateTime.Year, dateTime.Month, dateTime.Day,
                             dateTime.Hour, dateTime.Minute, dateTime.Second,
                             dateTime.Millisecond, System.DateTimeKind.Local)
                                .ToUniversalTime().ToString("yyyy-MM-dd\\THH:mm:ss.fff\\Z", CultureInfo.InvariantCulture);
        }
    }
}
