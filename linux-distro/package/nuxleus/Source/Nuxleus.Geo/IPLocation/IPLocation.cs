using System;
using System.Xml;

namespace Nuxleus.Geo
{
    public class IPLocation
    {
        string m_city;
        string m_country;

        public static string DefaultCity { get; set; }
        public static string DefaultCountry { get; set; }

        public string City
        {
            get { return (m_city.ToLower().Contains("unknown city")) ? DefaultCity : m_city; }
            set { m_city = value; }
        }
        public string Country
        {
            get { return (m_country.ToLower().Contains("unknown country")) ? DefaultCountry : m_country; }
            set { m_country = value; }
        }
        public string CountryCode { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public string[] LocationArray { get; set; }

        public IPLocation(string ip)
        {
            LocationArray = Parse(ip);
            City = LocationArray[0];
            Country = LocationArray[1];
            CountryCode = LocationArray[2];
            Lat = LocationArray[3];
            Long = LocationArray[4];
        }
        public IPLocation(string[] geoInfo)
        {
            City = geoInfo[0];
            Country= geoInfo[1];
            CountryCode = geoInfo[2];
            Lat = geoInfo[3];
            Long = geoInfo[4];
            LocationArray = geoInfo;
        }

        public static string ToDelimitedString(string delimiter, IPLocation location)
        {
            return String.Join(delimiter, location.LocationArray);
        }

        public static string[] Parse(string ip)
        {
            string[] geoInfoArray = new string[5];

            XmlReader xGeoIPReader = XmlReader.Create(String.Format("http://api.hostip.info/?ip=", ip));
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