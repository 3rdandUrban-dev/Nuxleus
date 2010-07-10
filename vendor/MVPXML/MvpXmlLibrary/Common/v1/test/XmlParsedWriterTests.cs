using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;

using NUnit.Framework;

namespace Mvp.Xml.Tests
{
	[TestFixture]
	public class XmlParsedWriterTests
	{
		[Test]
		public void Test()
		{
			StringWriter sw = new StringWriter();
            XmlTextWriter tw = new XmlTextWriter(sw);

			tw.WriteStartDocument(true);
			tw.WriteStartElement("root", "urn:mvp-xml");
			tw.WriteAttributeString("id", "0003");
			tw.WriteAttributeString("title", "urn:mvp-xml", "The Document");
			tw.WriteElementString("foreign", "urn:aspnet2.com", "the value");
			tw.WriteElementString("local", "the local value");
			tw.WriteElementString("nonamespace", "", "no namespace element");
			tw.WriteEndDocument();
			tw.Flush();
			
			string xml = sw.ToString();

			//XmlParsedWriter w = new XmlParsedWriter();
		}
	}
}
