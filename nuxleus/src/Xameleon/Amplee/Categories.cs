//
// categories.cs: AtomPub categories implementation
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
    public class AtomPubCategories
    {
        private bool isfixed = false;
        private IList<AtomCategory> categories = new List<AtomCategory>();

        public AtomPubCategories() { }

        public bool Fixed
        {
            get
            {
                return this.isfixed;
            }
            set
            {
                this.isfixed = value;
            }
        }

        public IList<AtomCategory> Categories
        {
            get
            {
                return this.categories;
            }
        }

        public XmlDocument Document
        {
            get
            {
                XmlDocument doc = new XmlDocument();
                XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", null);

                XmlElement root = doc.CreateElement("app", "categories", "http://www.w3.org/2007/app");

                if (this.Fixed == true)
                {
                    root.SetAttribute("fixed", "yes");
                }

                doc.InsertBefore(decl, doc.DocumentElement);
                doc.AppendChild(root);

                foreach (AtomCategory c in this.Categories)
                {
                    XmlNode node = doc.ImportNode(c.Node, true);
                    root.AppendChild(node);
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