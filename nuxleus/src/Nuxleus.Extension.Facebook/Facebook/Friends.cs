
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Nuxleus.Extension.Facebook
{
    [XmlRootAttribute("friends_get_response", Namespace = "http://api.facebook.com/1.0/", IsNullable = false)]
    public class FriendsRoster
    {
        private static XmlSerializerNamespaces xmlns = null;

        [XmlElement(ElementName = "uid", DataType = "int")]
        public int[] Uids;

        public static FriendsRoster Parse(string xml)
        {
            XmlReader reader = XmlReader.Create(new StringReader(xml));
            XmlSerializer serializer = new XmlSerializer(typeof(FriendsRoster));
            return (FriendsRoster)serializer.Deserialize(reader);
        }

        public static FriendsRoster Parse(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(FriendsRoster));
            return (FriendsRoster)serializer.Deserialize(stream);
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
            XmlSerializer serializer = new XmlSerializer(typeof(FriendsRoster));
            serializer.Serialize(writer, this);
            return sb.ToString();
        }
    }

}