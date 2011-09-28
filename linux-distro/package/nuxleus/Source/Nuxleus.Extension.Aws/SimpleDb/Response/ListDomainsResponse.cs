using System;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Nuxleus.Core;

namespace Nuxleus.Extension.Aws.SimpleDb
{
    [XmlRootAttribute("ListDomainsResponse", Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/", IsNullable = false)]
    public class ListDomainsResponse : IResponse, IXmlSerializable
    {
        public WebHeaderCollection Headers {
            get;
            set;
        }

        [XmlElementAttribute(ElementName = "ListDomainsResult")]
        public IResult Result {
            get;
            set;
        }


        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema() {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader) {
            ListDomainsResult result = new ListDomainsResult();
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
