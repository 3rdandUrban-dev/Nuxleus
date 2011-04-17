using System;
using System.IO;
using System.Xml.XPath;

using Mvp.Xml.Common.XPath;
using NUnit.Framework;

namespace Mvp.Xml.Tests
{
    [TestFixture]
    public class SingletonXPathNodeIteratorTests
    {
        [Test]
        public void Test1()
        {
            XPathDocument doc = new XPathDocument(new StringReader("<foo/>"));
            XPathNavigator node = doc.CreateNavigator().SelectSingleNode("/*");
            SingletonXPathNodeIterator ni = new SingletonXPathNodeIterator(node);
            Assert.IsTrue(ni.MoveNext());
            Assert.IsTrue(ni.Current == node);
            Assert.IsFalse(ni.MoveNext());
        }        
    }
}
