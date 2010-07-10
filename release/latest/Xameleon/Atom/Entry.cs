//
// entry.cs: 
//
// Author:
//   Sylvain Hellegouarch (sh@defuze.org)
//
// Copyright (C) 2007, Sylvain Hellegouarch
// 

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Text;

namespace Xameleon.Atom
{
  public class AtomEntry
  {
    private string id = null;
    private string title = null;
    private DateTime? updated = null; 
    private DateTime? published = null;
    private DateTime? edited = null;
    private IList<Link> links = new List<Link>();
    private IList<Author> authors = new List<Author>();
    private TextConstruct summary = null;
    private TextConstruct rights = null;
    private TextConstruct content = null;

    public AtomEntry() {}

    public  string Id {
      get {
	return this.id;
      }
      set {
	this.id = value;
      }
    }
    
    public  string Title {
      get {
	return this.title;
      }
      set {
	this.title = value;
      }
    }

    public  DateTime? Updated {
      get {
	return this.updated;
      }
      set {
	this.updated = value;
      }
    }

    public DateTime? Published {
      get {
	return this.published;
      }
      set {
	this.published = value;
      }
    }

    public  DateTime? Edited {
      get {
	return this.edited;
      }
      set {
	this.edited = value;
      }
    }

    public IList<Link> Links {
      get {
	return this.links;
      }
    }

    public IList<Author> Authors {
      get {
	return this.authors;
      }
    }

    public TextConstruct Summary {
      get {
	return this.summary;
      }
      set {
	this.summary = value;
      }
    }

    public TextConstruct Rights {
      get {
	return this.rights;
      }
      set {
	this.rights = value;
      }
    }

    public TextConstruct Content {
      get {
	return this.content;
      }
      set {
	this.content = value;
      }
    }

    public XmlDocument Document {
      get {
	XmlDocument doc = new XmlDocument();
	XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", null); 

	XmlElement root  = doc.CreateElement("atom", "entry", "http://www.w3.org/2005/Atom");
        doc.InsertBefore(decl, doc.DocumentElement); 
        doc.AppendChild(root);

	XmlElement e = null;

	if(this.Id != null) {
	  e = doc.CreateElement("atom", "id", "http://www.w3.org/2005/Atom");
	  e.InnerText = this.Id;
	  root.AppendChild(e);
	}
		
	if(this.Title != null) {
	  e = doc.CreateElement("atom", "title", "http://www.w3.org/2005/Atom");
	  e.InnerText = this.Title;
	  root.AppendChild(e);
	}	

	if(this.Updated.HasValue) {
	  e = doc.CreateElement("atom", "updated", "http://www.w3.org/2005/Atom");
	  e.InnerText = this.Updated.Value.ToString("o");
	  root.AppendChild(e);
	}

	if(this.Published.HasValue) {
	  e = doc.CreateElement("atom", "published", "http://www.w3.org/2005/Atom");
	  e.InnerText = this.Published.Value.ToString("o");
	  root.AppendChild(e);
	}

	if(this.Edited.HasValue) {
	  e = doc.CreateElement("app", "edited", "http://www.w3.org/2007/app");
	  e.InnerText = this.Edited.Value.ToString("o");
	  root.AppendChild(e);
	}

	XmlNode node = null;

	foreach(Author author in this.Authors) {
	  node = doc.ImportNode(author.Node, true);
	  root.AppendChild(node);
	}

	foreach(Link link in this.Links) {
	  node = doc.ImportNode(link.Node, true);
	  root.AppendChild(node);
	}

	if(this.Summary != null) {
	  node = doc.ImportNode(this.Summary.Node, true);
	  root.AppendChild(node);
	}

	if(this.Content != null) {
	  node = doc.ImportNode(this.Content.Node, true);
	  root.AppendChild(node);
	}

	return doc;
      }

      set {
	this.Node = value.DocumentElement;
      }
    }

    public XmlNode Node {
      get {
	XmlDocument doc = this.Document;
	return doc.DocumentElement;
      }
      set {
	XmlDocument doc = value.OwnerDocument;
	XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
	nsmgr.AddNamespace("atom",  "http://www.w3.org/2005/Atom");
	nsmgr.AddNamespace("app",  "http://www.w3.org/2007/app");

	XmlNode root = value;

	XmlNode e = root.SelectSingleNode("./atom:id", nsmgr);
	if(e != null) {
	  this.Id = e.InnerText;
	}
	
	e = root.SelectSingleNode("./atom:updated", nsmgr);
	if(e != null) {
	  this.Updated = DateTime.Parse(e.InnerText);
	}

	e = root.SelectSingleNode("./atom:published", nsmgr);
	if(e != null) {
	  this.Published = DateTime.Parse(e.InnerText);
	}

	e = root.SelectSingleNode("./app:edited", nsmgr);
	if(e != null) {
	  this.Edited = DateTime.Parse(e.InnerText);
	}

	XmlNodeList links = root.SelectNodes("./atom:link", nsmgr); 
	foreach (XmlNode link in links){
	  Link l = new Link();
	  l.Node = link;
	  this.Links.Add(l);
	}

	XmlNodeList authors = root.SelectNodes("./atom:author", nsmgr); 
	foreach (XmlNode author in authors){
	  Author a = new Author();
	  a.Node = author;
	  this.Authors.Add(a);
	}

	e = root.SelectSingleNode("./atom:content", nsmgr);
	if(e != null) {
	  this.Content = new TextConstruct();
	  this.Content.Node = e;
	}

	e = root.SelectSingleNode("./atom:summary", nsmgr);
	if(e != null) {
	  this.Summary = new TextConstruct();
	  this.Summary.Node = e;
	}
      }
    }
  }
}