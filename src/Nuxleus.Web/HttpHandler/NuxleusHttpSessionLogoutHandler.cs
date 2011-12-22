using System;
using System.Net;
using System.Web;
using System.Xml;
using Nuxleus.Geo;
using Nuxleus.Core;
using System.Collections.Generic;
using Enyim.Caching;
using System.IO;
using Nuxleus.Geo.MaxMind;


namespace Nuxleus.Web.HttpHandler
{
    public class NuxleusHttpSessionLogoutHandler : IHttpAsyncHandler
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
                HttpCookie sessionid = m_cookieCollection["sessionid"];
                HttpCookie userid = m_cookieCollection["userid"];
                HttpCookie username = m_cookieCollection["username"];
                HttpCookie name = m_cookieCollection["name"];
                HttpCookie uservalidated = m_cookieCollection["uservalidated"];
                DateTime expires = DateTime.Now.AddDays(-1);

                sessionid.Expires = expires;
                userid.Expires = expires;
                username.Expires = expires;
                uservalidated.Expires = expires;
                name.Expires = expires;

                m_response.Cookies.Add(sessionid);
                m_response.Cookies.Add(userid);
                m_response.Cookies.Add(username);
                m_response.Cookies.Add(name);
                m_response.Cookies.Add(uservalidated);

                m_logoutSuccessful = true;
            }
            catch (Exception e)
            {
                this.LogError(e.Message);
            }


            //Message responseMessage = new Message("redirect", ResponseType.REDIRECT);
            //responseMessage.WriteResponseMessage(XmlWriter.Create(m_response.Output), null, m_asyncResult);
            m_asyncResult.CompleteCall();
            return m_asyncResult;
        }

        public void EndProcessRequest (IAsyncResult result)
        {
            m_response.Redirect("/");
        }
    }
}