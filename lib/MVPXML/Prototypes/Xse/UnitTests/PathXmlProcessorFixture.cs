#if NUNIT
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif
#if VSTS
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Mvp.Xml.Core.UnitTests
{
	[TestClass]
	public class PathXmlProcessorFixture : TestFixtureBase
	{
		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void ThrowsIfExpressionNull()
		{
			new PathXmlProcessor((string)null, delegate { }, new NameTable());
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void ThrowsIfActionNull()
		{
			new PathXmlProcessor("/root", null, new NameTable());
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void ThrowsIfNameTableNull()
		{
			new PathXmlProcessor("/root", delegate { }, (XmlNameTable)null);
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void ThrowsIfNamespaceManagerNull()
		{
			new PathXmlProcessor("/root", delegate { }, (XmlNamespaceManager)null);
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void ThrowsIfMatchListNull()
		{
			new PathXmlProcessor((IList<XmlMatch>)null, delegate {}, new NameTable());
		}

		[ExpectedException(typeof(ArgumentException))]
		[TestMethod]
		public void ThrowsIfMatchListEmpty()
		{
			new PathXmlProcessor(new List<XmlMatch>(), delegate { }, new NameTable());
		}

		[TestMethod]
		public void CanMatchRootElement()
		{
			List<XmlMatch> match = new List<XmlMatch>();
			match.Add(new ElementMatch("root", MatchMode.Element | MatchMode.RootElement));
			int matchCount = 0;

			PerformMatch(match, "<root><foo><bar/></foo></root>", delegate { matchCount++; });

			Assert.AreEqual(1, matchCount);
		}

		[TestMethod]
		public void CanMatchRootEndElement()
		{
			List<XmlMatch> match = new List<XmlMatch>();
			match.Add(new ElementMatch("root", MatchMode.EndElement | MatchMode.RootElement));
			int matchCount = 0;
			
			PerformMatch(match, "<root><foo><bar/></foo></root>", 
				delegate(XmlReader actionReader) 
				{ 
					matchCount++; 
					Assert.IsTrue(actionReader.NodeType == XmlNodeType.EndElement);
				}
			);

			Assert.AreEqual(1, matchCount);
		}

		[TestMethod]
		public void CanMatchSubElement()
		{
			List<XmlMatch> match = new List<XmlMatch>();
			match.Add(new ElementMatch("root", MatchMode.RootElement | MatchMode.Element));
			match.Add(new ElementMatch("foo"));
			int matchCount = 0;

			PerformMatch(match, "<root><foo><bar/></foo></root>", delegate { matchCount++; });

			Assert.AreEqual(1, matchCount);
		}

		[TestMethod]
		public void CanMatchSubElementWithPrefix()
		{
			List<XmlMatch> match = new List<XmlMatch>();
			match.Add(new ElementMatch("root", MatchMode.RootElement | MatchMode.Element));
			match.Add(new ElementMatch("p", "foo"));
			int matchCount = 0;

			PerformMatch(match, "<root xmlns:p='pp'><p:foo><n:bar xmlns:n='nn'/></p:foo></root>", delegate { matchCount++; });

			Assert.AreEqual(1, matchCount);
		}

		[TestMethod]
		public void CanMatchSubElement2()
		{
			List<XmlMatch> match = new List<XmlMatch>();
			match.Add(new ElementMatch("root", MatchMode.RootElement));
			match.Add(new ElementMatch("foo"));
			int matchCount = 0;

			PerformMatch(match, @"
				<root>
					<foo>
						<foo></foo>
					</foo>
					<foo></foo>
				</root>", 
				delegate { matchCount++; }
			);

			Assert.AreEqual(2, matchCount);
		}

		[TestMethod]
		public void CanMatchEmptySubElement()
		{
			List<XmlMatch> match = new List<XmlMatch>();
			match.Add(new ElementMatch("root", MatchMode.RootElement));
			match.Add(new ElementMatch("foo"));
			int matchCount = 0;

			PerformMatch(match, @"
				<root>
					<foo/>
					<foo></foo>
					<foo/>
				</root>",
				delegate { matchCount++; }
			);

			Assert.AreEqual(3, matchCount);
		}

		[TestMethod]
		public void RootDoesNotMatchAnywhere()
		{
			List<XmlMatch> match = new List<XmlMatch>();
			match.Add(new ElementMatch("root", MatchMode.RootElement));
			match.Add(new ElementMatch("foo"));
			int matchCount = 0;

			PerformMatch(match, @"
				<root>
					<foo/>
					<bar>
						<root><foo/></root>
					</bar>
				</root>",
				delegate { matchCount++; }
			);

			Assert.AreEqual(1, matchCount);
		}

		[TestMethod]
		public void NonRootSimpleElementMatchesAnywhere()
		{
			List<XmlMatch> match = new List<XmlMatch>();
			match.Add(new ElementMatch("foo"));
			int matchCount = 0;

			PerformMatch(match, @"
				<root>
					<foo/>
					<bar>
					</bar>
					<bar>
					    <foo/>
					</bar>
					<foo/>
				</root>",
				delegate { matchCount++; }
			);

					//<bar>
					//    <foo/>
					//    <bar>
					//        <foo><baz/></foo>
					//        <root><foo/></root>
					//    <foo/>
					//</bar>


			Assert.AreEqual(3, matchCount);
		}

		[TestMethod]
		public void NonRootSimpleEndElementMatchesAnywhere()
		{
			List<XmlMatch> match = new List<XmlMatch>();
			match.Add(new ElementMatch("foo", MatchMode.EndElement));
			int matchCount = 0;

			PerformMatch(match, @"
				<foo>
					<foo></foo>
					<bar>
						<root><foo></foo></root>
					</bar>
				</foo>",
				delegate { matchCount++; }
			);

			Assert.AreEqual(3, matchCount);
		}

		[TestMethod]
		public void EndElementMatchesEmptyElement()
		{
			List<XmlMatch> match = new List<XmlMatch>();
			match.Add(new ElementMatch("foo", MatchMode.EndElement));
			int matchCount = 0;

			PerformMatch(match, @"
				<foo>
					<foo/>
					<bar>
						<root><foo/></root>
					</bar>
				</foo>",
				delegate { matchCount++; }
			);

			Assert.AreEqual(3, matchCount);
		}

		[TestMethod]
		public void CanMatchAttribute()
		{
			List<XmlMatch> match = new List<XmlMatch>();
			match.Add(
				new ElementAttributeMatch(
					new ElementMatch("foo"), 
					new AttributeMatch("id")));
			
			int matchCount = 0;

			PerformMatch(match, @"
				<foo id='5' title='foo'>
					<foo/>
					<bar id='1'>
						<root><foo id='6' desc='desc'/></root>
					</bar>
				</foo>",
				delegate { matchCount++; }
			);

			Assert.AreEqual(2, matchCount);
		}

		[TestMethod]
		public void CanMatchAttributeAnywhere()
		{
			List<XmlMatch> match = new List<XmlMatch>();
			match.Add(new ElementAttributeMatch(
				new ElementMatch("*"),
				new AttributeMatch("type")));

			int matchCount = 0;

			string xml = @"
<configuration>
  <configSections>
    <section name='appSettings' type='foo' restartOnExternalChanges='false' requirePermission='false' />
    <sectionGroup name='system.xml.serialization' type='foo'>
      <section name='xmlSerializer' type='foo' requirePermission='false' />
    </sectionGroup>
  </configSections>
</configuration>";

	//<sectionGroup name='system.net' type='foo'>
	//</sectionGroup>

			PerformMatch(match, xml, 
				delegate { matchCount++; }
			);

			Assert.AreEqual(3, matchCount);
		}

		[TestMethod]
		public void MatchingResumesOnWrongSibling()
		{
			List<XmlMatch> match = new List<XmlMatch>();
			match.Add(new ElementMatch("foo", MatchMode.Element));
			match.Add(new ElementMatch("bar", MatchMode.Element));
			match.Add(new ElementMatch("foo", MatchMode.Element));
			int matchCount = 0;

			PerformMatch(match, @"
				<foo>
					<bar>
						<baz>
							<foo>
								<bar><foo/></bar>
							</foo>
						</baz>
						<foo/>
						<bar/>
						<foo/>
					</bar>
				</foo>",
				delegate { matchCount++; }
			);

			Assert.AreEqual(3, matchCount);
		}

		[TestMethod]
		public void IntegrationTest()
		{
			// count(//section)
			int sectionCount = 0;
			// count(/configuration/configSections/section)
			int topLevelSectionCount = 0;
			// count(//*/@type)
			int typeCount = 0;
			// count(@type)
			int typeAttrCount = 0;
			// count(//add)
			int addCount = 0;
			// count(/configuration/runtime/r:assemblyBinding/r:dependentAssembly)
			int dependentAssemblyCount = 0;

			XmlProcessorReader reader = new XmlProcessorReader(XmlReader.Create("machine.config"));
			reader.Processors.Add(new PathXmlProcessor("//section", delegate { sectionCount++; }, reader.NameTable));
			reader.Processors.Add(new PathXmlProcessor("/configuration/configSections/section", delegate { topLevelSectionCount++; }, reader.NameTable));
			reader.Processors.Add(new PathXmlProcessor("//*/@type", delegate { typeCount++; }, reader.NameTable));
			reader.Processors.Add(new PathXmlProcessor("@type", delegate { typeAttrCount++; }, reader.NameTable));
			reader.Processors.Add(new PathXmlProcessor("//add", delegate { addCount++; }, reader.NameTable));

			XmlNamespaceManager mgr = new XmlNamespaceManager(reader.NameTable);
			mgr.AddNamespace("r", "urn:schemas-microsoft-com:asm.v1");
			reader.Processors.Add(new PathXmlProcessor("/configuration/runtime/r:assemblyBinding/r:dependentAssembly", delegate { dependentAssemblyCount++; }, mgr));

			reader.ReadToEnd();

			Assert.AreEqual(70, sectionCount);
			Assert.AreEqual(19, topLevelSectionCount);
			Assert.AreEqual(12, addCount);
			Assert.AreEqual(10, dependentAssemblyCount);
			Assert.AreEqual(87, typeCount);
			Assert.AreEqual(87, typeAttrCount);
		}

		[TestMethod]
		public void MatchingComponent()
		{
			XmlReader reader = GetReader(@"
				<root>
					<foo id='23'>
						<baz>
							<bar/>
						</baz>
					</foo>
					<foo id='25'>
						<bar>
							<baz>
								<foo>
									<bar />
								</foo>
							</baz>
						</bar>
						<bar/>
					</foo>
				</root>");

		}

		private void PerformMatch(List<XmlMatch> matchList, string xml, Action<XmlReader> action)
		{
			XmlReader reader = GetReader(xml);
			PathXmlProcessor processor = new PathXmlProcessor(matchList, action, reader.NameTable);

			while (reader.Read())
			{
				processor.Process(reader);
				for (bool go = reader.MoveToFirstAttribute(); go; go = reader.MoveToNextAttribute())
				{
					processor.Process(reader);
				}
				if (reader.HasAttributes) reader.MoveToElement();
			}
		}
	}
}
