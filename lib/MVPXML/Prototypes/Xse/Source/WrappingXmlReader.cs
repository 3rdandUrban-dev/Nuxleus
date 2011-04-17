using System;
using System.Xml;

namespace Mvp.Xml.Core
{
	public abstract class WrappingXmlReader : XmlReader, IXmlLineInfo
	{
		XmlReader innerReader;

		public WrappingXmlReader(XmlReader reader)
		{
			Guard.ArgumentNotNull(reader, "reader");

			innerReader = reader;
		}

		protected XmlReader InnerReader
		{
			get { return innerReader; }
			set
			{
				Guard.ArgumentNotNull(value, "value");
				innerReader = value;
			}
		}

		public override bool CanReadBinaryContent { get { return innerReader.CanReadBinaryContent; } }

		public override bool CanReadValueChunk { get { return innerReader.CanReadValueChunk; } }

		public override bool CanResolveEntity { get { return innerReader.CanResolveEntity; } }

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				((IDisposable)innerReader).Dispose();
			}
		}

		public override bool Read() { return innerReader.Read(); }

		public override void Close() { innerReader.Close(); }

		public override string GetAttribute(int i) { return innerReader.GetAttribute(i); }

		public override string GetAttribute(string name) { return innerReader.GetAttribute(name); }

		public override string GetAttribute(string localName, string namespaceURI) { { return innerReader.GetAttribute(localName, namespaceURI); } }

		public override string LookupNamespace(string prefix) { return innerReader.LookupNamespace(prefix); }

		public override void MoveToAttribute(int i) { innerReader.MoveToAttribute(i); }

		public override bool MoveToAttribute(string name) { return innerReader.MoveToAttribute(name); }

		public override bool MoveToAttribute(string localName, string namespaceURI) { return innerReader.MoveToAttribute(localName, namespaceURI); }

		public override bool MoveToElement() { return innerReader.MoveToElement(); }

		public override bool MoveToFirstAttribute() { return innerReader.MoveToFirstAttribute(); }

		public override bool MoveToNextAttribute() { return innerReader.MoveToNextAttribute(); }

		public override bool ReadAttributeValue() { return innerReader.ReadAttributeValue(); }

		public override void ResolveEntity() { innerReader.ResolveEntity(); }

		public override int AttributeCount { get { return innerReader.AttributeCount; } }

		public override string BaseURI { get { return innerReader.BaseURI; } }

		public override int Depth { get { return innerReader.Depth; } }

		public override bool EOF { get { return innerReader.EOF; } }

		public override bool HasValue { get { return innerReader.HasValue; } }

		public override bool IsDefault { get { return innerReader.IsDefault; } }

		public override bool IsEmptyElement { get { return innerReader.IsEmptyElement; } }

		public override string this[int i] { get { return innerReader[i]; } }

		public override string this[string name] { get { return innerReader[name]; } }

		public override string this[string name, string namespaceURI] { get { return innerReader[name, namespaceURI]; } }

		public override string LocalName { get { return innerReader.LocalName; } }

		public override string Name { get { return innerReader.Name; } }

		public override string NamespaceURI { get { return innerReader.NamespaceURI; } }

		public override XmlNameTable NameTable { get { return innerReader.NameTable; } }

		public override XmlNodeType NodeType { get { return innerReader.NodeType; } }

		public override string Prefix { get { return innerReader.Prefix; } }

		public override char QuoteChar { get { return innerReader.QuoteChar; } }

		public override ReadState ReadState { get { return innerReader.ReadState; } }

		public override string Value { get { return innerReader.Value; } }

		public override string XmlLang { get { return innerReader.XmlLang; } }

		public override XmlSpace XmlSpace { get { return innerReader.XmlSpace; } }

		public override int ReadValueChunk(char[] buffer, int index, int count) { return innerReader.ReadValueChunk(buffer, index, count); }

		//public override bool HasAttributes { get { return innerReader.HasAttributes; } }

		//public override bool IsStartElement() { return innerReader.IsStartElement(); }

		//public override bool IsStartElement(string localname, string ns) { return innerReader.IsStartElement(localname, ns); }

		//public override bool IsStartElement(string name) { return innerReader.IsStartElement(name); }

		//public override XmlNodeType MoveToContent() { return innerReader.MoveToContent(); }

		//public override int ReadContentAsBase64(byte[] buffer, int index, int count) { return innerReader.ReadContentAsBase64(buffer, index, count); }

		//public override int ReadContentAsBinHex(byte[] buffer, int index, int count) { return innerReader.ReadContentAsBinHex(buffer, index, count); }

		//public override bool ReadContentAsBoolean() { return innerReader.ReadContentAsBoolean(); }


		#region IXmlLineInfo Members

		public bool HasLineInfo()
		{
			IXmlLineInfo info = innerReader as IXmlLineInfo;
			if (info != null)
			{
				return info.HasLineInfo();
			}

			return false;
		}

		public int LineNumber
		{
			get
			{
				IXmlLineInfo info = innerReader as IXmlLineInfo;
				if (info != null)
				{
					return info.LineNumber;
				}

				return 0;
			}
		}

		public int LinePosition
		{
			get
			{
				IXmlLineInfo info = innerReader as IXmlLineInfo;
				if (info != null)
				{
					return info.LinePosition;
				}

				return 0;
			}
		}

		#endregion
	}
}
