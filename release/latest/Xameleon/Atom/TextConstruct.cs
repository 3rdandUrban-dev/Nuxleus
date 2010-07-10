//
// textconstruct.cs: 
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
    public enum TextElement
    {
        Content,
        Summary,
        Rights,
    }

    public class TextConstruct
    {
        private TextElement type = TextElement.Content;
        private string mediatype = null;
        private string content = null;
        private string xmlContent = null;
        private string refContent = null;
        private string encodedContent = null;

        public TextConstruct() { }

        public TextConstruct(TextElement type)
        {
            this.type = type;
        }

        public TextElement Type
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

        public string Mediatype
        {
            get
            {
                return this.mediatype;
            }
            set
            {
                this.mediatype = value;
            }
        }

        public string TextContent
        {
            get
            {
                return this.content;
            }
            set
            {
                this.content = value;
            }
        }

        public string XmlContent
        {
            get
            {
                return this.xmlContent;
            }
            set
            {
                this.xmlContent = value;
            }
        }

        public string RefContent
        {
            get
            {
                return this.refContent;
            }
            set
            {
                this.refContent = value;
            }
        }

        public string EncodedContent
        {
            get
            {
                return this.encodedContent;
            }
            set
            {
                this.encodedContent = value;
            }
        }

        public XmlDocument Document
        {
            get
            {
                XmlDocument doc = new XmlDocument();
                XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", null);

                string elName = null;
                if (this.Type == TextElement.Content)
                {
                    elName = "content";
                }
                else if (this.Type == TextElement.Summary)
                {
                    elName = "summary";
                }
                else if (this.Type == TextElement.Rights)
                {
                    elName = "rights";
                }

                XmlElement root = doc.CreateElement("atom", elName, "http://www.w3.org/2005/Atom");
                doc.InsertBefore(decl, doc.DocumentElement);
                doc.AppendChild(root);

                if (this.Mediatype != null)
                {
                    root.SetAttribute("type", this.Mediatype);
                }

                if (this.XmlContent != null)
                {
                    XmlDocumentFragment frag = doc.CreateDocumentFragment();
                    frag.InnerXml = this.XmlContent;
                    root.AppendChild(frag);
                }
                else if (this.TextContent != null)
                {
                    root.InnerText = this.TextContent;
                }
                else if (this.RefContent != null)
                {
                    root.SetAttribute("src", this.RefContent);
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
                XmlDocument doc = value.OwnerDocument;
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                nsmgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");
                nsmgr.AddNamespace("xh", "http://www.w3.org/1999/xhtml");

                XmlElement e = (XmlElement)value;

                if (value.LocalName == "content")
                {
                    this.Type = TextElement.Content;
                }
                else if (value.LocalName == "summary")
                {
                    this.Type = TextElement.Summary;
                }
                else if (value.LocalName == "rights")
                {
                    this.Type = TextElement.Rights;
                }

                string type = e.GetAttribute("type");
                this.Mediatype = type;

                if (type == "xhtml")
                {
                    XmlNode div = value.SelectSingleNode("./xh:div", nsmgr);
                    if (div != null)
                    {
                        this.XmlContent = div.OuterXml;
                    }
                }
                else if ((type == "html") || (type == "text"))
                {
                    this.TextContent = value.InnerText;
                }
                else if ((type.EndsWith("+xml")) || (type.EndsWith("/xml")))
                {
                    this.XmlContent = value.InnerXml;
                }
                else if (type.StartsWith("text/"))
                {
                    this.TextContent = value.InnerText;
                }
                else
                {
                    this.EncodedContent = value.InnerText;
                }

                string src = e.GetAttribute("src");
                if (src != String.Empty)
                {
                    this.RefContent = src;
                }
            }
        }
    }
}