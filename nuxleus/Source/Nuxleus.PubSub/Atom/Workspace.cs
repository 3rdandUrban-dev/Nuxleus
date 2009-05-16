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

namespace Nuxleus.Atom {
    public class Workspace {
        [XmlElement(ElementName="title", Namespace="http://www.w3.org/2005/Atom",
             IsNullable=false)]
        public string Title;

        [XmlElement(ElementName="collection")]
        public Collection[] Collections;
    }
}