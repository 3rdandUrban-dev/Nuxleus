// Copyright (c) 2006 by M. David Peterson
// The code contained in this file is licensed under a Creative Commons (Attribution 3.0) license
// Please see http://creativecommons.org/licenses/by/3.0/us/ for specific detail.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Nuxleus.Core;
using Nuxleus.Cryptography;
using Nuxleus.Extension.Aws.SimpleDb;
using System.Xml.Serialization;
using AwsSdbModel = Nuxleus.Extension.Aws.SimpleDb;
using System.Xml;

namespace Nuxleus.Web.HttpHandler
{

    public class NuxleusHttpMediaCollectionCreationServiceOperationHandler : IHttpAsyncHandler
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

        static NuxleusHttpMediaCollectionCreationServiceOperationHandler()
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
            m_returnLocation = form.Get("return-url");
            //string collection_id = form.Get("collection-id");
            string collection_name = form.Get("collection-name");
            string collection_id = Guid.NewGuid().ToString();
            string path = m_request.MapPath(String.Format("/member/{0}/media/{1}", userid.Value, collection_id));

            if (!Directory.Exists(path))
            {
                DirectoryInfo directory = Directory.CreateDirectory(path);
            }

            List<AwsSdbModel.Attribute> attributes = new List<AwsSdbModel.Attribute>();
            attributes.Add(new AwsSdbModel.Attribute { Name = "CollectionName", Value = collection_name });
            attributes.Add(new AwsSdbModel.Attribute { Name = "CollectionOwner", Value = userid.Value });

            PutAttributesTask task = new PutAttributesTask { 
                DomainName = new Domain {
                    Name = "collections" }, 
                    Item = new Item { 
                        ItemName = collection_id,
                        Attribute = attributes 
                    } 
            };
            IResponse response = task.Invoke();
            PutAttributesResult result = (PutAttributesResult)response.Result;
            foreach (string header in response.Headers)
            {
                m_response.Write(String.Format("ResponseHeader: {0}: {1}", header, response.Headers[header]));
            }
            //m_response.Write(result.Metadata.RequestId);
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
