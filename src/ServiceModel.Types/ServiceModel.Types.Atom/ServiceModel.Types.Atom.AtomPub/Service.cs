/*
// File: AtomPub.Service.cs:
// Author:
//  Sylvain Hellegouarch <sh@defuze.org>
//  M. David Peterson <m.david@3rdandUrban.com>
//
// Copyright � 2007-2011 3rd&Urban, LLC
//
// The code contained in this file is licensed under The MIT License
// Please see http://www.opensource.org/licenses/mit-license.php for specific detail.
*/
using System.Runtime.Serialization;

namespace Nuxleus.ServiceModel.Types.AtomPub
{
    [DataContract(Name = "service", Namespace = "http://www.w3.org/2007/app")]
    public partial class Service {
        [DataMember(Name = "workspace", IsRequired = true)]
        public Workspace[] Workspaces;
    }
}