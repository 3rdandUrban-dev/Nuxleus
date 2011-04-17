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
	public class XmlAttributeThumbprintTests
	{
		public XmlAttributeThumbprintTests()
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
			XmlAttributeAttribute attribute1 = new XmlAttributeAttribute("myname");
			XmlAttributeAttribute attribute2 = new XmlAttributeAttribute("myname");

			atts1.XmlAttribute = attribute1;
			atts2.XmlAttribute = attribute2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[Test]
		public void XmlArrayDifferentTypes()
		{
			XmlAttributeAttribute attribute1 = new XmlAttributeAttribute("myname");
			XmlAttributeAttribute attribute2 = new XmlAttributeAttribute("myname");

			atts1.XmlAttribute = attribute1;
			atts2.XmlAttribute = attribute2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMeToo), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[Test]
		public void XmlArrayDifferentNames()
		{
			XmlAttributeAttribute attribute1 = new XmlAttributeAttribute("myname");
			XmlAttributeAttribute attribute2 = new XmlAttributeAttribute("myothername");

			atts1.XmlAttribute = attribute1;
			atts2.XmlAttribute = attribute2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[Test]
		public void XmlArraySameNamespace()
		{
			XmlAttributeAttribute attribute1 = new XmlAttributeAttribute("myname");
			attribute1.Namespace = "mynamespace";

			XmlAttributeAttribute attribute2 = new XmlAttributeAttribute("myname");
			attribute2.Namespace = "mynamespace";

			atts1.XmlAttribute = attribute1;
			atts2.XmlAttribute = attribute2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[Test]
		public void XmlArrayDifferentNamespace()
		{
			XmlAttributeAttribute attribute1 = new XmlAttributeAttribute("myname");
			attribute1.Namespace = "mynamespace";
			XmlAttributeAttribute attribute2 = new XmlAttributeAttribute("myname");
			attribute2.Namespace = "myothernamespace";

			atts1.XmlAttribute = attribute1;
			atts2.XmlAttribute = attribute2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[Test]
		public void XmlArraySameForm()
		{
			XmlAttributeAttribute attribute1 = new XmlAttributeAttribute("myname");
			attribute1.Form = System.Xml.Schema.XmlSchemaForm.Qualified;
			XmlAttributeAttribute attribute2 = new XmlAttributeAttribute("myname");
			attribute2.Form = System.Xml.Schema.XmlSchemaForm.Qualified;

			atts1.XmlAttribute = attribute1;
			atts2.XmlAttribute = attribute2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[Test]
		public void XmlArrayDifferentForm()
		{
			XmlAttributeAttribute attribute1 = new XmlAttributeAttribute("myname");
			attribute1.Form = System.Xml.Schema.XmlSchemaForm.Qualified;
			XmlAttributeAttribute attribute2 = new XmlAttributeAttribute("myname");
			attribute2.Form = System.Xml.Schema.XmlSchemaForm.Unqualified;

			atts1.XmlAttribute = attribute1;
			atts2.XmlAttribute = attribute2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[Test]
		public void XmlArraySameMemberName()
		{
			XmlAttributeAttribute attribute1 = new XmlAttributeAttribute("myname");
			XmlAttributeAttribute attribute2 = new XmlAttributeAttribute("myname");

			atts1.XmlAttribute = attribute1;
			atts2.XmlAttribute = attribute2;

			ov1.Add(typeof(SerializeMe), "TheMember", atts1);
			ov2.Add(typeof(SerializeMe), "TheMember", atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[Test]
		public void DifferentMemberName()
		{
			XmlAttributeAttribute attribute1 = new XmlAttributeAttribute("myname");
			XmlAttributeAttribute attribute2 = new XmlAttributeAttribute("myname");

			atts1.XmlAttribute = attribute1;
			atts2.XmlAttribute = attribute2;

			ov1.Add(typeof(SerializeMe), "TheMember", atts1);
			ov2.Add(typeof(SerializeMe), "TheOtherMember", atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}
	}
}
