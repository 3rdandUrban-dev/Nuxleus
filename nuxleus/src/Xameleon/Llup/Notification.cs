using System;
using System.Collections;
using System.Xml;
using System.IO;
using System.Text;

namespace Xameleon.Llup {
  public class Notification {
    private static string LLUP_NS = "http://www.x2x2x.org/llup";
    private static string ATOM_NS = "http://www.w3.org/2005/Atom";
    

    private string recipient = String.Empty;
    private DateTime expires = DateTime.MinValue;
    private DateTime published = DateTime.MinValue;
    private DateTime updated = DateTime.MinValue;
    private string action = String.Empty;
    private IList authors = new ArrayList();
    private IList categories = new ArrayList();
    private IList links = new ArrayList();

    public Notification() {}

    public string Recipient { get { return recipient; } set {recipient = value; }}
    public string Action { get { return action; } set {action = value; }}
    public DateTime Published { get { return published; } set { published = value; }}
    public DateTime Updated { get { return updated; } set { updated = value; }}
    public DateTime Expires { get { return expires; } set { expires = value; }}
    public IList Authors { get { return authors; }}
    public IList Categories { get { return categories; }}
    public IList Links { get { return links; }}

    public static Notification Parse(string xml) {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(xml);

      XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
      nsmgr.AddNamespace("atom", ATOM_NS);
      nsmgr.AddNamespace("llup", LLUP_NS);

      Notification n = new Notification();

      XmlElement root = doc.DocumentElement;

      n.Action = root.GetAttribute("action");
      XmlNode node = null;
      XmlNodeList nodes = null;

      node = root.SelectSingleNode("/llup:notification/llup:recipient", nsmgr);
      if(node != null) {
	n.Recipient = ((XmlElement)node).GetAttribute("href");
      }

      node = root.SelectSingleNode("/llup:notification/atom:published", nsmgr);
      if(node != null) {
	n.Published = DateTime.Parse(node.InnerText);
      }

      node = root.SelectSingleNode("/llup:notification/atom:updated", nsmgr);
      if(node != null) {
	n.Updated = DateTime.Parse(node.InnerText);
      }

      node = root.SelectSingleNode("/llup:notification/llup:expires", nsmgr);
      if(node != null) {
	n.Expires = DateTime.Parse(node.InnerText);
      }

      nodes = root.SelectNodes("/llup:notification/atom:link", nsmgr);
      if(nodes != null) {
	foreach(XmlElement nd in nodes) {
	  Link l = new Link(nd.GetAttribute("href"),
			    nd.GetAttribute("rel"), nd.GetAttribute("type"));
	  n.Links.Add(l);
	}
      }
  
      nodes = root.SelectNodes("/llup:notification/atom:author", nsmgr);
      if(nodes != null) {
	foreach(XmlElement nd in nodes) {
	  Author a = new Author();
	  node = nd.SelectSingleNode("./atom:name", nsmgr);
	  if(node != null) {
	    a.Name = node.InnerText;
	  }
	  node = nd.SelectSingleNode("./atom:uri", nsmgr);
	  if(node != null) {
	    a.Uri = node.InnerText;
	  }
	  node = nd.SelectSingleNode("./atom:email", nsmgr);
	  if(node != null) {
	    a.Email = node.InnerText;
	  }
	  n.Authors.Add(a);
	}
      }
      
      nodes = root.SelectNodes("/llup:notification/atom:category", nsmgr);
      if(nodes != null) {
	foreach(XmlElement nd in nodes) {
	  Category c = new Category(nd.GetAttribute("term"),
				    nd.GetAttribute("scheme"), nd.GetAttribute("label"),
				    nd.GetAttribute("lang"));
	  n.Categories.Add(c);
	}
      }
      
      return n;
    }

    public static string Xml(Notification n) {
      MemoryStream ms = new MemoryStream();
      
      XmlWriterSettings settings = new XmlWriterSettings();
      settings.Indent = false;
      settings.Encoding = Encoding.UTF8;
      settings.OmitXmlDeclaration = true;

      XmlWriter xw = XmlWriter.Create(ms, settings);
      string xml = null;
      
      try {
	xw.WriteStartElement("llup", "notification", LLUP_NS);
	//xw.WriteAttributeString("xmlns", "llup", LLUP_NS);
	xw.WriteAttributeString("xmlns", null, ATOM_NS);

	if(n.Action != String.Empty) {
	  xw.WriteAttributeString("action", n.Action);
	}

	if(n.Recipient != String.Empty) {
	  xw.WriteStartElement("llup", "recipient", LLUP_NS);
	  xw.WriteAttributeString("href", n.Recipient);
	  xw.WriteEndElement();
	}

	if(n.Published != DateTime.MinValue) {
	  xw.WriteElementString("published", ATOM_NS, 
				n.Published.ToString("o"));
	}

	if(n.Updated != DateTime.MinValue) {
	  xw.WriteElementString("updated", ATOM_NS, 
				n.Updated.ToString("o"));
	}

	if(n.Expires != DateTime.MinValue) {
	  xw.WriteElementString("llup", "expires", LLUP_NS, 
				n.Expires.ToString("o"));
	}

	foreach(Author a in n.Authors) {
	  xw.WriteStartElement("author", ATOM_NS);
	  if(a.Name != String.Empty) {
	    xw.WriteElementString("name", ATOM_NS, a.Name);
	  }
	  if(a.Uri != String.Empty) {
	    xw.WriteElementString("uri", ATOM_NS, a.Uri);
	  }
	  if(a.Email != String.Empty) {
	    xw.WriteElementString("email", ATOM_NS, a.Email);
	  }
	  xw.WriteEndElement();
	}
	
	foreach(Category c in n.Categories) {
	  xw.WriteStartElement("category", ATOM_NS);
	  if(c.Term != String.Empty) {
	    xw.WriteAttributeString("term", c.Term);
	  }
	  if(c.Scheme != String.Empty) {
	    xw.WriteAttributeString("scheme", c.Scheme);
	  }
	  if(c.Label != String.Empty) {
	    xw.WriteAttributeString("label", c.Label);
	  }
	  if(c.Lang != String.Empty) {
	    xw.WriteAttributeString("xml", "lang", c.Lang);
	  }
	  xw.WriteEndElement();
	}

	foreach(Link l in n.Links) {
	  xw.WriteStartElement("link", ATOM_NS);
	  if(l.Rel != String.Empty) {
	    xw.WriteAttributeString("rel", l.Rel);
	  }
	  if(l.Href != String.Empty) {
	    xw.WriteAttributeString("href", l.Href);
	  }
	  if(l.MediaType != String.Empty) {
	    xw.WriteAttributeString("type", l.MediaType);
	  }
	  xw.WriteEndElement();
	}

	xw.WriteEndElement(); 
	xw.Flush();
      } finally {
	if(xw != null) {
	  StreamReader sr = new StreamReader(ms);
	  ms.Seek(0, SeekOrigin.Begin);
	  xml = sr.ReadToEnd();
	  ms.Close();
	  xw.Close();
	}
      }

      return xml;

    }
  }
}