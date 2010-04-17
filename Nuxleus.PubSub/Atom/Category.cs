//
// category.cs: 
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

    public class Category {
        [XmlAttribute("term")]
        public string Term;

        [XmlAttribute("scheme")]
        public string Scheme;

        [XmlAttribute("label")]
        public string Label;
    }
}