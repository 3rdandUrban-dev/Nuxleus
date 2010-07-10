using System;
using System.Xml.XPath;
using System.Xml;
using System.Text;
using System.IO;

using Mvp.Xml.Tests;
using Mvp.Xml.XPointer;
using NUnit.Framework;

namespace Mvp.Xml.XPointer.Test
{
	/// <summary>
	/// Unit tests for XPointerReader class.
	/// </summary>
	[TestFixture]
	public class XPointerReaderTests
	{                    
        /// <summary>
        /// xmlns() + xpath1() + namespaces works
        /// </summary>
        [Test]
        public void XmlNsXPath1SchemeTest() 
        {
            string xptr = "xmlns(m=mvp-xml)xpath1(m:dsPubs/m:publishers[m:pub_id='1389']/m:pub_name)";            
            XmlReader reader = new XmlTextReader(Globals.GetResource(Globals.PubsNsResource));
            XPointerReader xpr = new XPointerReader(reader, xptr);
            StringBuilder sb = new StringBuilder();
            while (xpr.Read()) 
            {
                sb.Append(xpr.ReadOuterXml());
            }
            string expected = @"<pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name>";
            Assert.AreEqual(sb.ToString(), expected);
        }

        /// <summary>
        /// xpath1() + namespaces doesn't work w/o xmlns()
        /// </summary>
        [Test]    
        [ExpectedException(typeof(NoSubresourcesIdentifiedException))]
        public void XPath1SchemeWithoutXmlnsTest() 
        {
            string xptr = "xpath1(m:dsPubs/m:publishers[m:pub_id='1389']/m:pub_name)";            
            XmlReader reader = new XmlTextReader(Globals.GetResource(Globals.PubsNsResource));
            XPointerReader xpr = new XPointerReader(reader, xptr);
            StringBuilder sb = new StringBuilder();
            while (xpr.Read()) {
                sb.Append(xpr.ReadOuterXml());
            }
            string expected = @"<pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name>";
            Assert.AreEqual(sb.ToString(), expected);
        }

        /// <summary>
        /// xpath1() that doesn't select a node w/o namespaces
        /// </summary>
        [Test]
        [ExpectedException(typeof(NoSubresourcesIdentifiedException))]
        public void XPath1SchemeNoSelectedNodeTest() 
        {
            string xptr = "xpath1(no-such-node/foo)";            
            XmlReader reader = new XmlTextReader(Globals.GetResource(Globals.PubsNsResource));
            XPointerReader xpr = new XPointerReader(reader, xptr);            
            while (xpr.Read()) {}            
        }
        
        /// <summary>
        /// xpath1() that returns scalar value, not a node
        /// </summary>
        [Test]
        [ExpectedException(typeof(NoSubresourcesIdentifiedException))]
        public void XPath1SchemeScalarResultTest() 
        {
            string xptr = "xpath1(2+2)";            
            XmlReader reader = new XmlTextReader(Globals.GetResource(Globals.PubsNsResource));
            XPointerReader xpr = new XPointerReader(reader, xptr);            
            while (xpr.Read()) {}            
        }

        /// <summary>
        /// xmlns() + xpointer() + namespaces works
        /// </summary>
        [Test]
        public void XmlNsXPointerSchemeTest() 
        {
            string xptr = "xmlns(m=mvp-xml)xpointer(m:dsPubs/m:publishers[m:pub_id='1389']/m:pub_name)";            
            XmlReader reader = new XmlTextReader(Globals.GetResource(Globals.PubsNsResource));
            XPointerReader xpr = new XPointerReader(reader, xptr);
            StringBuilder sb = new StringBuilder();
            while (xpr.Read()) 
            {
                sb.Append(xpr.ReadOuterXml());
            }
            string expected = @"<pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name>";
            Assert.AreEqual(sb.ToString(), expected);
        }

