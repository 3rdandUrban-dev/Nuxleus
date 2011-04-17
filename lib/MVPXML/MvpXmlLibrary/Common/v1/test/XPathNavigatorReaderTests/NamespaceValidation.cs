#region using

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml.Schema;
using System.Xml.Xsl;

using NUnit.Framework;
using Mvp.Xml.Common.XPath;

#endregion using 

namespace Mvp.Xml.Tests.XPathNavigatorReaderTests
{
	[TestFixture]
	public class NamespaceValidation
	{
		[Test]
		public void LoadAndValidate()
		{
			string path = this.GetType().Namespace + ".";
			Stream stm = this.GetType().Assembly.GetManifestResourceStream( 
				path + "mixedNs.xml");
			string xml = new StreamReader(stm).ReadToEnd();

			XPathDocument doc = new XPathDocument(new StringReader(xml));
			XPathNavigatorReader nr = new XPathNavigatorReader(doc.CreateNavigator());

			XmlTextReader tr = new XmlTextReader(new StringReader(xml));
			XmlValidatingReader vr = new XmlValidatingReader(tr);
			using (StreamReader sr = new StreamReader(this.GetType().Assembly.GetManifestResourceStream
					   (path + "ImportedSchema1.xsd")))
			{
				vr.Schemas.Add(XmlSchema.Read(sr, null));
			}
			using (StreamReader sr = new StreamReader(this.GetType().Assembly.GetManifestResourceStream
					   (path + "ImportedSchema2.xsd")))
			{
				vr.Schemas.Add(XmlSchema.Read(sr, null));
			}
			using (StreamReader sr = new StreamReader(this.GetType().Assembly.GetManifestResourceStream
					   (path + "RootSchema.xsd")))
			{
				vr.Schemas.Add(XmlSchema.Read(sr, null));
			}

			while (vr.Read()) {}

			vr = new XmlValidatingReader(nr);
			using (StreamReader sr = new StreamReader(this.GetType().Assembly.GetManifestResourceStream
					   (path + "ImportedSchema1.xsd")))
			{
				vr.Schemas.Add(XmlSchema.Read(sr, null));
			}
			using (StreamReader sr = new StreamReader(this.GetType().Assembly.GetManifestResourceStream
					   (path + "ImportedSchema2.xsd")))
			{
				vr.Schemas.Add(XmlSchema.Read(sr, null));
			}
			using (StreamReader sr = new StreamReader(this.GetType().Assembly.GetManifestResourceStream
					   (path + "RootSchema.xsd")))
			{
				vr.Schemas.Add(XmlSchema.Read(sr, null));
			}

			while (vr.Read()) {}

			Console.ReadLine();
		}
	}
}
