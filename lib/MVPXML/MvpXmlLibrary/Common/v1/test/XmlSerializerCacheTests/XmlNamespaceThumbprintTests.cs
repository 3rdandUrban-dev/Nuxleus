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
	public class XmlNamespaceThumbprintTests
	{
		public XmlNamespaceThumbprintTests()
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
		public void KeepNamespaces()
		{
			atts1.Xmlns = true;
			atts2.Xmlns = true;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[Test]
		public void DontKeepNamespaces()
		{
			atts1.Xmlns = true;
			atts2.Xmlns = false;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}
	}
}
