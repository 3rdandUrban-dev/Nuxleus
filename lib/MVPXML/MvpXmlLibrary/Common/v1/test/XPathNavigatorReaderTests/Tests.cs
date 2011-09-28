using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml.Schema;
using System.Xml.Xsl;

using NUnit.Framework;
using Mvp.Xml.Common;
using Mvp.Xml.Common.XPath;

namespace Mvp.Xml.Tests.XPathNavigatorReaderTests
{
	[TestFixture]
	public class Tests
	{
		[Test]
		public void TestComment()
		{
			string xml = @"<?xml version='1.0'?>
<!-- comment -->
<?an arbitrary PI ?>
<!-- another comment -->
<foo />";

			XmlReader tr = new XmlTextReader(new StringReader(xml));
			XmlWriter tw = new XmlTextWriter(Console.Out);
			//tw.WriteNode(tr, false);
			//Console.WriteLine();

			XPathDocument doc = new XPathDocument(new StringReader(xml));
			XPathNavigator nav = doc.CreateNavigator();

			XPathNavigatorReader reader = new XPathNavigatorReader(nav);
			//tw.WriteNode(reader, false);
			tw.Close();

			reader = new XPathNavigatorReader(nav);
			Assert.IsTrue(reader.Read());
			Assert.AreEqual(XmlNodeType.Comment, reader.NodeType);
			Assert.IsTrue(reader.Read());
			Assert.AreEqual(XmlNodeType.ProcessingInstruction, reader.NodeType);
			Assert.IsTrue(reader.Read());            
			Assert.AreEqual(XmlNodeType.Comment, reader.NodeType);
			Assert.IsTrue(reader.Read());
			Assert.AreEqual(XmlNodeType.Element, reader.NodeType);
			Assert.IsTrue(reader.IsEmptyElement);
			Assert.IsFalse(reader.Read());
		}

		[Test]
		public void OlegEvilTest()
		{
			string xml = @"<foo><?pi text?>evil</foo>";
			XPathDocument doc = new XPathDocument(new StringReader(xml));
			XPathNodeIterator ni =
				doc.CreateNavigator().Select("//processing-instruction()");
			Assert.IsTrue(ni.MoveNext());
			XPathNavigatorReader r = new XPathNavigatorReader(ni.Current);
			Assert.IsTrue(r.Read());
			Console.WriteLine("NodeType: {0}, Name: {1}, Value: {2}",
				r.NodeType, r.Name, r.Value);
			Assert.AreEqual(XmlNodeType.ProcessingInstruction, r.NodeType);
			Assert.IsTrue(r.Read());
            Assert.AreEqual(XmlNodeType.Text, r.NodeType);
            Assert.IsFalse(r.Read());
		}

		[Test]
		public void TestEmptyRoot()
		{
			string xml = "<root/>";
			XPathDocument doc = new XPathDocument(new StringReader(xml));
			XPathNavigator nav = doc.CreateNavigator();

			XPathNavigatorReader reader = new XPathNavigatorReader(nav);
			Assert.IsTrue(reader.Read());
			Assert.AreEqual(XmlNodeType.Element, reader.NodeType);
			Assert.IsTrue(reader.IsEmptyElement);
			Assert.IsFalse(reader.Read());
		}

