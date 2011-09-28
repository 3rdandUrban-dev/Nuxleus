//
// link.cs: 
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
    public class Link
    {
        private string rel = null;
        private string href = null;
        private string type = null;
        private string hreflang = null;
        private string title = null;
        private int? length = null;

        public Link() { }

        public Link(string rel, string href, string type)
        {
            this.rel = rel;
            this.href = href;
            this.type = type;
        }

        public string Rel
        {
            get
            {
                return this.rel;
            }
            set
            {
                this.rel = value;
            }
        }

        public string Href
        {
            get
            {
                return this.href;
            }
            set
            {
                this.href = value;
            }
        }

        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                this.title = value;
            }
        }

        public string Mediatype
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
            }
        }

        public string Hreflang
        {
            get
            {
                return this.hreflang;
            }
            set
            {
                this.hreflang = value;
            }
        }

        public int? Length
        {
            get
            {
                return this.length;
            }
            set
            {
                this.length = value;
            }
        }

        public XmlDocument Document
        {
            get
            {
                XmlDocument doc = new XmlDocument();
                XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", null);

                XmlElement root = doc.CreateElement("atom", "link", "http://www.w3.org/2005/Atom");
                doc.InsertBefore(decl, doc.DocumentElement);
                doc.AppendChild(root);

                if ((this.Mediatype != null) && (this.Mediatype != String.Empty))
                {
                    root.SetAttribute("type", this.Mediatype);
                }

                if ((this.Hreflang != null) && (this.Hreflang != String.Empty))
                {
                    root.SetAttribute("hreflang", this.Hreflang);
                }

                if (this.Length.HasValue)
                {
                    root.SetAttribute("length", Convert.ToString(this.Length.Value));
                }

                if ((this.Href != null) && (this.Href != String.Empty))
                {
                    root.SetAttribute("href", this.Href);
                }

                if ((this.Rel != null) && (this.Rel != String.Empty))
                {
                    root.SetAttribute("rel", this.Rel);
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
                this.Rel = el.GetAttribute("rel");
                this.Hreflang = el.GetAttribute("hreflang");
                this.Title = el.GetAttribute("title");
                this.Mediatype = el.GetAttribute("type");
                this.Href = el.GetAttribute("href");
                string length = el.GetAttribute("length");
                if (length != String.Empty)
                {
                    this.Length = Convert.ToInt32(length);
                }
            }
        }
    }
}