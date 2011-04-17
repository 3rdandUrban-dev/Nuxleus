// Copyright (c) 2006 by M. David Peterson
// The code contained in this file is licensed under a Creative Commons (Attribution 3.0) license
// Please see http://creativecommons.org/licenses/by/3.0/us/ for specific detail.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Nuxleus.Core;
using Nuxleus.Geo;
using Nuxleus.Geo.MaxMind;

namespace Nuxleus.Web.HttpHandler
{

    public class NuxleusHttpSessionManagementServiceOperationHandler : IHttpAsyncHandler
    {

        HttpRequest m_request;
        HttpResponse m_response;
        static Dictionary<string, string> m_cookieDict;
        HttpCookieCollection m_cookieCollection;
        NuxleusAsyncResult m_asyncResult;
        static object m_lock = new object();
        static Encoding m_encoding = new UTF8Encoding();
        static XNamespace r = "http://nuxleus.com/message/response";
        static LookupService m_lookupService = new LookupService(HttpContext.Current.Request.MapPath("~/App_Data/GeoLiteCity.dat"), LookupService.GEOIP_STANDARD);


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
            m_cookieDict = new Dictionary<string,string>();
            m_cookieDict.Add("sessionid", "not-set");
            m_cookieDict.Add("userid", "not-set");
            m_cookieDict.Add("name", "not-set");
            m_cookieDict.Add("username", "not-set");
            m_cookieDict.Add("uservalidated", "not-set");
            m_cookieDict.Add("location:country", "not-set");
            m_cookieDict.Add("location:region", "not-set");
            m_cookieDict.Add("location:city", "not-set");
            m_cookieDict.Add("address:street", "not-set");
            m_cookieDict.Add("address:region", "not-set");
            m_cookieDict.Add("address:city", "not-set");
            m_cookieDict.Add("address:postalCode", "not-set");
        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {

            HttpRequest request = context.Request;
            Dictionary<string, string> cookies = new Dictionary<string, string>(m_cookieDict);
            m_request = context.Request;
            m_response = context.Response;
            m_cookieCollection = context.Request.Cookies;
            m_asyncResult = new NuxleusAsyncResult(cb, extraData);
            string ip = m_request.UserHostAddress;

            for (var i = 0; i < m_cookieCollection.Count; i++ )
            {
                cookies[m_cookieCollection[i].Name] = m_cookieCollection[i].Value;
            }


            if (ip == "::1" || ip == "127.0.0.1")
                //ip = GetLocalIPAddress();
                ip = "99.89.54.217";

            LatLongLocation location = new LatLongLocation(GetIPLocation(ip));

            NameValueCollection form = request.Form;

            
            //IPLocation.DefaultCity = "Salt Lake City";
            //IPLocation.DefaultCountry = "UNITED STATES";

            //IPLocation location = new IPLocation(ip);

            //Select task = new Select { DomainName = "account", SelectExpression = String.Format("select name from account where itemName() = '{0}'", "0123456") };

            //IResponse response = task.Invoke();
            //TextReader tReader = new StringReader(response.Response);
            //XmlReader reader = XmlReader.Create(tReader);

            XDocument doc = new XDocument(
		new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement(r + "message",
                    new XAttribute("id", Guid.NewGuid()),
                    new XAttribute("date", DateTime.Now.Date),
                    new XAttribute("time", DateTime.Now.TimeOfDay),
                    new XElement(r + "session",
                        new XAttribute("id", cookies["sessionid"]),
                        new XElement(r + "name", cookies["name"]),
                        new XElement(r + "username", cookies["username"]),
                        new XElement(r + "userid", cookies["userid"]),
                        new XElement(r + "uservalidated", cookies["uservalidated"]),
                        new XElement(r + "preferences",
                            new XElement(r + "location",
                                new XElement(r + "country", (cookies["location:country"] == "not-set") ? location.Country : cookies["location:country"]),
                                new XElement(r + "region", (cookies["location:region"] == "not-set") ? location.Region : cookies["location:region"]),
                                new XElement(r + "city", (cookies["location:city"] == "not-set") ? location.City : cookies["location:city"])
                            ),
                            new XElement(r + "address",
                                new XElement(r + "street", (cookies["address:street"] == "not-set") ? location.Country : cookies["address:street"]),
                                new XElement(r + "region", (cookies["address:region"] == "not-set") ? location.Region : cookies["address:region"]),
                                new XElement(r + "city", (cookies["address:city"] == "not-set") ? location.City : cookies["address:city"]),
                                new XElement(r + "postalCode", (cookies["address:postalCode"] == "not-set") ? location.PostalCode : cookies["address:postalCode"])
                            )
                        )
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
	    XmlWriterSettings settings = new XmlWriterSettings();
	    settings.Encoding = Encoding.UTF8;
	    
            using (XmlWriter writer = XmlWriter.Create(m_response.Output, settings))
            {
		doc.Save(writer);
                writer.Flush();
            }
            m_asyncResult.CompleteCall();
            return m_asyncResult;
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            m_response.ContentType = "text/xml";
        }

        //private vCard GetAccountInfo(string accountID)
        //{

        //}

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
