//
// entry.cs: 
//
// Author:
//   Sylvain Hellegouarch (sh@defuze.org)
//
// Copyright (C) 2007, Sylvain Hellegouarch
// 

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Nuxleus.Atom
{
  [XmlRootAttribute("entry", Namespace="http://www.w3.org/2005/Atom", IsNullable=false)]
    public class Entry
    {
      private static XmlSerializerNamespaces xmlns = null;

      [XmlAttribute("lang", Form=System.Xml.Schema.XmlSchemaForm.Qualified, 
		    Namespace="http://www.w3.org/XML/1998/namespace")]
	public string Lang;

      [XmlAttribute("base", Form=System.Xml.Schema.XmlSchemaForm.Qualified, 
		    Namespace="http://www.w3.org/XML/1998/namespace")]
	public string Base;

      [XmlElement (ElementName="id", IsNullable=false)]
	public string Id;

      	[XmlElement (ElementName="title", IsNullable=false)]
	public string Title;

      	[XmlElement (ElementName="icon")]
	public string Icon;
      
      	[XmlElement (ElementName="logo")]
	public string Logo;

      [XmlElement (Type=typeof(DateTime), ElementName="published")]
	public DateTime? Published;

      	[XmlElement (Type=typeof(DateTime),ElementName="updated")]
	public DateTime? Updated = DateTime.UtcNow;

      [XmlElement (Type=typeof(DateTime), ElementName="edited", 
		   Namespace = "http://www.w3.org/2007/app")]
      public DateTime? Edited;

      [XmlElement (Type=typeof(Author), ElementName="author")]
	public Author[] Authors;

      [XmlElement (Type=typeof(Contributor), ElementName="contributor")]
	public Contributor[] Contributors;
      
      [XmlElement (Type=typeof(Category), ElementName="category")]
	public Category[] Categories;
      
      [XmlElement (Type=typeof(Link), ElementName="link")]
	public Link[] Links;
      
      [XmlElement (Type=typeof(Nuxleus.Atom.Summary), ElementName="summary")]
	public Nuxleus.Atom.Summary Summary;

      [XmlElement (Type=typeof(Nuxleus.Atom.Rights), ElementName="rights")]
	public Nuxleus.Atom.Rights Rights;

      [XmlElement (Type=typeof(Nuxleus.Atom.Content), ElementName="content")]
      public Nuxleus.Atom.Content Content;

      [XmlElement (DataType="ulong", ElementName="total", 
		   Namespace="http://purl.org/syndication/thread/1.0")]
      public ulong? TotalResponses;
      
      [XmlElement (Type=typeof(Nuxleus.Atom.InReplyTo), ElementName="in-reply-to", 
		   Namespace="http://purl.org/syndication/thread/1.0")]
      public Nuxleus.Atom.InReplyTo InReplyTo;

      
      public static Entry Parse(string xml) {
	XmlReader reader = XmlReader.Create(new StringReader(xml));
	XmlSerializer serializer = new XmlSerializer(typeof(Entry));
	return (Entry)serializer.Deserialize(reader);
      }
      
      public static Entry Parse(Stream stream) {
	XmlSerializer serializer = new XmlSerializer(typeof(Entry));
	return (Entry)serializer.Deserialize(stream);
      }
      
      public override string ToString() {
	if(xmlns == null) {
	  xmlns = new XmlSerializerNamespaces();
	  xmlns.Add(String.Empty, "http://www.w3.org/2005/Atom");
	  xmlns.Add("app", "http://www.w3.org/2007/app");
	  xmlns.Add("thr", "http://purl.org/syndication/thread/1.0");
	}
	StringBuilder sb = new StringBuilder();
	StringWriter writer = new StringWriter(sb);
	XmlSerializer serializer = new XmlSerializer(typeof(Entry));
	serializer.Serialize(writer, this);
	return sb.ToString();
      }
    }

}