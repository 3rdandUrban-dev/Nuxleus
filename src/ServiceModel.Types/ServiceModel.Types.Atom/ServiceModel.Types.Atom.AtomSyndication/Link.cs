/*
// File: AtomSyndication.Link.cs:
// Author:
//  Sylvain Hellegouarch <sh@defuze.org>
//  M. David Peterson <m.david@3rdandUrban.com>
//
// Copyright � 2007-2011 3rd&Urban, LLC
//
// The code contained in this file is licensed under The MIT License
// Please see http://www.opensource.org/licenses/mit-license.php for specific detail.
*/

using System;
using System.Xml;
using System.Xml.Serialization;

namespace Nuxleus.ServiceModel.Types.Atom
{
    public class Link
    {
        [XmlAttribute("rel")]
        public string Rel;

        [XmlAttribute("href")]
        public string Href;

        [XmlAttribute("type")]
        public string Type;

        [XmlAttribute("hreflang")]
        public string HrefLang;

        [XmlAttribute("title")]
        public string Title;

        [XmlAttribute("length", DataType = "int")]
        public int Length;

        [XmlAttribute("count", DataType = "int",
               Namespace = "http://purl.org/syndication/thread/1.0")]
        public int ThreadCount;

        [XmlAttribute("updated",
               Namespace = "http://purl.org/syndication/thread/1.0")]
        public DateTime ThreadUpdated;
    }
}