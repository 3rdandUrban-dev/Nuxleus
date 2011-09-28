using System;
using System.Xml;
using System.Xml.Serialization;

namespace Nuxleus.PubSub.LLUP
{
    [XmlRootAttribute("relevance", Namespace = "http://www.llup.org/blip#", IsNullable = false)]
    public class Relevance
    {
        [XmlAttribute("rel")]
        public string Rel;

        [XmlAttribute("href", DataType = "anyURI", Type = typeof(Uri))]
        public string Href;
    }
}