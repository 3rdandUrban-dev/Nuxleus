using System;
using System.Xml;
using System.Xml.XPath;
using Mvp.Xml.Common;
using NUnit.Framework;

namespace Mvp.Xml.Tests.XmlBaseAwareXmlTextReaderTests
{
	[TestFixture]
	public class Tests
	{
        [Test]
        public void BasicTest() 
        {
            XmlTextReader r = new XmlBaseAwareXmlTextReader(
                Globals.GetResource( 
                this.GetType().Namespace + ".test.xml"));
            while (r.Read()) 
            {
                if (r.NodeType == XmlNodeType.Element) 
                {
                    switch (r.Name) 
                    {
                        case "catalog":
                            Assert.IsTrue(r.BaseURI == "");
                            break;
                        case "files":
                            Assert.IsTrue(r.BaseURI == "file:///d:/Files/");
                            break;
                        case "file":
                            Assert.IsTrue(r.BaseURI == "file:///d:/Files/");
                            break;
                        case "a":
                            Assert.IsTrue(r.BaseURI == "");
                            break;
                        case "b":
                            Assert.IsTrue(r.BaseURI == "file:///d:/Files/a/");
                            break;
                        case "c":
                            Assert.IsTrue(r.BaseURI == "file:///d:/Files/c/");
                            break;
                        case "e":
                            Assert.IsTrue(r.BaseURI == "file:///d:/Files/c/");
                            break;
                        case "d":
                            Assert.IsTrue(r.BaseURI == "file:///d:/Files/a/");
                            break;
                    }
                }
                else if (r.NodeType == XmlNodeType.Text && r.Value.Trim() != "") 
                {
                    Assert.IsTrue(r.BaseURI == "file:///d:/Files/c/");                    
                }
                else if (r.NodeType == XmlNodeType.Comment) 
                {
                    Assert.IsTrue(r.BaseURI == "file:///d:/Files/a/");                    
                }
                else if (r.NodeType == XmlNodeType.ProcessingInstruction) 
                {
                    Assert.IsTrue(r.BaseURI == "file:///d:/Files/");                    
                }
            }
            r.Close();
        }   

		[Test]
		public void ReaderWithPath() 
		{
			XmlTextReader r = new XmlBaseAwareXmlTextReader(@"..\..\XmlBaseAwareXmlTextReaderTests\relativeTest.xml");
			r.WhitespaceHandling = WhitespaceHandling.None;
			XPathDocument doc = new XPathDocument(r);
			XPathNavigator nav = doc.CreateNavigator();
			XPathNodeIterator ni = nav.Select("/catalog");
			ni.MoveNext();
			Assert.IsTrue(ni.Current.BaseURI.EndsWith("/XmlBaseAwareXmlTextReaderTests/relativeTest.xml"));
			ni = nav.Select("/catalog/relative/relativenode");
			ni.MoveNext();
			Console.WriteLine(ni.Current.BaseURI);
			Assert.IsTrue(ni.Current.BaseURI.IndexOf("/XmlBaseAwareXmlTextReaderTests/") != -1);
		}   

	}
}
