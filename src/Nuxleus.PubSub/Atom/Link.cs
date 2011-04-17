//
// link.cs: 
//
// Author:
//   Sylvain Hellegouarch (sh@defuze.org)
//
// Copyright (C) 2007, 3rd&Urban, LLC
// 

using System;
using System.Xml;
using System.Xml.Serialization;

namespace Nuxleus.Atom
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