		[Test]
		public void FragmentValidation()
		{
			#region <publishers> framgment validation

			XmlSchema sch = XmlSchema.Read(Globals.GetResource(
				this.GetType().Namespace + ".pubspublishers.xsd"), null);

			XPathDocument doc = new XPathDocument(Globals.GetResource(Globals.PubsNsResource));
			XPathNavigator nav = doc.CreateNavigator();

			// Read and validate all publishers with the partial schema.
			XmlNamespaceManager mgr = new XmlNamespaceManager(nav.NameTable);
			mgr.AddNamespace("mvp", "mvp-xml");

			XPathExpression expr = nav.Compile("/mvp:dsPubs/mvp:publishers");
			expr.SetContext(mgr);

			XPathNodeIterator it = nav.Select(expr);
			while (it.MoveNext())
			{
				// Create a validating reader over an XmlNavigatorReader for the current node.
				XmlValidatingReader vr = new XmlValidatingReader(new XPathNavigatorReader(it.Current));
				vr.Schemas.Add(sch);
				// Validate it!
				while (vr.Read()) {}
			}

			#endregion <publishers> framgment validation

			#region <titles> framgment validation

			// Now validate expensive titles separately.

			sch = XmlSchema.Read(Globals.GetResource(
				this.GetType().Namespace + ".pubstitles.xsd"), null);

			expr = nav.Compile("//mvp:titles[mvp:price>10]");
			expr.SetContext(mgr);

			it = nav.Select(expr);
			while (it.MoveNext())
			{
				// Create a validating reader over an XPathNavigatorReader for the current node.
				XmlValidatingReader vr = new XmlValidatingReader(new XPathNavigatorReader(it.Current));
				vr.Schemas.Add(sch);
				// Validate it!
				while (vr.Read()) {}
			}

			#endregion <titles> framgment validation
		}

		[Test]
		public void FragmentDeserialization()
		{
			#region <publishers> framgment deserialization

			XPathDocument doc = new XPathDocument(Globals.GetResource(Globals.PubsNsResource));
			XPathNavigator nav = doc.CreateNavigator();

			// Deserialize all publishers.
			XmlNamespaceManager mgr = new XmlNamespaceManager(nav.NameTable);
			mgr.AddNamespace("mvp", "mvp-xml");

			XPathExpression expr = nav.Compile("/mvp:dsPubs/mvp:publishers");
			expr.SetContext(mgr);

			XmlSerializer ser = new XmlSerializer(typeof(publishers));			

			XPathNodeIterator it = nav.Select(expr);
			while (it.MoveNext())
			{
				XPathNavigatorReader r = new XPathNavigatorReader(it.Current);
				object publisher = ser.Deserialize(r);
				Assert.IsNotNull(publisher);
			}

			#endregion <publishers> framgment deserialization

			#region <titles> framgment deserialization

			// Now deserialize expensive titles separately.

			expr = nav.Compile("//mvp:titles[mvp:price>10]");
			expr.SetContext(mgr);

			ser = new XmlSerializer(typeof(titles));

			it = nav.Select(expr);
			while (it.MoveNext())
			{
				XPathNavigatorReader r = new XPathNavigatorReader(it.Current);
				object title = ser.Deserialize(r);
				Assert.IsNotNull(title);
			}

			#endregion <titles> framgment deserialization
		}

		[Test]
		public void MoveToContent()
		{
			string xml = @"<root><element>1</element><element></element><element>2</element></root>";

			XPathNavigator nav = new XPathDocument(new StringReader(xml)).CreateNavigator();

			XmlReader reader = new XPathNavigatorReader(nav);

			reader.MoveToContent();
			Console.WriteLine(reader.ReadOuterXml());

			reader = new XmlTextReader(new StringReader(xml));

			reader.MoveToContent();
			Console.WriteLine(reader.ReadOuterXml());
		}

		[Test]
		public void EmptyElementRead()
		{
			string xml = @"<root><element>1</element><element></element><element>2</element></root>";

			XPathNavigator nav = new XPathDocument(new XmlTextReader(new StringReader(xml))).CreateNavigator();
			XmlReader reader = new XPathNavigatorReader(nav);
			XmlDocument doc = new XmlDocument();
			doc.Load(reader);

			Console.WriteLine(doc.OuterXml);

			xml = @"<root><element>1</element><element/><element>2</element></root>";

			nav = new XPathDocument(new XmlTextReader(new StringReader(xml))).CreateNavigator();
			reader = new XPathNavigatorReader(nav);
			doc = new XmlDocument();
			doc.Load(reader);

			Console.WriteLine(doc.OuterXml);
		}

