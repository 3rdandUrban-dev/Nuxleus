//
// category.cs: Atom category implementation
//
// Author:
//   Sylvain Hellegouarch (sh@defuze.org)
//
// Copyright (C) 2007, Sylvain Hellegouarch
// 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;
using Xameleon.Atom;

namespace Xameleon.Amplee
{
    public class AtomCategory
    {
        private string term = null;
        private string label = null;
        private string scheme = null;

        public AtomCategory() { }

        public string Term
        {
            get
            {
                return this.term;
            }
            set
            {
                this.term = value;
            }
        }

        public string Scheme
        {
            get
            {
                return this.scheme;
            }
            set
            {
                this.scheme = value;
            }
        }

        public string Label
        {
            get
            {
                return this.label;
            }
            set
            {
                this.label = value;
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

                if (this.Term != null)
                {
                    root.SetAttribute("term", this.Term);
                }

                if (this.Scheme != null)
                {
                    root.SetAttribute("scheme", this.Scheme);
                }

                if (this.Label != null)
                {
                    root.SetAttribute("label", this.Label);
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
        }
    }
}