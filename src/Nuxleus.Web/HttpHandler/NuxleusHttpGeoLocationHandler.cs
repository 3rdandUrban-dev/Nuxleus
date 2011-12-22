using System;
using System.Net;
using System.Web;
using System.Xml;
using Nuxleus.Geo;
using System.Collections.Generic;
using Enyim.Caching;
using System.IO;
using System.Collections.Specialized;
using Nuxleus.Core;


namespace Nuxleus.Web.HttpHandler
{
    public class NuxleusHttpGeoLocationHandler : IHttpAsyncHandler
    {
        NuxleusAsyncResult m_asyncResult;

        public void ProcessRequest(HttpContext context)
        {

        }

        public bool IsReusable
        {
            get { return false; }
        }

        #region IHttpAsyncHandler Members

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            m_asyncResult = new NuxleusAsyncResult(cb, extraData);

            using (XmlWriter writer = XmlWriter.Create(context.Response.Output))
            {
                bool useMemcached = (bool)context.Application["usememcached"];
                MemcachedClient client = (MemcachedClient)context.Application["memcached"];
                NameValueCollection queryString = context.Request.QueryString;
                LatLongByCityName location;
                string cityName = queryString["name"];


                if (useMemcached && client != null)
                {
                    object keyValue;
                    if (client.TryGet(cityName, out keyValue))
                    {
                        location = new LatLongByCityName(((String)keyValue).Split(new char[] { ',' }));
                    }
                    else
                    {
                        location = new LatLongByCityName(cityName);
                        //client.Add(cityName, LatLongByCityName.ToDelimitedString(",", location));
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

            return m_asyncResult;
        }

        public void EndProcessRequest(IAsyncResult result)
        {

        }

        #endregion
    }
}