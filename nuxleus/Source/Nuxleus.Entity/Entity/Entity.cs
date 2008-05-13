using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Nuxleus.Entity {

    [Serializable]
    [XmlRootAttribute("Entity", Namespace="http://nuxleus.com/entity",
          IsNullable=false)]
    public struct Entity : IEntity {
        string m_term;
        string m_label;
        string m_scheme;
        static string m_DEFAULTSCHEME = "http://nuxleus.com/entity";

        public Entity ( string term )
            : this(term, term, m_DEFAULTSCHEME) {
        }

        public Entity ( string term, string label )
            : this(term, label, m_DEFAULTSCHEME) {
        }

        public Entity ( string term, string label, string scheme ) {
            m_term = term;
            m_label = label;
            m_scheme = scheme;
        }
        [XmlElement(ElementName="Term")]
        public string Term { get { return m_term; } set { m_term = value; } }

        [XmlElement(ElementName="Label")]
        public string Label { get { return m_label; } set { m_label = value; } }

        [XmlElement(ElementName="Scheme")]
        public string Scheme { get { return m_scheme; } set { m_scheme = value; } }

    }
}

