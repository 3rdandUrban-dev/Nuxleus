//
// workspace.cs: AtomPub workspace implementation
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
    public class AtomPubWorkspace
    {
        private AtomPubService service = null;
        private IList<AtomPubCollection> collections = new List<AtomPubCollection>();
        private string xmlLang = null;
        private string xmlBase = null;
        private string name = null;

        public AtomPubWorkspace()
        {
        }

        public AtomPubWorkspace(AtomPubService service)
        {
            this.service = service;
            this.service.Workspaces.Add(this);
        }

        public AtomPubService Service
        {
            get
            {
                return this.service;
            }
            set
            {
                this.service = value;
            }
        }

        public string Lang
        {
            get
            {
                return this.xmlLang;
            }
            set
            {
                this.xmlLang = value;
            }
        }

        public string Base
        {
            get
            {
                if (this.xmlBase != null)
                    return this.xmlBase;
                return this.Service.Base;
            }
            set
            {
                this.xmlBase = value;
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

        public IList<AtomPubCollection> Collections
        {
            get
            {
                return this.collections;
            }
        }

        public AtomPubCollection GetCollection(string name)
        {
            foreach (AtomPubCollection c in this.Collections)
            {
                if (c.Name == name)
                {
                    return c;
                }
            }
            return null;
        }

        public XmlDocument Document
        {
            get
            {
                XmlDocument doc = new XmlDocument();
                XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", null);

                XmlElement root = doc.CreateElement("app", "workspace", "http://www.w3.org/2007/app");
                doc.InsertBefore(decl, doc.DocumentElement);
                doc.AppendChild(root);

                if (this.Base != null)
                {
                    root.SetAttribute("base", "http://www.w3.org/XML/1998/namespace", this.Base);
                }

                if (this.Lang != null)
                {
                    root.SetAttribute("lang", "http://www.w3.org/XML/1998/namespace", this.Lang);
                }

                foreach (AtomPubCollection c in this.collections)
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
