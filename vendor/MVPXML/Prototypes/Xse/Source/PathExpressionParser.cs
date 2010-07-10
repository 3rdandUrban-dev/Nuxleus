using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Mvp.Xml.Core
{
	/// <summary>
	/// Helper class that parses expressions into a corresponding 
	/// list of <see cref="XmlMatch"/>.
	/// </summary>
	public class PathExpressionParser
	{
		public static List<XmlMatch> Parse(string expression)
		{
			return Parse(expression, false);
		}

		public static List<XmlMatch> Parse(string expression, bool matchEndElement)
		{
			Guard.ArgumentNotNullOrEmptyString(expression, "expression");

			bool isRoot = false;

			List<XmlMatch> names = new List<XmlMatch>();
			string normalized = expression;
			if (normalized.StartsWith("//", StringComparison.Ordinal))
			{
				normalized = normalized.Substring(2);
			}
			else if (normalized.StartsWith("/", StringComparison.Ordinal))
			{
				isRoot = true;
				normalized = normalized.Substring(1);
			}

			string[] paths = normalized.Split('/');

			try
			{
				for (int i = 0; i < paths.Length; i++)
				{
					string path = paths[i];
					if (path.Length == 0)
						ThrowInvalidPath(expression);

					// Attribute match can only be the last in the expression.
					if (path.StartsWith("@", StringComparison.Ordinal) && 
						i != paths.Length - 1)
					{
						throw new ArgumentException(String.Format(
							CultureInfo.CurrentCulture, 
							Properties.Resources.AttributeAxisInvalid, 
							expression));
					}

					XmlMatch match;

					string[] xmlName = path.Split(':');
					if (xmlName.Length == 2)
					{
						string prefix = xmlName[0];
						string name = xmlName[1];

						if (prefix.Length == 0)
							ThrowInvalidPath(expression);
						else if (name.Length == 0)
							ThrowInvalidPath(expression);

						match = CreateMatch(
							isRoot && i == 0, 
							matchEndElement && i == paths.Length - 1,
							prefix, name);
					}
					else
					{
						string name = xmlName[0];
						match = CreateMatch(
							isRoot && i == 0,
							matchEndElement && i == paths.Length - 1,
							String.Empty, xmlName[0]);
					}

					if (match is AttributeMatch && names.Count > 0)
					{
						// Build a composite that matches element with the given attribute.
						ElementMatch parent = names[names.Count - 1] as ElementMatch;

						if (matchEndElement)
							throw new ArgumentException(Properties.Resources.AttributeMatchCannotBeEndElement);
						
						names.RemoveAt(names.Count - 1);
						names.Add(new ElementAttributeMatch(parent, (AttributeMatch)match));
					}
					else
					{
						names.Add(match);
					}
				}
			}
			catch (ArgumentException aex)
			{
				throw new ArgumentException(String.Format(
					CultureInfo.CurrentCulture,
					Properties.Resources.InvalidPath,
					expression), aex);
			}


			return names;
		}

		private static XmlMatch CreateMatch(bool isRootMatch, bool isEndElement, string prefix, string name)
		{
			if (name.StartsWith("@", StringComparison.Ordinal))
			{
				return new AttributeMatch(prefix, name.Substring(1));
			}
			else if (prefix.StartsWith("@", StringComparison.Ordinal))
			{
				return new AttributeMatch(prefix.Substring(1), name);
			}
			else
			{
				MatchMode mode;
				if (isRootMatch)
				{
					mode = isEndElement ? MatchMode.RootEndElement : MatchMode.RootElement;
				}
				else
				{
					mode = isEndElement ? MatchMode.EndElement : MatchMode.Element;
				}

				return new ElementMatch(prefix, name, mode);
			}
		}

		private static void ThrowInvalidPath(string path)
		{
			throw new ArgumentException(String.Format(
				CultureInfo.CurrentCulture,
				Properties.Resources.InvalidPath,
				path));
		}
	}
}
