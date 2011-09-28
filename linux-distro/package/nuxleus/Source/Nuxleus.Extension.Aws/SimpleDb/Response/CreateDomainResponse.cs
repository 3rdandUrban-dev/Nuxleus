using System;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Nuxleus.Core;

namespace Nuxleus.Extension.Aws.SimpleDb
{
    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/", IsNullable = false)]
    public class CreateDomainResponse : IResponse, IXmlSerializable
    {

        public WebHeaderCollection Headers {
            get;
            set;
        }

        [XmlElementAttribute(ElementName = "CreateDomainResult")]
        public IResult Result {
            get;
            set;
        }

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema() {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader) {
            CreateDomainResult result = new CreateDomainResult();
            XElement elements = XElement.Load(reader);
            result.Metadata = Utility.GetSdbResponseMetadata(elements);
            Result = result;
        }

        public void WriteXml(XmlWriter writer) {
            throw new NotImplementedException();
        }

        #endregion
    }
}
