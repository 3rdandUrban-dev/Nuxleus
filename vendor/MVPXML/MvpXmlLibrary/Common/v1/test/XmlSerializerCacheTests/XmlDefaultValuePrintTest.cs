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
	public class XmlDefaultValuePrintTest
	{
		public XmlDefaultValuePrintTest()
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
		public void DifferentValue()
		{
			string val1 = "bla";
			int val2 = 1;

			atts1.XmlDefaultValue = val1;
			atts2.XmlDefaultValue = val2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[Test]
		public void SameValue()
		{
			string val1 = "bla";
			string val2 = "bla";

			atts1.XmlDefaultValue = val1;
			atts2.XmlDefaultValue = val2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

	}
}
