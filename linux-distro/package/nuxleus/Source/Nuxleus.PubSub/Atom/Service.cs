//
// service.cs: 
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
    [XmlRootAttribute("service", Namespace="http://www.w3.org/2007/app", IsNullable=false)]
    public class Service {
        [XmlElement(ElementName="workspace")]
        public Workspace[] Workspaces;
    }
}