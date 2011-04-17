#region using

using System;
using System.Xml.Xsl;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using NUnit.Framework;
using Mvp.Xml.Common.XPath;

#endregion using

namespace Mvp.Xml.Tests.XmlNodeNavigatorTests 
{
	[TestFixture]
	public class Tests
	{		
		[Test]
		public void Test()
		{
			XslTransform xslt = new XslTransform();     
			xslt.Load(new XmlTextReader(
				Globals.GetResource(this.GetType().Namespace + ".test.xsl")));
			XmlDocument doc = new XmlDocument();
			doc.Load(new XmlTextReader(
				Globals.GetResource(Globals.LibraryResource)));

			//Navigator over first child of document element
			XPathNavigator nav = new XmlNodeNavigator(doc.DocumentElement.FirstChild);		
            StringWriter sw = new StringWriter();
			xslt.Transform(nav, null, sw, null);            
            Assert.AreEqual(sw.ToString(), doc.DocumentElement.FirstChild.OuterXml);
		}
	}
}