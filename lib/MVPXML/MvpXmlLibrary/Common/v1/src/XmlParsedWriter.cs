#region using

using System;
using System.IO;
using System.Xml;

#endregion using 

namespace Mvp.Xml.Common
{
	/// <summary>
	/// Writes XML to a memory buffer that can be 
	/// retrieved as a pre-parsed <see cref="XmlReader"/> for 
	/// fast processing.
	/// </summary>
	public class XmlParsedWriter : XmlTextWriter
	{
		#region Ctors

		//XmlTextWriter _writer = new XmlTextWriter(new NullTextWriter());

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		public XmlParsedWriter() : base(new XmlParsedWriter.NullTextWriter())
		{
		}

		#endregion Ctors

		#region XmlWriter overrides

		#region Methods

//		/// <summary>
//		/// See <see cref="XmlTextWriter.Close"/>.
//		/// </summary>
//		public override void Close() {}
//
//		/// <summary>
//		/// See <see cref="XmlTextWriter.Flush"/>.
//		/// </summary>
//		public override void Flush() {}
//
//		/// <summary>
//		/// See <see cref="XmlTextWriter.LookupPrefix"/>.
//		/// </summary>
//		public override string LookupPrefix(string ns) { return _writer.LookupPrefix(ns); }
//
//		/// <summary>
//		/// See <see cref="XmlTextWriter.WriteBase64"/>.
//		/// </summary>
//		public override void WriteBase64(byte[] buffer, int index, int count) {}
//
//		/// <summary>
//		/// See <see cref="XmlTextWriter.WriteBinHex"/>.
//		/// </summary>
//		public override void WriteBinHex(byte[] buffer, int index, int count) {}

		/// <summary>
		/// See <see cref="XmlTextWriter.WriteCData"/>.
		/// </summary>
		public override void WriteCData(string text) {}
		
		/// <summary>
		/// See <see cref="XmlTextWriter.WriteCharEntity"/>.
		/// </summary>
		public override void WriteCharEntity(char ch) {}
		
		/// <summary>
		/// See <see cref="XmlTextWriter.WriteChars"/>.
		/// </summary>
		public override void WriteChars(char[] buffer, int index, int count) {}
		/// <summary>
		/// See <see cref="XmlTextWriter.WriteComment"/>.
		/// </summary>
		public override void WriteComment(string text) {}
		/// <summary>
		/// See <see cref="XmlTextWriter.WriteDocType"/>.
		/// </summary>
		public override void WriteDocType(string name, string pubid, string sysid, string subset) {}
		/// <summary>
		/// See <see cref="XmlTextWriter.WriteEndAttribute"/>.
		/// </summary>
		public override void WriteEndAttribute() {}
		/// <summary>
		/// See <see cref="XmlTextWriter.WriteEndDocument"/>.
		/// </summary>
		public override void WriteEndDocument() {}
		/// <summary>
		/// See <see cref="XmlTextWriter.WriteEndElement"/>.
		/// </summary>
		public override void WriteEndElement() {}
		/// <summary>
		/// See <see cref="XmlTextWriter.WriteEntityRef"/>.
		/// </summary>
		public override void WriteEntityRef(string name) {}
		/// <summary>
		/// See <see cref="XmlTextWriter.WriteFullEndElement"/>.
		/// </summary>
		public override void WriteFullEndElement() {}
		/// <summary>
		/// See <see cref="XmlTextWriter.WriteName"/>.
		/// </summary>
		public override void WriteName(string name) {}
		/// <summary>
		/// See <see cref="XmlTextWriter.WriteNmToken"/>.
		/// </summary>
		public override void WriteNmToken(string name) {}
		/// <summary>
		/// See <see cref="XmlTextWriter.WriteProcessingInstruction"/>.
		/// </summary>
		public override void WriteProcessingInstruction(string name, string text) {}
		/// <summary>
		/// See <see cref="XmlTextWriter.WriteQualifiedName"/>.
		/// </summary>
		public override void WriteQualifiedName(string localName, string ns) {}
		/// <summary>
		/// See <see cref="XmlTextWriter.WriteRaw"/>.
		/// </summary>
		public override void WriteRaw(string data) {}
		/// <summary>
		/// See <see cref="XmlTextWriter.WriteRaw"/>.
		/// </summary>
		public override void WriteRaw(char[] buffer, int index, int count) {}
		/// <summary>
		/// See <see cref="XmlTextWriter.WriteStartAttribute"/>.
		/// </summary>
		public override void WriteStartAttribute(string prefix, string localName, string ns) {}
		/// <summary>
		/// See <see cref="XmlTextWriter.WriteStartDocument"/>.
		/// </summary>
		public override void WriteStartDocument() {}
		/// <summary>
		/// See <see cref="XmlTextWriter.WriteStartDocument"/>.
		/// </summary>
		public override void WriteStartDocument(bool standalone) {}
		/// <summary>
		/// See <see cref="XmlTextWriter.WriteStartElement"/>.
		/// </summary>
		public override void WriteStartElement(string prefix, string localName, string ns) {}
		/// <summary>
		/// See <see cref="XmlTextWriter.WriteString"/>.
		/// </summary>
		public override void WriteString(string text) {}
		/// <summary>
		/// See <see cref="XmlTextWriter.WriteSurrogateCharEntity"/>.
		/// </summary>
		public override void WriteSurrogateCharEntity(char lowChar, char highChar) {}
		/// <summary>
		/// See <see cref="XmlTextWriter.WriteWhitespace"/>.
		/// </summary>
		public override void WriteWhitespace(string ws) {}

