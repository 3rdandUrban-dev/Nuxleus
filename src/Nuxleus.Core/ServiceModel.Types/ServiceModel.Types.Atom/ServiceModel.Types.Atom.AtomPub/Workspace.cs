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

namespace Nuxleus.ServiceModel.Types.AtomPub
{
    public partial class Workspace
    {
        [XmlElement(ElementName = "title", Namespace = "http://www.w3.org/2005/Atom",
             IsNullable = false)]
        public string Title;

        [XmlElement(ElementName = "collection")]
        public Collection[] Collections;
    }
}