		[Test]
		public void XmlReaderNames()
		{
			string xml = "<customer id='1' pp:id='aba' xmlns='urn-kzu' xmlns:pp='urn-pepenamespace'><pp:order /><order id='1'>Chocolates</order></customer>";

			XPathNavigator nav = new XPathDocument(new StringReader(xml)).CreateNavigator();
			XmlReader reader = new XPathNavigatorReader(nav);

			reader.MoveToContent();
			Assert.AreEqual("customer", reader.Name);
			
			reader.MoveToFirstAttribute();
			Assert.AreEqual("id", reader.Name);

			reader.MoveToNextAttribute();
			Assert.AreEqual("pp:id", reader.Name);
		
			// Namespace order is not guaranteed.
            reader.MoveToNextAttribute();
			Assert.IsTrue( reader.Name == "xmlns:pp" || reader.Name == "xmlns" );
			reader.MoveToNextAttribute();
			Assert.IsTrue( reader.Name == "xmlns:pp" || reader.Name == "xmlns" );

			reader.MoveToElement();
			reader.Read();

			Assert.AreEqual("pp:order", reader.Name);
		}

		
		[Test]
		public void XmlDocumentFragmentLoading()
		{
			string xml = "<customer id='1' pp:id='aba' xmlns='urn-kzu' xmlns:pp='urn-pepenamespace'><pp:order /><order id='1'>Chocolates</order></customer>";

			XPathNavigator nav = new XPathDocument(new StringReader(xml)).CreateNavigator();

			// <customer>
			nav.MoveToFirstChild();
			// <pp:order>
			nav.MoveToFirstChild();
			// <order>
			nav.MoveToNext();
			
			XmlReader reader = new XPathNavigatorReader(nav);
			XmlDocument doc = new XmlDocument(nav.NameTable);
			doc.Load( reader );
           
			Assert.AreEqual("order", doc.DocumentElement.LocalName);
			Assert.AreEqual("urn-kzu", doc.DocumentElement.NamespaceURI);
			Assert.AreEqual(1, doc.DocumentElement.Attributes.Count);
		}

		
		[Test]
		public void XmlDocumentLoading()
		{
			string xml = "<customer id='1' pp:id='aba' xmlns='urn-kzu' xmlns:pp='urn-pepenamespace'><pp:order /><order id='1'>Chocolates</order></customer>";

			XPathNavigator nav = new XPathDocument(new StringReader(xml)).CreateNavigator();

			XmlReader reader = new XPathNavigatorReader(nav);
			//XmlReader reader = new XmlTextReader(new StringReader(xml));

			//reader.MoveToContent();
			//Console.WriteLine(reader.ReadOuterXml());

			System.CodeDom.Compiler.IndentedTextWriter tw = new System.CodeDom.Compiler.IndentedTextWriter(Console.Out);

			while (reader.Read())
			{
				tw.Indent = reader.Depth;
				tw.WriteLine("Name={0}, Type={1}", reader.Name, reader.NodeType);
				int count = reader.AttributeCount;
				//reader.MoveToFirstAttribute();
				//for (int i = 0; i < count; i++) 
				while (reader.MoveToNextAttribute())
				{
					tw.Indent = reader.Depth;
					tw.Write("{0}=", reader.Name);
					reader.ReadAttributeValue();
					tw.WriteLine(reader.Value);
					//reader.MoveToNextAttribute();
				}
			}

			//reader = new DebuggingXmlTextReader(new XPathNavigatorReader(nav));
			reader = new XPathNavigatorReader(nav);
			XmlDocument doc = new XmlDocument(nav.NameTable);
			doc.Load( reader );
           
			Assert.AreEqual("customer", doc.DocumentElement.LocalName);
			Assert.AreEqual("urn-kzu", doc.DocumentElement.NamespaceURI);
			Assert.AreEqual(4, doc.DocumentElement.Attributes.Count);
			Assert.AreEqual(2, doc.DocumentElement.ChildNodes.Count);
		}

		
		[Test]
		public void XPathDocumentLoading()
		{
			string xml = "<customer id='1' pp:id='aba' xmlns='urn-kzu' xmlns:pp='urn-pepenamespace'><pp:order /><order id='1'>Chocolates</order></customer>";

			XPathNavigator nav = new XPathDocument(new StringReader(xml)).CreateNavigator();

			XmlReader reader = new XPathNavigatorReader(nav);
			XPathNavigator clone = new XPathDocument(reader).CreateNavigator();
           
			clone.MoveToFirstChild();
			Assert.AreEqual("customer", clone.LocalName);
			Assert.AreEqual("urn-kzu", clone.NamespaceURI);

			clone.MoveToAttribute("id", "urn-pepenamespace");
			Assert.AreEqual("aba", clone.Value);

			clone.MoveToParent();
			clone.MoveToFirstChild();
			Assert.AreEqual("pp:order", clone.Name);
			Assert.IsTrue(clone.IsEmptyElement);			
		}

		
		[Test]
		public void TrasformItems()
		{
			string xml = @"<library>
  <book genre='novel' ISBN='1-861001-57-5'>
     <title>Pride And Prejudice</title>
  </book>
  <book genre='novel' ISBN='1-81920-21-2'>
     <title>Hook</title>
  </book>
</library>";
			string xsl = @"<stylesheet version='1.0' xmlns='http://www.w3.org/1999/XSL/Transform' >
  <output method='text' /> 
  <template match='/'>
     <variable name='first' select='/*[1]/*[1]' />
     <variable name='second' select='/publishers/pub_id' />
     <variable name='third' select='//*[title_id=""BU2075""]' />
  </template>
</stylesheet>";

			XslTransform xslt = new XslTransform();     
			xslt.Load(new XmlTextReader(new StringReader(xsl)), 
				null, this.GetType().Assembly.Evidence);

			XPathDocument wp = new XPathDocument(Globals.GetResource(Globals.PubsResource));
			XmlDocument wd = new XmlDocument();
			wd.Load(Globals.GetResource(Globals.PubsResource));

			PerfTest p = new PerfTest();
			p.Start();

			// Load the entire doc.
			XPathDocument doc = new XPathDocument(Globals.GetResource(Globals.PubsResource));
			// Create a new document for each child
			XPathNodeIterator books = doc.CreateNavigator().Select("/dsPubs/publishers");
			while (books.MoveNext())
			{
				XPathDocument tmpDoc = new XPathDocument(
					new XPathNavigatorReader(books.Current)); 

				// Pass the document containing the node fragment 
				// to the Transform method.
				xslt.Transform(tmpDoc, null, Console.Out, null);
			}

			Console.WriteLine("\nElapsed: {0}", p.Stop());

			p = new PerfTest();
			p.Start();

			// Load the entire doc.
			XmlDocument xdoc = new XmlDocument();
			xdoc.Load(Globals.GetResource(Globals.PubsResource));
			// Create a new document containing just the node fragment.
			
			// Create a new document for each child
			foreach (XmlNode testNode in xdoc.SelectNodes("/dsPubs/publishers"))
			{
				XmlDocument tmpDoc = new XmlDocument(); 
				tmpDoc.LoadXml(testNode.OuterXml);
				// Pass the document containing the node fragment 
				// to the Transform method.
				xslt.Transform(tmpDoc, null, Console.Out, null);
			}

			Console.WriteLine("\nElapsed: {0}", p.Stop());
		}

