//
// categories.cs: 
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
    public class Categories {
        [XmlAttribute("fixed")]
        public string Fixed;

        [XmlElement(ElementName="category",
             Namespace="http://www.w3.org/2005/Atom")]
        public Category[] Category;
    }
}