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
    public class NuxleusHttpSessionLogoutHandler : IHttpHandler
    {

        HttpRequest m_request;
        HttpResponse m_response;
        HttpCookieCollection m_cookieCollection;
        bool m_logoutSuccessful = false;
        string m_returnLocation = "http://dev.amp.fm/";
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
            m_request = context.Request;
            m_response = context.Response;
            m_cookieCollection = context.Request.Cookies;
            m_asyncResult = new NuxleusAsyncResult(cb, extraData);


            if (m_cookieCollection.Count > 0)
            {
                try
                {
                    m_cookieCollection.Remove("guid");
                    m_cookieCollection.Remove("openid");
                    m_logoutSuccessful = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return new Message("redirect", ResponseType.RETURN_LOCATION).WriteResponseMessage(XmlWriter.Create(m_response.Output), m_asyncResult);
        }

        public void EndProcessRequest (IAsyncResult result)
        {
            
        }
    }
}