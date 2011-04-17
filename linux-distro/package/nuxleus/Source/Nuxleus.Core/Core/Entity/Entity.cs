using System;
using System.Xml.Serialization;

namespace Nuxleus.Entity
{
    [Serializable]
    [XmlRootAttribute("Entity", Namespace = "http://nuxleus.com/entity",
          IsNullable = false)]
    public struct Entity : IEntity
    {
        [XmlElement(ElementName = "Term")]
        public string Term { get; set; }

        [XmlElement(ElementName = "Label")]
        public string Label { get; set; }

        [XmlElement(ElementName = "Scheme")]
        public string Scheme { get; set; }

    }
}

