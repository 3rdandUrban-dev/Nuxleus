using System;
using System.Xml;
using System.Globalization;

namespace Mvp.Xml.Core
{
	/// <summary>
	/// An XML name with optional prefix and local name that may contain 
	/// namespace and/or local name wildcards to specify that 
	/// the name represents any namespace and/or any local name.
	/// </summary>
	public abstract class XmlNameMatch : XmlMatch
	{
		/// <summary>
		/// The wildcard string identifier, the value <c>*</c>.
		/// </summary>
		public const string Wildcard = "*";

		bool anyNamespace = true;
		bool anyName = true;
		string name;
		string prefix;
		string fullName;
	
		/// <summary>
		/// Constructs the <see cref="XmlName"/> with the given name and 
		/// and no prefix.
		/// The value may be a <see cref="Wildcard"/>.
		/// </summary>
		public XmlNameMatch(string name) : this(String.Empty, name) { }

		/// <summary>
		/// Constructs the <see cref="XmlName"/> with the given name and prefix.
		/// Both values may be <see cref="Wildcard"/>s, and the <paramref name="prefix"/> may be 
		/// an empty string if no prefix/namespace applies.
		/// </summary>
		public XmlNameMatch(string prefix, string name)
		{
			Guard.ArgumentNotNull(prefix, "prefix");
			Guard.ArgumentNotNullOrEmptyString(name, "name");

			ThrowIfInvalidPrefix(prefix);
			ThrowIfInvalidName(name);

			this.name = name;
			this.prefix = prefix;
			anyName = IsWildcard(name);
			anyNamespace = IsWildcard(prefix);

			if (prefix.Length == 0)
			{
				fullName = name;
			}
			else
			{
				fullName = prefix + ":" + name;
			}
		}

		/// <summary>
		/// Determines whether the <paramref name="value"/> is a <see cref="Wildcard"/>.
		/// </summary>
		/// <param name="value">The value to evaluate.</param>
		public static bool IsWildcard(string value)
		{
			return value.Length == 1 && value[0] == '*';
		}

		private void ThrowIfInvalidPrefix(string prefix)
		{
			bool hasPrefix = prefix.Length > 0 && !IsWildcard(prefix);
			bool validPrefix = hasPrefix &&
				XmlReader.IsName(prefix) &&
				prefix.IndexOf(':') == -1;

			if (hasPrefix && !validPrefix)
			{
				throw new ArgumentException(String.Format(
						CultureInfo.CurrentCulture,
						Properties.Resources.InvalidPrefix,
						prefix));
			}
		}

		private void ThrowIfInvalidName(string name)
		{
			bool hasName = !IsWildcard(name);
			bool validName = hasName &&
				XmlReader.IsName(name) &&
				name.IndexOf(':') == -1;

			if (hasName && !validName)
			{
				throw new ArgumentException(String.Format(
					CultureInfo.CurrentCulture,
					Properties.Resources.InvalidName,
					name));
			}
		}

		/// <summary>
		/// Gets whether the <see cref="Prefix"/> is a wildcard (matches any namespace).
		/// </summary>
		public bool IsAnyNamespace
		{
			get { return anyNamespace; }
		}

		/// <summary>
		/// Gets whether the <see cref="Name"/> is a wildcard (matches any local name).
		/// </summary>
		public bool IsAnyName
		{
			get { return anyName; }
		}

		/// <summary>
		/// Gets the local name.
		/// </summary>
		public override string Name
		{
			get { return name; }
		}

		/// <summary>
		/// Gets the prefix.
		/// </summary>
		public override string Prefix
		{
			get { return prefix; }
		}

		/// <summary>
		/// Gets the full name (prefix and name).
		/// </summary>
		public override string FullName
		{
			get { return fullName; }
		}

		public override bool Matches(XmlReader reader, IXmlNamespaceResolver resolver)
		{
			bool nsMatches = IsAnyNamespace;
			bool nameMatches = IsAnyName;

			if (!nsMatches)
			{
				string ns = String.Empty;
				if (prefix.Length > 0)
				{
					if (resolver == null)
					{
						throw new ArgumentException(Properties.Resources.NamespaceResolverRequired, "resolver");
					}

					ns = resolver.LookupNamespace(prefix);

					if (ns == null)
					{
						throw new ArgumentException(String.Format(
							CultureInfo.CurrentCulture,
							Properties.Resources.CannotResolvePrefix,
							prefix));
					}
				}

				nsMatches = (ns == reader.NamespaceURI);
			}

			if (!nameMatches)
			{
				nameMatches = (name == reader.LocalName);
			}

			return nsMatches && nameMatches;
		}
	}
}
