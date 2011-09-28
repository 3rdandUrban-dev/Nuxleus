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
	public class XmlEnumAttributeThumbprintTests
	{
		public XmlEnumAttributeThumbprintTests()
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
		public void SameName()
		{
			XmlEnumAttribute enum1 = new XmlEnumAttribute("enum1");
			XmlEnumAttribute enum2 = new XmlEnumAttribute("enum1");

			atts1.XmlEnum = enum1;
			atts2.XmlEnum = enum2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[Test]
		public void DifferentName()
		{
			XmlEnumAttribute enum1 = new XmlEnumAttribute("enum1");
			XmlEnumAttribute enum2 = new XmlEnumAttribute("enum2");

			atts1.XmlEnum = enum1;
			atts2.XmlEnum = enum2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}
	}
}
