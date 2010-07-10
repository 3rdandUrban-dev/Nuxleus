﻿#region Using directives

using System;
using System.Collections;
using System.Text;
using System.Xml.Serialization;
using Mvp.Xml.Common.Serialization;
using NUnit.Framework;

#endregion

namespace Mvp.Xml.Serialization.Tests
{
	[TestFixture]
	public class XmlElementThumbprintTests
	{
		public XmlElementThumbprintTests()
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
		public void SameItemType()
		{
			XmlElementAttribute element1 = new XmlElementAttribute("myname", typeof(SerializeMe));
			XmlElementAttribute element2 = new XmlElementAttribute("myname", typeof(SerializeMe));

			atts1.XmlElements.Add(element1);
			atts2.XmlElements.Add(element2);

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[Test]
		public void DifferentItemTypes()
		{
			XmlElementAttribute element1 = new XmlElementAttribute("myname", typeof(SerializeMe));
			XmlElementAttribute element2 = new XmlElementAttribute("myname", typeof(SerializeMeToo));

			atts1.XmlElements.Add(element1);
			atts2.XmlElements.Add(element2);

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[Test]
		public void SameDataType()
		{
			XmlElementAttribute element1 = new XmlElementAttribute("myname", typeof(SerializeMe));
			element1.DataType = "myfirstxmltype";
			XmlElementAttribute element2 = new XmlElementAttribute("myname", typeof(SerializeMe));
			element2.DataType = "myfirstxmltype";

			atts1.XmlElements.Add(element1);
			atts2.XmlElements.Add(element2);

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[Test]
		public void DifferentDataTypes()
		{
			XmlElementAttribute element1 = new XmlElementAttribute();
			element1.DataType = "typeone";
			XmlElementAttribute element2 = new XmlElementAttribute();
			element2.DataType = "typetwo";

			atts1.XmlElements.Add(element1);
			atts2.XmlElements.Add(element2);

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[Test]
		public void SameElementName()
		{
			XmlElementAttribute element1 = new XmlElementAttribute("myname");
			XmlElementAttribute element2 = new XmlElementAttribute("myname");

			atts1.XmlElements.Add(element1);
			atts2.XmlElements.Add(element2);

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[Test]
		public void DifferentElementNames()
		{
			XmlElementAttribute element1 = new XmlElementAttribute("myname");
			XmlElementAttribute element2 = new XmlElementAttribute("myothername");

			atts1.XmlElements.Add(element1);
			atts2.XmlElements.Add(element2);

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[Test]
		public void DifferentTypes()
		{
			XmlElementAttribute element1 = new XmlElementAttribute("myname");
			XmlElementAttribute element2 = new XmlElementAttribute("myname");

			atts1.XmlElements.Add(element1);
			atts2.XmlElements.Add(element2);

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMeToo), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[Test]
		public void SameNamespace()
		{
			XmlElementAttribute element1 = new XmlElementAttribute("myname");
			element1.Namespace = "mynamespace";

			XmlElementAttribute element2 = new XmlElementAttribute("myname");
			element2.Namespace = "mynamespace";

			atts1.XmlElements.Add(element1);
			atts2.XmlElements.Add(element2);

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[Test]
		public void DifferentNamespace()
		{
			XmlElementAttribute element1 = new XmlElementAttribute("myname");
			element1.Namespace = "mynamespace";
			XmlElementAttribute element2 = new XmlElementAttribute("myname");
			element2.Namespace = "myothernamespace";

			atts1.XmlElements.Add(element1);
			atts2.XmlElements.Add(element2);

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[Test]
		public void SameNullable()
		{
			XmlElementAttribute element1 = new XmlElementAttribute("myname");
			element1.IsNullable = true;
			XmlElementAttribute element2 = new XmlElementAttribute("myname");
			element2.IsNullable = true;

			atts1.XmlElements.Add(element1);
			atts2.XmlElements.Add(element2);

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[Test]
		public void DifferentNullable()
		{
			XmlElementAttribute element1 = new XmlElementAttribute("myname");
			element1.IsNullable = true;
			XmlElementAttribute element2 = new XmlElementAttribute("myname");
			element2.IsNullable = false;

			atts1.XmlElements.Add(element1);
			atts2.XmlElements.Add(element2);

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[Test]
		public void SameForm()
		{
			XmlElementAttribute element1 = new XmlElementAttribute("myname");
			element1.Form = System.Xml.Schema.XmlSchemaForm.Qualified;
			XmlElementAttribute element2 = new XmlElementAttribute("myname");
			element2.Form = System.Xml.Schema.XmlSchemaForm.Qualified;

			atts1.XmlElements.Add(element1);
			atts2.XmlElements.Add(element2);

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[Test]
		public void DifferentForm()
		{
			XmlElementAttribute element1 = new XmlElementAttribute("myname");
			element1.Form = System.Xml.Schema.XmlSchemaForm.Qualified;
			XmlElementAttribute element2 = new XmlElementAttribute("myname");
			element2.Form = System.Xml.Schema.XmlSchemaForm.Unqualified;

			atts1.XmlElements.Add(element1);
			atts2.XmlElements.Add(element2);

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[Test]
		public void SameMemberName()
		{
			XmlElementAttribute element1 = new XmlElementAttribute("myname");
			XmlElementAttribute element2 = new XmlElementAttribute("myname");

			atts1.XmlElements.Add(element1);
			atts2.XmlElements.Add(element2);

			ov1.Add(typeof(SerializeMe), "TheMember", atts1);
			ov2.Add(typeof(SerializeMe), "TheMember", atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[Test]
		public void DifferentMemberName()
		{
			XmlElementAttribute element1 = new XmlElementAttribute("myname");
			XmlElementAttribute element2 = new XmlElementAttribute("myname");

			atts1.XmlElements.Add(element1);
			atts2.XmlElements.Add(element2);

			ov1.Add(typeof(SerializeMe), "TheMember", atts1);
			ov2.Add(typeof(SerializeMe), "TheOtherMember", atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}
	}
}
