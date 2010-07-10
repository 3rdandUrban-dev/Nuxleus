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
using System.IO;

namespace Mvp.Xml.Core.UnitTests
{
	[TestClass]
	public class WrappingXmlReaderFixture : TestFixtureBase
	{
		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void ThrowsIfReaderNull()
		{
			new MockReader(null);
		}

		[TestMethod]
		public void ReaderKeepsFidelity()
		{
			string actual = ReadToEnd(new MockReader(GetReader(File.ReadAllText("machine.config"))));
			string expected = ReadToEnd(GetReader(File.ReadAllText("machine.config")));

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void DoesNotReportLineInfoIfNotSupported()
		{
			XmlDocument doc = new XmlDocument();
			doc.Load("machine.config");
			WrappingXmlReader reader = new MockReader(new XmlNodeReader(doc.DocumentElement));
			reader.MoveToContent();
			reader.Read();

			Assert.AreEqual(0, reader.LineNumber);
			Assert.AreEqual(0, reader.LinePosition);
			Assert.AreEqual(false, reader.HasLineInfo());
		}

		class MockReader : WrappingXmlReader
		{
			public MockReader(XmlReader reader) : base(reader) {}
		}
	}
}
