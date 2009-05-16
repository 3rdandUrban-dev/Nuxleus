//
// personconstruct.cs: 
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

    public class PersonConstruct {
        [XmlElement("name")]
        public string Name;

        [XmlElement("email")]
        public string Email;

        [XmlElement("uri")]
        public string Uri;
    }
}