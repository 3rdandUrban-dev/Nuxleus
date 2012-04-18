/*
// File: AtomPub.Workspace.cs:
// Author:
//  Sylvain Hellegouarch <sh@defuze.org>
//  M. David Peterson <m.david@3rdandUrban.com>
//
// Copyright � 2007-2011 3rd&Urban, LLC
//
// The code contained in this file is licensed under The MIT License
// Please see http://www.opensource.org/licenses/mit-license.php for specific detail.
*/
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace Nuxleus.ServiceModel.Types.AtomPub
{
    [DataContract(Name = "workspace", Namespace = "http://www.w3.org/2007/app")]
    public partial class Workspace
    {
        [DataMember(Name = "title", IsRequired = true)]
        public string Title;

        [DataMember(Name = "collection")]
        public Collection[] Collections;
    }
}