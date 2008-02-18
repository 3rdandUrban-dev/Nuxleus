using System;
using System.IO;
using System.Data;
using System.Configuration;
using System.Threading;
using System.Security.Principal;
using System.Web;
using System.Web.Caching;
using System.Web.Security;
using System.Web.SessionState;
using System.Collections;
using Memcached.ClientLibrary;
using System.Text;
using Saxon.Api;
using System.Xml;
using Nuxleus.Configuration;
using Nuxleus.Transform;
using System.Collections.Generic;
using Nuxleus.Memcached;
using Nuxleus.Cryptography;
using Nuxleus.Atom;
using Nuxleus.Storage;
using Nuxleus.Geo;
using Nuxleus.Async;
using System.Net;

namespace Nuxleus.Web.HttpHandler
{
    public struct NuxleusHttpAsyncRequestValidationHandler : IHttpAsyncHandler
    {
        NuxleusAsyncResult m_asyncResult;
        
        public void ProcessRequest(HttpContext context)
        {
            
        }

        public bool IsReusable
        {
            get { return false; }
        }

        #region IHttpAsyncHandler Members

        public IAsyncResult BeginProcessRequest (HttpContext context, AsyncCallback cb, object extraData)
        {
            m_asyncResult = new NuxleusAsyncResult(cb, extraData);
            IPLocation location = new IPLocation(context.Request.UserHostAddress);
            HttpCookieCollection collection = context.Request.Cookies;

            string guid = "not-set";
            string openid = "not-set";

            if (collection.Count > 0)
            {
                if (collection.Get("guid").Value != null)
                    guid = collection.Get("guid").Value;
            }

            using(XmlWriter writer = XmlWriter.Create(context.Response.Output))
            {
                DateTime now = DateTime.Now;

                writer.WriteStartDocument();
                    writer.WriteStartElement("message", "http://nuxleus.com/message/response");
                        writer.WriteStartElement("session");
                            writer.WriteStartAttribute("session-id");
                                writer.WriteString(guid);
                            writer.WriteEndAttribute();
                            //writer.WriteStartAttribute("openid");
                            //    writer.WriteString(openid);
                            //writer.WriteEndAttribute();
                        writer.WriteEndElement();
                        writer.WriteStartElement("request-date");
                            writer.WriteString(now.ToShortDateString());
                        writer.WriteEndElement();
                        writer.WriteStartElement("request-time");
                            writer.WriteString(now.ToLongTimeString());
                        writer.WriteEndElement();
                        writer.WriteStartElement("request-guid");
                            writer.WriteString(Guid.NewGuid().ToString());
                        writer.WriteEndElement();
                        writer.WriteStartElement("geo");
                            writer.WriteStartElement("ip");
                                writer.WriteString(context.Request.UserHostAddress);
                            writer.WriteEndElement();
                            writer.WriteStartElement("city");
                                writer.WriteString(location.City);
                            writer.WriteEndElement();
                            writer.WriteStartElement("country");
                                writer.WriteString(location.Country);
                            writer.WriteEndElement();
                            writer.WriteStartElement("lat");
                                writer.WriteString(location.Lat);
                            writer.WriteEndElement();
                            writer.WriteStartElement("long");
                                writer.WriteString(location.Long);
                            writer.WriteEndElement();
                        writer.WriteEndElement();
                    writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            m_asyncResult.CompleteCall();

            return m_asyncResult;
        }

        public void EndProcessRequest (IAsyncResult result)
        {
            
        }

        #endregion
    }
}