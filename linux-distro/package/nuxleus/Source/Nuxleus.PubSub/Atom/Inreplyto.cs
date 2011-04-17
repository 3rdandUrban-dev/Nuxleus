//
// inreplyto.cs: 
//
// Author:
//   Sylvain Hellegouarch (sh@defuze.org)
//
// Copyright (C) 2007, 3rd&Urban, LLC
// 

using System;
using System.Xml;
using System.Xml.Serialization;

namespace Nuxleus.Atom {
    public class InReplyTo {
        [XmlAttribute("ref")]
        public string Ref;

        [XmlAttribute("href")]
        public string Href;

        [XmlAttribute("type")]
        public string Type;

        [XmlAttribute("source")]
        public string Source;
    }
}