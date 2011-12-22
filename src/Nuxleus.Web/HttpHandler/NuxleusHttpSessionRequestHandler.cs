using System;
using System.Web;
using System.Xml;
using Enyim.Caching;
using Enyim.Caching.Memcached;
using Nuxleus.Core;
using Nuxleus.Geo;
using Nuxleus.Geo.MaxMind;


namespace Nuxleus.Web.HttpHandler {

    public class NuxleusHttpSessionRequestHandler : IHttpAsyncHandler {

        static LookupService m_lookupService = new LookupService(HttpContext.Current.Request.MapPath("~/App_Data/GeoLiteCity.dat"), LookupService.GEOIP_MEMORY_CACHE);
        NuxleusAsyncResult m_asyncResult;

        public void ProcessRequest (HttpContext context) {
            //not used with IHttpAsyncHandler
        }

        public bool IsReusable {
            get { return false; }
        }

        public IAsyncResult BeginProcessRequest (HttpContext context, AsyncCallback cb, object extraData) {
            m_asyncResult = new NuxleusAsyncResult(cb, extraData);

            using (XmlWriter writer = XmlWriter.Create(context.Response.Output)) {
                bool useMemcached = (bool)context.Application["usememcached"];
                MemcachedClient client = (MemcachedClient)context.Application["memcached"];
                HttpCookieCollection cookieCollection = context.Request.Cookies;
                String hostAddress = context.Request.UserHostAddress;
                LatLongLocation location;
                String guid = "not-set";
                String openid = "not-set";

                if (hostAddress == "127.0.0.1") {
                    //hostAddress = Dns.GetHostAddresses(Dns.GetHostName()).GetValue(0).ToString();
                    hostAddress = "66.93.224.237";
                }

                if (context.Request.QueryString.Count > 0) {
                    if (context.Request.QueryString.Get("ip") != null) {
                        hostAddress = context.Request.QueryString.Get("ip");
                    }
                }

                if (useMemcached && client != null) {
                    object keyValue;
                    if (client.TryGet(hostAddress, out keyValue)) {
                        location = new LatLongLocation(((String)keyValue).Split(new char[] { '|' }));
                    } else {
                        location = new LatLongLocation(GetIPLocation(hostAddress));
                        client.Store(StoreMode.Add, hostAddress, LatLongLocation.ToDelimitedString("|", location));
                    }
                } else {
                    location = new LatLongLocation(GetIPLocation(hostAddress));
                }


                if (cookieCollection.Count > 0) {
                    try {
                        guid = cookieCollection.Get("guid").Value;
                        openid = cookieCollection.Get("openid").Value;
                    } catch (Exception e) {
                        this.LogError(e.Message);
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
                writer.WriteString(DateTime.Now.ToLongDateString());
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

                // Begin navigation section

                //IEnumerator pathEnumerator = context.Request.FilePath.Split(new char[] { '/' }).GetEnumerator();

                writer.WriteStartElement("navigation");
                writer.WriteStartElement("path");
                writer.WriteElementString("Home", "/");
                writer.WriteElementString("Profile", "./profile");
                //for (int i = 0; pathEnumerator.MoveNext(); i++)
                //{
                //    writer.WriteElementString("path", ((string)pathEnumerator.Current));
                //}
                writer.WriteEndElement();
                writer.WriteStartElement("member");
                writer.WriteElementString("blog", "./blog");
                writer.WriteElementString("event", "./event");
                writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            m_asyncResult.CompleteCall();
            return m_asyncResult;
        }

        public void EndProcessRequest (IAsyncResult result) {

        }

        private string[] GetIPLocation (String hostAddress) {

            string[] location;
            Location maxMindLocation = m_lookupService.getLocation(hostAddress);

            if (maxMindLocation != null) {

                location = new string[] { 
                    maxMindLocation.city,
                    maxMindLocation.countryName,
                    maxMindLocation.countryCode,
                    maxMindLocation.latitude.ToString(),
                    maxMindLocation.longitude.ToString()
                };

            } else {
                location = new string[] { 
                    "Unknown City",
                    "US",
                    "1",
                    "38",
                    "-97"
              };
            }

            return location;
        }
    }
}