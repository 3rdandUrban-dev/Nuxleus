//
// author.cs: 
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
    public class Category
    {
        private string term = String.Empty;
    private string scheme = String.Empty;
    private string label = String.Empty;
    private string lang = String.Empty;
    
    public Category() {}

    public Category(string term, string scheme, string label, string lang) {
      this.term = term;
      this.scheme = scheme;
      this.label = label;
      this.lang = lang;
    }

    public string Term {
      get {
	return this.term;
      }
      set {
	this.term = value;
      }
    }

    public string Scheme {
      get {
	return this.scheme;
      }
      set {
	this.scheme = value;
      }
    }

    public string Label {
      get {
	return this.label;
      }
      set {
	this.label = value;
      }
    }

    public string Lang {
      get {
	return this.lang;
      }
      set {
	this.lang = value;
      }
    }
        public XmlDocument Document
        {
            get
            {
                XmlDocument doc = new XmlDocument();
                XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", null);

                XmlElement root = doc.CreateElement("atom", "category", "http://www.w3.org/2005/Atom");
                doc.InsertBefore(decl, doc.DocumentElement);
                doc.AppendChild(root);

                XmlElement e = null;

		if ((this.Term != null) && (this.Term != String.Empty))
                {
                    root.SetAttribute("term", this.Term);
                }

		if ((this.Label != null) && (this.Label != String.Empty))
                {
                    root.SetAttribute("label", this.Label);
                }

		if ((this.Scheme != null) && (this.Scheme != String.Empty))
                {
                    root.SetAttribute("scheme", this.Scheme);
                }

		if ((this.Lang != null) && (this.Lang != String.Empty))
                {
                    root.SetAttribute("lang", this.Lang);
                }

                return doc;
            }
        }

        public XmlNode Node
        {
            get
            {
                XmlDocument doc = this.Document;
                return doc.DocumentElement;
            }
            set
            {
                XmlElement el = (XmlElement)value;
                this.Term = el.GetAttribute("term");
                this.Lang = el.GetAttribute("lang");
                this.Label = el.GetAttribute("label");
                this.Scheme = el.GetAttribute("scheme");
            }
        }
    }
}