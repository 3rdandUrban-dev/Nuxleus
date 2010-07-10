using System;
using System.Xml;
using System.Collections.Generic;

namespace Mvp.Xml.Core
{
	/// <summary>
	/// Implements the streaming processing reader. 
	/// </summary>
	/// <remarks>
	/// This reader also modifies the input stream so that 
	/// all elements have full end elements, so that no 
	/// empty elements are reported. This change does not 
	/// modify the infoset for a document, but allows the 
	/// important feature of being able to match or perform 
	/// processing when elements end, in a deterministic way.
	/// </remarks>
	public class XmlProcessorReader : WrappingXmlReader
	{
		private List<XmlProcessor> processors = new List<XmlProcessor>();

		public XmlProcessorReader(XmlReader reader) : base(new FullEndElementReader(reader))
		{ 
		}

		/// <summary>
		/// Reads the next element from the source.
		/// </summary>
		public override bool Read()
		{
			bool read = base.Read();

			if (read)
			{
				foreach (XmlProcessor processor in processors)
				{
					XmlReader chained = processor.Process(base.InnerReader);

					// Call processors for attributes.
					for (bool go = base.MoveToFirstAttribute(); go; go = base.MoveToNextAttribute())
					{
						chained = processor.Process(chained);
					}

					if (base.HasAttributes) base.MoveToElement();

					base.InnerReader = chained;
				}
			}

			return read;
		}

		/// <summary>
		/// Reads the entire stream, calling the processors as appropriate.
		/// </summary>
		public void ReadToEnd()
		{
			while (Read()) { }
		}

		public List<XmlProcessor> Processors
		{
			get { return processors; }
		}
	}
}