// Copyright (c) 2006 by M. David Peterson
// The code contained in this file is licensed under a Creative Commons (Attribution 3.0) license
// Please see http://creativecommons.org/licenses/by/3.0/us/ for specific detail.

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

namespace Nuxleus.Web.HttpHandler
{

    public class NuxleusHttpDeleteEventServiceOperationHandler : IHttpAsyncHandler
    {
        HttpRequest m_request;
        HttpResponse m_response;
        static Encoding m_encoding = new UTF8Encoding();
        HttpCookieCollection m_cookieCollection;
        NuxleusAsyncResult m_asyncResult;
        static XNamespace r = "http://nuxleus.com/message/response";
        static List<string> sessionCookies = new List<string>();
        static List<AwsSdbModel.Attribute> deleteAttributes = new List<AwsSdbModel.Attribute>();

        public void ProcessRequest(HttpContext context)
        {
            //not called
        }

        public bool IsReusable
        {
            get { return false; }
        }

        static NuxleusHttpDeleteEventServiceOperationHandler()
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
                DeleteAttributesTask task = new DeleteAttributesTask { DomainName = "event", ItemName = m_request.QueryString["eventid"], Attribute = deleteAttributes };
                IResponse response = task.Invoke();

                DeleteAttributesResult result = (DeleteAttributesResult)response.Result;

                SelectTask selectTask = new SelectTask { DomainName = new Domain { Name = "event" }, SelectExpression = String.Format("select * from event where eventcreator = '{0}'", currentUserId) };
                IResponse selectResponse = selectTask.Invoke();

                SelectResult selectResult = (SelectResult)selectResponse.Result;

                List<AwsSdbModel.Attribute> attributes = new List<AwsSdbModel.Attribute>();

                WriteDebugXmlToOutputStream(selectResult.Item);
            }


            m_asyncResult.CompleteCall();
            return m_asyncResult;
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
