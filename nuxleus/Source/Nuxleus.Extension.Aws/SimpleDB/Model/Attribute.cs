using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Nuxleus.Extension.Aws.SimpleDb.Model {

    [XmlTypeAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/")]
    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/", IsNullable = false)]
    public struct Attribute {

        string m_name;
        string m_value;
        static bool m_replace = false;

        [XmlElementAttribute(ElementName = "Name")]
        public string Name { get { return m_name; } set { m_name = value; } }

        [XmlElementAttribute(ElementName = "Value")]
        public string Value { get { return m_value; } set { m_value = value; } }

        [XmlElementAttribute(ElementName = "Replace")]
        public bool Replace { get { return m_replace; } set { m_replace = value; } }

        public Attribute(string name, string value) {
            m_name = name;
            m_value = value;
        }

        public Attribute(string name, string value, bool replace) {
            m_name = name;
            m_value = value;
            m_replace = replace;
        }
    }
}
