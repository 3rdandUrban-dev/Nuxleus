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
	public class XmlNameMatchFixture : TestFixtureBase
	{
		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void ThrowsIfNameNull()
		{
			new ElementMatch(null);
		}

		[ExpectedException(typeof(ArgumentException))]
		[TestMethod]
		public void ThrowsIfNameEmpty()
		{
			new ElementMatch(String.Empty);
		}

		[ExpectedException(typeof(ArgumentException))]
		[TestMethod]
		public void ThrowsIfNameEmpty2()
		{
			new ElementMatch("foo", String.Empty);
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void ThrowsIfPrefixNull()
		{
			new ElementMatch(null, "foo");
		}

		[TestMethod]
		public void FullNameContainsPrefixAndName()
		{
			XmlMatch name = new ElementMatch("foo", "bar");

			Assert.AreEqual("foo:bar", name.FullName);
		}

		[TestMethod]
		public void FullNameContainsPrefixAndName2()
		{
			XmlMatch name = new AttributeMatch("foo", "bar");

			Assert.AreEqual("foo:bar", name.FullName);
		}

		[TestMethod]
		public void ToStringEqualsFullName()
		{
			XmlMatch name = new ElementMatch("foo", "bar");

			Assert.AreEqual(name.FullName, name.ToString());
		}

		[TestMethod]
		public void FullNameDoesNotContainColonForEmptyPrefix()
		{
			XmlMatch name = new ElementMatch(String.Empty, "foo");

			Assert.AreEqual("foo", name.FullName);
		}

		[ExpectedException(typeof(ArgumentException))]
		[TestMethod]
		public void ThrowsIfPrefixIsInvalidNCName()
		{
			new ElementMatch("123", "foo");
		}

		[ExpectedException(typeof(ArgumentException))]
		[TestMethod]
		public void ThrowsIfPrefixIsInvalidNCName2()
		{
			new ElementMatch("bar:", "foo");
		}

		[ExpectedException(typeof(ArgumentException))]
		[TestMethod]
		public void ThrowsIfPrefixIsInvalidNCName3()
		{
			new ElementMatch("*:", "foo");
		}

		[ExpectedException(typeof(ArgumentException))]
		[TestMethod]
		public void ThrowsIfNameIsInvalidNCName()
		{
			new ElementMatch("123");
		}

		[ExpectedException(typeof(ArgumentException))]
		[TestMethod]
		public void ThrowsIfNameIsInvalidNCName2()
		{
			new ElementMatch("foo:bar");
		}

		[ExpectedException(typeof(ArgumentException))]
		[TestMethod]
		public void ThrowsIfNameIsInvalidNCName3()
		{
			new ElementMatch("foo:*");
		}

		[TestMethod]
		public void MatchCanReceiveNullResolver()
		{
			XmlMatch match = new ElementMatch("foo");
			XmlReader reader = GetReader("<foo/>");

			match.Matches(reader, null);
		}

		[TestMethod]
		public void MatchesLocalNameWithoutPrefix()
		{
			XmlMatch match = new ElementMatch("foo");
			XmlReader reader = GetReader("<foo></foo>");
			reader.MoveToContent();

			Assert.IsTrue(match.Matches(reader, null));

			reader.Read();
			match = new ElementMatch("foo", MatchMode.EndElement);

			Assert.IsTrue(match.Matches(reader, null));
		}

		[TestMethod]
		public void DoesNotMatchWrongLocalName()
		{
			XmlMatch match = new ElementMatch("foo");
			XmlReader reader = GetReader("<root><foo/></root>");
			reader.MoveToContent();

			Assert.IsFalse(match.Matches(reader, null));

			match = new ElementMatch("foo", MatchMode.EndElement);
			reader.Read();
			Assert.IsFalse(match.Matches(reader, null));
			reader.Read();
			Assert.IsTrue(match.Matches(reader, null));
			reader.Read();
			Assert.IsFalse(match.Matches(reader, null));
		}

		[ExpectedException(typeof(ArgumentException))]
		[TestMethod]
		public void MatchThrowsIfPrefixButNoResolver()
		{
			XmlMatch match = new ElementMatch("foo", "bar");
			XmlReader reader = GetReader("<foo:bar xmlns:foo='xml-mvp'/>");
			reader.MoveToContent();

			match.Matches(reader, null);
		}

		[TestMethod]
		public void MatchesNameWithPrefix()
		{
			XmlMatch match = new ElementMatch("foo", "bar");
			XmlReader reader = GetReader("<foo:bar xmlns:foo='xml-mvp'></foo:bar>");
			reader.MoveToContent();

			XmlNamespaceManager ns = new XmlNamespaceManager(reader.NameTable);
			ns.AddNamespace("foo", "xml-mvp");

			Assert.IsTrue(match.Matches(reader, ns));

			reader.Read();

			match = new ElementMatch("foo", "bar", MatchMode.EndElement);
			Assert.IsTrue(match.Matches(reader, ns));
		}

		[TestMethod]
		public void MatchesNameWithPrefixDocumentWithoutPrefixButNamespace()
		{
			XmlMatch match = new ElementMatch("foo", "bar");
			XmlReader reader = GetReader("<bar xmlns='xml-mvp'/>");
			reader.MoveToContent();

			XmlNamespaceManager ns = new XmlNamespaceManager(reader.NameTable);
			ns.AddNamespace("foo", "xml-mvp");

			Assert.IsTrue(match.Matches(reader, ns));
		}

		[TestMethod]
		public void DoesNotMatchSameLocalNameButWrongNamespace()
		{
			XmlMatch match = new ElementMatch("foo", "bar");
			XmlReader reader = GetReader("<bar xmlns='wrong-namespace'/>");
			reader.MoveToContent();

			XmlNamespaceManager ns = new XmlNamespaceManager(reader.NameTable);
			ns.AddNamespace("foo", "xml-mvp");

			Assert.IsFalse(match.Matches(reader, ns));
		}

		[TestMethod]
		public void DoesNotMatchSameLocalNameButWrongNamespace2()
		{
			XmlMatch match = new ElementMatch("foo", "bar");
			XmlReader reader = GetReader("<bar/>");
			reader.MoveToContent();

			XmlNamespaceManager ns = new XmlNamespaceManager(reader.NameTable);
			ns.AddNamespace("foo", "xml-mvp");

			Assert.IsFalse(match.Matches(reader, ns));
		}

		[ExpectedException(typeof(ArgumentException))]
		[TestMethod]
		public void ThrowsIfPrefixUnresolvable()
		{
			XmlMatch match = new ElementMatch("foo", "bar");
			XmlReader reader = GetReader("<bar/>");
			reader.MoveToContent();

			XmlNamespaceManager ns = new XmlNamespaceManager(reader.NameTable);

			match.Matches(reader, ns);
		}

		[TestMethod]
		public void CanResolveToEmptyNamespace()
		{
			XmlMatch match = new ElementMatch("foo", "bar");
			XmlReader reader = GetReader("<bar/>");
			reader.MoveToContent();

			XmlNamespaceManager ns = new XmlNamespaceManager(reader.NameTable);
			ns.AddNamespace("foo", String.Empty);

			Assert.IsTrue(match.Matches(reader, ns));
		}

		[TestMethod]
		public void ElementMatchOptionDoesNotMatchEndElement()
		{
			XmlMatch match = new ElementMatch("bar");
			XmlReader reader = GetReader("<bar></bar>");
			reader.MoveToContent();
			reader.Read();

			Assert.IsFalse(match.Matches(reader, null));
		}

		[TestMethod]
		public void EndElementMatchOptionMatchesEndElement()
		{
			XmlMatch match = new ElementMatch("bar", MatchMode.EndElement);
			XmlReader reader = GetReader("<bar></bar>");
			reader.MoveToContent();
			reader.Read();

			Assert.IsTrue(match.Matches(reader, null));
		}

		[TestMethod]
		public void ElementMatchOptionMatchesOnlyElement()
		{
			XmlMatch match = new ElementMatch("id");
			XmlReader reader = GetReader("<bar id='23'>hello</bar>");
			reader.MoveToContent();
			reader.MoveToFirstAttribute();

			Assert.IsFalse(match.Matches(reader, null));

			match = new ElementMatch("hello");
			reader.MoveToElement();
			reader.Read();

			Assert.AreEqual(XmlNodeType.Text, reader.NodeType);

			Assert.IsFalse(match.Matches(reader, null));
		}

		[TestMethod]
		public void AttributeMatchMatchesOnlyAttribute()
		{
			XmlMatch match = new AttributeMatch("id");
			XmlReader reader = GetReader("<bar id='23'><foo>hello</foo></bar>");
			reader.MoveToContent();

			Assert.IsFalse(match.Matches(reader, null));
			reader.MoveToFirstAttribute();
			Assert.IsTrue(match.Matches(reader, null));

			reader.MoveToElement();
			reader.Read();

			Assert.IsFalse(match.Matches(reader, null));
		}

		[TestMethod]
		public void CanGetName()
		{
			XmlMatch match = new ElementMatch("foo");

			Assert.AreEqual("foo", match.Name);
		}

		[TestMethod]
		public void CanGetPrefix()
		{
			XmlMatch match = new ElementMatch("foo", "*");

			Assert.AreEqual("foo", match.Prefix);
		}

		[TestMethod]
		public void IsAnyNamespaceIfPrefixIsWildcard()
		{
			XmlNameMatch name = new ElementMatch(XmlNameMatch.Wildcard, "foo");

			Assert.IsTrue(name.IsAnyNamespace);
		}

		[TestMethod]
		public void IsAnyNameIfNameIsWildcard()
		{
			XmlNameMatch name = new ElementMatch(XmlNameMatch.Wildcard);

			Assert.IsTrue(name.IsAnyName);
		}

		[TestMethod]
		public void WildcardNameMatchesAnyElement()
		{
			XmlMatch match = new ElementMatch("*");
			XmlReader reader = GetReader("<foo><bar/></foo>");
			reader.MoveToContent();

			Assert.IsTrue(match.Matches(reader, null));

			reader.Read();

			Assert.IsTrue(match.Matches(reader, null));
		}

		[TestMethod]
		public void WildcardNameMatchesAnyElementWithPrefix()
		{
			XmlMatch match = new ElementMatch("x", "*");
			XmlReader reader = GetReader("<x:foo xmlns:x='xml-mvp'><x:bar/></x:foo>");
			reader.MoveToContent();

			XmlNamespaceManager ns = new XmlNamespaceManager(reader.NameTable);
			ns.AddNamespace("x", "xml-mvp");

			Assert.IsTrue(match.Matches(reader, ns));

			reader.Read();

			Assert.IsTrue(match.Matches(reader, ns));
		}

		[ExpectedException(typeof(ArgumentException))]
		[TestMethod]
		public void ThrowsIfWildcardNameCannotResolvePrefix()
		{
			XmlMatch match = new ElementMatch("x", "*");
			XmlReader reader = GetReader("<x:foo xmlns:x='xml-mvp'><x:bar/></x:foo>");
			reader.MoveToContent();

			XmlNamespaceManager ns = new XmlNamespaceManager(reader.NameTable);
			ns.AddNamespace("y", "xml-mvp");

			match.Matches(reader, ns);
		}

		[ExpectedException(typeof(ArgumentException))]
		[TestMethod]
		public void ThrowsIfWildcardNameWithPrefixAndNoResolver()
		{
			XmlMatch match = new ElementMatch("x", "*");
			XmlReader reader = GetReader("<x:foo xmlns:x='xml-mvp'><x:bar/></x:foo>");
			reader.MoveToContent();

			match.Matches(reader, null);
		}

		[TestMethod]
		public void WildcardNameDoesNotMatchWrongElementNamespace()
		{
			XmlMatch match = new ElementMatch("*");
			XmlReader reader = GetReader("<foo xmlns='mvp-xml'><bar xmlns=''/></foo>");
			reader.MoveToContent();

			Assert.IsFalse(match.Matches(reader, null));

			reader.Read();

			Assert.IsTrue(match.Matches(reader, null));
		}

		[TestMethod]
		public void WildcardNameMatchesAnyEndElement()
		{
		    XmlMatch match = new ElementMatch("*", MatchMode.EndElement);
		    XmlReader reader = GetReader("<foo><bar></bar></foo>");
		    reader.MoveToContent();
		    reader.Read();

		    Assert.IsFalse(match.Matches(reader, null));

		    reader.Read();

		    Assert.IsTrue(match.Matches(reader, null));

		    reader.Read();

		    Assert.IsTrue(match.Matches(reader, null));
		}

		[TestMethod]
		public void WildcardNameDoesNotMatchWrongEndElementNamespace()
		{
		    XmlMatch match = new ElementMatch("*", MatchMode.EndElement);
		    XmlReader reader = GetReader("<foo xmlns='mvp-xml'><bar xmlns=''></bar></foo>");
		    reader.MoveToContent();
		    reader.Read();
			
		    Assert.IsFalse(match.Matches(reader, null));
			
		    reader.Read();

		    Assert.IsTrue(match.Matches(reader, null));

		    reader.Read();

		    Assert.IsFalse(match.Matches(reader, null));
		}

		[TestMethod]
		public void WildcardNameMatchesAnyAttribute()
		{
			XmlMatch match = new AttributeMatch("*");
			XmlReader reader = GetReader("<foo id='1' enabled='true'></foo>");
			reader.MoveToContent();
			reader.MoveToFirstAttribute();

			Assert.IsTrue(match.Matches(reader, null));

			reader.MoveToNextAttribute();

			Assert.IsTrue(match.Matches(reader, null));
		}

		[TestMethod]
		public void WildcardNameDoesNotMatchWrongAttributeNamespace()
		{
			XmlMatch match = new AttributeMatch("*");
			XmlReader reader = GetReader("<foo id='1' x:enabled='true' xmlns:x='mvp-xml'><bar x:enabled='true' xmlns:x='mvp-xml'/></foo>");

			reader.MoveToContent();
			Assert.IsFalse(match.Matches(reader, null));

			reader.MoveToFirstAttribute();
			Assert.IsTrue(match.Matches(reader, null));

			reader.MoveToNextAttribute();
			Assert.IsFalse(match.Matches(reader, null));

			reader.MoveToNextAttribute();
			Assert.IsFalse(match.Matches(reader, null));

			reader.Read();
			Assert.IsFalse(match.Matches(reader, null));
		}

		[TestMethod]
		public void WildcardNamespaceMatchesElementsInAnyNamespace()
		{
			XmlMatch match = new ElementMatch("*", "foo");
			XmlReader reader = GetReader("<foo><foo xmlns='mvp-xml'/></foo>");
			reader.MoveToContent();

			Assert.IsTrue(match.Matches(reader, null));

			reader.Read();

			Assert.IsTrue(match.Matches(reader, null));
		}

		[TestMethod]
		public void WildcardNamespaceMatchesEndElementsInAnyNamespace()
		{
		    XmlMatch match = new ElementMatch("*", "foo", MatchMode.EndElement);
		    XmlReader reader = GetReader("<foo><foo xmlns='mvp-xml'></foo></foo>");
		    reader.MoveToContent();
		    reader.Read();
		    reader.Read();

		    Assert.IsTrue(match.Matches(reader, null));

		    reader.Read();

		    Assert.IsTrue(match.Matches(reader, null));
		}

		[TestMethod]
		public void WildcardNamespaceMatchesAttributesInAnyNamespace()
		{
			XmlMatch match = new AttributeMatch("*", "id");
			XmlReader reader = GetReader("<root id='1' x:id='1' xmlns:x='mvp-xml'></foo>");
			reader.MoveToContent();
			reader.MoveToFirstAttribute();

			Assert.IsTrue(match.Matches(reader, null));

			reader.MoveToNextAttribute();

			Assert.IsTrue(match.Matches(reader, null));
		}

		[TestMethod]
		public void BothWildcardMatchesAnyElementAndNamespace()
		{
			XmlMatch match = new ElementMatch("*", "*");
			XmlReader reader = GetReader("<foo><bar xmlns='xml-mvp'/></foo>");
			reader.MoveToContent();

			Assert.IsTrue(match.Matches(reader, null));

			reader.Read();

			Assert.IsTrue(match.Matches(reader, null));
		}

		[TestMethod]
		public void BothWildcardMatchesAnyEndElementAndNamespace()
		{
		    XmlMatch match = new ElementMatch("*", "*", MatchMode.EndElement);
		    XmlReader reader = GetReader("<foo><bar xmlns='xml-mvp'></bar></foo>");
		    reader.MoveToContent();
		    reader.Read();
		    reader.Read();

		    Assert.IsTrue(match.Matches(reader, null));

		    reader.Read();

		    Assert.IsTrue(match.Matches(reader, null));
		}

		[TestMethod]
		public void BothWildcardMatchesAnyAttributeAndNamespace()
		{
			XmlMatch match = new AttributeMatch("*", "*");
			XmlReader reader = GetReader("<root foo='1' x:id='1' xmlns:x='mvp-xml'></root>");
			reader.MoveToContent();
			reader.MoveToFirstAttribute();

			Assert.IsTrue(match.Matches(reader, null));

			reader.MoveToNextAttribute();

			Assert.IsTrue(match.Matches(reader, null));
		}
	}
}
