using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Mvp.Xml.Core
{
	internal class FullEndElementReader : WrappingXmlReader
	{
		bool fakeEndElement;

		public FullEndElementReader(XmlReader reader) : base(reader) { }

		public override bool Read()
		{
			if (!fakeEndElement && InnerReader.IsEmptyElement)
			{
				fakeEndElement = true;
				return true;
			}
			if (fakeEndElement)
			{
				fakeEndElement = false;
			}

			return base.Read();
		}

		public override bool IsEmptyElement
		{
			get { return false; }
		}

		public override int AttributeCount
		{
			get
			{
				if (fakeEndElement)
					return 0;

				return base.AttributeCount;
			}
		}

		public override bool HasAttributes
		{
			get
			{
				if (fakeEndElement)
					return false;

				return base.HasAttributes;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				if (fakeEndElement)
					return XmlNodeType.EndElement;

				return base.NodeType;
			}
		}

		public override string GetAttribute(int i)
		{
			if (fakeEndElement)
				return String.Empty;

			return base.GetAttribute(i);
		}

		public override string GetAttribute(string localName, string namespaceURI)
		{
			if (fakeEndElement)
				return String.Empty;

			return base.GetAttribute(localName, namespaceURI);
		}

		public override string GetAttribute(string name)
		{
			if (fakeEndElement)
				return String.Empty;

			return base.GetAttribute(name);
		}

		public override void MoveToAttribute(int i)
		{
			if (!fakeEndElement)
				base.MoveToAttribute(i);
		}

		public override bool MoveToAttribute(string localName, string namespaceURI)
		{
			if (fakeEndElement)
				return false;

			return base.MoveToAttribute(localName, namespaceURI);
		}

		public override bool MoveToAttribute(string name)
		{
			if (fakeEndElement)
				return false;

			return base.MoveToAttribute(name);
		}

		public override bool MoveToFirstAttribute()
		{
			if (fakeEndElement)
				return false;

			return base.MoveToFirstAttribute();
		}

		public override bool MoveToNextAttribute()
		{
			if (fakeEndElement)
				return false;

			return base.MoveToNextAttribute();
		}

		public override bool ReadAttributeValue()
		{
			if (fakeEndElement)
				return false;

			return base.ReadAttributeValue();
		}
	}
}
