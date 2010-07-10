using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml.Schema;

using NUnit.Framework;
using Mvp.Xml.Common.XPath;

namespace Mvp.Xml.Tests.XPathIteratorReaderTests
{
	[TestFixture]
	public class Tests
	{
		[Test]
		public void TestRss()
		{
			string theWord = "XML";
			XPathDocument doc = new XPathDocument(Globals.GetResource( 
				this.GetType().Namespace + ".rss.xml"));
			XPathNodeIterator it = doc.CreateNavigator().Select(
				"/rss/channel/item[contains(title,'" + theWord + "')]");

			XPathIteratorReader reader = new XPathIteratorReader(it);
			reader.MoveToContent();
			string xml = reader.ReadOuterXml();

			using (StreamWriter sw = new StreamWriter(@"c:\subset.xml", false))
			{
				XmlTextWriter tw = new XmlTextWriter(sw);
				tw.WriteNode(new XPathIteratorReader(it), false);
				tw.Close();
			}

			Assert.IsTrue(xml != String.Empty);
		}

		[Test]
		public void Test1()
		{
			XPathDocument doc = new XPathDocument(Globals.GetResource(Globals.PubsResource));
			XPathNodeIterator it = doc.CreateNavigator().Select("//price[text() < 5]");

			XPathIteratorReader reader = new XPathIteratorReader(it, "prices");
			reader.MoveToContent();
			string xml = reader.ReadOuterXml();

			Assert.IsTrue(xml != String.Empty);
		}

		[Test]
		public void FunctionalTest()
		{
			XPathDocument doc = new XPathDocument(new StringReader(
				"<customer xmlns='mvp-xml'><order id='1'/><order id='2'/><order id='5'/></customer>"));
			XPathNavigator nav = doc.CreateNavigator();

			XmlNamespaceManager mgr = new XmlNamespaceManager(nav.NameTable);
			mgr.AddNamespace("mvp", "mvp-xml");
			// On purpose, the query is wrong because it doesn't use the prefix.
			XPathExpression expr = nav.Compile("//order[@id < 3]");
			expr.SetContext(mgr);
			XPathNodeIterator it = nav.	Select(expr);

			XPathIteratorReader reader = new XPathIteratorReader(it, "orders");
			reader.MoveToContent();
			string xml = reader.ReadOuterXml();

			Assert.AreEqual("<orders></orders>", xml);

			// With the right query now.
			expr = nav.Compile("//mvp:order[@id < 3]");
			expr.SetContext(mgr);
			it = nav.Select(expr);

			reader = new XPathIteratorReader(it, "orders");
			reader.MoveToContent();
			xml = reader.ReadOuterXml();

			Assert.AreEqual("<orders><order id=\"1\" xmlns=\"mvp-xml\" /><order id=\"2\" xmlns=\"mvp-xml\" /></orders>", xml);
		}
	}
}