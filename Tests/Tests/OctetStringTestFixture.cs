/*
 * Created by SharpDevelop.
 * User: lextm
 * Date: 2008/5/2
 * Time: 12:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using NUnit.Framework;
using System.Text;

#pragma warning disable 1591,0618
namespace Lextm.SharpSnmpLib.Tests
{
    [TestFixture]
    public class OctetStringTestFixture
    {
        [Test]
        public void TestException()
        {
            Assert.Throws<ArgumentNullException>(() => new OctetString(0, null));
            Assert.Throws<ArgumentNullException>(() => new OctetString((byte[])null));
            Assert.Throws<ArgumentNullException>(() => OctetString.Empty.ToString(null));
            Assert.Throws<ArgumentNullException>(() => OctetString.Empty.AppendBytesTo(null));
        }

        [Test]
        public void TestMethod()
        {
            byte[] expected = new byte[] {0x04, 0x06, 0x70, 0x75, 0x62, 0x6C, 0x69, 0x63};
            ISnmpData data = DataFactory.CreateSnmpData(expected);
            Assert.AreEqual(SnmpType.OctetString, data.TypeCode);
            OctetString s = (OctetString)data;
            Assert.AreEqual("public", s.ToString());
        }

        [Test]
        public void TestEncoding()
        {
            var temp = OctetString.DefaultEncoding;
            OctetString.DefaultEncoding = Encoding.UTF8;
            Assert.AreEqual(Encoding.UTF8, OctetString.DefaultEncoding);
            OctetString.DefaultEncoding = temp;
        }

        [Test]
        public void TestPhysical()
        {
            var mac = new OctetString(new byte[] {80, 90, 64, 87, 11, 99});
            Assert.AreEqual("505A40570B63", mac.ToPhysicalAddress().ToString());

            var invalid = new OctetString(new byte[] {89});
            Assert.Throws<InvalidCastException>(() => invalid.ToPhysicalAddress());
        }
        
        [Test]
        public void TestEqual()
        {
            var left = new OctetString("public");
            var right = new OctetString("public");
            Assert.AreEqual(left, right);
            Assert.IsTrue(left != OctetString.Empty);
// ReSharper disable EqualExpressionComparison
            Assert.IsTrue(left == left);
// ReSharper restore EqualExpressionComparison

        }

        [Test]
        public void TestToBytes()
        {
            Assert.AreEqual(2, new OctetString("").ToBytes().Length);
        }
        
        [Test]
        public void TestEmpty()
        {
            Assert.AreEqual("", OctetString.Empty.ToString());
        }
        
        [Test]
        public void TestChinese()
        {
            Assert.AreEqual("�й�", new OctetString("�й�", Encoding.Unicode).ToString(Encoding.Unicode));
        }
    }
}
#pragma warning restore 1591,0618