        /// <summary>
        /// xpointer() + namespaces doesn't work w/o xmlns()
        /// </summary>
        [Test]    
        [ExpectedException(typeof(NoSubresourcesIdentifiedException))]
        public void XPointerSchemeWithoutXmlnsTest() 
        {
            string xptr = "xpointer(m:dsPubs/m:publishers[m:pub_id='1389']/m:pub_name)";            
            XmlReader reader = new XmlTextReader(Globals.GetResource(Globals.PubsNsResource));
            XPointerReader xpr = new XPointerReader(reader, xptr);
            StringBuilder sb = new StringBuilder();
            while (xpr.Read()) 
            {
                sb.Append(xpr.ReadOuterXml());
            }
            string expected = @"<pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name>";
            Assert.AreEqual(sb.ToString(), expected);
        }

        /// <summary>
        /// xpointer() that doesn't select a node w/o namespaces
        /// </summary>
        [Test]
        [ExpectedException(typeof(NoSubresourcesIdentifiedException))]
        public void XPointerSchemeNoSelectedNodeTest() 
        {
            string xptr = "xpointer(no-such-node/foo)";            
            XmlReader reader = new XmlTextReader(Globals.GetResource(Globals.PubsNsResource));
            XPointerReader xpr = new XPointerReader(reader, xptr);            
            while (xpr.Read()) {}            
        }
        
        /// <summary>
        /// xpointer() that returns scalar value, not a node
        /// </summary>
        [Test]
        [ExpectedException(typeof(NoSubresourcesIdentifiedException))]
        public void XPointerSchemeScalarResultTest() 
        {
            string xptr = "xpointer(2+2)";            
            XmlReader reader = new XmlTextReader(Globals.GetResource(Globals.PubsNsResource));
            XPointerReader xpr = new XPointerReader(reader, xptr);            
            while (xpr.Read()) {}            
        }

        /// <summary>
        /// superfluous xmlns() doesn't hurt
        /// </summary>
        [Test]
        public void SuperfluousXmlNsSchemeTest() 
        {
            string xptr = "xmlns(m=mvp-xml)xpointer(dsPubs/publishers[pub_id='1389']/pub_name)";            
            XmlReader reader = new XmlTextReader(Globals.GetResource(Globals.PubsResource));
            XPointerReader xpr = new XPointerReader(reader, xptr);
            StringBuilder sb = new StringBuilder();
            while (xpr.Read()) 
            {
                sb.Append(xpr.ReadOuterXml());
            }
            string expected = @"<pub_name>Algodata Infosystems</pub_name>";
            Assert.AreEqual(sb.ToString(), expected);
        }

        /// <summary>
        /// xpointer() + xmlns() + namespaces doesn't work
        /// </summary>
        [Test]
        [ExpectedException(typeof(NoSubresourcesIdentifiedException))]
        public void XmlnsAfterTest() 
        {
            string xptr = "xpointer(m:dsPubs/m:publishers[m:pub_id='1389']/m:pub_name)xmlns(m=mvp-xml)";            
            XmlReader reader = new XmlTextReader(Globals.GetResource(Globals.PubsNsResource));
            XPointerReader xpr = new XPointerReader(reader, xptr);            
            while (xpr.Read()) {}            
        }
        
        /// <summary>
        /// namespace re3efinition doesn't hurt
        /// </summary>
        [Test]
        public void NamespaceRedefinitionTest() 
        {
            string xptr = "xmlns(m=mvp-xml)xmlns(m=http://foo.com)xmlns(m=mvp-xml)xpointer(m:dsPubs/m:publishers[m:pub_id='1389']/m:pub_name)";            
            XmlReader reader = new XmlTextReader(Globals.GetResource(Globals.PubsNsResource));
            XPointerReader xpr = new XPointerReader(reader, xptr);
            StringBuilder sb = new StringBuilder();
            while (xpr.Read()) 
            {
                sb.Append(xpr.ReadOuterXml());
            }
            string expected = @"<pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name>";
            Assert.AreEqual(sb.ToString(), expected);
        }
                
