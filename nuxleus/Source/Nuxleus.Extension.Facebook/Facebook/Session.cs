
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Nuxleus.Extension.Facebook
{
    [XmlRootAttribute("auth_getSession_response", Namespace = "http://api.facebook.com/1.0/", IsNullable = false)]
    public class Session
    {
        private static XmlSerializerNamespaces xmlns = null;

        [XmlElement(ElementName = "session_key", IsNullable = false)]
        public string SessionKey;

        [XmlElement(ElementName = "uid", IsNullable = false, DataType = "int")]
        public int Uid;

        [XmlElement(ElementName = "expires")]
        public string Expires;

        public static Session Parse(string xml)
        {
            XmlReader reader = XmlReader.Create(new StringReader(xml));
            XmlSerializer serializer = new XmlSerializer(typeof(Session));
            return (Session)serializer.Deserialize(reader);
        }

        public static Session Parse(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Session));
            return (Session)serializer.Deserialize(stream);
        }

        public override string ToString()
        {
            if (xmlns == null)
            {
                xmlns = new XmlSerializerNamespaces();
                xmlns.Add(String.Empty, "http://api.facebook.com/1.0/");
            }
            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);
            XmlSerializer serializer = new XmlSerializer(typeof(Session));
            serializer.Serialize(writer, this);
            return sb.ToString();
        }
    }

}