using System;
using System.Xml;
using System.Globalization;

namespace Mvp.Xml.Core
{
	/// <summary>
	/// A <see cref="XmlNameMatch"/> that only matches elements, optionally 
	/// only at the root of the document (<c>XmlReader.Depth == 0</c>).
	/// </summary>
	public class ElementMatch : XmlNameMatch
	{
		MatchMode mode = MatchMode.Default;

		/// <summary>
		/// Constructs the <see cref="XmlName"/> with the given name and 
		/// and no prefix.
		/// </summary>
		public ElementMatch(string name)
			: base(name)
		{
		}


		/// <summary>
		/// Constructs the <see cref="XmlName"/> with the given name and 
		/// and no prefix.
		/// </summary>
		public ElementMatch(string name, MatchMode mode)
			: base(name)
		{
			if (!Enum.IsDefined(typeof(MatchMode), mode))
				throw new ArgumentException(Properties.Resources.InvalidMode, "mode");
			this.mode = mode;
		}

		/// <summary>
		/// Constructs the <see cref="XmlName"/> with the given name and 
		/// and no prefix.
		/// </summary>
		public ElementMatch(string prefix, string name)
			: base(prefix, name)
		{
		}

		/// <summary>
		/// Constructs the <see cref="XmlName"/> with the given name and prefix.
		/// </summary>
		public ElementMatch(string prefix, string name, MatchMode mode)
			: base(prefix, name)
		{
			this.mode = mode;
		}

		public override bool Matches(XmlReader reader, IXmlNamespaceResolver resolver)
		{
			bool preCondition = false;

			switch (mode)
			{
				case MatchMode.RootElement:
					preCondition = reader.NodeType == XmlNodeType.Element && reader.Depth == 0;
					break;
				case MatchMode.RootEndElement:
					preCondition = reader.NodeType == XmlNodeType.EndElement && reader.Depth == 0;
					break;
				case MatchMode.Element:
					preCondition = reader.NodeType == XmlNodeType.Element;
					break;
				case MatchMode.EndElement:
					preCondition = reader.NodeType == XmlNodeType.EndElement;
					break;
			}

			return preCondition && base.Matches(reader, resolver);
		}

		public MatchMode Mode
		{
			get { return mode; }
		}
	}
}
