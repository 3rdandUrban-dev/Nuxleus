using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.IO;

using Mvp.Xml.Common.Xsl;
using Mvp.Xml.Tests;
using NUnit.Framework;

namespace Mvp.Xml.Tests.XslReaderTests
{
    [TestFixture]
    public class XslReaderTests
    {
        static string copyTransform =
    @"<xsl:stylesheet version='1.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform'>
          <xsl:template match='/'>
            <xsl:copy-of select='/' />
          </xsl:template>
        </xsl:stylesheet>";


        public static XmlReader GetReader(string xml)
        {
            XmlReaderSettings s =  new XmlReaderSettings();
            s.ProhibitDtd = false;
            return XmlReader.Create(Globals.GetResource(xml), s);
        }
 
        /// <summary>
        /// Compare with standard XmlReader test
        /// </summary>
        [Test]
        public void Test1()
        {
            CompareWithStandardReader(true, 16);
        }

        private void CompareWithStandardReader(bool multiThread, int bufSize)
        {
            XmlReader r = GetReader(Globals.NorthwindResource);
            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load("../../XslReaderTests/test1.xslt");
            MemoryStream ms = new MemoryStream();
            XmlWriterSettings s = new XmlWriterSettings();
            s.OmitXmlDeclaration = true;
            XmlWriter w = XmlWriter.Create(ms, s);
            xslt.Transform(r, w);
            r.Close();
            w.Close();
            byte[] buf = ms.ToArray();
            XmlReader standard = XmlReader.Create(new MemoryStream(buf));
            XslReader xslReader = new XslReader(xslt, multiThread, bufSize);
            xslReader.StartTransform(new XmlInput(GetReader(Globals.NorthwindResource)), null);
            CompareReaders(standard, xslReader);
        }

        private void CompareReaders(XmlReader standard, XmlReader custom)
        {
            while (standard.Read())
            {
                Assert.IsTrue(custom.Read());
                CompareReaderProperties(standard, custom);                

                if (standard.HasAttributes)
                {
                    while (standard.MoveToNextAttribute())
                    {
                        Assert.IsTrue(custom.MoveToNextAttribute());
                        CompareReaderProperties(standard, custom);
                    }
                    standard.MoveToElement();
                    Assert.IsTrue(custom.MoveToElement());                    
                }                                
            }
        }

        private static void CompareReaderProperties(XmlReader standard, XmlReader custom)
        {
            Assert.AreEqual(standard.AttributeCount, custom.AttributeCount);
            Assert.AreEqual(standard.BaseURI, custom.BaseURI);
            Assert.AreEqual(standard.Depth, custom.Depth);
            Assert.AreEqual(standard.EOF, custom.EOF);
            Assert.AreEqual(standard.HasAttributes, custom.HasAttributes);
            Assert.AreEqual(standard.HasValue, custom.HasValue);
            Assert.AreEqual(standard.IsDefault, custom.IsDefault);
            Assert.AreEqual(standard.IsEmptyElement, custom.IsEmptyElement);
            Assert.AreEqual(standard.LocalName, custom.LocalName);
            Assert.AreEqual(standard.Name, custom.Name);
            Assert.AreEqual(standard.NamespaceURI, custom.NamespaceURI);
            Assert.AreEqual(standard.NodeType, custom.NodeType);
            Assert.AreEqual(standard.Prefix, custom.Prefix);
            Assert.AreEqual(standard.QuoteChar, custom.QuoteChar);
            Assert.AreEqual(standard.ReadState, custom.ReadState);
            Assert.AreEqual(standard.Value, custom.Value);
            Assert.AreEqual(standard.ValueType, custom.ValueType);
            Assert.AreEqual(standard.XmlLang, custom.XmlLang);
            Assert.AreEqual(standard.XmlSpace, custom.XmlSpace);
            Assert.AreEqual(standard.LookupNamespace("foo"), custom.LookupNamespace("foo"));            
        }

        /// <summary>
        /// Test LookupNamespace()
        /// </summary>
        [Test]
        public void Test2()
        {
            string xml = @"<foo xmlns:f=""bar""/>";            
            XmlReader standard = XmlReader.Create(new StringReader(xml));

            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load(XmlReader.Create(new StringReader(copyTransform)));
            XslReader xslReader = new XslReader(xslt);
            xslReader.StartTransform(new XmlInput(new StringReader(xml)), null);

            standard.MoveToContent();
            xslReader.MoveToContent();

            Assert.IsTrue(standard.NodeType == xslReader.NodeType);
            Assert.IsTrue(standard.Name == xslReader.Name);
            string nsUri1 = standard.LookupNamespace("f");
            string nsUri2 = xslReader.LookupNamespace("f");
            Assert.IsTrue(nsUri1 == nsUri2,
                string.Format("'{0}' != '{1}'", nsUri1, nsUri2));
        }

        /// <summary>
        /// Test Read() after EOF
        /// </summary>
        [Test]        
        public void Test3()
        {
            string xml = @"<foo xmlns:f=""bar""/>";            
            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load(XmlReader.Create(new StringReader(copyTransform)));
            XslReader xslReader = new XslReader(xslt);
            xslReader.StartTransform(new XmlInput(new StringReader(xml)), null);
            while (!xslReader.EOF)
            {
                xslReader.Read();
            }
            Assert.IsFalse(xslReader.Read());
        }

        /// <summary>
        /// Test singlethread with small buffer
        /// </summary>
        [Test]
        public void Test4()
        {
            CompareWithStandardReader(false, 2);
        }

        /// <summary>
        /// Test different bufer sizes
        /// </summary>
        [Test]
        public void Test5()
        {
            for (int b = -1024; b < 1024; b+=100)
            {
                CompareWithStandardReader(false, b);
            }
            for (int b = -1024; b < 1024; b+=100)
            {
                CompareWithStandardReader(true, b);
            }
            CompareWithStandardReader(true, 0);
            CompareWithStandardReader(true, 1);
            CompareWithStandardReader(true, int.MinValue);            
        }

        /// <summary>
        /// Test reader restart
        /// </summary>
        [Test]
        public void Test6()
        {
            XmlReader r = GetReader(Globals.NorthwindResource);
            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load("../../XslReaderTests/test1.xslt");
            MemoryStream ms = new MemoryStream();
            XmlWriterSettings s = new XmlWriterSettings();
            s.OmitXmlDeclaration = true;
            XmlWriter w = XmlWriter.Create(ms, s);
            xslt.Transform(r, w);
            r.Close();
            w.Close();
            byte[] buf = ms.ToArray();
            XmlReader standard = XmlReader.Create(new MemoryStream(buf));
            XslReader xslReader = new XslReader(xslt, true, 16);
            xslReader.StartTransform(new XmlInput(GetReader(Globals.NorthwindResource)), null);
            xslReader.MoveToContent();
            xslReader.Read();
            //Now restart it
            xslReader.StartTransform(new XmlInput(GetReader(Globals.NorthwindResource)), null);
            CompareReaders(standard, xslReader);
        }

        [Test]
        [ExpectedException(typeof(OverflowException))]
        public void Test7()
        {
            CompareWithStandardReader(true, int.MaxValue / 2 + 2);
        }
    }
}
