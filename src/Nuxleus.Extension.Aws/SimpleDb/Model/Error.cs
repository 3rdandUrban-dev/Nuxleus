using System;
using System.Xml.Serialization;

namespace Nuxleus.Extension.Aws.SimpleDb
{
    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/", IsNullable = false)]
    public class Error
    {
        [XmlElementAttribute(ElementName = "Type")]
        public String Type { get; set; }

        [XmlElementAttribute(ElementName = "Code")]
        public String Code { get; set; }

        [XmlElementAttribute(ElementName = "Message")]
        public String Message { get; set; }

        [XmlElementAttribute(ElementName = "Detail")]
        public Object Detail { get; set; }
    }
}