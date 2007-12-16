using System;
using System.Net;
using System.Web;
using System.Xml;
using Nuxleus.Geo;
using System.Collections.Generic;
using Memcached.ClientLibrary;
using System.IO;
using System.Collections.Specialized;


namespace Nuxleus.Web.HttpHandler
{
    public class NuxleusHttpGeoLocationHandler : IHttpHandler
    {

        public void ProcessRequest (HttpContext context)
        {
            using (XmlWriter writer = XmlWriter.Create(context.Response.Output))
            {
                bool useMemcached = (bool)context.Application["usememcached"];
                MemcachedClient client = (MemcachedClient)context.Application["memcached"];
                NameValueCollection queryString = context.Request.QueryString;
                LatLongByCityName location;
                string cityName = queryString["name"];


                if (useMemcached && client != null)
                {
                    if (client.KeyExists(cityName))
                    {
                        location = new LatLongByCityName(((String)client.Get(cityName)).Split(new char[] { ',' }));
                    }
                    else
                    {
                        location = new LatLongByCityName(cityName);
                        client.Add(cityName, LatLongByCityName.ToDelimitedString(",", location));
                    }
                }
                else
                {
                    location = new LatLongByCityName(cityName);
                }

                writer.WriteStartDocument();
                writer.WriteStartElement("message", "http://nuxleus.com/message/response");
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