using System;
using System.Net;
using System.Web;
using System.Xml;
using Nuxleus.Geo;
using Nuxleus.Async;
using System.Collections.Generic;
using Memcached.ClientLibrary;
using System.IO;
using Nuxleus.Geo.MaxMind;


namespace Nuxleus.Web.HttpHandler
{
    public struct NuxleusHttpSessionLogoutHandler : IHttpAsyncHandler
    {

        HttpRequest m_request;
        HttpResponse m_response;
        HttpCookieCollection m_cookieCollection;
        bool m_logoutSuccessful;
        string m_returnLocation;
        NuxleusAsyncResult m_asyncResult;

        public void ProcessRequest (HttpContext context)
        {

        }

        public bool IsReusable
        {
            get { return false; }
        }

        public IAsyncResult BeginProcessRequest (HttpContext context, AsyncCallback cb, object extraData)
        {
            m_logoutSuccessful = false;
            m_returnLocation = "http://dev.amp.fm/";
            m_request = context.Request;
            m_response = context.Response;
            m_cookieCollection = context.Response.Cookies;
            m_asyncResult = new NuxleusAsyncResult(cb, extraData);

            try
            {
                HttpCookie guid = m_cookieCollection["guid"];
                HttpCookie openid = m_cookieCollection["openid"];
                DateTime expires = DateTime.Now.AddDays(-1);

                guid.Expires = expires;
                openid.Expires = expires;

                m_response.Cookies.Add(guid);
                m_response.Cookies.Add(openid);

                m_logoutSuccessful = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


            Message responseMessage = new Message("redirect", ResponseType.RETURN_LOCATION);
            responseMessage.WriteResponseMessage(XmlWriter.Create(m_response.Output), null, m_asyncResult);
            return m_asyncResult;
        }

        public void EndProcessRequest (IAsyncResult result)
        {
            m_response.ContentType = "text/xml";
        }
    }
}