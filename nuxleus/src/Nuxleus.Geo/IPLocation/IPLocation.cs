using System.Xml;
using System.Text;
using System;

namespace Nuxleus.Geo
{
    public struct IPLocation
    {
        string m_city;
        string m_country;
        string m_countryCode;
        string m_lat;
        string m_long;
        string[] m_locationArray;

        public string City
        {
            get { return m_city; }
            set { m_city = value; }
        }
        public string Country
        {
            get { return m_country; }
            set { m_country = value; }
        }
        public string CountryCode
        {
            get { return m_countryCode; }
            set { m_countryCode = value; }
        }
        public string Lat
        {
            get { return m_lat; }
            set { m_lat = value; }
        }
        public string Long
        {
            get { return m_long; }
            set { m_long = value; }
        }
        public string[] LocationArray
        {
            get { return m_locationArray; }
            set { m_locationArray = value; }
        }
        public IPLocation (string ip)
        {
            m_locationArray = Parse(ip);
            m_city = m_locationArray[0];
            m_country = m_locationArray[1];
            m_countryCode = m_locationArray[2];
            m_lat = m_locationArray[3];
            m_long = m_locationArray[4];
        }
        public IPLocation (string[] geoInfo)
        {
            m_city = geoInfo[0];
            m_country = geoInfo[1];
            m_countryCode = geoInfo[2];
            m_lat = geoInfo[3];
            m_long = geoInfo[4];
            m_locationArray = geoInfo;
        }

        public static string ToDelimitedString (string delimiter, IPLocation location)
        {
            return String.Join(delimiter, location.LocationArray);
        }

        public static string[] Parse (string ip)
        {
            string[] geoInfoArray = new string[5];

            XmlReader xGeoIPReader = XmlReader.Create("http://api.hostip.info/?ip=" + ip);
            while (xGeoIPReader.Read())
            {
                if (xGeoIPReader.IsStartElement())
                {
                    switch (xGeoIPReader.Name)
                    {
                        case "gml:name":
                            {
                                geoInfoArray[0] = xGeoIPReader.ReadString();
                                break;
                            }
                        case "countryName":
                            {
                                geoInfoArray[1] = xGeoIPReader.ReadString();
                                break;
                            }
                        case "countryAbbrev":
                            {
                                geoInfoArray[2] = xGeoIPReader.ReadString();
                                break;
                            }
                        case "gml:coordinates":
                            {
                                string[] latlon = xGeoIPReader.ReadString().Split(new char[] { ',' });

                                geoInfoArray[3] = latlon[1];
                                geoInfoArray[4] = latlon[0];
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
            return geoInfoArray;
        }
    }
}