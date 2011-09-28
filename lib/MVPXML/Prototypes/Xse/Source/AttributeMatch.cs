using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Mvp.Xml.Core
{
	public class AttributeMatch : XmlNameMatch
	{

		/// <summary>
		/// Constructs the <see cref="XmlName"/> with the given name and 
		/// and no prefix.
		/// </summary>
		public AttributeMatch(string name)
			: base(name) 
		{
		}

		/// <summary>
		/// Constructs the <see cref="XmlName"/> with the given name and 
		/// and no prefix.
		/// </summary>
		public AttributeMatch(string prefix, string name)
			: base(prefix, name)
		{
		}

		public override bool Matches(XmlReader reader, IXmlNamespaceResolver resolver)
		{
			return reader.NodeType == XmlNodeType.Attribute &&
				base.Matches(reader, resolver);
		}
	}
}
