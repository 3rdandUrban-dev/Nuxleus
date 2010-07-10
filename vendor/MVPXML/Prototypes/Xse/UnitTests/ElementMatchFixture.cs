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
	public class ElementMatchFixture : TestFixtureBase
	{
		[TestMethod]
		public void ShouldMatchRootElement()
		{
			XmlReader reader = GetReader("<root/>");
			ElementMatch match = new ElementMatch("root", MatchMode.RootElement);

			reader.MoveToContent();

			Assert.IsTrue(match.Matches(reader, null));
		}

		[TestMethod]
		public void ShouldMatchRootEndElement()
		{
			XmlReader reader = GetReader("<root></root>");
			ElementMatch match = new ElementMatch("root", MatchMode.RootEndElement);

			reader.MoveToContent();
			reader.Read();

			Assert.IsTrue(match.Matches(reader, null));
		}

		[TestMethod]
		public void ShouldMatchElement()
		{
			XmlReader reader = GetReader("<root><foo></foo></root>");
			ElementMatch match = new ElementMatch("foo", MatchMode.Element);

			reader.MoveToContent();
			reader.Read();

			Assert.IsTrue(match.Matches(reader, null));
		}

		[TestMethod]
		public void ShouldMatchEndElement()
		{
			XmlReader reader = GetReader("<root><foo></foo></root>");
			ElementMatch match = new ElementMatch("foo", MatchMode.EndElement);

			reader.MoveToContent();
			reader.Read();
			reader.Read();

			Assert.IsTrue(match.Matches(reader, null));
		}

		[TestMethod]
		public void ShouldMatchElementForRootToo()
		{
			XmlReader reader = GetReader("<root/>");
			ElementMatch match = new ElementMatch("root", MatchMode.Element);

			reader.MoveToContent();

			Assert.IsTrue(match.Matches(reader, null));
		}

		[TestMethod]
		public void ShouldMatchEndElementForRootToo()
		{
			XmlReader reader = GetReader("<root></root>");
			ElementMatch match = new ElementMatch("root", MatchMode.EndElement);

			reader.MoveToContent();
			reader.Read();

			Assert.IsTrue(match.Matches(reader, null));
		}

		[ExpectedException(typeof(ArgumentException))]
		[TestMethod]
		public void ThrowsIfUnknownMode()
		{
			ElementMatch match = new ElementMatch("root", (MatchMode)10);
		}
	}
}
