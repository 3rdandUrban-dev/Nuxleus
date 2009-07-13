// Copyright (c) 2006 by M. David Peterson
// The code contained in this file is licensed under a Creative Commons (Attribution 3.0) license
// Please see http://creativecommons.org/licenses/by/3.0/us/ for specific detail.

using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Nuxleus.Core;
using Nuxleus.Geo;
using Nuxleus.Geo.MaxMind;
using System.Net;

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
            string ip = m_request.UserHostAddress;
            HttpCookie sessionid = m_cookieCollection["sessionid"];
            HttpCookie userid = m_cookieCollection["userid"];
            HttpCookie name = m_cookieCollection["name"];
            HttpCookie username = m_cookieCollection["username"];
            HttpCookie uservalidated = m_cookieCollection["uservalidated"];

            if (ip == "::1" || ip == "127.0.0.1")
                //ip = GetLocalIPAddress();
                ip = "75.169.248.106";

            LatLongLocation location = new LatLongLocation(GetIPLocation(ip));

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
                        new XElement(r + "region", location.Region),
                        new XElement(r + "postalCode", location.PostalCode),
                        new XElement(r + "areaCode", location.AreaCode),
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
                    maxMindLocation.longitude.ToString(),
                    maxMindLocation.region,
                    maxMindLocation.postalCode,
                    maxMindLocation.area_code.ToString()
                };

            }
            else
            {
                location = new string[] { 
                    "Unknown City",
                    "US",
                    "1",
                    "0",
                    "0",
                    "Unknown Region",
                    "Unknown Postal Code",
                    "Unknown Area Code"
              };
            }

            return location;
        }
        private string GetLocalIPAddress()
        {
            IPHostEntry host;
            string localIP = String.Empty;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }
    }


}
