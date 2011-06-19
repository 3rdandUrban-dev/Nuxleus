using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Nuxleus.Core;

namespace Nuxleus.Extension.Aws.SimpleDb
{

    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/", IsNullable = false)]
    public class CreateDomainResult : IResult, IXmlSerializable
    {

        [XmlElementAttribute(ElementName = "ResponseMetadata")]
        public SdbResponseMetadata Metadata {
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
