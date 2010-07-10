using System;
using System.Diagnostics;
using System.Xml;
using System.IO;
using System.Text;

using Mvp.Xml.XInclude;
using Mvp.Xml.Common;
using NUnit.Framework;

namespace Mvp.Xml.XInclude.Test
{
    /// <summary>
    /// XIncludeReader general tests.
    /// </summary>
    [TestFixture]
    public class XIncludeReaderTests
    {        

        public XIncludeReaderTests() 
        {
            Debug.Listeners.Add(new TextWriterTraceListener(Console.Error));
        }
        
        /// <summary>
        /// Utility method for running tests.
        /// </summary>        
        public static void RunAndCompare(string source, string result) 
        {
            RunAndCompare(source, result, false);
        }        

        /// <summary>
        /// Utility method for running tests.
        /// </summary>        
        public static void RunAndCompare(string source, string result, bool textAsCDATA) 
        {
            RunAndCompare(source, result, textAsCDATA, null);
        }
        
        /// <summary>
        /// Utility method for running tests.
        /// </summary>        
        public static void RunAndCompare(string source, string result, bool textAsCDATA, XmlResolver resolver) 
        {                                 
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;                  
            XIncludingReader xir = new XIncludingReader(source);
            if (resolver != null)
                xir.XmlResolver = resolver;
            xir.ExposeTextInclusionsAsCDATA = textAsCDATA;            
//            while (xir.Read()) 
//            {
//                Console.WriteLine("{0} | {1} | {2} | {3}", xir.NodeType, xir.Name, xir.Value, xir.IsEmptyElement);                
//            }
//            throw new Exception();
            try 
            {
                doc.Load(xir);
            } 
            catch (Exception e)
            {
                xir.Close();
                throw e;
            }
            xir.Close();
            XmlTextReader r1 = new XmlTextReader(new StringReader(doc.OuterXml));            
            r1.WhitespaceHandling = WhitespaceHandling.Significant;    
            XmlTextReader r2 = new XmlTextReader(result);            
            r2.WhitespaceHandling = WhitespaceHandling.Significant;
            try 
            {
                while (r1.Read()) 
                {
                    Assert.IsTrue(r2.Read()); 
                    while (r1.NodeType == XmlNodeType.XmlDeclaration ||
                        r1.NodeType == XmlNodeType.Whitespace)
                        r1.Read();
                    while (r2.NodeType == XmlNodeType.XmlDeclaration ||
                        r2.NodeType == XmlNodeType.Whitespace)
                        r2.Read();
                    Assert.AreEqual(r1.XmlLang, r2.XmlLang);
                    switch (r1.NodeType) 
                    {
                        case XmlNodeType.Attribute:
                            Assert.AreEqual(r2.NodeType, XmlNodeType.Attribute);
                            Assert.AreEqual(r1.Name, r2.Name);
                            Assert.AreEqual(r1.LocalName, r2.LocalName);
                            Assert.AreEqual(r1.NamespaceURI, r2.NamespaceURI);
                            Assert.AreEqual(r1.Value, r2.Value);
                            break;                   
                        case XmlNodeType.CDATA:
                            Assert.IsTrue(r2.NodeType == XmlNodeType.CDATA || r2.NodeType == XmlNodeType.Text);
                            Assert.AreEqual(r1.Value, r2.Value);
                            break;
                        case XmlNodeType.Comment:
                            Assert.AreEqual(r2.NodeType, XmlNodeType.Comment);
                            Assert.AreEqual(r1.Value, r2.Value);
                            break;
                        case XmlNodeType.DocumentType:
                            Assert.AreEqual(r2.NodeType, XmlNodeType.DocumentType);
                            Assert.AreEqual(r1.Name, r2.Name);                        
                            //Ok, don't compare DTD content
                            //Assert.AreEqual(r1.Value, r2.Value);
                            break;
                        case XmlNodeType.Element:
                            Assert.AreEqual(r2.NodeType, XmlNodeType.Element);
                            Assert.AreEqual(r1.Name, r2.Name);
                            Assert.AreEqual(r1.LocalName, r2.LocalName);
                            Assert.AreEqual(r1.NamespaceURI, r2.NamespaceURI);
                            Assert.AreEqual(r1.Value, r2.Value);
                            break;
                        case XmlNodeType.Entity:
                            Assert.AreEqual(r2.NodeType, XmlNodeType.Entity);
                            Assert.AreEqual(r1.Name, r2.Name);
                            Assert.AreEqual(r1.Value, r2.Value);
                            break;
                        case XmlNodeType.EndElement:
                            Assert.AreEqual(r2.NodeType, XmlNodeType.EndElement);                        
                            break;
                        case XmlNodeType.EntityReference:
                            Assert.AreEqual(r2.NodeType, XmlNodeType.EntityReference);
                            Assert.AreEqual(r1.Name, r2.Name);
                            Assert.AreEqual(r1.Value, r2.Value);
                            break;
                        case XmlNodeType.Notation:
                            Assert.AreEqual(r2.NodeType, XmlNodeType.Notation);
                            Assert.AreEqual(r1.Name, r2.Name);
                            Assert.AreEqual(r1.Value, r2.Value);
                            break;
                        case XmlNodeType.ProcessingInstruction:
                            Assert.AreEqual(r2.NodeType, XmlNodeType.ProcessingInstruction);
                            Assert.AreEqual(r1.Name, r2.Name);
                            Assert.AreEqual(r1.Value, r2.Value);
                            break;
                        case XmlNodeType.SignificantWhitespace:
                            Assert.AreEqual(r2.NodeType, XmlNodeType.SignificantWhitespace);
                            Assert.AreEqual(r1.Value, r2.Value);
                            break;
                        case XmlNodeType.Text:
                            Assert.IsTrue(r2.NodeType == XmlNodeType.CDATA || r2.NodeType == XmlNodeType.Text);
                            Assert.AreEqual(r1.Value.Replace("\r\n", "\n").Replace("\r", "").Trim(), r2.Value.Replace("\r\n", "\n").Replace("\r", "").Trim());
                            break;                    
                        default:
                            break;
                    }                     
                }
                Assert.IsFalse(r2.Read());
                Assert.IsTrue(r1.ReadState == ReadState.EndOfFile || r1.ReadState == ReadState.Closed);
                Assert.IsTrue(r2.ReadState == ReadState.EndOfFile || r2.ReadState == ReadState.Closed);
            } 
            catch(Exception e) 
            {
                r1.Close();
                r1 = null;
                r2.Close();
                r2 = null;
                ReportResults(result, doc);
                throw e;
            }
            finally 
            {                
                if (r1 != null)
                    r1.Close();
                if (r2 != null)
                    r2.Close();
            }
            ReportResults(result, doc);
        }    
    
