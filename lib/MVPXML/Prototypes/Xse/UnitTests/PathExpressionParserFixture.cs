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
	public class PathExpressionParserFixture : TestFixtureBase
	{
		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void ThrowsIfExpressionNull()
		{
			PathExpressionParser.Parse(null);
		}

		[ExpectedException(typeof(ArgumentException))]
		[TestMethod]
		public void ThrowsIfExpressionEmptyString()
		{
			PathExpressionParser.Parse(string.Empty);
		}

		[ExpectedException(typeof(ArgumentException))]
		[TestMethod]
		public void ThrowsIfInvalidPath()
		{
			PathExpressionParser.Parse("~");
		}

		[TestMethod]
		public void ShouldParseSingleElementName()
		{
			List<XmlMatch> names = PathExpressionParser.Parse("Foo");

			Assert.AreEqual(1, names.Count);
			Assert.AreEqual("Foo", names[0].ToString());
		}

		[TestMethod]
		public void ShouldParseSingleElementNameWithNamespace()
		{
			List<XmlMatch> names = PathExpressionParser.Parse("foo:bar");

			Assert.AreEqual(1, names.Count);
			Assert.AreEqual("foo:bar", names[0].FullName);
		}

		[TestMethod]
		public void ShouldParseRootElement()
		{
			List<XmlMatch> names = PathExpressionParser.Parse("/foo:bar");

			Assert.AreEqual(1, names.Count);
			Assert.IsTrue(names[0] is ElementMatch);
			Assert.IsTrue(((ElementMatch)names[0]).Mode == MatchMode.RootElement);
		}

		[TestMethod]
		public void ShouldParseWithDescendentOrSelf()
		{
			List<XmlMatch> names = PathExpressionParser.Parse("//foo:bar");

			Assert.AreEqual(1, names.Count);
			Assert.IsTrue(names[0] is ElementMatch);
			Assert.IsTrue(((ElementMatch)names[0]).Mode == MatchMode.Element);
		}

		[TestMethod]
		public void ShouldParseWildcardPrefix()
		{
			List<XmlMatch> names = PathExpressionParser.Parse("//*:bar");

			Assert.AreEqual(1, names.Count);
			Assert.IsTrue(names[0] is XmlNameMatch);
			Assert.IsTrue(((XmlNameMatch)names[0]).IsAnyNamespace);
		}

		[TestMethod]
		public void ShouldParseWildcardName()
		{
			List<XmlMatch> names = PathExpressionParser.Parse("//foo:*");

			Assert.AreEqual(1, names.Count);
			Assert.IsTrue(names[0] is XmlNameMatch);
			Assert.IsTrue(((XmlNameMatch)names[0]).IsAnyName);
		}

		[TestMethod]
		public void ShouldParseAttribute()
		{
			List<XmlMatch> names = PathExpressionParser.Parse("@foo");

			Assert.AreEqual(1, names.Count);
			Assert.IsTrue(names[0] is AttributeMatch);
		}

		[TestMethod]
		public void ShouldParseAttributeWithPrefix()
		{
			List<XmlMatch> names = PathExpressionParser.Parse("@foo:bar");

			Assert.AreEqual(1, names.Count);
			Assert.IsTrue(names[0] is AttributeMatch);
			Assert.AreEqual("foo:bar", names[0].FullName);
		}

		[TestMethod]
		public void ShouldParseAttributeWithWildcardNamespace()
		{
			List<XmlMatch> names = PathExpressionParser.Parse("@*:bar");

			Assert.AreEqual(1, names.Count);
			Assert.IsTrue(names[0] is AttributeMatch);
			Assert.IsTrue(((AttributeMatch)names[0]).IsAnyNamespace);
		}

		[TestMethod]
		public void ShouldParseAttributeWithWildcardName()
		{
			List<XmlMatch> names = PathExpressionParser.Parse("@foo:*");

			Assert.AreEqual(1, names.Count);
			Assert.IsTrue(names[0] is AttributeMatch);
			Assert.IsTrue(((AttributeMatch)names[0]).IsAnyName);
		}

		[TestMethod]
		public void ShouldParseEndElementMode()
		{
			List<XmlMatch> names = PathExpressionParser.Parse("foo:bar", true);

			Assert.AreEqual(1, names.Count);
			Assert.IsTrue(names[0] is ElementMatch);
			Assert.IsTrue(((ElementMatch)names[0]).Mode == MatchMode.EndElement);
		}

		[TestMethod]
		public void ShouldParseRootEndElementMode()
		{
			List<XmlMatch> names = PathExpressionParser.Parse("/foo:bar", true);

			Assert.AreEqual(1, names.Count);
			Assert.IsTrue(names[0] is ElementMatch);
			Assert.IsTrue(((ElementMatch)names[0]).Mode == MatchMode.RootEndElement);
		}

		[TestMethod]
		public void ShouldParseLastElementAsEndElement()
		{
			List<XmlMatch> names = PathExpressionParser.Parse("foo/bar", true);

			Assert.AreEqual(2, names.Count);
			Assert.AreEqual(MatchMode.Element, ((ElementMatch)names[0]).Mode);
			Assert.AreEqual(MatchMode.EndElement, ((ElementMatch)names[1]).Mode);
		}

		[TestMethod]
		public void ShouldParseLastElementAsEndElementWithRootStart()
		{
			List<XmlMatch> names = PathExpressionParser.Parse("/foo/bar", true);

			Assert.AreEqual(2, names.Count);
			Assert.AreEqual(MatchMode.RootElement, ((ElementMatch)names[0]).Mode);
			Assert.AreEqual(MatchMode.EndElement, ((ElementMatch)names[1]).Mode);
		}

		[TestMethod]
		public void ShouldParseElementAttributeMatchTogether()
		{
			List<XmlMatch> names = PathExpressionParser.Parse("/foo/bar/@id");

			Assert.AreEqual(2, names.Count);
			Assert.AreEqual(MatchMode.RootElement, ((ElementMatch)names[0]).Mode);
			Assert.IsTrue(names[1] is ElementAttributeMatch);
		}

		[ExpectedException(typeof(ArgumentException))]
		[TestMethod]
		public void ThrowsIfEndElementModeAndAttributeSpecified()
		{
			List<XmlMatch> names = PathExpressionParser.Parse("/foo/bar/@id", true);
		}

		[ExpectedException(typeof(ArgumentException))]
		[TestMethod]
		public void ThrowsIfDoubleSlashInExpression()
		{
			List<XmlMatch> names = PathExpressionParser.Parse("/foo//bar/@id");
		}

		[ExpectedException(typeof(ArgumentException))]
		[TestMethod]
		public void ThrowsIfAttributePathNotEndExpression()
		{
			List<XmlMatch> names = PathExpressionParser.Parse("/foo/@bar/@id");
		}

		[ExpectedException(typeof(ArgumentException))]
		[TestMethod]
		public void ThrowsIfEmptyPrefix()
		{
			List<XmlMatch> names = PathExpressionParser.Parse("/foo/:bar");
		}

		[ExpectedException(typeof(ArgumentException))]
		[TestMethod]
		public void ThrowsIfEmptyName()
		{
			List<XmlMatch> names = PathExpressionParser.Parse("/foo/bar:/@id");
		}

	}
}
