using System.Xml.Linq;
using Nuxleus.Core;


namespace Nuxleus.Extension.Aws.SimpleDb
{

    public static class Utility
    {
        static readonly string m_sdbNamespace = "http://sdb.amazonaws.com/doc/2007-11-07/";

        static readonly XName m_responseMetadata = XName.Get("ResponseMetadata", m_sdbNamespace);
        static readonly XName m_boxUsage = XName.Get("BoxUsage", m_sdbNamespace);
        static readonly XName m_requestId = XName.Get("RequestId", m_sdbNamespace);

        public static SdbResponseMetadata GetSdbResponseMetadata(XElement sourceElement) {
            XElement metaData = sourceElement.Element(m_responseMetadata);
            return new SdbResponseMetadata {
                BoxUsage = metaData.Element(m_boxUsage).Value,
                RequestId = metaData.Element(m_requestId).Value
            };
        }
    }
}
