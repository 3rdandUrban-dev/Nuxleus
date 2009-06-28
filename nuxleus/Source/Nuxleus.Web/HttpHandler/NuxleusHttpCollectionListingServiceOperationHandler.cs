// Copyright (c) 2006 by M. David Peterson
// The code contained in this file is licensed under a Creative Commons (Attribution 3.0) license
// Please see http://creativecommons.org/licenses/by/3.0/us/ for specific detail.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;
using Nuxleus.Core;
using Nuxleus.Extension.Aws.SimpleDb;
using AwsSdbModel = Nuxleus.Extension.Aws.SimpleDb;
using System.Net;

namespace Nuxleus.Web.HttpHandler
{

    public class NuxleusHttpCollectionListingServiceOperationHandler : IHttpAsyncHandler
    {
        HttpRequest m_request;
        HttpResponse m_response;
        static Encoding m_encoding = new UTF8Encoding();
        HttpCookieCollection m_cookieCollection;
        NuxleusAsyncResult m_asyncResult;
        static XNamespace r = "http://nuxleus.com/message/response";
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

        static NuxleusHttpCollectionListingServiceOperationHandler()
        {

        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {

            m_request = context.Request;
            m_response = context.Response;
            m_cookieCollection = context.Request.Cookies;
            m_asyncResult = new NuxleusAsyncResult(cb, extraData);
            String sessionid = m_cookieCollection["sessionid"].Value;
            String userid = m_cookieCollection["userid"].Value;
            String username = m_cookieCollection["username"].Value;
            String name = m_cookieCollection["name"].Value;
            //String uservalidated = m_cookieCollection["uservalidated"].Value;

            string ip = m_request.UserHostAddress.ToString();

            m_task = new SelectTask { DomainName = new Domain { Name = "collections" }, SelectExpression = String.Format("select * from collections where CollectionOwner = '{0}'", userid) };
            m_iTaskResult = new NuxleusAsyncResult(cb, extraData);

            m_task.Transaction.OnSuccessfulTransaction += new OnSuccessfulTransaction(Transaction_OnSuccessfulTransaction);
            m_task.Transaction.OnFailedTransaction += new OnFailedTransaction(Transaction_OnFailedTransaction);
            return m_task.BeginInvoke(m_iTaskResult);
        }

        public void EndProcessRequest(IAsyncResult result)
        {

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

        void WriteDebugXmlToOutputStream(List<Item> items)
        {
            XDocument doc = new XDocument(
                new XElement(r + "message",
                         GenerateXElementItems(items)
                )
            );
            m_response.ContentType = "text/xml";
            doc.Save(m_response.Output);
        }

        IEnumerable<XElement> GenerateXElementItems(List<Item> items)
        {
            foreach (Item item in items)
            {
                yield return new XElement(r + "MediaCollection",
                    new XElement(r + "ItemName", item.ItemName),
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