        private static void ReportResults(string expected, XmlDocument actual) 
        {
            StreamReader sr = new StreamReader(expected);
            string expectedResult = sr.ReadToEnd();
            sr.Close();                
            MemoryStream ms = new MemoryStream();
            actual.Save(new StreamWriter(ms, Encoding.UTF8));  
            ms.Position = 0;
            string actualResult = new StreamReader(ms).ReadToEnd();
            Console.WriteLine("\n-----------Expected result:-----------\n{0}", expectedResult);
            Console.WriteLine("-----------Actual result:-----------\n{0}", actualResult);
        }
      
        /// <summary>
        /// General test - it should work actually.
        /// </summary>
        [Test]
        public void ItWorksAtLeast() 
        {
            RunAndCompare("../../tests/document.xml", "../../results/document.xml");            
        }
        

        /// <summary>
        /// Non XML character in the included document.
        /// </summary>
        [Test]
        [ExpectedException(typeof(NonXmlCharacterException))]
        public void NonXMLChar() 
        {
            RunAndCompare("../../tests/nonxmlchar.xml", "../../results/nonxmlchar.xml");            
        }        

        /// <summary>
        /// File not found and no fallback.
        /// </summary>
        [Test]
        [ExpectedException(typeof(FatalResourceException))]
        public void FileNotFound() 
        {
            RunAndCompare("../../tests/filenotfound.xml", "../../results/filenotfound.xml");            
        }        

        /// <summary>
        /// Includes itself by url.
        /// </summary>
        [Test]        
        public void IncludesItselfByUrl() 
        {
            RunAndCompare("../../tests/includesitself.xml", "../../results/includesitself.xml");            
        }        

        /// <summary>
        /// Includes itself by url - no href - as text.
        /// </summary>
        [Test]        
        [ExpectedException(typeof(FatalResourceException))]
        public void IncludesItselfNoHrefText() 
        {
            RunAndCompare("../../tests/includesitself-nohref-text.xml", "../../results/includesitself-nohref-text.xml");            
        }        

        /// <summary>
        /// Text inclusion. 
        /// </summary>
        [Test]        
        public void TextInclusion() 
        {
            RunAndCompare("../../tests/working_example.xml", "../../results/working_example.xml");            
        }
        
        /// <summary>
        /// Text inclusion. 
        /// </summary>
        [Test]        
        public void TextInclusion2() 
        {
            RunAndCompare("../../tests/working_example2.xml", "../../results/working_example2.xml");            
        }        