        /// <summary>
        /// Shorthand pointer works
        /// </summary>
        [Test]
        public void ShorthandTest() 
        {           
            string xptr = "o10535";                        
            XmlReader reader = new XmlTextReader("../../northwind.xml");            
            XPointerReader xpr = new XPointerReader(reader, xptr);
            string expected = @"<Item orderID=""o10535"">
                <OrderDate> 6/13/95</OrderDate>
                <ShipAddress> Mataderos  2312</ShipAddress>
            </Item>";
            while (xpr.Read()) 
            {
                Assert.AreEqual(xpr.ReadOuterXml(), expected);
                return;
            }            
            throw new InvalidOperationException("This means shorthand XPointer didn't work as expected.");
        }

        /// <summary>
        /// Shorthand pointer works via stream
        /// </summary>
        [Test]
        public void ShorthandViaStreamTest() 
        {           
            string xptr = "o10535";                        
            FileInfo file = new FileInfo("../../northwind.xml");
            using (FileStream fs = file.OpenRead()) 
            {
                XPointerReader xpr = new XPointerReader(
                    new XmlTextReader(file.FullName, fs), xptr);
                string expected = @"<Item orderID=""o10535"">
                <OrderDate> 6/13/95</OrderDate>
                <ShipAddress> Mataderos  2312</ShipAddress>
            </Item>";
                while (xpr.Read()) 
                {
                    Assert.AreEqual(xpr.ReadOuterXml(), expected);
                    return;
                }            
                throw new InvalidOperationException("This means shorthand XPointer didn't work as expected.");
            }
        }

        /// <summary>
        /// Shorthand pointer points to nothing
        /// </summary>
        [Test]
         [ExpectedException(typeof(NoSubresourcesIdentifiedException))]
        public void ShorthandNotFoundTest() 
        {           
            string xptr = "no-such-id";                        
            XmlReader reader = new XmlTextReader("../../northwind.xml");            
            XPointerReader xpr = new XPointerReader(reader, xptr);                        
        }

        /// <summary>
        /// element() scheme pointer works
        /// </summary>
        [Test]
        public void ElementSchemeTest() 
        {           
            string xptr = "element(o10535)";                        
            XmlReader reader = new XmlTextReader("../../northwind.xml");            
            XPointerReader xpr = new XPointerReader(reader, xptr);
            string expected = @"<Item orderID=""o10535"">
                <OrderDate> 6/13/95</OrderDate>
                <ShipAddress> Mataderos  2312</ShipAddress>
            </Item>";
            while (xpr.Read()) 
            {
                Assert.AreEqual(xpr.ReadOuterXml(), expected);
                return;
            }            
            throw new InvalidOperationException("This means XPointer didn't work as expected.");
        }

        /// <summary>
        /// element() scheme pointer works
        /// </summary>
        [Test]
        public void ElementSchemeTest2() 
        {           
            string xptr = "element(o10535/1)";                        
            XmlReader reader = new XmlTextReader("../../northwind.xml");            
            XPointerReader xpr = new XPointerReader(reader, xptr);
            string expected = @"<OrderDate> 6/13/95</OrderDate>";
            while (xpr.Read()) 
            {
                Assert.AreEqual(xpr.ReadOuterXml(), expected);
                return;
            }            
            throw new InvalidOperationException("This means XPointer didn't work as expected.");
        }

        /// <summary>
        /// element() scheme pointer works
        /// </summary>
        [Test]
        public void ElementSchemeTest3() 
        {           
            string xptr = "element(/1/1/2)";                        
            XmlReader reader = new XmlTextReader("../../northwind.xml");            
            XPointerReader xpr = new XPointerReader(reader, xptr);
            string expected = @"<CompanyName> Alfreds Futterkiste</CompanyName>";
            while (xpr.Read()) 
            {
                Assert.AreEqual(xpr.ReadOuterXml(), expected);
                return;
            }            
            throw new InvalidOperationException("This means XPointer didn't work as expected.");
        }        

