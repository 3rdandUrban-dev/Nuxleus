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

namespace Nuxleus.Web.HttpHandler
{

    public class NuxleusHttpEventCreationServiceOperationHandler : IHttpAsyncHandler
    {
        HttpRequest m_request;
        HttpResponse m_response;
        static Encoding m_encoding = new UTF8Encoding();
        HttpCookieCollection m_cookieCollection;
        NuxleusAsyncResult m_asyncResult;
        string m_returnLocation;
        static XNamespace r = "http://nuxleus.com/message/response";


        public void ProcessRequest(HttpContext context)
        {
            //not called
        }

        public bool IsReusable
        {
            get { return false; }
        }

        static NuxleusHttpEventCreationServiceOperationHandler()
        {

        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            m_request = context.Request;
            m_response = context.Response;
            m_cookieCollection = context.Request.Cookies;
            m_asyncResult = new NuxleusAsyncResult(cb, extraData);
            HttpCookie sessionid = m_cookieCollection["sessionid"];
            HttpCookie userid = m_cookieCollection["userid"];
            HttpCookie username = m_cookieCollection["username"];
            HttpCookie name = m_cookieCollection["name"];
            HttpCookie uservalidated = m_cookieCollection["uservalidated"];

            string ip = m_request.UserHostAddress.ToString();

            NameValueCollection form = m_request.Form;
            m_returnLocation = form.Get("return_url");
            string eventname = form.Get("name");
            string description = form.Get("description");
            //string location = form.Get("location");
            string venue = form.Get("venue");
            string genre = form.Get("genre");
            string startdate = form.Get("startdate");
            string starttime = form.Get("starttime");
            string endtime = form.Get("endtime");
            string tags = form.Get("tags");
            string eventid = Guid.NewGuid().ToString();

            List<AwsSdbModel.Attribute> attributes = new List<AwsSdbModel.Attribute>();
            attributes.Add(new AwsSdbModel.Attribute { Name = "name", Value = eventname });
            attributes.Add(new AwsSdbModel.Attribute { Name = "description", Value = description });
            attributes.Add(new AwsSdbModel.Attribute { Name = "venue", Value = venue });
            attributes.Add(new AwsSdbModel.Attribute { Name = "genre", Value = genre });
            attributes.Add(new AwsSdbModel.Attribute { Name = "startdate", Value = startdate });
            attributes.Add(new AwsSdbModel.Attribute { Name = "starttime", Value = starttime });
            attributes.Add(new AwsSdbModel.Attribute { Name = "endtime", Value = endtime });
            attributes.Add(new AwsSdbModel.Attribute { Name = "tags", Value = tags });
            attributes.Add(new AwsSdbModel.Attribute { Name = "eventid", Value = eventid });
            attributes.Add(new AwsSdbModel.Attribute { Name = "eventcreator", Value = userid.Value });

            PutAttributesTask task = new PutAttributesTask { DomainName = new Domain { Name = "event" }, Item = new Item { ItemName = eventid, Attribute = attributes } };
            IResponse response = task.Invoke();

            PutAttributesResult result = (PutAttributesResult)response.Result;
            WriteDebugXmlToOutputStream(attributes);
            m_asyncResult.CompleteCall();
            return m_asyncResult;
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            m_response.Redirect(m_returnLocation);
        }

        void WriteDebugXmlToOutputStream(List<AwsSdbModel.Attribute> attributes)
        {
            XDocument doc = new XDocument(
                new XElement(r + "message",
                    new XElement(r + "event",
                         GenerateXElementArray(attributes)
                    )
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

        IEnumerable<XElement> GenerateXElementArray(List<AwsSdbModel.Attribute> attributes)
        {
            foreach (AwsSdbModel.Attribute attribute in attributes)
            {
                yield return new XElement(r + attribute.Name, attribute.Value);
            }
        }
    }
}
