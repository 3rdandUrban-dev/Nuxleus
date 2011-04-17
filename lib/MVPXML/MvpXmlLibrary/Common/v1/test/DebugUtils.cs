using System;
using System.Xml;
using System.Xml.XPath;

namespace Mvp.Xml.Tests
{
	/// <summary>
	/// Miscelaneous debug utilities.
	/// </summary>
	public class DebugUtils
	{
		private DebugUtils() {}

		public static void XPathNodeIteratorToConsole(XPathNodeIterator iterator)
		{
			Console.WriteLine(new string('-', 50));
			XmlTextWriter tw = new XmlTextWriter(Console.Out);
			tw.Formatting = Formatting.Indented;

			while (iterator.MoveNext())
			{
				tw.WriteNode(new Mvp.Xml.Common.XPath.XPathNavigatorReader(iterator.Current), false);
			}

			tw.Flush();
			Console.WriteLine();
			Console.WriteLine(new string('-', 50));
		}
	}
}
