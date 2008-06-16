using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using EeekSoft.Asynchronous;
using System.Xml.Linq;
using Nuxleus.Extension.AWS.SimpleDB.Model;
using System.Collections;
using Nuxleus.MetaData;
using System.Net;
using System.Xml;
using System.IO;
using System.Security.Cryptography;
using System.Globalization;

namespace Nuxleus.Extension.AWS.SimpleDB {

    [XmlTypeAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/")]
    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/", IsNullable = false)]
    public struct PutAttributes : ITask {

        static string AWS_PUBLIC_KEY = System.Environment.GetEnvironmentVariable("AWS_PUBLIC_KEY");
        static string AWS_PRIVATE_KEY = System.Environment.GetEnvironmentVariable("AWS_PRIVATE_KEY");

        string m_domainName;
        string m_itemName;
        ArrayList m_attributeArray;

        static XNamespace aws = "http://sdb.amazonaws.com/doc/2007-11-07/";
        static XNamespace i = "http://www.w3.org/2001/XMLSchema-instance";

        [XmlElementAttribute(ElementName = "DomainName")]
        public string DomainName {
            get { return m_domainName; }
            set { m_domainName = value; }
        }

        [XmlElementAttribute(ElementName = "ItemName")]
        public string ItemName {
            get { return m_itemName; }
            set { m_itemName = value; }
        }

        public ArrayList AttributeArray {
            get { return m_attributeArray; }
            set { m_attributeArray = value; }
        }


        public XElement[] GetXMLBody {
            get {
                return CreateAttributeElements();
            }
        }

        public RequestType RequestType { get { return RequestType.PutAttributes; } }

        private XElement GetRequest() {
            XNamespace s = "http://schemas.xmlsoap.org/soap/envelope/";
            return new XElement(s + "Envelope",
                    new XElement(s + "Body",
                        SdbAction.PutAttributes(DomainName, ItemName, AttributeArray)
                    )
                );
        }

        public IEnumerable<IAsync> Invoke<T>(Dictionary<XElement, T> responseList) {


            XNamespace s = "http://schemas.xmlsoap.org/soap/envelope/";

            XElement awsSOAPMessage = 
                new XElement(s + "Envelope",
                    new XElement(s + "Body",
                        SdbAction.PutAttributes(DomainName, ItemName, AttributeArray)
                    )
                );

            foreach (Nuxleus.Extension.AWS.SimpleDB.Model.Attribute attribute in AttributeArray) {
                System.Console.WriteLine("Name: {0}, Value: {1}, Replace: {2}", attribute.Name, attribute.Value, attribute.Replace);
            }

            string rType = LabelAttribute.FromMember(RequestType);

            Encoding encoding = new UTF8Encoding();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://sdb.amazonaws.com/");

            request.Timeout = 10000 /*TODO: This should be set dynamically*/;
            request.KeepAlive = true;
            request.Pipelined = true;

            XmlReader xreader = awsSOAPMessage.CreateReader();
            StringBuilder output = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?>");

            byte[] buffer = null;

            do {
                if (xreader.IsStartElement()) {
                    output.Append(xreader.ReadOuterXml());
                    buffer = encoding.GetBytes(output.ToString());
                }
            } while (xreader.Read());

            int contentLength = buffer.Length;
            request.ContentLength = contentLength;
            request.Method = "POST";
            request.ContentType = "application/soap+xml";
            request.Headers.Add("SOAPAction", rType);

            request.Timeout = 10000 /*TODO: This should be set dynamically*/;
            request.KeepAlive = true;
            request.Pipelined = true;

            //Console.WriteLine("Start Request: Thread is background: {0}, Thread ID: {1}, Thread is managed: {2}", Thread.CurrentThread.IsBackground, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);

            using (Stream newStream = request.GetRequestStream()) {

                newStream.Write(buffer, 0, contentLength);
                Async<WebResponse> response = request.GetResponseAsync();
                yield return response;
                //Console.WriteLine("[] got response on thread: {0}", Thread.CurrentThread.ManagedThreadId);
                Stream stream = response.Result.GetResponseStream();
                Async<T> responseObject = stream.ReadToEndAsync<T>().ExecuteAsync<T>();
                yield return responseObject;
                responseList.Add(awsSOAPMessage, responseObject.Result);
            }
        }

        private XElement[] CreateAttributeElements() {
            XElement[] xElements = new XElement[m_attributeArray.Count];
            int i = 0;
            foreach (Attribute attribute in m_attributeArray) {
                xElements[i] = CreateAttributeElement(attribute);
                i++;
            }
            return xElements;
        }

        private static XElement CreateAttributeElement(Attribute attribute) {
            return new XElement(aws + "Attribute",
                new XElement(aws + "Name", attribute.Name),
                new XElement(aws + "Value", attribute.Value),
                new XElement(aws + "Replace", attribute.Replace)
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
            return new XElement(aws + "Signature", Sign(string.Format("{0}{1}", action, timestamp), AWS_PRIVATE_KEY));
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
