using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace Nuxleus.Extension.Aws.SimpleDb
{
    public class Item
    {
        [XmlElementAttribute(ElementName = "ItemName")]
        public String ItemName { get; set; }

        [XmlElementAttribute(ElementName = "Attribute")]
        public List<Attribute> Attribute { get; set; }
    }
}