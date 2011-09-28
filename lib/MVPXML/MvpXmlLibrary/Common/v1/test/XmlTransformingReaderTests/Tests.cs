#region using

using System;
using System.Xml;
using System.Xml.Xsl;
using System.IO;
using System.Diagnostics;

using Mvp.Xml.Common;
using NUnit.Framework;

#endregion


namespace Mvp.Xml.Tests.XmlTransformingReaderTests 
{
	[TestFixture]
	public class Tests 
	{
		[Test]
		public void TransformingTest() 
		{
			XslTransform tx = new XslTransform();
			tx.Load(new XmlTextReader(
				Globals.GetResource(this.GetType().Namespace + ".test.xsl")));

			XmlReader xtr = new XmlTransformingReader(
				new XmlTextReader(
				Globals.GetResource(this.GetType().Namespace + ".input.xml")),
				tx);

			//That's it, now let's dump out XmlTransformingReader to see what it returns
			StringWriter sw = new StringWriter();
			XmlTextWriter w = new XmlTextWriter(sw);
			w.Formatting = Formatting.Indented;
			w.WriteNode(xtr, false);
			xtr.Close();
			w.Close();

			Assert.AreEqual(sw.ToString(), @"<parts>
  <item SKU=""1001"" name=""Lawnmower"" price=""299.99"" />
  <item SKU=""1001"" name=""Electric drill"" price=""99.99"" />
  <item SKU=""1001"" name=""Hairdrier"" price=""39.99"" />
</parts>");
		}
	}
}