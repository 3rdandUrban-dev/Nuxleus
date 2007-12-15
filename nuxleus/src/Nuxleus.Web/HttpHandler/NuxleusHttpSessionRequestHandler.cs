using System;
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

        LookupService m_lookupService;

        public void ProcessRequest (HttpContext context)
        {
            using (XmlWriter writer = XmlWriter.Create(context.Response.Output))
            {
                MemcachedClient client = (MemcachedClient)context.Application["memcached"];
                HttpCookieCollection collection = context.Request.Cookies;
                string guid = "not-set";
                string openid = "not-set";
                bool useMemcached = (bool)context.Application["usememcached"];
                string hostAddress = context.Request.UserHostAddress;
                IPLocation location = new IPLocation();
                Dictionary<String, IPLocation> geoIP = (Dictionary<String, IPLocation>)context.Application["geoIPLookup"];

                if (!geoIP.ContainsKey(hostAddress))
                {
                    if (useMemcached && client != null)
                    {
                        if (client.KeyExists(hostAddress))
                        {
                            location = new IPLocation(((String)client.Get(hostAddress)).Split(new char[] { ',' }));
                        }
                        else
                        {
                            location = GetIPLocation(context.Request, hostAddress);
                            client.Add(hostAddress, IPLocation.ToDelimitedString(",", location));
                        }
                    }
                    else
                    {
                        location = GetIPLocation(context.Request, hostAddress);
                    }

                    geoIP.Add(hostAddress, location);

                }
                else
                {
                    location = geoIP[hostAddress];
                }

                if (collection.Count > 0)
                {
                    try
                    {
                        guid = collection.Get("guid").Value;
                        openid = collection.Get("openid").Value;
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

        private IPLocation GetIPLocation (HttpRequest request, String hostAddress)
        {
            IPLocation location = new IPLocation(hostAddress);

            if (location.City.Contains("Unknown City?"))
            {
                Location maxMindLocation;

                if (m_lookupService != null)
                {
                    maxMindLocation = m_lookupService.getLocation(hostAddress);
                }
                else
                {
                    m_lookupService = new LookupService(request.MapPath("/App_Data/GeoLiteCity.dat"), LookupService.GEOIP_MEMORY_CACHE);
                    maxMindLocation = m_lookupService.getLocation(hostAddress);
                }
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