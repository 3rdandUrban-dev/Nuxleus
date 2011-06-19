/*
// File: Account.ProfileNameStatus.cs:
// Author:
//  M. David Peterson <m.david@3rdandUrban.com>
//
// Copyright © 2007-2011 3rd&Urban, LLC
//
// The code contained in this file is licensed under The MIT License
// Please see http://www.opensource.org/licenses/mit-license.php for specific detail.
*/
using System.ComponentModel;
using System.Runtime.Serialization;
using Nuxleus.ServiceModel.Types;
using ServiceStack.ServiceHost;

namespace Nuxleus.ServiceModel.Operations
{

    [Description("GET the availability status of {ProfileName}\n")]
    [RestService("/profilename/{ProfileName}", "GET")]
    [DataContract]
    public class AccountProfileNameStatus
    {
        [DataMember]
        public string ProfileName { get; set; }

    }
}