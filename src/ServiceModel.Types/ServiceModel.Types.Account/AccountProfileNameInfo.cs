/*
// File: Account.ProfileNameInfo.cs:
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

namespace Nuxleus.ServiceModel.Types.Account
{
    public class AccountProfileNameInfo
    {
        public string ProfileName { get; set; }

        public bool IsAvailable { get; set; }
    }

    public class AccountProfileNameStatusResponse : IHasResponseStatus
    {
        public AccountProfileNameInfo ProfileNameInfo { get; set; }

        //Auto inject and serialize web service exceptions
        public ResponseStatus ResponseStatus { get; set; }
    }
}