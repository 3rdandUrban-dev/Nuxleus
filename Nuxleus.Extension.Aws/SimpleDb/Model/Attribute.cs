using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Nuxleus.Extension.Aws.SimpleDb
{

    [XmlTypeAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/")]
    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/", IsNullable = true)]
    public class Attribute
    {
        [XmlElementAttribute(ElementName = "Name")]
        public string Name { get; set; }

        [XmlElementAttribute(ElementName = "Value")]
        public string Value { get; set; }

        [XmlElementAttribute(ElementName = "Replace", IsNullable = true)]
        public bool? Replace { get; set; }
    }
}
