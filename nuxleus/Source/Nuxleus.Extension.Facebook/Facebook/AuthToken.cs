
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Nuxleus.Extension.Facebook
{
    [XmlRootAttribute("auth_createToken_response", Namespace = "http://api.facebook.com/1.0/", IsNullable = false)]
    public class AuthToken
    {
        private static XmlSerializerNamespaces xmlns = null;

        [XmlText]
        public string Value;

        public static AuthToken Parse(string xml)
        {
            XmlReader reader = XmlReader.Create(new StringReader(xml));
            XmlSerializer serializer = new XmlSerializer(typeof(AuthToken));
            return (AuthToken)serializer.Deserialize(reader);
        }

        public static AuthToken Parse(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(AuthToken));
            return (AuthToken)serializer.Deserialize(stream);
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
            XmlSerializer serializer = new XmlSerializer(typeof(AuthToken));
            serializer.Serialize(writer, this);
            return sb.ToString();
        }
    }

}