using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Mvp.Xml.Core
{
	public class ElementAttributeMatch : XmlMatch
	{
		ElementMatch elementMatch;
		AttributeMatch attributeMatch;
		bool matchedElement;

		public ElementAttributeMatch(ElementMatch elementMatch, AttributeMatch attributeMatch)
		{
			Guard.ArgumentNotNull(elementMatch, "elementMatch");
			Guard.ArgumentNotNull(attributeMatch, "attributeMatch");

			this.elementMatch = elementMatch;
			this.attributeMatch = attributeMatch;
		}

		public override string Name
		{
			get { return String.Empty; }
		}

		public override string Prefix
		{
			get { return String.Empty; }
		}

		public override string FullName
		{
			get
			{
				return elementMatch.ToString() + "/@" + attributeMatch.ToString();
			}
		}

		public override bool Matches(XmlReader reader, IXmlNamespaceResolver resolver)
		{
			Guard.ArgumentNotNull(reader, "reader");
			Guard.ArgumentNotNull(resolver, "resolver");

			if (reader.NodeType == XmlNodeType.Attribute)
			{
				if (matchedElement && attributeMatch.Matches(reader, resolver))
					return true;
			}
			else
			{
				matchedElement = elementMatch.Matches(reader, resolver);
			}

			return false;
		}
	}
}
