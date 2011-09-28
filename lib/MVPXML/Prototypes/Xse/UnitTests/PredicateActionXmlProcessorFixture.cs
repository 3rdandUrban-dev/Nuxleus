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
	public class PredicateActionXmlProcessorFixture : TestFixtureBase
	{
		[TestMethod]
		public void PredicateTrueAndActionCalledForEachRead()
		{
			string xml = "<root>foo</root>";
			XmlProcessorReader reader = new XmlProcessorReader(GetReader(xml));
			int predicateCalls = 0;
			int actionCalls = 0;

			reader.Processors.Add(new PredicateActionXmlProcessor(
				delegate { predicateCalls++; return true; },
				delegate { actionCalls++; }
				)
			);

			while (reader.Read()) { }

			Assert.AreEqual(3, predicateCalls);
			Assert.AreEqual(3, actionCalls);
		}

		[TestMethod]
		public void ActionCalledIfPredicateTrue()
		{
			string xml = "<root>foo</root>";
			XmlProcessorReader reader = new XmlProcessorReader(GetReader(xml));
			int actionCalls = 0;

			reader.Processors.Add(new PredicateActionXmlProcessor(
				delegate(XmlReader predicateReader) { return predicateReader.NodeType == XmlNodeType.Text; },
				delegate { actionCalls++; }
				)
			);

			while (reader.Read()) { }

			Assert.AreEqual(1, actionCalls);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ThrowsIfPredicateNull()
		{
			new PredicateActionXmlProcessor(null, delegate { });
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ThrowsIfActionNull()
		{
			new PredicateActionXmlProcessor(delegate { return true; }, null);
		}

		[ExpectedException(typeof(InvalidOperationException))]
		[TestMethod]
		public void DerivedProcessorMustInitializeBase()
		{
			string xml = "<root>foo</root>";
			XmlProcessorReader reader = new XmlProcessorReader(GetReader(xml));

			reader.Processors.Add(new MockProcessorDerived());

			while (reader.Read()) { }
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void DerivedProcessorCannotInitializeNullPredicate()
		{
			string xml = "<root>foo</root>";
			XmlProcessorReader reader = new XmlProcessorReader(GetReader(xml));

			reader.Processors.Add(new MockProcessorDerivedNullPredicate());

			while (reader.Read()) { }
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void DerivedProcessorCannotInitializeNullAction()
		{
			string xml = "<root>foo</root>";
			XmlProcessorReader reader = new XmlProcessorReader(GetReader(xml));

			reader.Processors.Add(new MockProcessorDerivedNullAction());

			while (reader.Read()) { }
		}

		[TestMethod]
		public void DerivedProcessorCanInitializeAndRetrieveDelegates()
		{
			Predicate<XmlReader> predicate = delegate { return false; };
			Action<XmlReader> action = delegate { };

			MockProcessorDerivedInitialize proc = new MockProcessorDerivedInitialize();
			proc.Initialize(predicate, action);

			Assert.AreEqual(predicate, proc.GetPredicate());
			Assert.AreEqual(action, proc.GetAction());
		}

		class MockProcessorDerived : PredicateActionXmlProcessor
		{
			public MockProcessorDerived()
			{
			}
		}

		class MockProcessorDerivedNullPredicate : PredicateActionXmlProcessor
		{
			public MockProcessorDerivedNullPredicate()
			{
				InitializeProcessor(null, delegate { });
			}
		}

		class MockProcessorDerivedNullAction : PredicateActionXmlProcessor
		{
			public MockProcessorDerivedNullAction()
			{
				InitializeProcessor(delegate { return true; }, null);
			}
		}

		class MockProcessorDerivedInitialize : PredicateActionXmlProcessor
		{
			public MockProcessorDerivedInitialize()
			{
			}

			public new void Initialize(Predicate<XmlReader> predicate, Action<XmlReader> action)
			{
				base.InitializeProcessor(predicate, action);
			}

			public Predicate<XmlReader> GetPredicate() { return base.Predicate; }
			public Action<XmlReader> GetAction() { return base.Action; }
		}

		class MockDelegateContainer
		{
			public int PredicateCalls;
			public int ActionCalls;

			public bool OnPredicate(XmlReader reader)
			{
				PredicateCalls++;
				return true;
			}

			public void OnAction(XmlReader reader)
			{
				ActionCalls++;
			}
		}
	}
}
