using System.Xml;
using System.Text;
using System;

namespace Nuxleus.Geo
{
    public class LatLongByCityName : ILatLongLocation
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
        public string Region
        {
            get;
            set;
        }
        public string PostalCode
        {
            get;
            set;
        }
        public string AreaCode
        {
            get;
            set;
        }
        public string[] LocationArray
        {
            get { return m_locationArray; }
            set { m_locationArray = value; }
        }
        public LatLongByCityName (string name)
        {
            m_locationArray = Parse(name);
            m_city = m_locationArray[0];
            m_country = m_locationArray[1];
            m_countryCode = m_locationArray[2];
            m_lat = m_locationArray[3];
            m_long = m_locationArray[4];
            Region = m_locationArray[5];
            PostalCode = m_locationArray[6];
            AreaCode = m_locationArray[7];
        }
        public LatLongByCityName (string[] geoInfo)
        {
            m_city = geoInfo[0];
            m_country = geoInfo[1];
            m_countryCode = geoInfo[2];
            m_lat = geoInfo[3];
            m_long = geoInfo[4];
            Region = geoInfo[5];
            PostalCode = geoInfo[6];
            AreaCode = geoInfo[7];
            m_locationArray = geoInfo;
        }

        public static string ToDelimitedString (string delimiter, LatLongByCityName location)
        {
            return String.Join(delimiter, location.LocationArray);
        }

        public static string[] Parse (string name)
        {
            string[] geoInfoArray = new string[5];
            int maxRows = 1;

            XmlReader xGeoIPReader = XmlReader.Create(String.Format("http://ws.geonames.org/search?name={0}&maxRows={1}", name, maxRows));
            while (xGeoIPReader.Read())
            {
                if (xGeoIPReader.IsStartElement())
                {
                    switch (xGeoIPReader.Name)
                    {
                        case "name":
                            {
                                geoInfoArray[0] = xGeoIPReader.ReadString();
                                break;
                            }
                        case "countryName":
                            {
                                geoInfoArray[1] = xGeoIPReader.ReadString();
                                break;
                            }
                        case "countryCode":
                            {
                                geoInfoArray[2] = xGeoIPReader.ReadString();
                                break;
                            }
                        case "lat":
                            {
                                geoInfoArray[3] = xGeoIPReader.ReadString();
                                break;
                            }
                        case "lng":
                            {
                                geoInfoArray[4] = xGeoIPReader.ReadString();
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