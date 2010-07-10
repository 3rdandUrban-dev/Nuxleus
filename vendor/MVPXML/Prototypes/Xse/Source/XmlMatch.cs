using System;
using System.Xml;
using System.Globalization;

namespace Mvp.Xml.Core
{
	/// <summary>
	/// Represents a prefix, local name and matching logic 
	/// to determine if an <see cref="XmlReader"/> is positioned 
	/// in an XML node that matches the instance.
	/// </summary>
	/// <remarks>
	/// Does not implement the rules for XML naming so that it 
	/// allows derived classes to implement wildcard support.
	/// </remarks>
	public abstract class XmlMatch
	{
		protected XmlMatch() { }

		/// <summary>
		/// Gets the local name.
		/// </summary>
		public abstract string Name { get; }

		/// <summary>
		/// Gets the prefix.
		/// </summary>
		public abstract string Prefix { get; }

		/// <summary>
		/// Gets the full name (prefix and name).
		/// </summary>
		public virtual string FullName
		{
			get
			{
				if (Prefix.Length == 0)
				{
					return Name;
				}
				else
				{
					return Prefix + ":" + Name;
				}
			}
		}

		/// <summary>
		/// When implemented in a derived class, determines whether the received 
		/// <see cref="XmlReader"/> current position matches the current instance.
		/// </summary>
		public abstract bool Matches(XmlReader reader, IXmlNamespaceResolver resolver);

		/// <summary>
		/// Renders the string representation of the <see cref="XmlName"/>.
		/// </summary>
		public override string ToString()
		{
			return FullName;
		}
	}
}