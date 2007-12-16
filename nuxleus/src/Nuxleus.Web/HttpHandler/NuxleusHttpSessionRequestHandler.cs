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
    public class NuxleusHttpSessionRequestHandler : IHttpHandler
    {

        static LookupService m_lookupService = new LookupService(HttpContext.Current.Request.MapPath("/App_Data/GeoLiteCity.dat"), LookupService.GEOIP_MEMORY_CACHE);
        static String guid = "not-set";
        static String openid = "not-set";

        public void ProcessRequest (HttpContext context)
        {
            using (XmlWriter writer = XmlWriter.Create(context.Response.Output))
            {
                bool useMemcached = (bool)context.Application["usememcached"];
                MemcachedClient client = (MemcachedClient)context.Application["memcached"];
                HttpCookieCollection cookieCollection = context.Request.Cookies;
                String hostAddress = context.Request.UserHostAddress;
                IPLocation location = new IPLocation();
                //Dictionary<String, IPLocation> geoIP = (Dictionary<String, IPLocation>)context.Application["geoIPLookup"];

                if(hostAddress == "127.0.0.1")
                {
                    hostAddress = Dns.GetHostAddresses(Dns.GetHostName()).GetValue(0).ToString();
                }

                if (context.Request.QueryString.Count > 0)
                {
                    if (context.Request.QueryString.Get("ip") != null)
                    {
                        hostAddress = context.Request.QueryString.Get("ip");
                    }
                }

                //Console.WriteLine(hostAddress);
                //Console.WriteLine(context.Request.QueryString.Get("ip"));

                //hostAddress = "71.199.4.128";

                    if (useMemcached && client != null)
                    {
                        if (client.KeyExists(hostAddress))
                        {
                            location = new IPLocation(((String)client.Get(hostAddress)).Split(new char[] { '|' }));
                        }
                        else
                        {
                            location = GetIPLocation(hostAddress);
                            client.Add(hostAddress, IPLocation.ToDelimitedString("|", location));
                        }
                    }
                    else
                    {
                        location = GetIPLocation(hostAddress);
                    }


                if (cookieCollection.Count > 0)
                {
                    try
                    {
                        guid = cookieCollection.Get("guid").Value;
                        openid = cookieCollection.Get("openid").Value;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                writer.WriteStartDocument();
                writer.WriteStartElement("message", "http://nuxleus.com/message/response");
                writer.WriteStartElement("session");
                writer.WriteStartAttribute("session-id");
                writer.WriteString(guid);
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("openid");
                writer.WriteString(openid);
                writer.WriteEndAttribute();
                writer.WriteEndElement();
                writer.WriteStartElement("request-date");
                writer.WriteString(DateTime.Now.ToShortDateString());
                writer.WriteEndElement();
                writer.WriteStartElement("request-guid");
                writer.WriteString(Guid.NewGuid().ToString());
                writer.WriteEndElement();
                writer.WriteStartElement("geo");
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
        }

        public bool IsReusable
        {
            get { return false; }
        }

        private IPLocation GetIPLocation (String hostAddress)
        {
            IPLocation location = new IPLocation(hostAddress);

            if (location.City.Contains("Unknown"))
            {
                Location maxMindLocation = m_lookupService.getLocation(hostAddress);

                try
                {
                    location.City = maxMindLocation.city;
                    location.Country = maxMindLocation.countryName;
                    location.CountryCode = maxMindLocation.countryCode;
                    location.Lat = maxMindLocation.latitude.ToString();
                    location.Long = maxMindLocation.longitude.ToString();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                return location;
            }
            else
            {
                return location;
            }
        }
    }
}