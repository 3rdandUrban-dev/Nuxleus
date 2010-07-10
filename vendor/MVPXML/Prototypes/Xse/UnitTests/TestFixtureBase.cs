using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Xml;
using System.IO;

namespace Mvp.Xml.Core.UnitTests
{
	public abstract class TestFixtureBase
	{
		[Conditional("DEBUG")]
		protected void WriteIfDebugging(string message)
		{
			if (Debugger.IsAttached)
			{
				Debug.WriteLine(message);
			}
		}

		protected static XmlReader GetReader(string xml)
		{
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.IgnoreWhitespace = true;
			settings.CheckCharacters = true;

			return new FullEndElementReader(XmlReader.Create(new StringReader(xml), settings));
		}

		protected static string ReadToEnd(XmlReader reader)
		{
			StringWriter sw = new StringWriter();
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.OmitXmlDeclaration = true;
			XmlWriter writer = XmlWriter.Create(sw, settings);
			writer.WriteNode(reader, false);
			writer.Close();
			return sw.ToString();
		}
	}
}
