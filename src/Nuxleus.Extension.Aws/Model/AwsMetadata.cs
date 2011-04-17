using System;
using System.Xml.Serialization;

namespace Nuxleus.Extension.Aws
{
    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/", IsNullable = false)]
    public class AwsResponseMetadata
    {
        [XmlElementAttribute(ElementName = "RequestId")]
        public String RequestId { get; set; }
    }
}