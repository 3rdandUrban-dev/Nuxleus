//
// service.cs: AtomPub service implementation
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
    public class AtomPubService
    {
        StoreManager _store;
        IList<AtomPubWorkspace> workspaces = new List<AtomPubWorkspace>();
        string _xmlLang = null;
        string _xmlBase = null;

        public AtomPubService()
        {
        }

        public AtomPubService(StoreManager store)
        {
            _store = store;
        }

        public StoreManager Store
        {
            get
            {
                return _store;
            }
            set
            {
                _store = value;
            }
        }

        public string Lang
        {
            get
            {
                return _xmlLang;
            }
            set
            {
                _xmlLang = value;
            }
        }

        public string Base
        {
            get
            {
                return _xmlBase;
            }
            set
            {
                _xmlBase = value;
            }
        }

        public AtomPubWorkspace GetWorkspace(string name)
        {
            foreach (AtomPubWorkspace w in this.workspaces)
            {
                if (w.Name == name)
                {
                    return w;
                }
            }
            return null;
        }

        public IList<AtomPubWorkspace> Workspaces
        {
            get
            {
                return this.workspaces;
            }
        }

        public IList<AtomPubCollection> Collections
        {
            get
            {
                IList<AtomPubCollection> collections = new List<AtomPubCollection>();
                foreach (AtomPubWorkspace w in this.workspaces)
                {
                    foreach (AtomPubCollection c in w.Collections)
                    {
                        collections.Add(c);
                    }
                }
                return collections;
            }
        }

        public AtomPubCollection GetCollection(string name)
        {
            foreach (AtomPubWorkspace w in this.workspaces)
            {
                foreach (AtomPubCollection c in w.Collections)
                {
                    if (c.Name == name)
                    {
                        return c;
                    }
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

                XmlElement root = doc.CreateElement("app", "service", "http://www.w3.org/2007/app");
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

                foreach (AtomPubWorkspace w in this.workspaces)
                {
                    XmlNode node = doc.ImportNode(w.Node, true);
                    root.AppendChild(node);
                }

                return doc;
            }
        }
    }
}