		[Test]
		public void XPathDocumentFragmentLoading()
		{
			string xml = "<customer id='1' pp:id='aba' xmlns='urn-kzu' xmlns:pp='urn-pepenamespace'><pp:order /><order id='1'>Chocolates</order></customer>";

			XPathNavigator nav = new XPathDocument(new StringReader(xml)).CreateNavigator();

			// <customer>
			nav.MoveToFirstChild();
			// <pp:order>
			nav.MoveToFirstChild();
			// <order>
			nav.MoveToNext();
			
			XmlReader reader = new XPathNavigatorReader(nav);
			XPathNavigator clone = new XPathDocument(reader).CreateNavigator();
			clone.MoveToFirstChild();
           
			Assert.AreEqual("order", clone.LocalName);
			Assert.AreEqual("urn-kzu", clone.NamespaceURI);
		}

		[Test]
		public void ExposingXmlFragment()
		{
			string xml = @"
<library>
  <book genre='novel' ISBN='1-861001-57-5'>
     <title>Pride And Prejudice</title>
  </book>
  <book genre='novel' ISBN='1-81920-21-2'>
     <title>Hook</title>
  </book>
</library>";

			XPathNavigator nav = new XPathDocument(new StringReader(xml)).CreateNavigator();

			// <library>
			nav.MoveToFirstChild();
			// <book>
			nav.MoveToFirstChild();
			
			// Enable fragment reading.
			XPathNavigatorReader reader = new XPathNavigatorReader(nav, true);
			reader.MoveToContent();
			string books = reader.ReadFragmentXml();
           
			Assert.AreEqual(
				"<book genre=\"novel\" ISBN=\"1-861001-57-5\"><title>Pride And Prejudice</title></book><book genre=\"novel\" ISBN=\"1-81920-21-2\"><title>Hook</title></book>", 
				books);
		}

