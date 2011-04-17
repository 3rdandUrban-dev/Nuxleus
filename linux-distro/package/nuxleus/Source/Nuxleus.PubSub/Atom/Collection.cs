//
// collection.cs: 
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

    public class Collection {
        [XmlElement(ElementName="title", Namespace="http://www.w3.org/2005/Atom",
             IsNullable=false)]
        public string Title;

        [XmlAttribute("href")]
        public string Href;

        [XmlElement(ElementName="accept")]
        public Accept[] Accepts;

        [XmlElement(ElementName="categories")]
        public Nuxleus.Atom.Categories[] Categories;
    }
}