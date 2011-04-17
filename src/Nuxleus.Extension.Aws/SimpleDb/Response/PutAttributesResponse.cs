using System.IO;
using System.Net;
using Nuxleus.Core;
using System.Xml.Serialization;
using System.Xml;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Nuxleus.Extension.Aws.SimpleDb {

    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/", IsNullable = false)]
    public class PutAttributesResponse : IResponse, IXmlSerializable
    {
        public WebHeaderCollection Headers { get; set; }

        [XmlElementAttribute(ElementName = "PutAttributesResult")]
        public IResult Result
        {
            get;
            set;
        }

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema() {
            throw new System.NotImplementedException();
        }

        public void ReadXml(XmlReader reader) {
            PutAttributesResult result = new PutAttributesResult();
            XElement elements = XElement.Load(reader);
            result.Metadata = Utility.GetSdbResponseMetadata(elements);
            Result = result;
        }

        public void WriteXml(XmlWriter writer) {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