		[Test]
		public void ExposingSubtreeXmlFragment()
		{
			string xml = @"
<library>
  <book genre='novel' ISBN='1-861001-57-5'>
     <title>Pride And Prejudice</title>
  </book>
  <book genre='novel' ISBN='1-81920-21-2'>
     <title>Hook</title>
  </book>
</library>";

			XPathNavigator nav = new XPathDocument(new StringReader(xml)).CreateNavigator();

			// <library>
			nav.MoveToFirstChild();
			// <book>
			nav.MoveToFirstChild();

			SubtreeeXPathNavigator sub = new SubtreeeXPathNavigator(nav, true);			
			// Enable fragment reading.
			XPathNavigatorReader reader = new XPathNavigatorReader(sub, true);
			reader.MoveToContent();
			string books = reader.ReadFragmentXml();

			sub.MoveToRoot();
			reader = new XPathNavigatorReader(sub, true);
			reader.MoveToContent();
			string books2 = reader.ReadFragmentXml();

			Assert.AreEqual(books, books2);
			Assert.AreEqual(
				"<book genre=\"novel\" ISBN=\"1-861001-57-5\"><title>Pride And Prejudice</title></book><book genre=\"novel\" ISBN=\"1-81920-21-2\"><title>Hook</title></book>", 
				books);
		}

