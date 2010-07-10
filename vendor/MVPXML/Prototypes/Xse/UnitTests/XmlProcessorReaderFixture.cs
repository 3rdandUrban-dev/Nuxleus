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
	public class XmlProcessorReaderFixture : TestFixtureBase
	{
		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void ThrowsIfReaderNull()
		{
			new XmlProcessorReader(null);
		}

		[TestMethod]
		public void CanAddXmlProcessor()
		{
			string xml = "<root>foo</root>";
			XmlProcessorReader reader = new XmlProcessorReader(GetReader(xml));

			reader.Processors.Add(new MockProcessor());
		}

		[TestMethod]
		public void XmlProcessorCalledOnEachRead()
		{
			string xml = "<root>foo</root>";
			XmlProcessorReader reader = new XmlProcessorReader(GetReader(xml));
			MockProcessor processor = new MockProcessor();

			reader.Processors.Add(processor);

			string output = ReadToEnd(reader);

			WriteIfDebugging(output);
			Assert.AreEqual(3, processor.Calls);
		}

		[TestMethod]
		public void XmlProcessorCalledOnEachReadAndAttributes()
		{
			string xml = "<root id='1'>foo</root>";
			XmlProcessorReader reader = new XmlProcessorReader(GetReader(xml));
			MockProcessor processor = new MockProcessor();

			reader.Processors.Add(processor);

			string output = ReadToEnd(reader);

			WriteIfDebugging(output);
			Assert.AreEqual(4, processor.Calls);
		}

		[TestMethod]
		public void ProcessorReaderCanReadToEnd()
		{
			string xml = "<root>foo</root>";
			XmlProcessorReader reader = new XmlProcessorReader(GetReader(xml));
			MockProcessor processor = new MockProcessor();

			reader.Processors.Add(processor);
			reader.ReadToEnd();

			Assert.AreEqual(3, processor.Calls);
		}

		[TestMethod]
		public void CanChainReader()
		{
			string xml = "<root>foo</root>";
			XmlProcessorReader reader = new XmlProcessorReader(GetReader(xml));
			ChainReaderMockProcessor processor = new ChainReaderMockProcessor();

			reader.Processors.Add(processor);

			string output = ReadToEnd(reader);

			WriteIfDebugging(output);
			Assert.AreEqual(1, processor.Calls);
		}

		[TestMethod]
		public void CanChainMutatingReader()
		{
			string xml = "<root>foo</root>";
			XmlProcessorReader reader = new XmlProcessorReader(GetReader(xml));
			XmlProcessor processor = new MutateMockProcessor();

			reader.Processors.Add(processor);

			string output = ReadToEnd(reader);

			WriteIfDebugging(output);
			Assert.AreEqual("<Root>foo</Root>", output);
		}

		[TestMethod]
		public void CanChainMultipleProcessors()
		{
			string xml = "<root>foo</root>";
			XmlProcessorReader reader = new XmlProcessorReader(GetReader(xml));
			MockProcessor processor = new MockProcessor();

			reader.Processors.Add(new MutateMockProcessor());
			reader.Processors.Add(new ChainReaderMockProcessor());
			reader.Processors.Add(processor);

			string output = ReadToEnd(reader);

			WriteIfDebugging(output);
			Assert.AreEqual("<Root />", output);
			Assert.AreEqual(1, processor.Calls);
		}

		[TestMethod]
		public void CanChainMutatingProcessors()
		{
			string xml = "<root>foo</root>";
			XmlProcessorReader reader = new XmlProcessorReader(GetReader(xml));

			reader.Processors.Add(new MutateMockProcessor());
			reader.Processors.Add(new AddNamespaceMutateMockProcessor());

			string output = ReadToEnd(reader);

			WriteIfDebugging(output);
			Assert.AreEqual("<clarius:Root xmlns:clarius=\"http://clariusconsulting.net/kzu\">foo</clarius:Root>", output);
		}

		// Can add predicate and action
		// 

		class MockProcessor : XmlProcessor
		{
			public int Calls;

			public override XmlReader Process(XmlReader reader)
			{
				Calls++;
				return reader;
			}
		}

		class ChainReaderMockProcessor : XmlProcessor
		{
			public int Calls;

			public override XmlReader Process(XmlReader reader)
			{
				Calls++;
				return new SkipXmlReader(reader);
			}

			class SkipXmlReader : WrappingXmlReader
			{
				bool skipped = false;

				public SkipXmlReader(XmlReader innerReader) : base(innerReader) {}

				public override bool Read()
				{
					if (!skipped)
					{
						skipped = true;
						base.Skip();
					}
					return base.Read();
				}
			}
		}

		class MutateMockProcessor : XmlProcessor
		{
			public int Calls;

			public override XmlReader Process(XmlReader reader)
			{
				Calls++;
				return new MutateXmlReader(reader);
			}

			class MutateXmlReader : WrappingXmlReader
			{
				bool skipped = false;

				public MutateXmlReader(XmlReader innerReader) : base(innerReader) { }

				public override string LocalName
				{
					get
					{
						if (base.LocalName == "root")
						{
							return "Root";
						}
						return base.LocalName;
					}
				}

			}
		}

		class AddNamespaceMutateMockProcessor : XmlProcessor
		{
			public int Calls;

			public override XmlReader Process(XmlReader reader)
			{
				Calls++;
				return new MutateXmlReader(reader);
			}

			class MutateXmlReader : WrappingXmlReader
			{
				bool skipped = false;

				public MutateXmlReader(XmlReader innerReader) : base(innerReader) { }

				public override string NamespaceURI
				{
					get
					{
						return "http://clariusconsulting.net/kzu";
					}
				}

				public override string Prefix
				{
					get
					{
						return "clarius";
					}
				}
			}
		}
	}
}
