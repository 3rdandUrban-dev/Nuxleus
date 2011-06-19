/*
// File: AtomSyndication.TextConstruct.cs:
// Author:
//  Sylvain Hellegouarch <sh@defuze.org>
//  M. David Peterson <m.david@3rdandUrban.com>
//
// Copyright � 2007-2011 3rd&Urban, LLC
//
// The code contained in this file is licensed under The MIT License
// Please see http://www.opensource.org/licenses/mit-license.php for specific detail.
*/

using System.Xml;
using System.Xml.Serialization;

namespace Nuxleus.ServiceModel.Types.Atom
{
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