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
using System.Xml.Serialization;
using System.Xml.Xsl;
using Nuxleus.Atom;
using Nuxleus.Core;
using Nuxleus.Extension.Aws.SimpleDb;
using Sgml;
using AwsSdbModel = Nuxleus.Extension.Aws.SimpleDb;
using System.Xml;
using Nuxleus.Geo;
using Nuxleus.Geo.MaxMind;

namespace Nuxleus.Web.HttpHandler
{

    public class NuxleusHttpSessionManagementServiceOperationHandler : IHttpAsyncHandler
    {

        HttpRequest m_request;
        HttpResponse m_response;
        HttpCookieCollection m_cookieCollection;
        NuxleusAsyncResult m_asyncResult;
        static object m_lock = new object();
        static Encoding m_encoding = new UTF8Encoding();
        static XNamespace r = "http://nuxleus.com/message/response";
        static LookupService m_lookupService = new LookupService(HttpContext.Current.Request.MapPath("~/App_Data/GeoLiteCity.dat"), LookupService.GEOIP_MEMORY_CACHE);


        public void ProcessRequest(HttpContext context)
        {
            //not called
        }

        public bool IsReusable
        {
            get { return false; }
        }

        static NuxleusHttpSessionManagementServiceOperationHandler()
        {

        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {

            HttpRequest request = context.Request;

            m_request = context.Request;
            m_response = context.Response;
            m_cookieCollection = context.Request.Cookies;
            m_asyncResult = new NuxleusAsyncResult(cb, extraData);
            string ip = m_request.UserHostAddress.ToString();
            HttpCookie sessionid = m_cookieCollection["sessionid"];
            HttpCookie userid = m_cookieCollection["userid"];
            HttpCookie name = m_cookieCollection["name"];
            HttpCookie username = m_cookieCollection["username"];
            HttpCookie uservalidated = m_cookieCollection["uservalidated"];

            String hostAddress = context.Request.UserHostAddress;
            LatLongLocation location = new LatLongLocation(GetIPLocation(hostAddress));

            NameValueCollection form = request.Form;
            string user_id = null;
            string session_id = null;
            string session_name = null;
            string user_name = null;
            string user_validated = null;
            try
            {
                user_id = userid.Value;
            }
            catch
            {
                user_id = "not-set";
            }
            try
            {
                session_name = name.Value;
            }
            catch
            {
                session_name = "not-set";
            }
            try
            {
                session_id = sessionid.Value;
            }
            catch
            {
                session_id = "not-set";
            }
            try
            {
                user_name = username.Value;
            }
            catch
            {
                user_name = "not-set";
            }
            try
            {
                user_validated = uservalidated.Value;
            }
            catch
            {
                user_validated = "not-set";
            }
            if (ip == "::1")
                ip = "71.32.225.49";

            //IPLocation.DefaultCity = "Salt Lake City";
            //IPLocation.DefaultCountry = "UNITED STATES";

            //IPLocation location = new IPLocation(ip);

            //Select task = new Select { DomainName = "account", SelectExpression = String.Format("select name from account where itemName() = '{0}'", "0123456") };

            //IResponse response = task.Invoke();
            //TextReader tReader = new StringReader(response.Response);
            //XmlReader reader = XmlReader.Create(tReader);


            XDocument doc = new XDocument(
                new XElement(r + "message",
                    new XAttribute("id", Guid.NewGuid()),
                    new XAttribute("date", DateTime.Now),
                    new XAttribute("time", DateTime.Now.TimeOfDay),
                    new XElement(r + "session",
                        new XAttribute("id", session_id),
                        new XElement(r + "name", session_name),
                        new XElement(r + "username", user_name),
                        new XElement(r + "userid", user_id),
                        new XElement(r + "uservalidated", user_validated)
                    ),
                    new XElement(r + "geo",
                        new XElement(r + "lat", location.Lat),
                        new XElement(r + "long", location.Long),
                        new XElement(r + "city", location.City),
                        new XElement(r + "country", location.Country),
                        new XElement(r + "ip", ip)
                    )
                )
            );
            string xmlOutput = null;
            using (TextWriter writer = new StringWriter())
            {
                doc.Save(writer);
                xmlOutput = writer.ToString();
                m_response.Write(xmlOutput);
            }
            m_asyncResult.CompleteCall();
            return m_asyncResult;
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            m_response.ContentType = "text/xml";
        }

        private string[] GetIPLocation(String hostAddress)
        {

            string[] location;
            Location maxMindLocation = m_lookupService.getLocation(hostAddress);

            if (maxMindLocation != null)
            {

                location = new string[] { 
                    maxMindLocation.city,
                    maxMindLocation.countryName,
                    maxMindLocation.countryCode,
                    maxMindLocation.latitude.ToString(),
                    maxMindLocation.longitude.ToString()
                };

            }
            else
            {
                location = new string[] { 
                    "Unknown City",
                    "US",
                    "1",
                    "0",
                    "0"
              };
            }

            return location;
        }
    }
}