        /// <summary>
        /// Fallback.
        /// </summary>
        [Test]        
        public void Fallback() 
        {
            RunAndCompare("../../tests/fallback.xml", "../../results/fallback.xml");            
        }        

        /// <summary>
        /// XPointer.
        /// </summary>
        [Test]        
        public void XPointer() 
        {
            RunAndCompare("../../tests/xpointer.xml", "../../results/xpointer.xml");            
        }        

        /// <summary>
        /// xml:lang fixup
        /// </summary>
        [Test]        
        public void XmlLangTest() 
        {
            RunAndCompare("../../tests/langtest.xml", "../../results/langtest.xml");                                    
        }        

        /// <summary>
        /// ReadOuterXml() test.
        /// </summary>
        [Test]
        public void OuterXmlTest() 
        {
            XIncludingReader xir = new XIncludingReader("../../tests/document.xml");
            xir.MoveToContent();
            string outerXml = xir.ReadOuterXml();
            xir.Close();
            xir = new XIncludingReader("../../tests/document.xml");
            xir.MoveToContent();
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.Load(xir);              
            string outerXml2 = doc.DocumentElement.OuterXml;
            Assert.AreEqual(outerXml, outerXml2);
        }        

        /// <summary>
        /// ReadInnerXml() test.
        /// </summary>
        [Test]
        public void InnerXmlTest() 
        {
            XIncludingReader xir = new XIncludingReader("../../tests/document.xml");
            xir.MoveToContent();
            string innerXml = xir.ReadInnerXml();
            xir.Close();
            xir = new XIncludingReader("../../tests/document.xml");
            xir.MoveToContent();
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.Load(xir);            
            string innerXml2 = doc.DocumentElement.InnerXml;
            Assert.AreEqual(innerXml, innerXml2);
        }        

        /// <summary>
        /// Depth test.
        /// </summary>
        [Test]
        public void DepthTest() 
        {
            XIncludingReader xir = new XIncludingReader("../../tests/document.xml");
            StringBuilder sb = new StringBuilder();
            while (xir.Read()) 
            {
                Console.WriteLine("{0} | {1} | {2} | {3}", 
                    xir.NodeType, xir.Name, xir.Value, xir.Depth);
                sb.Append(xir.Depth);
            }
            string expected = "00011211111111223221100";
            Assert.AreEqual(sb.ToString(), expected);
        }
        
        /// <summary>
        /// Custom resolver test.
        /// </summary>
        [Test]        
        public void CustomResolver() 
        {
            RunAndCompare("../../tests/resolver.xml", "../../results/resolver.xml", false, new TestResolver());            
        }

        /// <summary>
        /// Test for a bug discovered by Martin Wickett.
        /// </summary>
        [Test]        
        public void Test_Martin() 
        {
            RunAndCompare("../../tests/test-Martin.xml", "../../results/test-Martin.xml");            
        }

        /// <summary>
        /// Test for string as input (no base URI)
        /// </summary>
        [Test]        
        [ExpectedException(typeof(FatalResourceException))]
        public void NoBaseURITest() 
        {
            StreamReader sr = new StreamReader("../../tests/document.xml");
            string xml = sr.ReadToEnd();
            sr.Close();
            XIncludingReader xir = new XIncludingReader(new StringReader(xml));
            XmlTextWriter w = new XmlTextWriter(Console.Out);
            while (xir.Read());                
        }

        /// <summary>
        /// Caching test.
        /// </summary>
        [Test]        
        public void CachingTest() 
        {
            RunAndCompare("../../tests/caching.xml", "../../results/caching.xml");            
        }

        /// <summary>
        /// Infinite loop (bug 1187498)
        /// </summary>
        [Test]        
        public void LoopTest() 
        {
            RunAndCompare("../../tests/loop.xml", "../../results/loop.xml");            
        }

    }
    
    public class TestResolver : XmlUrlResolver 
    {
        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            if (absoluteUri.Scheme == "textreader")
                return new StringReader(@"<text attr=""val"">From custom resolver (as TextReader)</text>"); 
            else if (absoluteUri.Scheme == "stream") 
            {
                return File.OpenRead("../../results/document.xml");
            }
            else if (absoluteUri.Scheme == "xmlreader") 
            {
                return new XmlTextReader(absoluteUri.AbsoluteUri, new StringReader(@"<text attr=""val"">From custom resolver (as XmlReader)</text>")); 
            }
            else
                return base.GetEntity(absoluteUri, role, ofObjectToReturn);
        }

    }
}
