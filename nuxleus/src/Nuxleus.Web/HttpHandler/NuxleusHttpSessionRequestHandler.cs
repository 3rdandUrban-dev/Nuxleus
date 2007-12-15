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

        LookupService m_lookupService = new LookupService("/Users/mdavid/Projects/Nuxleus/Public/Data/GeoLiteCity.dat", LookupService.GEOIP_MEMORY_CACHE);
        
        public void ProcessRequest(HttpContext context)
        {
            using(XmlWriter writer = XmlWriter.Create(context.Response.Output))
            {
                MemcachedClient client = (MemcachedClient)context.Application["memcached"];
                HttpCookieCollection collection = context.Request.Cookies;
                string guid = "not-set";
                string openid = "not-set";
                bool useMemcached = (bool)context.Application["usememcached"];
                string hostAddress = context.Request.UserHostAddress;
                IPLocation location;
                Dictionary<String, IPLocation> geoIP = (Dictionary<String, IPLocation>)context.Application["geoIPLookup"];
                
                if(!geoIP.ContainsKey(hostAddress))
                {
                    Location maxMindLocation = m_lookupService.getLocation(hostAddress);

                    if (maxMindLocation != null && !(maxMindLocation.city.Contains("(Unknown City?)")))
                    {
                        location.City = maxMindLocation.city;
                        location.Country = maxMindLocation.countryName;
                        location.CountryCode = maxMindLocation.countryCode;
                        location.Lat = maxMindLocation.latitude.ToString();
                        location.Long = maxMindLocation.longitude.ToString();
                    }
                    else if (useMemcached && client.KeyExists(hostAddress)) //TODO: This is an null value exception just waiting to happen.  Fix this!
                    {
                        location = new IPLocation(((String)client.Get(hostAddress)).Split(new char[] { ',' }));
                    }
                    else
                    {
                        location = new IPLocation(context.Request.UserHostAddress);
                        if (useMemcached)
                        {
                            client.Add(hostAddress, IPLocation.ToDelimitedString(",", location));
                        }
                    }
                }

                if (collection.Count > 0)
                {
                  try
                  {
                        guid = collection.Get("guid").Value;
                        openid = collection.Get("openid").Value;
                  }
                  catch(Exception e)
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
    }
}