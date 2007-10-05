using System;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using Nuxleus.Atom;

namespace Nuxleus.Llup
{
      [XmlRootAttribute("notification", Namespace="http://www.x2x2x.org/llup", 
			IsNullable=false)]
    public class Notification
    {
      private static XmlSerializerNamespaces xmlns = null;

	[XmlElement (ElementName="recipient")]
	public string Recipient;

	[XmlAttribute ("action")]
	public string Action;

	[XmlElement (Type=typeof(DateTime), ElementName="published",
		     Namespace="http://www.w3.org/2005/Atom")]
	public DateTime? Published;

      	[XmlElement (Type=typeof(DateTime),ElementName="updated",
		     Namespace="http://www.w3.org/2005/Atom")]
	public DateTime? Updated;

      	[XmlElement (Type=typeof(DateTime),ElementName="expires")]
	public DateTime? Expires;

      [XmlElement (Type=typeof(Author), ElementName="author",
		     Namespace="http://www.w3.org/2005/Atom")]
	public Author[] Authors;

      [XmlElement (Type=typeof(Category), ElementName="category",
		     Namespace="http://www.w3.org/2005/Atom")]
	public Category[] Categories;
      
      [XmlElement (Type=typeof(Link), ElementName="link",
		     Namespace="http://www.w3.org/2005/Atom")]
	public Link[] Links;

      
      public static Notification Parse(string xml) {
	XmlReader reader = XmlReader.Create(new StringReader(xml));
	XmlSerializer serializer = new XmlSerializer(typeof(Notification));
	return (Notification)serializer.Deserialize(reader);
      }
      
      public static Notification Parse(Stream stream) {
	XmlSerializer serializer = new XmlSerializer(typeof(Notification));
	return (Notification)serializer.Deserialize(stream);
      }
      
      public override string ToString() {
	if(xmlns == null) {
	  xmlns = new XmlSerializerNamespaces();
	  xmlns.Add("llup", "http://www.x2x2x.org/llup");
	  xmlns.Add(String.Empty, "http://www.w3.org/2005/Atom");
	}
	
	StringBuilder sb = new StringBuilder();
	StringWriter writer = new StringWriter(sb);
	XmlSerializer serializer = new XmlSerializer(typeof(Notification));
	serializer.Serialize(writer, this, xmlns);
	return sb.ToString();
      }
    }
}