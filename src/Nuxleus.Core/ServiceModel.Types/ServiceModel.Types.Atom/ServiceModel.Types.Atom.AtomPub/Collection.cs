/*
// File: AtomPub.Collection.cs:
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
    [DataContract(Name = "collection", Namespace = "http://www.w3.org/2005/Atom")]
    public partial class Collection
    {
        [DataMember(Name = "title", IsRequired = true )]
        public string Title;

        [DataMember(Name = "href")]
        public string Href;

        [DataMember(Name = "accept")]
        public Accept[] Accepts;

        [DataMember(Name = "categories")]
        public Categories[] Categories;
    }

}