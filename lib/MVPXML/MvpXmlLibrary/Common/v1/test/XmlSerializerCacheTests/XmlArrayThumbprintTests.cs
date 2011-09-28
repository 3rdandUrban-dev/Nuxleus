﻿#region Using directives

using System;
using System.Collections;
using System.Text;
using NUnit.Framework;
using Mvp.Xml.Common.Serialization;
using System.Xml.Serialization;
#endregion

namespace Mvp.Xml.Serialization.Tests
{
	[TestFixture]
	public class XmlArrayThumbprintTests
	{
		public XmlArrayThumbprintTests()
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
		public void XmlArraySameName()
		{
			XmlArrayAttribute array1 = new XmlArrayAttribute("myname");
			XmlArrayAttribute array2 = new XmlArrayAttribute("myname");

			atts1.XmlArray = array1;
			atts2.XmlArray = array2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[Test]
		public void XmlArrayDifferentTypes()
		{
			XmlArrayAttribute array1 = new XmlArrayAttribute("myname");
			XmlArrayAttribute array2 = new XmlArrayAttribute("myname");

			atts1.XmlArray = array1;
			atts2.XmlArray = array2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMeToo), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[Test]
		public void XmlArrayDifferentNames()
		{
			XmlArrayAttribute array1 = new XmlArrayAttribute("myname");
			XmlArrayAttribute array2 = new XmlArrayAttribute("myothername");

			atts1.XmlArray = array1;
			atts2.XmlArray = array2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[Test]
		public void XmlArraySameNamespace()
		{
			XmlArrayAttribute array1 = new XmlArrayAttribute("myname");
			array1.Namespace = "mynamespace";

			XmlArrayAttribute array2 = new XmlArrayAttribute("myname");
			array2.Namespace = "mynamespace";

			atts1.XmlArray = array1;
			atts2.XmlArray = array2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[Test]
		public void XmlArrayDifferentNamespace()
		{
			XmlArrayAttribute array1 = new XmlArrayAttribute("myname");
			array1.Namespace = "mynamespace";
			XmlArrayAttribute array2 = new XmlArrayAttribute("myname");
			array2.Namespace = "myothernamespace";

			atts1.XmlArray = array1;
			atts2.XmlArray = array2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[Test]
		public void XmlArraySameNullable()
		{
			XmlArrayAttribute array1 = new XmlArrayAttribute("myname");
			array1.IsNullable = true;
			XmlArrayAttribute array2 = new XmlArrayAttribute("myname");
			array2.IsNullable = true;

			atts1.XmlArray = array1;
			atts2.XmlArray = array2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[Test]
		public void XmlArrayDifferentNullable()
		{
			XmlArrayAttribute array1 = new XmlArrayAttribute("myname");
			array1.IsNullable = true;
			XmlArrayAttribute array2 = new XmlArrayAttribute("myname");
			array2.IsNullable = false;

			atts1.XmlArray = array1;
			atts2.XmlArray = array2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[Test]
		public void XmlArraySameForm()
		{
			XmlArrayAttribute array1 = new XmlArrayAttribute("myname");
			array1.Form = System.Xml.Schema.XmlSchemaForm.Qualified;
			XmlArrayAttribute array2 = new XmlArrayAttribute("myname");
			array2.Form = System.Xml.Schema.XmlSchemaForm.Qualified;

			atts1.XmlArray = array1;
			atts2.XmlArray = array2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[Test]
		public void XmlArrayDifferentForm()
		{
			XmlArrayAttribute array1 = new XmlArrayAttribute("myname");
			array1.Form = System.Xml.Schema.XmlSchemaForm.Qualified;
			XmlArrayAttribute array2 = new XmlArrayAttribute("myname");
			array2.Form = System.Xml.Schema.XmlSchemaForm.Unqualified;

			atts1.XmlArray = array1;
			atts2.XmlArray = array2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[Test]
		public void XmlArraySameMemberName()
		{
			XmlArrayAttribute array1 = new XmlArrayAttribute("myname");
			XmlArrayAttribute array2 = new XmlArrayAttribute("myname");

			atts1.XmlArray = array1;
			atts2.XmlArray = array2;

			ov1.Add(typeof(SerializeMe), "TheMember", atts1);
			ov2.Add(typeof(SerializeMe), "TheMember", atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[Test]
		public void XmlArrayDifferentMemberName()
		{
			XmlArrayAttribute array1 = new XmlArrayAttribute("myname");
			XmlArrayAttribute array2 = new XmlArrayAttribute("myname");

			atts1.XmlArray = array1;
			atts2.XmlArray = array2;

			ov1.Add(typeof(SerializeMe), "TheMember", atts1);
			ov2.Add(typeof(SerializeMe), "TheOtherMember", atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}
	}
}
