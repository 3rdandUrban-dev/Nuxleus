using System;
using System.Xml.Serialization;

namespace Nuxleus.Extension.Aws.SimpleDb
{
    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/", IsNullable = false)]
    public class SdbResponseMetadata : AwsResponseMetadata
    {
        [XmlElementAttribute(ElementName = "BoxUsage")]
        public String BoxUsage { get; set; }
    }
}