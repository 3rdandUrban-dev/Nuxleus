//
// generator.cs: 
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
    public class Generator {
        [XmlAttribute("uri")]
        public string Uri;

        [XmlAttribute("version")]
        public string Version;

        [XmlText]
        public string Text;
    }
}