        /// <summary>
        /// element() scheme pointer points to nothing
        /// </summary>
        [Test]
        [ExpectedException(typeof(NoSubresourcesIdentifiedException))]
        public void ElementSchemeNotFoundTest() 
        {           
            string xptr = "element(no-such-id)";                        
            XmlReader reader = new XmlTextReader("../../northwind.xml");            
            XPointerReader xpr = new XPointerReader(reader, xptr);                        
        }

        /// <summary>
        /// compound pointer
        /// </summary>
        [Test]
        public void CompoundPointerTest() 
        {           
            string xptr = "xmlns(p=12345)xpath1(/no/such/node) xpointer(/and/such) element(/1/1/2) element(o10535/1)";                        
            XmlReader reader = new XmlTextReader("../../northwind.xml");            
            XPointerReader xpr = new XPointerReader(reader, xptr);
            string expected = @"<CompanyName> Alfreds Futterkiste</CompanyName>";
            while (xpr.Read()) 
            {
                Assert.AreEqual(xpr.ReadOuterXml(), expected);
                return;
            }            
            throw new InvalidOperationException("This means XPointer didn't work as expected.");
        }       

        /// <summary>
        /// Unknown scheme pointer
        /// </summary>
        [Test]
        public void UnknownSchemeTest() 
        {           
            string xptr = "dummy(foo) element(/1/1/2)";                        
            XmlReader reader = new XmlTextReader("../../northwind.xml");            
            XPointerReader xpr = new XPointerReader(reader, xptr);
            string expected = @"<CompanyName> Alfreds Futterkiste</CompanyName>";
            while (xpr.Read()) 
            {
                Assert.AreEqual(xpr.ReadOuterXml(), expected);
                return;
            }            
            throw new InvalidOperationException("This means XPointer didn't work as expected.");
        }      
 
        /// <summary>
        /// Unknown scheme pointer
        /// </summary>
        [Test]
        public void UnknownSchemeTest2() 
        {           
            string xptr = "foo:dummy(bar) element(/1/1/2)";                        
            XmlReader reader = new XmlTextReader("../../northwind.xml");            
            XPointerReader xpr = new XPointerReader(reader, xptr);
            string expected = @"<CompanyName> Alfreds Futterkiste</CompanyName>";
            while (xpr.Read()) 
            {
                Assert.AreEqual(xpr.ReadOuterXml(), expected);
                return;
            }            
            throw new InvalidOperationException("This means XPointer didn't work as expected.");
        }      

        /// <summary>
        /// Unknown scheme pointer
        /// </summary>
        [Test]        
        public void UnknownSchemeTest3() 
        {           
            string xptr = "xmlns(foo=http://foo.com/schemas)foo:dummy(bar) element(/1/1/2)";                        
            XmlReader reader = new XmlTextReader("../../northwind.xml");            
            XPointerReader xpr = new XPointerReader(reader, xptr);
            string expected = @"<CompanyName> Alfreds Futterkiste</CompanyName>";
            while (xpr.Read()) 
            {
                Assert.AreEqual(xpr.ReadOuterXml(), expected);
                return;
            }            
            throw new InvalidOperationException("This means XPointer didn't work as expected.");
        }      
        
        /// <summary>
        /// XSD-defined ID
        /// </summary>
        //[Test]        
        //public void XSDDefnedIDTest() 
        //{           
        //    string xptr = "element(id1389/1)";                        
        //    XmlReader reader = new XmlTextReader("../../pubsNS.xml");
        //    XPointerReader xpr = new XPointerReader(reader, xptr, true);
        //    string expected = @"<pub_name>Algodata Infosystems</pub_name>";
        //    while (xpr.Read()) 
        //    {
        //        Assert.AreEqual(xpr.ReadOuterXml(), expected);
        //        return;
        //    }            
        //    throw new InvalidOperationException("This means XPointer didn't work as expected.");
        //}      
	}
}
