using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Nuxleus.Web.HttpHandler {
    //class temp {

    //    public temp() {}

    //    public static void DoFoo () {
    //        XNode node = null;
    //        XElement parent = null;
    //        do {
    //            switch (reader.NodeType) {
    //                case XmlNodeType.Element:
    //                    XElement element = new XElement(XName.Get(reader.LocalName, reader.NamespaceURI));
    //                    if (reader.MoveToFirstAttribute()) {
    //                        do {
    //                            element.Add(new XAttribute(XName.Get(reader.LocalName, reader.NamespaceURI), reader.Value));
    //                        } while (reader.MoveToNextAttribute());
    //                        reader.MoveToElement();
    //                    }
    //                    if (lineInfo.HasLineInfo()) {
    //                        element.SetLineInfo(new LineInfo(lineInfo.LineNumber, lineInfo.LinePosition));
    //                    }
    //                    if (!reader.IsEmptyElement) {
    //                        if (parent != null) {
    //                            parent.Add(element);
    //                        }
    //                        parent = element;
    //                        continue;
    //                    } else {
    //                        node = element;
    //                    }
    //                    break;
    //                case XmlNodeType.EndElement:
    //                    if (parent == null)
    //                        return null;
    //                    if (parent.IsEmpty) {
    //                        parent.Add(string.Empty);
    //                    }
    //                    if (parent.Parent == null)
    //                        return parent;
    //                    parent = parent.Parent;
    //                    continue;
    //                case XmlNodeType.Text:
    //                case XmlNodeType.SignificantWhitespace:
    //                case XmlNodeType.Whitespace:
    //                    node = new XText(reader.Value);
    //                    break;
    //                case XmlNodeType.CDATA:
    //                    node = new XText(reader.Value, TextType.CData);
    //                    break;
    //                case XmlNodeType.Comment:
    //                    node = new XComment(reader.Value);
    //                    break;
    //                case XmlNodeType.ProcessingInstruction:
    //                    node = new XProcessingInstruction(reader.Name, reader.Value);
    //                    break;
    //                case XmlNodeType.DocumentType:
    //                    node = new XDocumentType(reader.LocalName, reader.GetAttribute("PUBLIC"), reader.GetAttribute("SYSTEM"), reader.Value);
    //                    break;
    //                case XmlNodeType.EntityReference:
    //                    reader.ResolveEntity();
    //                    continue;

    //                case XmlNodeType.XmlDeclaration:
    //                case XmlNodeType.EndEntity:
    //                    continue;
    //                default:
    //                    throw new InvalidOperationException();
    //            }
    //            if (parent == null)
    //                return node;
    //            parent.Add(node);
    //        } while (reader.Read());
    //    }
    //}
}
