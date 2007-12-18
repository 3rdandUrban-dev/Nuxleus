using System;
using System.Net;
using System.Web;
using System.Xml;
using Nuxleus.Geo;
using System.Collections.Generic;
using Memcached.ClientLibrary;
using System.IO;
using Nuxleus.Geo.MaxMind;


namespace Nuxleus.Web.HttpHandler
{
    public class NuxleusHttpSessionLogoutHandler : IHttpHandler
    {

        static String guid = "not-set";
        static String openid = "not-set";

        public void ProcessRequest (HttpContext context)
        {
            using (XmlWriter writer = XmlWriter.Create(context.Response.Output))
            {

                HttpCookieCollection cookieCollection = context.Request.Cookies;

                if (cookieCollection.Count > 0)
                {
                    try
                    {
                        cookieCollection.Remove("guid");
                        cookieCollection.Remove("openid");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                writer.WriteStartDocument();
                writer.WriteProcessingInstruction("xml-stylesheet", "type='text/xsl' href='/service/transform/openid-redirect.xsl'");
                writer.WriteStartElement("message", "http://nuxleus.com/message/response");
                writer.WriteStartElement("session");
                writer.WriteStartAttribute("session-id");
                writer.WriteString(guid);
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("openid");
                writer.WriteString(openid);
                writer.WriteEndAttribute();
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}