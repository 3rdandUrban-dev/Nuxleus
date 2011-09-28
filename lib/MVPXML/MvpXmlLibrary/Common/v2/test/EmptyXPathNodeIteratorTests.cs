using System;
using System.Xml.XPath;

using Mvp.Xml.Common.XPath;
using NUnit.Framework;

namespace Mvp.Xml.Tests
{
    [TestFixture]
    public class EmptyXPathNodeIteratorTests
    {
        [Test]
        public void Test1()
        {
            EmptyXPathNodeIterator ni = EmptyXPathNodeIterator.Instance;
            while (ni.MoveNext())
            {
                Assert.Fail("EmptyXPathNodeIterator must be empty");   
            }
        }

        [Test]
        public void Test2()
        {
            EmptyXPathNodeIterator ni = EmptyXPathNodeIterator.Instance;
            Assert.IsTrue(ni.MoveNext() == false);
            Assert.IsTrue(ni.Count == 0);
            Assert.IsTrue(ni.Current == null);
            Assert.IsTrue(ni.CurrentPosition == 0);
        }
    }
}
