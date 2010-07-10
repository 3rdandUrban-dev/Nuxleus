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
	public class XmlRootThumbprintTests
	{
		public XmlRootThumbprintTests()
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
		public void SameDataType()
		{
			XmlRootAttribute root1 = new XmlRootAttribute("myname");
			root1.DataType = "myfirstxmltype";
			XmlRootAttribute root2 = new XmlRootAttribute("myname");
			root2.DataType = "myfirstxmltype";

			atts1.XmlRoot = root1;
			atts2.XmlRoot = root2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[Test]
		public void DifferentDataType()
		{
			XmlRootAttribute root1 = new XmlRootAttribute("myname");
			root1.DataType = "myfirstxmltype";
			XmlRootAttribute root2 = new XmlRootAttribute("myname");
			root2.DataType = "mysecondxmltype";

			atts1.XmlRoot = root1;
			atts2.XmlRoot = root2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[Test]
		public void SameElementName()
		{
			XmlRootAttribute root1 = new XmlRootAttribute("myname");
			XmlRootAttribute root2 = new XmlRootAttribute("myname");

			atts1.XmlRoot = root1;
			atts2.XmlRoot = root2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[Test]
		public void DifferentElementName()
		{
			XmlRootAttribute root1 = new XmlRootAttribute("myname");
			XmlRootAttribute root2 = new XmlRootAttribute("myothername");

			atts1.XmlRoot = root1;
			atts2.XmlRoot = root2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[Test]
		public void SameNullable()
		{
			XmlRootAttribute root1 = new XmlRootAttribute("myname");
			root1.IsNullable = true;
			XmlRootAttribute root2 = new XmlRootAttribute("myname");
			root2.IsNullable = true;

			atts1.XmlRoot = root1;
			atts2.XmlRoot = root2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[Test]
		public void DifferentNullable()
		{
			XmlRootAttribute root1 = new XmlRootAttribute("myname");
			root1.IsNullable = true;
			XmlRootAttribute root2 = new XmlRootAttribute("myname");
			root2.IsNullable = false;

			atts1.XmlRoot = root1;
			atts2.XmlRoot = root2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[Test]
		public void SameNamespace()
		{
			XmlRootAttribute root1 = new XmlRootAttribute("myname");
			root1.Namespace = "mynamespace";

			XmlRootAttribute root2 = new XmlRootAttribute("myname");
			root2.Namespace = "mynamespace";

			atts1.XmlRoot = root1;
			atts2.XmlRoot = root2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[Test]
		public void DifferentNamespace()
		{
			XmlRootAttribute root1 = new XmlRootAttribute("myname");
			root1.Namespace = "mynamespace";
			XmlRootAttribute root2 = new XmlRootAttribute("myname");
			root2.Namespace = "myothernamespace";

			atts1.XmlRoot = root1;
			atts2.XmlRoot = root2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}
	}
}
