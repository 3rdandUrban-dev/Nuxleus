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
	public class FullEndElementReaderFixture : TestFixtureBase
	{
		[TestMethod]
		public void CanFakeEndElement()
		{
			string xml = "<foo><bar/></foo>";

			FullEndElementReader reader = new FullEndElementReader(GetReader(xml));
			string expected = "<foo><bar></bar></foo>";
			string actual = ReadToEnd(reader);

			Assert.AreEqual(actual, expected);
		}

		[TestMethod]
		public void CanFakeEndElementWithAttributes()
		{
			string xml = "<foo><bar id='1' foo='2'/></foo>";

			FullEndElementReader reader = new FullEndElementReader(GetReader(xml));
			string expected = "<foo><bar id=\"1\" foo=\"2\"></bar></foo>";
			string actual = ReadToEnd(reader);

			Assert.AreEqual(actual, expected);
		}
	}
}
