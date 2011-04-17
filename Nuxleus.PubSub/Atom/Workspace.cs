//
// workspace.cs: 
//
// Author:
//   Sylvain Hellegouarch (sh@defuze.org)
//
// Copyright (C) 2007, 3rd&Urban, LLC
// 

using System;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace Nuxleus.Atom {
    [DataContract(Name = "service", Namespace = "http://www.w3.org/2007/app")]
    public class Workspace {
        [XmlElement(ElementName="title", Namespace="http://www.w3.org/2005/Atom",
             IsNullable=false)]
        public string Title;

        [XmlElement(ElementName="collection")]
        public Collection[] Collections;
    }
}