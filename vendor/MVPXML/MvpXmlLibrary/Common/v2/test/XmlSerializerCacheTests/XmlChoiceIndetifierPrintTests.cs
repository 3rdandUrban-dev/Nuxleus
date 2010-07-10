#region Using directives

using System;
using System.Collections;
using System.Text;
using System.Xml.Serialization;
using NUnit.Framework;
using Mvp.Xml.Common;

#endregion

namespace Mvp.Xml.Serialization.Tests
{

	internal class ChoiceIdentifierAttributeProvider : System.Reflection.ICustomAttributeProvider
	{

		string name;
		public ChoiceIdentifierAttributeProvider( string name )
		{
			this.name = name;
		}
#region ICustomAttributeProvider Members

public object[]  GetCustomAttributes(bool inherit)
{
    return new object[] { new XmlChoiceIdentifierAttribute(name) };
}

public object[]  GetCustomAttributes(Type attributeType, bool inherit)
{
	object[] o = null;
	if (attributeType == typeof(XmlChoiceIdentifierAttribute))
	{
		o = new object[1];
		o[0] = new XmlChoiceIdentifierAttribute(name);
	}
	else
	{
		o = new object[0];
	}

	return o;
}

public bool  IsDefined(Type attributeType, bool inherit)
{
	bool retVal = false;
	if( typeof(System.Xml.Serialization.XmlChoiceIdentifierAttribute) == attributeType )
	{
		retVal = true;
	}
 	return retVal;
}

#endregion
}

	[TestFixture]
	public class XmlChoiceIndetifierPrintTests
	{
		public XmlChoiceIndetifierPrintTests()
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
		}

		[Test]
		public void SameMemberName()
		{
			atts1 = new XmlAttributes(new ChoiceIdentifierAttributeProvider("myname"));
			atts2 = new XmlAttributes(new ChoiceIdentifierAttributeProvider("myname"));

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[Test]
		public void DifferentMemberName()
		{
			atts1 = new XmlAttributes(new ChoiceIdentifierAttributeProvider("myname"));
			atts2 = new XmlAttributes(new ChoiceIdentifierAttributeProvider("myothername"));

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);

		}
	}
}
