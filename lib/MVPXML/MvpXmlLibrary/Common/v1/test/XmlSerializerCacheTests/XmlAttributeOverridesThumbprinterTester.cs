#region Using directives

using System;
using System.Collections;
using System.Text;
using NUnit.Framework;
using System.Xml.Serialization;
using Mvp.Xml.Serialization;

#endregion

namespace Mvp.Xml.Serialization.Tests
{
	public class SerializeMe
	{
		public string MyString;
	}

	public class SerializeMeToo
	{
		public string MyString;
	}

	[TestFixture]
	public class XmlAttributeOverridesThumbprinterTester
	{
		public XmlAttributeOverridesThumbprinterTester()
		{

		}

		[Test]
		public void SameEmptyObjectTwice()
		{
			// the same object should most certainly
			// result in the same signature, even when it's empty
			XmlAttributeOverrides ov = new XmlAttributeOverrides();
			ThumbprintHelpers.SameThumbprint(ov, ov);
		}

		[Test]
		public void DifferentEmptyObjects()
		{
			XmlAttributeOverrides ov1 = new XmlAttributeOverrides();
			XmlAttributeOverrides ov2 = new XmlAttributeOverrides();

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[Test]
		public void TwoDifferentEmptyObjects()
		{
			XmlAttributeOverrides ov1 = new XmlAttributeOverrides();
			XmlAttributeOverrides ov2 = new XmlAttributeOverrides();

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[Test]
		public void SameObjectWithRootAttribute()
		{
			XmlAttributeOverrides ov = new XmlAttributeOverrides();
			XmlAttributes atts = new XmlAttributes();
			atts.XmlRoot = new XmlRootAttribute("myRoot");
			ov.Add(typeof(SerializeMe), atts);

			ThumbprintHelpers.SameThumbprint(ov, ov);
		}
		[Test]
		public void TwoObjectsWithSameRootAttributeDifferentTypes()
		{
			XmlAttributeOverrides ov1 = new XmlAttributeOverrides();
			XmlAttributeOverrides ov2 = new XmlAttributeOverrides();
			XmlAttributes atts1 = new XmlAttributes();
			atts1.XmlRoot = new XmlRootAttribute("myRoot");
			ov1.Add(typeof(SerializeMe), atts1);

			XmlAttributes atts2 = new XmlAttributes();
			atts2.XmlRoot = new XmlRootAttribute("myRoot");
			ov2.Add(typeof(SerializeMeToo), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[Test]
		public void TwoObjectsWithDifferentRootAttribute()
		{
			XmlAttributeOverrides ov1 = new XmlAttributeOverrides();
			XmlAttributeOverrides ov2 = new XmlAttributeOverrides();

			XmlAttributes atts1 = new XmlAttributes();
			atts1.XmlRoot = new XmlRootAttribute("myRoot");
			ov1.Add(typeof(SerializeMe), atts1);

			XmlAttributes atts2 = new XmlAttributes();
			atts2.XmlRoot = new XmlRootAttribute("myOtherRoot");
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[Test]
		public void TwoObjectsWithSameRootAttribute()
		{
			XmlAttributeOverrides ov1 = new XmlAttributeOverrides();
			XmlAttributeOverrides ov2 = new XmlAttributeOverrides();

			XmlAttributes atts1 = new XmlAttributes();
			atts1.XmlRoot = new XmlRootAttribute("myRoot");
			ov1.Add(typeof(SerializeMe), atts1);

			XmlAttributes atts2 = new XmlAttributes();
			atts2.XmlRoot = new XmlRootAttribute("myRoot");
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[Test]
		public void SameXmlTypeTwice()
		{
			XmlAttributeOverrides ov = new XmlAttributeOverrides();
			XmlTypeAttribute att = new XmlTypeAttribute("myType");

			XmlAttributes atts = new XmlAttributes();
			atts.XmlType = att;

			ov.Add(typeof(SerializeMe), atts);

			ThumbprintHelpers.SameThumbprint(ov, ov);
		}

		[Test]
		public void SameXmlTypeDifferentObjects()
		{
			XmlAttributeOverrides ov1 = new XmlAttributeOverrides();
			XmlAttributeOverrides ov2 = new XmlAttributeOverrides();

			XmlTypeAttribute att = new XmlTypeAttribute("myType");

			XmlAttributes atts1 = new XmlAttributes();
			atts1.XmlType = att;
			XmlAttributes atts2 = new XmlAttributes();
			atts2.XmlType = att;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);
			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}
		[Test]
		public void DifferentXmlTypes()
		{
			XmlAttributeOverrides ov1 = new XmlAttributeOverrides();
			XmlAttributeOverrides ov2 = new XmlAttributeOverrides();

			XmlTypeAttribute att1 = new XmlTypeAttribute("myType");
			XmlTypeAttribute att2 = new XmlTypeAttribute("myOtherType");

			XmlAttributes atts1 = new XmlAttributes();
			atts1.XmlType = att1;
			XmlAttributes atts2 = new XmlAttributes();
			atts2.XmlType = att2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);
			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[Test]
		public void DifferentTypesSameXmlTypes()
		{
			XmlAttributeOverrides ov1 = new XmlAttributeOverrides();
			XmlAttributeOverrides ov2 = new XmlAttributeOverrides();

			XmlTypeAttribute att1 = new XmlTypeAttribute("myType");
			XmlTypeAttribute att2 = new XmlTypeAttribute("myType");

			XmlAttributes atts1 = new XmlAttributes();
			atts1.XmlType = att1;
			XmlAttributes atts2 = new XmlAttributes();
			atts2.XmlType = att2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMeToo), atts2);
			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[Test]
		public void SameMemberSameEmptyAttributes()
		{
			XmlAttributeOverrides ov1 = new XmlAttributeOverrides();
			XmlAttributeOverrides ov2 = new XmlAttributeOverrides();

			XmlAttributes att = new XmlAttributes();
			ov1.Add(typeof(SerializeMe), "TheMember", att);
			ov2.Add(typeof(SerializeMe), "TheMember", att);
			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[Test]
		public void SameMemberSameEmptyAttibuteOverrides()
		{
			XmlAttributeOverrides ov1 = new XmlAttributeOverrides();

			XmlAttributes att = new XmlAttributes();
			ov1.Add(typeof(SerializeMe), "TheMember", att);
			ThumbprintHelpers.SameThumbprint(ov1, ov1);
		}


	}
}
