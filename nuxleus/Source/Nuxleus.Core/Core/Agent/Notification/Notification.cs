using System;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
//using Nuxleus.Atom;

namespace Nuxleus.Agent
{
    [XmlRootAttribute("notification", Namespace = "http://www.x2x2x.org/llup", IsNullable = false)]
    public class Notification
    {
        private static XmlSerializerNamespaces xmlns = null;
        private static XmlWriterSettings settings = null;

        [XmlElement(ElementName = "id")]
        public string Id;

        [XmlElement(ElementName = "recipient")]
        public string Recipient;

        [XmlAttribute("action")]
        public string Action;

        [XmlElement(Type = typeof(DateTime), ElementName = "published",
                 Namespace = "http://www.w3.org/2005/Atom")]
        public DateTime? Published;

        [XmlElement(Type = typeof(DateTime), ElementName = "updated",
             Namespace = "http://www.w3.org/2005/Atom")]
        public DateTime? Updated;

        [XmlElement(Type = typeof(DateTime), ElementName = "expires")]
        public DateTime? Expires;

        //[XmlElement(Type = typeof(Author), ElementName = "author",
        //       Namespace = "http://www.w3.org/2005/Atom")]
        //public Author[] Authors;

        //[XmlElement(Type = typeof(Category), ElementName = "category",
        //       Namespace = "http://www.w3.org/2005/Atom")]
        //public Category[] Categories;

        //[XmlElement(Type = typeof(Link), ElementName = "link",
        //       Namespace = "http://www.w3.org/2005/Atom")]
        //public Link[] Links;

        [XmlElement(ElementName = "point", Namespace = "http://www.georss.org/georss")]
        public string Coordinates;

        public static Notification Parse (string xml)
        {
            XmlReader reader = XmlReader.Create(new StringReader(xml));
            XmlSerializer serializer = new XmlSerializer(typeof(Notification));
            return (Notification)serializer.Deserialize(reader);
        }

        public static Notification Parse (Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Notification));
            return (Notification)serializer.Deserialize(stream);
        }

        public static Notification Parse (byte[] bytes)
        {
            MemoryStream stream = new MemoryStream(bytes);
            XmlSerializer serializer = new XmlSerializer(typeof(Notification));
            Notification m = (Notification)serializer.Deserialize(stream);
            stream.Close();
            return m;
        }

        public override string ToString ()
        {
            if (xmlns == null)
            {
                xmlns = new XmlSerializerNamespaces();
                xmlns.Add("llup", "http://www.x2x2x.org/llup");
                xmlns.Add("georss", "http://www.georss.org/georss");
                xmlns.Add(String.Empty, "http://www.w3.org/2005/Atom");
            }

            if (settings == null)
            {
                settings = new XmlWriterSettings();
                settings.Indent = false;
                settings.OmitXmlDeclaration = true;
            }

            StringBuilder sb = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(sb, settings);
            XmlSerializer serializer = new XmlSerializer(typeof(Notification));
            serializer.Serialize(writer, this, xmlns);
            return sb.ToString();
        }

        public static byte[] Serialize (Notification notification)
        {
            return Encoding.UTF8.GetBytes(notification.ToString());
        }

        public static byte[] Serialize (Notification notification, Encoding encoding)
        {
            return encoding.GetBytes(notification.ToString());
        }
    }
}