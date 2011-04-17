using System.Net;
using System.Xml.Linq;
using System.Xml.Serialization;
using Nuxleus.Core;

namespace Nuxleus.Extension.Aws.SimpleDb
{
    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/", IsNullable = false)]
    public class DeleteDomainResponse : IResponse, IXmlSerializable
    {
        public WebHeaderCollection Headers {
            get;
            set;
        }

        [XmlElementAttribute(ElementName = "DeleteDomainResult")]
        public IResult Result {
            get;
            set;
        }

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema() {
            throw new System.NotImplementedException();
        }

        public void ReadXml(System.Xml.XmlReader reader) {
            DeleteDomainResult result = new DeleteDomainResult();
            XElement elements = XElement.Load(reader);
            result.Metadata = Utility.GetSdbResponseMetadata(elements);
            Result = result;
        }

        public void WriteXml(System.Xml.XmlWriter writer) {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
