//
// ForeignElement.cs: 
//
// Author:
//   Sylvain Hellegouarch (sh@defuze.org)
//
// Copyright (C) 2007, 3rd&Urban, LLC
// 

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Specialized;

namespace Nuxleus.Atom {

    public class ForeignElement {

        private string name = null;
        private string prefix = null;
        private string ns = null;

        private NameValueCollection attrs = new NameValueCollection();

        private string content = null;

        private IList<ForeignElement> children = new List<ForeignElement>();

        public ForeignElement ( string name ) {
            this.name = name;
        }

        public ForeignElement ( string prefix, string name, string ns ) {
            this.prefix = prefix;
            this.name = name;
            this.ns = ns;
        }

        public ForeignElement ( string name, string ns ) {
            this.name = name;
            this.ns = ns;
        }

        public string Content {
            get { return this.content; }
            set { this.content = value; }
        }

        public NameValueCollection Attributes {
            get { return this.attrs; }
        }

        public IList<ForeignElement> Children {
            get { return this.children; }
        }

        public XmlDocument Document {
            get {
                XmlDocument doc = new XmlDocument();
                XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", null);


                XmlElement root = doc.CreateElement(this.prefix, this.name, this.ns);

                doc.InsertBefore(decl, doc.DocumentElement);
                doc.AppendChild(root);

                foreach (string key in this.Attributes) {
                    string value = this.Attributes[key];
                    root.SetAttribute(key, value);
                }

                if (this.Content != null) {
                    root.InnerText = this.Content;
                }

                XmlNode node = null;
                foreach (ForeignElement child in this.Children) {
                    node = doc.ImportNode(child.Node, true);
                    root.AppendChild(node);
                }

                return doc;
            }
        }

        public XmlNode Node {
            get {
                XmlDocument doc = this.Document;
                return doc.DocumentElement;
            }
        }
    }
}