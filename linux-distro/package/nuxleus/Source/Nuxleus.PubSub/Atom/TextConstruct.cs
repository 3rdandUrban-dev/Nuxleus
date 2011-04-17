//
// textconstruct.cs: 
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
    public class TextConstruct {
        [XmlAttribute("type")]
        public string Type = "text";

        [XmlAttribute("lang", Form=System.Xml.Schema.XmlSchemaForm.Qualified,
              Namespace="http://www.w3.org/XML/1998/namespace")]
        public string Lang;

        [XmlText]
        public string Text;

        [XmlElement(ElementName="div", Type=typeof(XHTMLBody),
            Namespace="http://www.w3.org/1999/xhtml")]
        public XHTMLBody Div;
    }

    public class XHTMLBody {
        [XmlAnyElement]
        public XmlNode[] Elements;
    }
}