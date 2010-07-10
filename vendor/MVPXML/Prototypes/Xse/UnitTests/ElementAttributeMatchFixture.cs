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
	public class ElementAttributeMatchFixture
	{
		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void ThrowsIfElementMatchNull()
		{
			new ElementAttributeMatch(null, new AttributeMatch("*"));
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void ThrowsIfAttributeMatchNull()
		{
			new ElementAttributeMatch(new ElementMatch("*"), null);
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void ThrowsIfReaderNull()
		{
			new ElementAttributeMatch(new ElementMatch("*"), new AttributeMatch("*")).Matches(null, new XmlNamespaceManager(new NameTable()));
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void ThrowsIfResolverNull()
		{
			new ElementAttributeMatch(new ElementMatch("*"), new AttributeMatch("*")).Matches(new XmlTextReader("foo"), null);
		}
	}
}
