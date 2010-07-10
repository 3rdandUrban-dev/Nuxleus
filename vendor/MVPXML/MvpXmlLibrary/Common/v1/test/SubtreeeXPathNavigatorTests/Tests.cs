#region using

using System;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.IO;
using System.Diagnostics;

using Mvp.Xml.Common.XPath;
using NUnit.Framework;

#endregion

namespace Mvp.Xml.Tests.SubtreeeXPathNavigatorTests
{
	[TestFixture]
	public class SubtreeTests
	{
		[Test]
		public void SubtreeSpeed() 
		{
			XPathDocument xdoc = new XPathDocument(Globals.GetResource(Globals.LibraryResource));
			XPathNavigator nav = xdoc.CreateNavigator();
			XmlDocument doc = new XmlDocument();
			doc.Load(Globals.GetResource(Globals.LibraryResource));

			XslTransform xslt = new XslTransform();
			xslt.Load(new XmlTextReader(
				Globals.GetResource(this.GetType().Namespace + ".print_root.xsl")));

			PerfTest pt = new PerfTest();

			// Warmup
			MemoryStream stmdom = new MemoryStream();
			XmlDocument wd = new XmlDocument(); 
			wd.LoadXml(doc.DocumentElement.FirstChild.OuterXml);
			xslt.Transform(wd, null, stmdom, null);
			MemoryStream stmxpath = new MemoryStream();
			nav.MoveToRoot();
			nav.MoveToFirstChild();
			nav.MoveToFirstChild();
			xslt.Transform(new SubtreeeXPathNavigator(nav), null, stmxpath, null);
			nav = doc.CreateNavigator();

			int count = 10;
			float dom = 0;
			float xpath = 0;

			for (int i = 0; i < count; i++)
			{
				GC.Collect();
				System.Threading.Thread.Sleep(1000);

				stmdom = new MemoryStream();

				pt.Start();

				// Create a new document for each child
				foreach (XmlNode testNode in doc.DocumentElement.ChildNodes)
				{
					XmlDocument tmpDoc = new XmlDocument(); 
					tmpDoc.LoadXml(testNode.OuterXml);

					// Transform the subset.
					xslt.Transform(tmpDoc, null, stmdom, null);
				}

				dom += pt.Stop();
                
				GC.Collect();
				System.Threading.Thread.Sleep(1000);
				
				stmxpath = new MemoryStream();

				XPathExpression expr = nav.Compile("/library/book");

				pt.Start();
				XPathNodeIterator books = nav.Select(expr);
				while (books.MoveNext())
				{
					xslt.Transform(new SubtreeeXPathNavigator(books.Current), null, stmxpath, null);
				}
				
				xpath += pt.Stop();
			}

			Console.WriteLine("XmlDocument transformation: {0}", dom / count);
			Console.WriteLine("SubtreeXPathNavigator transformation: {0}", xpath / count);

			stmdom.Position = 0;
			stmxpath.Position = 0;

			Console.WriteLine(new StreamReader(stmdom).ReadToEnd());
			Console.WriteLine(new string('*', 100));
			Console.WriteLine(new string('*', 100));
			Console.WriteLine(new StreamReader(stmxpath).ReadToEnd());
		}

		[Test]
		public void SubtreeTransform() 
		{
			XslTransform tx = new XslTransform();
			tx.Load(new XmlTextReader(
				Globals.GetResource(this.GetType().Namespace + ".test.xsl")));

			string xml = @"
	<root>
		<salutations>
			<salute>Hi there <name>kzu</name>.</salute>
			<salute>Bye there <name>vga</name>.</salute>
		</salutations>
		<other>
			Hi there without salutations.
		</other>
	</root>";

			XmlDocument dom = new XmlDocument();
			dom.LoadXml(xml);
			tx.Transform(new DebuggingXPathNavigator(new XmlNodeNavigator(dom.DocumentElement.FirstChild)), 
				null, Console.Out, null);

			Console.WriteLine();

			XPathDocument doc = new XPathDocument(new StringReader(xml));
			XPathNavigator nav = doc.CreateNavigator();
			nav.MoveToRoot();
			nav.MoveToFirstChild();
			// Salutations.
			nav.MoveToFirstChild();

			tx.Transform(new DebuggingXPathNavigator(new SubtreeeXPathNavigator(nav)), null, Console.Out);
		}

		[Test]
		public void TestBooks()
		{
			XslTransform xslt = new XslTransform();     
			xslt.Load(new XmlTextReader(
				Globals.GetResource(this.GetType().Namespace + ".nodecopy.xsl")));
			XPathDocument doc = new XPathDocument(Globals.GetResource(Globals.LibraryResource));

			XmlDocument dom = new XmlDocument();
			dom.Load(Globals.GetResource(Globals.LibraryResource));

			xslt.Transform(new XmlNodeNavigator(dom.DocumentElement.FirstChild), 
				null, Console.Out, null);

			Console.WriteLine();

			//Navigator over first child of document element
			XPathNavigator nav = doc.CreateNavigator();
			nav.MoveToRoot();
			nav.MoveToFirstChild();
			nav.MoveToFirstChild();

			xslt.Transform(new SubtreeeXPathNavigator(nav), null, Console.Out, null);
		}

		[Test]
		public void TestRead()
		{
			XPathDocument doc = new XPathDocument(Globals.GetResource(Globals.LibraryResource));

			//Navigator over first child of document element
			XPathNavigator nav = doc.CreateNavigator();
			nav.MoveToRoot();
			nav.MoveToFirstChild();
			nav.MoveToFirstChild();
			XPathNavigatorReader r = new XPathNavigatorReader(nav);
			r.MoveToContent();
            Console.WriteLine(r.ReadOuterXml());
		}
	}
}
