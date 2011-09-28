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
    public class Author
    {
        private string uri = null;
        private string email = null;
        private string name = null;

        public Author() { }

        public Author(string name)
        {
            this.name = name;
        }

        public string Email
        {
            get
            {
                return this.email;
            }
            set
            {
                this.email = value;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public string Uri
        {
            get
            {
                return this.uri;
            }
            set
            {
                this.uri = value;
            }
        }

        public XmlDocument Document
        {
            get
            {
                XmlDocument doc = new XmlDocument();
                XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", null);

                XmlElement root = doc.CreateElement("atom", "author", "http://www.w3.org/2005/Atom");
                doc.InsertBefore(decl, doc.DocumentElement);
                doc.AppendChild(root);

                XmlElement e = null;

                if (this.Name != null)
                {
                    e = doc.CreateElement("atom", "name", "http://www.w3.org/2005/Atom");
                    e.InnerText = this.Name;
                    root.AppendChild(e);
                }

                if (this.Uri != null)
                {
                    e = doc.CreateElement("atom", "uri", "http://www.w3.org/2005/Atom");
                    e.InnerText = this.Uri;
                    root.AppendChild(e);
                }

                if (this.Email != null)
                {
                    e = doc.CreateElement("atom", "email", "http://www.w3.org/2005/Atom");
                    e.InnerText = this.Email;
                    root.AppendChild(e);
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
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(value.OwnerDocument.NameTable);
                nsmgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");

                XmlNode n = value.SelectSingleNode("./atom:name", nsmgr);
                if (n != null)
                {
                    this.Name = n.InnerText;
                }

                n = value.SelectSingleNode("./atom:uri", nsmgr);
                if (n != null)
                {
                    this.Uri = n.InnerText;
                }

                n = value.SelectSingleNode("./atom:email", nsmgr);
                if (n != null)
                {
                    this.Email = n.InnerText;
                }
            }
        }
    }
}