		[Test]
		public void SimpleReading()
		{
			XPathDocument doc = new XPathDocument(new StringReader(
				"<customer id='1' pp:id='aba' xmlns='urn-kzu' xmlns:pp='urn-pepenamespace'><order id='1'>Chocolates</order></customer>"));
			XPathNavigator nav = doc.CreateNavigator();

			XmlReader reader = new XPathNavigatorReader(nav);

			reader.Read();
			Assert.AreEqual("customer", reader.LocalName);
			Assert.AreEqual("urn-kzu", reader.NamespaceURI);
			Assert.AreEqual(4, reader.AttributeCount);
			// @id
			reader.MoveToFirstAttribute();
			Assert.AreEqual("id", reader.LocalName);
			// @pp:id
			reader.MoveToNextAttribute();
			Assert.AreEqual("id", reader.LocalName);
			Assert.AreEqual("urn-pepenamespace", reader.NamespaceURI);
			// xmlns="urn-kzu" and xmlns:pp="urn-pepenamespace". Order is not guaranteed.
			reader.MoveToNextAttribute();
			Assert.AreEqual(XmlNamespaces.XmlNs, reader.NamespaceURI);
			reader.MoveToNextAttribute();
			Assert.AreEqual(XmlNamespaces.XmlNs, reader.NamespaceURI);
            
			reader.Read();
			Assert.AreEqual("order", reader.LocalName);
			Assert.AreEqual("urn-kzu", reader.NamespaceURI);
			Assert.AreEqual(1, reader.AttributeCount);
			reader.MoveToFirstAttribute();
			Assert.AreEqual("id", reader.LocalName);
			Assert.IsTrue(reader.ReadAttributeValue());
            Assert.AreEqual("1", reader.Value);
			Assert.IsFalse(reader.MoveToNextAttribute());

			reader.Read();
			Assert.AreEqual(XmlNodeType.Text, reader.NodeType);
			Assert.AreEqual("Chocolates", reader.Value);

			reader.Read();
			Assert.AreEqual("order", reader.LocalName);
			Assert.AreEqual(XmlNodeType.EndElement, reader.NodeType);

			reader.Read();
			Assert.AreEqual("customer", reader.LocalName);
			Assert.AreEqual(XmlNodeType.EndElement, reader.NodeType);

			reader.Read();
			Assert.AreEqual(ReadState.EndOfFile, reader.ReadState);
		}

		
		[Test]
		public void Reading()
		{
			Stream stm = Globals.GetResource(Globals.PubsNsResource);
			XmlTextReader tr = new XmlTextReader(stm);

			XPathDocument doc = new XPathDocument(tr);
			XPathNavigator nav = doc.CreateNavigator();

			// Dumps the entire document to output.
			XmlTextWriter tw = new XmlTextWriter(Console.Out);
			tw.WriteNode(new XPathNavigatorReader(nav), true);

			// Perform a query, then dump the first node (OuterXml) from the result.
			nav = doc.CreateNavigator();
			XmlNamespaceManager mgr = new XmlNamespaceManager(nav.NameTable);
			mgr.AddNamespace("mvp", "mvp-xml");
			XPathNodeIterator it = XPathCache.Select("//mvp:titles[mvp:title_id='BU2075']", nav, mgr);
			if (it.MoveNext())
			{
				XmlReader title = new XPathNavigatorReader(it.Current);
				// As it's a regular reader, we must first move it to content as usual.
				title.MoveToContent();
				Console.WriteLine(title.ReadOuterXml());
			}

			// Perform arbitrary movements, then serialize inner content.
			nav = doc.CreateNavigator();
			nav.MoveToFirstChild();
			nav.MoveToFirstChild();
			Console.WriteLine(new XPathNavigatorReader(nav).ReadInnerXml());
		}

		[Test]
		public void FullDocValidation()
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(Globals.GetResource(Globals.PubsNsResource));
			XPathDocument xdoc = new XPathDocument(Globals.GetResource(Globals.PubsNsResource));

			XmlSchema xsd = XmlSchema.Read(
				Globals.GetResource(Globals.PubsNsSchemaResource), null);
			xsd.Compile(null);

			// Warmup
			XmlReader r = new XmlValidatingReader(
				new XmlTextReader(new StringReader(doc.OuterXml)));
			while (r.Read()) {}
			r = new XmlValidatingReader(
				new XPathNavigatorReader(doc.CreateNavigator()));
			while (r.Read()) {}
			r = new XmlValidatingReader(
				new XPathNavigatorReader(xdoc.CreateNavigator()));
			while (r.Read()) {}

			PerfTest pt = new PerfTest();

			int count = 10;
			float xml = 0;
			float xpath = 0;
			float xdoctime = 0;

			for (int i = 0; i < count; i++)
			{
				pt.Start();

				r = new XmlValidatingReader(
					new XmlTextReader(new StringReader(doc.OuterXml)));
				while (r.Read()) {}

				xml += pt.Stop();
             	
				pt.Start();

				r = new XmlValidatingReader(
					new XPathNavigatorReader(doc.CreateNavigator()));
				while (r.Read()) {}
				
				xpath += pt.Stop();

				pt.Start();

				r = new XmlValidatingReader(
					new XPathNavigatorReader(xdoc.CreateNavigator()));
				while (r.Read()) {}

				xdoctime += pt.Stop();
			}

