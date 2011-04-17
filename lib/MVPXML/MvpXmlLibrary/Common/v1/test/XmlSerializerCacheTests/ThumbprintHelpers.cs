#region Using directives

using System;
using System.Collections;
using System.Text;
using System.Xml.Serialization;
using NUnit.Framework;
using Mvp.Xml.Common.Serialization;
#endregion

namespace Mvp.Xml.Serialization.Tests
{
	public class ThumbprintHelpers
	{
		public ThumbprintHelpers()
		{

		}

		internal static void SameThumbprint(XmlAttributeOverrides ov1, XmlAttributeOverrides ov2)
		{
			string print1 = XmlAttributeOverridesThumbprinter.GetThumbprint(ov1);
			string print2 = XmlAttributeOverridesThumbprinter.GetThumbprint(ov2);

			//Console.WriteLine("p1 {0}, p2 {1}", print1, print2);
			Assert.AreEqual(print1, print2);
		}
		internal static void DifferentThumbprint(XmlAttributeOverrides ov1, XmlAttributeOverrides ov2)
		{
			string print1 = XmlAttributeOverridesThumbprinter.GetThumbprint(ov1);
			string print2 = XmlAttributeOverridesThumbprinter.GetThumbprint(ov2);

			Assert.IsFalse(print1 == print2);
		}

	}
}
