/*
// File: Account.AccountInfo.cs:
// Author:
//  M. David Peterson <m.david@3rdandUrban.com>
//
// Copyright � 2007-2011 3rd&Urban, LLC
//
// The code contained in this file is licensed under The MIT License
// Please see http://www.opensource.org/licenses/mit-license.php for specific detail.
*/
using System;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using ServiceStack.ServiceInterface.ServiceModel;
using System.Collections.Generic;
using Nuxleus.ServiceModel.Operations;
using Nuxleus.ServiceModel.Types.Location;

namespace Nuxleus.ServiceModel.Types.Account
{


    [DataContract(Name = "user")]
    public class User
    {
        [DataMember(Name = "country", IsRequired = false)]
        public string Country { get; set; }

        [DataMember(Name = "locale", IsRequired = false)]
        public string Locale { get; set; }
    }

    [DataContract]
    public class Address
    {
        [DataMember]
        public string StreetAddress { get; set; }

        [DataMember]
        public string Locality { get; set; }

        [DataMember]
        public string Region { get; set; }

        [DataMember]
        public string CountryName { get; set; }

        [DataMember]
        public string PostalCode { get; set; }

        [DataMember]
        public int AreaCode { get; set; }

        [DataMember]
        public int MetroCode { get; set; }

        [DataMember]
        public int DMACode { get; set; }
    }

    [DataContract]
    public class GeoLocationInfo
    {
        [DataMember]
        public double Latitude { get; set; }

        [DataMember]
        public double Longitude { get; set; }

        [DataMember(Name = "ip")]
        public string IPAddress { get; set; }

        [DataMember(Name = "address")]
        public Address PhysicalAddress { get; set; }
    }

    [DataContract]
    public class SessionInfo
    {
        [DataMember]
        public double Latitude { get; set; }

        [DataMember]
        public double Longitude { get; set; }

        [DataMember(Name = "IP")]
        public string IPAddress { get; set; }

        [DataMember(Name = "Address")]
        public Address PhysicalAddress { get; set; }
    }

    [DataContract]
    public class AccountSettings
    {
        [DataMember(Name = "profilename")]
        public string ProfileName { get; set; }

        [DataMember(Name = "accounttype")]
        public string AccountType { get; set; }

        [DataMember(Name = "name", IsRequired = true)]
        public string DisplayName { get; set; }

        [DataMember(Name = "email", IsRequired = true)]
        public string PrimaryEmailAddress { get; set; }

        [DataMember(Name = "gender")]
        public string Gender { get; set; }

        [DataMember(Name = "birthday")]
        public DateTime Birthday { get; set; }

        [DataMember(Name = "location", IsRequired = true)]
        public Location.Location CurrentLocation { get; set; }

        [DataMember(Name = "defaultlocation", IsRequired = false)]
        public Location.Location DefaultLocation { get; set; }
    }

    [DataContract]
    public class AccountInfo
    {
        [DataMember(Name = "profilename")]
        public string ProfileName { get; set; }

        [DataMember(Name = "account")]
        public AccountSettings AccountSettings { get; set; }

        [DataMember(Name = "accountid")]
        public string InternalAccountId { get; set; }

    }

    [DataContract]
    public class FacebookAccountInfo : AccountInfo
    {
        [DataMember(Name = "user")]
        public User User { get; set; }

        [DataMember(Name = "user_id")]
        public long FacebookUserID { get; set; }
    }

    [DataContract]
    public class FacebookSessionInfo : FacebookAccountInfo
    {

        [DataMember(Name = "algorithm")]
        public HashAlgorithm HashAlgorithm { get; set; }

        [DataMember(Name = "expires")]
        public DateTime Expires { get; set; }

        [DataMember(Name = "issued_at")]
        public DateTime IssuedAt { get; set; }

        [DataMember(Name = "oauth_token")]
        public string OAuthToken { get; set; }
    }

    [DataContract]
    public class AccountResponse : IHasResponseStatus
    {
        [DataMember]
        public AccountInfo AccountInfo { get; set; }

        //Auto inject and serialize web service exceptions
        [DataMember]
        public ResponseStatus ResponseStatus { get; set; }
    }
}