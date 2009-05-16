using System.Xml.Serialization;
using System;
using Nuxleus.Core;

namespace Nuxleus.Extension.Aws.SimpleDb
{

    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/", IsNullable = false)]
    public class DomainMetadataResult : IResult, IXmlSerializable
    {
        [XmlElementAttribute(ElementName = "ResponseMetadata")]
        public SdbResponseMetadata Metadata {
            get;
            set;
        }

        [XmlElementAttribute(ElementName = "AttributeNameCount")]
        public String AttributeNameCount {
            get;
            set;
        }

        [XmlElementAttribute(ElementName = "AttributeNamesSizeBytes")]
        public String AttributeNamesSizeBytes {
            get;
            set;
        }

        [XmlElementAttribute(ElementName = "AttributeValueCount")]
        public String AttributeValueCount {
            get;
            set;
        }

        [XmlElementAttribute(ElementName = "AttributeValuesSizeBytes")]
        public String AttributeValuesSizeBytes {
            get;
            set;
        }

        [XmlElementAttribute(ElementName = "Timestamp")]
        public String Timestamp {
            get;
            set;
        }

        public System.Xml.Linq.XElement ToXElement() {
            throw new NotImplementedException();
        }

        public string ToXmlString() {
            throw new NotImplementedException();
        }


        public System.Xml.Schema.XmlSchema GetSchema() {
            throw new NotImplementedException();
        }

        public void ReadXml(System.Xml.XmlReader reader) {
            throw new NotImplementedException();
        }

        public void WriteXml(System.Xml.XmlWriter writer) {
            throw new NotImplementedException();
        }
    }
}