		#endregion Methods

		#region Properties

		/// <summary>
		/// See <see cref="XmlTextWriter.Close"/>.
		/// </summary>
		public override WriteState WriteState { get { return WriteState.Closed; } }
		/// <summary>
		/// See <see cref="XmlTextWriter.Close"/>.
		/// </summary>
		public override string XmlLang { get { return null; } }
		/// <summary>
		/// See <see cref="XmlTextWriter.Close"/>.
		/// </summary>
		public override XmlSpace XmlSpace { get { return XmlSpace.Default; } }

		#endregion Properties

		#endregion XmlWriter overrides

		#region NullTextWriter

		private sealed class NullTextWriter : TextWriter
		{
			StringWriter _writer;

			public void StartTracking()
			{
				_writer = new StringWriter();
			}

			public string StopTracking()
			{
				string ret = _writer.ToString();
				_writer = null;
				return ret;
			}

			#region Method overrides

			public override void Write(bool value) { if (_writer != null) _writer.Write(value); }

			public override void Write(char value) { if (_writer != null) _writer.Write(value); }
	
			public override void Write(double value) { if (_writer != null) _writer.Write(value); }
	
			public override void Write(int value) { if (_writer != null) _writer.Write(value); }
	
			public override void Write(long value) { if (_writer != null) _writer.Write(value); }
	
			public override void Write(object value) { if (_writer != null) _writer.Write(value); }
	
			public override void Write(float value) { if (_writer != null) _writer.Write(value); }
	
			public override void Write(string value) { if (_writer != null) _writer.Write(value); }

			[CLSCompliant(false)]
			public override void Write(uint value) { if (_writer != null) _writer.Write(value); }

			[CLSCompliant(false)]
			public override void Write(ulong value) { if (_writer != null) _writer.Write(value); }
	
			public override void Write(char[] buffer) { if (_writer != null) _writer.Write(buffer); }
	
			public override void Write(string format, object[] arg) { if (_writer != null) _writer.Write(format, arg); }

			public override void Write(string format, object arg0) { if (_writer != null) _writer.Write(format, arg0); }
	
			public override void Write(string format, object arg0, object arg1) { if (_writer != null) _writer.Write(format, arg0, arg1); }
	
			public override void Write(string format, object arg0, object arg1, object arg2) { if (_writer != null) _writer.Write(format, arg0, arg1, arg2); }

			public override void Write(char[] buffer, int index, int count) { if (_writer != null) _writer.Write(buffer, index, count); }
	
			public override void WriteLine(bool value) { if (_writer != null) _writer.WriteLine(value); }
	
			public override void WriteLine(char value) { if (_writer != null) _writer.WriteLine(value); }
	
			public override void WriteLine(double value) { if (_writer != null) _writer.WriteLine(value); }
	
			public override void WriteLine(int value) { if (_writer != null) _writer.WriteLine(value); }
	
			public override void WriteLine(long value) { if (_writer != null) _writer.WriteLine(value); }

			public override void WriteLine(object value) { if (_writer != null) _writer.WriteLine(value); }

			public override void WriteLine(float value) { if (_writer != null) _writer.WriteLine(value); }
	
			public override void WriteLine(string value) { if (_writer != null) _writer.WriteLine(value); }

			[CLSCompliant(false)]
			public override void WriteLine(uint value) { if (_writer != null) _writer.WriteLine(value); }

			[CLSCompliant(false)]
			public override void WriteLine(ulong value) { if (_writer != null) _writer.WriteLine(value); }
	
			public override void WriteLine(char[] buffer) { if (_writer != null) _writer.WriteLine(buffer); }

			public override void WriteLine(string format, object[] arg) { if (_writer != null) _writer.WriteLine(format, arg); }

			public override void WriteLine(string format, object arg0) { if (_writer != null) _writer.WriteLine(format, arg0); }
	
			public override void WriteLine(string format, object arg0, object arg1) { if (_writer != null) _writer.WriteLine(format, arg0, arg1); }

			public override void WriteLine(string format, object arg0, object arg1, object arg2) { if (_writer != null) _writer.WriteLine(format, arg0, arg1, arg2); }
	
			public override void WriteLine(char[] buffer, int index, int count) { if (_writer != null) _writer.WriteLine(buffer, index, count); }
	
			public override System.Text.Encoding Encoding { get { return System.Text.Encoding.Default; } }

			#endregion Method overrides 
		}

		#endregion NullTextWriter
	}
}