			Console.WriteLine("OuterXml validation: {0}", xml / count);
			Console.WriteLine("XPathNavigatorReader validation: {0}", xpath / count);
			Console.WriteLine("XPathDocument/Reader validation: {0}", xdoctime / count);
		}
		
		[Test]
		public void ReadInnerXml()
		{
			string xml = @"
<content type='application/xhtml+xml' xml:base='http://loluyede.blogspot.com' xml:lang='en-US' xml:space='preserve'>
	<div xmlns='http://www.w3.org/1999/xhtml'>I have 3 accounts to give away, let me know if you want them</div>
</content>";

			XPathDocument doc = new XPathDocument(new StringReader(xml));
			XPathNavigator nav = doc.CreateNavigator();

			XPathNavigatorReader r = new XPathNavigatorReader(nav);
			r.MoveToContent();
			string content = r.ReadInnerXml();
			
			Assert.AreEqual(
				"\r\n\t<div xmlns=\"http://www.w3.org/1999/xhtml\">I have 3 accounts to give away, let me know if you want them</div>\r\n",
				content);
		}

		[Test]
		public void TestSiblingRead()
		{
			string xml = @"<foo><bar/><baz/></foo>"; 

			XmlTextReader tr = new XmlTextReader(new StringReader(xml));
			tr.MoveToContent();
			Assert.AreEqual("foo", tr.LocalName);

			XPathDocument doc = new XPathDocument(new StringReader(xml)); 
			XPathNavigator nav = doc.CreateNavigator(); 
			XPathNodeIterator ni = nav.Select("/foo/bar"); 
			if (ni.MoveNext()) 
			{
				XPathNavigatorReader r = new XPathNavigatorReader(ni.Current);
				Assert.IsTrue(r.Read());
				Assert.AreEqual("bar", r.LocalName);
				Assert.IsFalse(r.Read());
				r.Close();
			}
		}

		#region Debug members

		[Test]
		[Ignore("For prototyping only")]
		public void DumpAbstractMembers()
		{
			Type t = typeof(XmlReader);
			MemberInfo[] members =
				t.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			System.Diagnostics.Debug.Listeners.Add(new 
				System.Diagnostics.TextWriterTraceListener(
				@"E:\abstracts.txt", "txt"));

			foreach (MemberInfo member in members)
			{
				if (member is MethodInfo)
				{
					MethodInfo method = (MethodInfo) member;
					if (method.IsAbstract && 
						!method.Name.StartsWith("get_") && 
						!method.Name.StartsWith("set_"))
					{
						System.Diagnostics.Debug.WriteLine(method.Name);
					}
				}
				else if (member is PropertyInfo)
				{
					PropertyInfo property = (PropertyInfo) member;
					MethodInfo m = property.GetAccessors(true)[0];
					if (m.IsAbstract)
					{
						System.Diagnostics.Debug.WriteLine(property.Name);
					}
				}
			}

			System.Diagnostics.Debug.Listeners["txt"].Flush();
		}

		[Test]
		[Ignore("For debugging only")]
		public void DebugXmlTextReader()
		{
			System.Diagnostics.Debug.Listeners.Add(new 
				System.Diagnostics.TextWriterTraceListener(
				@"E:\calls.txt", "txt"));
			StreamReader sr = new StreamReader(Globals.GetResource(Globals.PubsNsResource));

			XmlTextReader reader = new DebuggingXmlTextReader(sr);
//			XmlValidatingReader vr = new XmlValidatingReader(reader);
//			vr.Schemas.Add(System.Xml.Schema.XmlSchema.Read(
//				Globals.GetResource(Globals.PubsSchemaResource), null));
//
//			vr.MoveToContent();
//			System.Diagnostics.Debug.WriteLine(vr.ReadOuterXml());

			XmlTextWriter tw = new XmlTextWriter(new StringWriter());
			tw.WriteNode(reader, true);
		}

		#endregion Debug members
	}
}