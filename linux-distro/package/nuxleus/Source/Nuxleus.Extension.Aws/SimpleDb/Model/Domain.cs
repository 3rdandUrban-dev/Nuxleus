using System;
using System.Xml.Serialization;

namespace Nuxleus.Extension.Aws.SimpleDb
{
    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/")]
    public class Domain
    {
        [XmlElementAttribute(ElementName = "Name")]
        public String Name { get; set; }
    }
}
