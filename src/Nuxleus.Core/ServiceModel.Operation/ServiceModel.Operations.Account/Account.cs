/*
// File: Account.cs:
// Author:
//  M. David Peterson <m.david@3rdandUrban.com>
//
// Copyright � 2007-2011 3rd&Urban, LLC
//
// The code contained in this file is licensed under The MIT License
// Please see http://www.opensource.org/licenses/mit-license.php for specific detail.
*/
using System.ComponentModel;
using System.Runtime.Serialization;
using Nuxleus.ServiceModel.Types;
using ServiceStack.ServiceHost;
using Nuxleus.ServiceModel.Types.Account;

namespace Nuxleus.ServiceModel.Operations
{
    [Description("GET the Account info at {ProfileName}\n"
           + "POST multipart/formdata to create a new account using the {ProfileName} as the user chosen identifier.\n"
           + "PUT {ProfileName} to update the fields of an existing Account based on {ProfileName} as the account identifier.\n")]
    [RestService("/account/{ProfileName}")]
    public class Account
    {
        public string ProfileName { get; set; }

        public AccountSettings AccountSettings { get; set; }

    }
}