using System;
using System.IO;
using System.Xml;

using Mvp.Xml.Common;
using NUnit.Framework;

namespace Mvp.Xml.Tests.XmlFragments
{
	[TestFixture]
	public class Tests
	{
		[Test]
		public void ReadFragments()
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(new XmlTextReader(null, new XmlFragmentStream(Globals.GetResource(
				this.GetType().Namespace + ".publishers.xml"))));

			doc = new XmlDocument();
			XmlNamespaceManager mgr = new XmlNamespaceManager(doc.NameTable);
			XmlTextReader tr = new XmlTextReader(Globals.GetResource(
				this.GetType().Namespace + ".publishers.xml"), XmlNodeType.Element, 
				new XmlParserContext( doc.NameTable, mgr, null, XmlSpace.None));
			while (tr.Read())
			{
				Console.Write(doc.OuterXml);
			}
		}

		[Test]
		public void ReadFragmentsRoot()
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(new XmlTextReader(new XmlFragmentStream(Globals.GetResource(
				this.GetType().Namespace + ".publishers.xml"), "pubs")));
			Console.Write(doc.OuterXml);
		}

		[Test]
		public void ReadFragmentsRootNs()
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(new XmlTextReader(new XmlFragmentStream(Globals.GetResource(
				this.GetType().Namespace + ".publishers.xml"), "pubs", "mvp-xml")));
			Console.Write(doc.OuterXml);
		}
	}
}
