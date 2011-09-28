#region Using directives

using System;
using System.Collections;
using System.Text;
using Mvp.Xml.Common.Serialization;
using System.Xml.Serialization;
using NUnit.Framework;

#endregion

namespace Mvp.Xml.Serialization.Tests
{
	[TestFixture]
	public class XmlTextThumbprintTests
	{
		public XmlTextThumbprintTests()
		{

		}

		XmlAttributeOverrides ov1;
		XmlAttributeOverrides ov2;

		XmlAttributes atts1;
		XmlAttributes atts2;

		[SetUp]
		public void SetUp()
		{
			ov1 = new XmlAttributeOverrides();
			ov2 = new XmlAttributeOverrides();

			atts1 = new XmlAttributes();
			atts2 = new XmlAttributes();
		}

		[Test]
		public void SameType()
		{
			XmlTextAttribute text1 = new XmlTextAttribute(typeof(SerializeMe));
			XmlTextAttribute text2 = new XmlTextAttribute(typeof(SerializeMe));

			atts1.XmlText = text1;
			atts2.XmlText = text2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1,ov2);
		}

		[Test]
		public void DifferentTypes()
		{
			XmlTextAttribute text1 = new XmlTextAttribute(typeof(SerializeMe));
			XmlTextAttribute text2 = new XmlTextAttribute(typeof(SerializeMeToo));

			atts1.XmlText = text1;
			atts2.XmlText = text2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);

		}

		[Test]
		public void SameDataType()
		{
			XmlTextAttribute text1 = new XmlTextAttribute();
			text1.DataType = "xmltype1";
			XmlTextAttribute text2 = new XmlTextAttribute();
			text2.DataType = "xmltype1";

			atts1.XmlText = text1;
			atts2.XmlText = text2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[Test]
		public void DifferentDataTypes()
		{
			XmlTextAttribute text1 = new XmlTextAttribute();
			text1.DataType = "xmltype1";
			XmlTextAttribute text2 = new XmlTextAttribute();
			text2.DataType = "xmltype2";

			atts1.XmlText = text1;
			atts2.XmlText = text2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);

		}
	}
}
