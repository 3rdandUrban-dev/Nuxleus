using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.VisualStudio.TextManager.Interop;

namespace XmlMvp.XPathmania.Internal
{
    public class XmlNodeInfo : IVsTextMarkerClient, IDisposable
    {
        XmlNode node;
        TextSpan span;
        IVsTextLineMarker marker;

        public event EventHandler MarkerChanged;
        public event EventHandler MarkerDeleted;

        public IVsTextLineMarker Marker
        {
            get { return marker; }
            set { marker = value; }
        }

        public XmlNodeInfo(XmlNode node, int line, int col)
        {
            this.node = node;
            span.iStartLine = span.iEndLine = line - 1; // change to 0-based.
            span.iStartIndex = span.iEndIndex = col - 1;
        }

        public string Match
        {
            get { return ConvertToString(node); }
            set { }
        }

        public int OneBasedLine
        {
            get { return span.iStartLine +1; }
        }

        public int Line
        {
            get { return span.iStartLine; }
            set { span.iStartLine = value; }
        }

        public int Column
        {
            get { return span.iStartIndex; }
            set { span.iStartIndex = value; }
        }

        public TextSpan CurrentSpan
        {
            get
            {
                // this span tracks edits!
                TextSpan[] aSpan = new TextSpan[1];
                if (marker != null)
                {
                    marker.GetCurrentSpan(aSpan);
                    return aSpan[0];
                }
                return this.span;
            }
        }

        private static string ConvertToString(XmlNode node)
        {
            StringBuilder SBuilder = new StringBuilder();

            switch (node.NodeType)
            {
                case System.Xml.XmlNodeType.Attribute:
                    SBuilder.Append(node.Name);
                    SBuilder.Append(@"=""");
                    SBuilder.Append(((XmlAttribute)node).Value);
                    SBuilder.Append(@"""");
                    break;
                case System.Xml.XmlNodeType.CDATA:
                    SBuilder.Append(node.Value);
                    break;
                case System.Xml.XmlNodeType.Comment:
                    break;
                case System.Xml.XmlNodeType.Document:
                    break;
                case System.Xml.XmlNodeType.DocumentFragment:
                    break;
                case System.Xml.XmlNodeType.DocumentType:
                    break;
                case System.Xml.XmlNodeType.Element:
                    SBuilder.Append("<");
                    if (node.Prefix != string.Empty)
                    {
                        SBuilder.Append(node.Prefix);
                        SBuilder.Append(":");
                    }
                    SBuilder.Append(node.LocalName);
                    foreach (XmlAttribute att in ((XmlElement)node).Attributes)
                    {
                        SBuilder.Append(" ");
                        SBuilder.Append(ConvertToString(att));
                    }
                    SBuilder.Append(">");
                    break;
                case System.Xml.XmlNodeType.EndElement:
                    break;
                case System.Xml.XmlNodeType.EndEntity:
                    break;
                case System.Xml.XmlNodeType.Entity:
                    break;
                case System.Xml.XmlNodeType.EntityReference:
                    break;
                case System.Xml.XmlNodeType.None:
                    break;
                case System.Xml.XmlNodeType.Notation:
                    break;
                case System.Xml.XmlNodeType.ProcessingInstruction:
                    break;
                case System.Xml.XmlNodeType.SignificantWhitespace:
                    break;
                case System.Xml.XmlNodeType.Text:
                    SBuilder.Append(node.Value);
                    break;
                case System.Xml.XmlNodeType.Whitespace:
                    break;
                case System.Xml.XmlNodeType.XmlDeclaration:
                    break;
                default:
                    break;
            }
            return SBuilder.ToString();
        }


        #region IDisposable Members

        public void Dispose()
        {
            if (marker != null)
            {
                marker.Invalidate();
                marker = null;
            }
        }

        #endregion

        #region IVsTextMarkerClient Members

        public int ExecMarkerCommand(IVsTextMarker pMarker, int iItem)
        {
            return 0;
        }

        public int GetMarkerCommandInfo(IVsTextMarker pMarker, int iItem, string[] pbstrText, uint[] pcmdf)
        {
            return 0;
        }

        public int GetTipText(IVsTextMarker pMarker, string[] pbstrText)
        {
            if (pbstrText != null)
            {
                pbstrText[0] = ConvertToString(node);
            }
            return 0;
        }

        public void MarkerInvalidated()
        {
            MarkerDeleted(this, new EventArgs());
            marker = null;
        }

        public int OnAfterMarkerChange(IVsTextMarker pMarker)
        {
            if (MarkerChanged != null)
            {
                this.span = this.CurrentSpan;
                MarkerChanged(this, new EventArgs());
            }
            return 0;
        }

        public void OnAfterSpanReload()
        {
        }

        public void OnBeforeBufferClose()
        {
            MarkerDeleted(this, new EventArgs());
        }

        public void OnBufferSave(string pszFileName)
        {
        }

        #endregion
    }
}
