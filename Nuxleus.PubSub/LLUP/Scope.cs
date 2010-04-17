using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Nuxleus.PubSub.LLUP
{
    [XmlRootAttribute("scope", Namespace = "http://www.llup.org/blip#", IsNullable = false)]
    public class Scope
    {
        [XmlAttribute(AttributeName = "start", DataType = "dateTime", Type = typeof(DateTime))]
        public DateTime Start;

        [XmlAttribute(AttributeName = "publish", DataType = "dateTime", Type = typeof(DateTime))]
        public DateTime Publish;

        [XmlAttribute(AttributeName = "expire", DataType = "dateTime", Type = typeof(DateTime))]
        public DateTime? Expire;

        [XmlAttribute(AttributeName = "archive", DataType = "dateTime", Type = typeof(DateTime))]
        public DateTime? Archive;

        [XmlElementAttribute(Type = typeof(Relevance), ElementName = "link")]
        public List<Relevance> Relevance;
    }
}