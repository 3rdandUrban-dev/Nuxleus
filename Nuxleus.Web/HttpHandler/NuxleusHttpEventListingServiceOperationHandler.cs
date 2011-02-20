// Copyright (c) 2006 by M. David Peterson
// The code contained in this file is licensed under The MIT License
// Please see http://www.opensource.org/licenses/mit-license.php for specific detail.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Nuxleus.Core;
using Nuxleus.Extension.Aws.SimpleDb;
using AwsSdbModel = Nuxleus.Extension.Aws.SimpleDb;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Serialization;
using System.Net;

namespace Nuxleus.Web.HttpHandler
{

    public class NuxleusHttpEventListingServiceOperationHandler : IHttpAsyncHandler
    {
        HttpRequest m_request;
        HttpResponse m_response;
        static Encoding m_encoding = new UTF8Encoding();
        HttpCookieCollection m_cookieCollection;
        NuxleusAsyncResult m_asyncResult;
        static XNamespace r = "http://nuxleus.com/message/response";
        static List<string> sessionCookies = new List<string>();
        SelectTask m_task;
        NuxleusAsyncResult m_iTaskResult;

        public void ProcessRequest(HttpContext context)
        {
            //not called
        }

        public bool IsReusable
        {
            get { return false; }
        }

        static NuxleusHttpEventListingServiceOperationHandler()
        {
            sessionCookies.Add("sessionid");
            sessionCookies.Add("userid");
            sessionCookies.Add("username");
            sessionCookies.Add("name");
            sessionCookies.Add("uservalidated");
        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            m_request = context.Request;
            m_response = context.Response;
            m_cookieCollection = context.Request.Cookies;
            m_asyncResult = new NuxleusAsyncResult(cb, extraData);
            bool success = true;

            Dictionary<string, string> cookieDictionary = Nuxleus.Web.Utility.GetCookieValues(m_cookieCollection, out success, sessionCookies.ToArray());

            string ip = m_request.UserHostAddress.ToString();

            string currentUserId = String.Empty;
            if (cookieDictionary.TryGetValue("userid", out currentUserId))
            {
                m_task = new SelectTask { DomainName = new Domain { Name = "event" }, SelectExpression = String.Format("select * from event where eventcreator = '{0}'", currentUserId) };
                m_iTaskResult = new NuxleusAsyncResult(cb, extraData);

                m_task.Transaction.OnSuccessfulTransaction += new OnSuccessfulTransaction(Transaction_OnSuccessfulTransaction);
                m_task.Transaction.OnFailedTransaction += new OnFailedTransaction(Transaction_OnFailedTransaction);

            }
            return m_task.BeginInvoke(m_iTaskResult);
        }

        public void Transaction_OnSuccessfulTransaction()
        {
            WriteDebugXmlToOutputStream(((SelectResult)m_task.Transaction.Response.Result).Item);
            m_iTaskResult.CompleteCall();
        }

        public void Transaction_OnFailedTransaction()
        {
            WriteDebugXmlToOutputStream(((SelectResult)m_task.Transaction.Response.Result).Item);
            m_iTaskResult.CompleteCall();
        }


        public void EndProcessRequest(IAsyncResult result)
        {
            //m_response.Redirect(m_returnLocation);
        }

        void WriteDebugXmlToOutputStream(List<Item> items)
        {
            XDocument doc = new XDocument(
                new XElement(r + "message",
                         GenerateXElementItems(items)
                )
            );
            m_response.ContentType = "text/xml";
            string xmlOutput = null;
            using (TextWriter writer = new StringWriter())
            {
                doc.Save(writer);
                xmlOutput = writer.ToString();
                m_response.Write(xmlOutput);
            }
        }

        IEnumerable<XElement> GenerateXElementItems(List<Item> items)
        {
            foreach (Item item in items)
            {
                yield return new XElement(r + "event",
                         GenerateXElementArray(item.Attribute)
                    );
            }
        }

        IEnumerable<XElement> GenerateXElementArray(List<AwsSdbModel.Attribute> attributes)
        {
            foreach (AwsSdbModel.Attribute attribute in attributes)
            {
                yield return new XElement(r + attribute.Name, attribute.Value);
            }
        }
    }
}
