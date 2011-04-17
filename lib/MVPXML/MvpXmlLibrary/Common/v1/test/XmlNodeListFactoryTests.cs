#region using

using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

using NUnit.Framework;
using Mvp.Xml.Common;

#endregion using 

namespace Mvp.Xml.Tests
{
	/// <summary>
	/// </summary>
	[TestFixture]
	public class XmlNodeListFactoryTests
	{
		private XmlDocument pubsDocument;

		[SetUp]
		public void Setup()
		{
			pubsDocument = new XmlDocument();
			pubsDocument.Load(Globals.GetResource(Globals.PubsResource));
		}

		[Test]
		public void MultipleCompleteEnumerations()
		{
			XPathNodeIterator nodeIterator = pubsDocument.CreateNavigator().Select("/dsPubs/publishers");
			XmlNodeList nodeList = XmlNodeListFactory.CreateNodeList(nodeIterator);
			
			// Get the first node list enumerator.
			IEnumerator enumerator = nodeList.GetEnumerator();

			while (enumerator.MoveNext())
			{
				// Enumerate all publishers.
			}

			// Get the second node list enumerator.
			enumerator = nodeList.GetEnumerator();
			
			// Ensure that the second node list enumerator is in a usable state.
			Assert.IsTrue(enumerator.MoveNext());
		}

		[Test]
		public void List1()
		{
			string xml = @"<?xml version='1.0'?>
<root>
	<element>1</element>
	<element></element>
	<element/>
	<element>2</element>
</root>";

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xml);

			XPathNodeIterator iterator = doc.CreateNavigator().Select("//element");
			XmlNodeList list = XmlNodeListFactory.CreateNodeList(iterator);

			int count = 0;
			foreach (XmlNode n in list)
			{
				count++;
			}
			Assert.AreEqual(4, count);

			iterator = doc.CreateNavigator().Select("//element");
			list = XmlNodeListFactory.CreateNodeList(iterator);
			Assert.AreEqual(4, list.Count);
		}
	}
}
