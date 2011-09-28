using System;
namespace Nuxleus.Geo
{
    public interface ILatLongLocation
    {
        string City { get; set; }
        string Country { get; set; }
        string CountryCode { get; set; }
        string Lat { get; set; }
        string[] LocationArray { get; set; }
        string Long { get; set; }
        string Region { get; set; }
        string PostalCode { get; set; }
        string AreaCode { get; set; }
    }
}
