using System;
using System.Collections;
using System.IO;
using System.Xml;

using Algorithm.Diff;

using Mono.GetOptions;

using System.Reflection;
[assembly: AssemblyTitle("xmldiff - Compare Two Xml Files")]
[assembly: AssemblyCopyright("Copyright (c) 2004 Joshua Tauberer <tauberer@for.net>, released under the GPL.")]
[assembly: AssemblyDescription("A tool like GNU diff, but for Xml files.")]

[assembly: Mono.UsageComplement("leftfile rightfile")]

public class XmlDiff {
	
	private class OptsType : Options {
		[Option("Full output of all unchanged nodes.")]
		public bool full = false;

		[Option(-1, "A comma-delimited list of element {names} to use for context.")]
		public string[] context;
		
		[Option("The output method: xml, text, groff.")]
		public string output = "xml";
	}

	public static void Main(string[] args) {
		OptsType opts = new OptsType();
		opts.ProcessArgs(args);
		
		if (opts.RemainingArguments.Length != 2) {
			opts.DoHelp();
			return;
		}
		
		XmlDocument left = new XmlDocument();
		left.Load(opts.RemainingArguments[0]);

		XmlDocument right = new XmlDocument();
		right.Load(opts.RemainingArguments[1]);
		
		StringWriter buffer = null;
		XmlWriter output;
		
		if (opts.output == "xml") {
			XmlTextWriter textoutput = new XmlTextWriter(Console.Out);
			textoutput.Formatting = Formatting.Indented;
			output = textoutput;
		} else {
			buffer = new StringWriter();
			output = new XmlTextWriter(buffer);
		}
		
		StructuredDiff test = new XmlOutputStructuredDiff(output, opts.full ? null : opts.context == null ? "*" : String.Join(",", opts.context));

		XmlNodeInterface xmlint = new XmlNodeInterface();
		
		test.AddInterface(typeof(XmlAttribute), xmlint);
		test.AddInterface(typeof(XmlCDataSection), xmlint);
		test.AddInterface(typeof(XmlComment), xmlint);
		test.AddInterface(typeof(XmlDeclaration), xmlint);
		test.AddInterface(typeof(XmlDocument), xmlint);
		test.AddInterface(typeof(XmlDocumentType), xmlint);
		test.AddInterface(typeof(XmlElement), xmlint);
		test.AddInterface(typeof(XmlEntity), xmlint);
		test.AddInterface(typeof(XmlEntityReference), xmlint);
		test.AddInterface(typeof(XmlNotation), xmlint);
		test.AddInterface(typeof(XmlProcessingInstruction), xmlint);
		test.AddInterface(typeof(XmlSignificantWhitespace), xmlint);
		test.AddInterface(typeof(XmlText), xmlint);
		test.AddInterface(typeof(XmlWhitespace), xmlint);
		
		test.Compare(left, right);
		
		output.Close();
		
		if (opts.output != "xml") {
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(buffer.ToString());

			TextWriter rawoutput;
			XmlTextWriter textoutput;
			
			if (opts.output == "groff") {
				System.Diagnostics.Process groff = new System.Diagnostics.Process();
				groff.StartInfo.FileName = "groff";
				groff.StartInfo.Arguments = "-Tlatin1 -Ww";
				groff.StartInfo.RedirectStandardInput = true;
				groff.StartInfo.UseShellExecute = false;				
				groff.Start();
				textoutput = new XmlTextWriter(new RoffWriter(groff.StandardInput));
				rawoutput = groff.StandardInput;
			} else {
				textoutput = new XmlTextWriter(Console.Out);
				rawoutput = Console.Out;
			}
			textoutput.Formatting = Formatting.Indented;
			
			WriteNode((XmlElement)doc.DocumentElement.SelectSingleNode("*"), opts.output, textoutput, rawoutput);
		}
	}
	
	public class RoffWriter : TextWriter {
		TextWriter writer;
		
		public RoffWriter(TextWriter writer) { this.writer = writer; }
		
		public override System.Text.Encoding Encoding { get { return null; } }
		
		public override void Write(char c) {
			if (c == '\\')
				writer.Write("\\\\");
			else if (c == '.')
				writer.Write("\\.");
			else
				writer.Write(c);
		}
	}
	
	public static void WriteNodeRaw(XmlElement node, string method, XmlTextWriter output) {
		if (node.Name == "string") {
			output.WriteString(node.InnerText + "\n");
			return;
		} else if (node.Name == "Text") {
			output.WriteString(node.Value);
			return;
		} else if (node.Name == "Attribute") {
			output.WriteAttributeString(node.GetAttribute("Name"), "", node.InnerText);
			return;
		} else if (node.Name != "Element") {
			output.WriteStartElement(node.Name);
		} else {
			output.WriteStartElement(node.GetAttribute("Name"));
		}

		foreach (XmlNode child in node.SelectNodes("@*|*")) {
			if (child is XmlAttribute && child.Name == "Status")
				continue;
			child.WriteTo(output);
		}
		
		if (node.FirstChild is XmlText)
			output.WriteString(node.FirstChild.Value);
		
		output.WriteEndElement();
	}
	
	public static void WriteNode(XmlElement node, string method, XmlTextWriter output, TextWriter raw) {
		if (node.Name == "string") {
			output.WriteString(node.InnerText);
			return;
		}
		
		if (node.Name == "Attribute")
			output.WriteStartAttribute(node.GetAttribute("Name"), "");
		else if (node.Name == "Text") {
		}
		else if (node.Name != "Element")
			output.WriteStartElement(node.Name);
		else
			output.WriteStartElement(node.GetAttribute("Name"));
		
		foreach (XmlElement child in node.SelectNodes("*")) {
			string status = child.GetAttribute("Status");
			if (status == "") {
				WriteNode(child, method, output, raw);
			} else if (status == "Same") {
				WriteNodeRaw(child, method, output);
			} else if (status == "Added" || status == "Removed") {
				output.Flush();
				if (method == "text") {
					if (status == "Added") raw.Write("\n+"); else raw.Write("\n-");
				} else if (method == "groff") {
					if (status == "Added") raw.Write("\\m[blue]"); else raw.Write("\\m[red]");
				}
				raw.Flush();
				
				WriteNodeRaw((XmlElement)child.FirstChild, method, output);

				output.Flush();
				if (method == "text") {
					raw.Write("\n");
				} else if (method == "groff") {
					if (status == "Added") raw.Write("\\m[]"); else raw.Write("\\m[]");
				}
				raw.Flush();
			} else if (status == "Changed") {
				output.Flush();
				if (method == "text") {
					raw.Write("\n-");
				} else if (method == "groff") {
					raw.Write("\\m[green]");
				}
				raw.Flush();
				
				WriteNodeRaw((XmlElement)child.FirstChild, method, output);

				output.Flush();
				if (method == "text") {
					raw.Write("\n+");
				} else if (method == "groff") {
					raw.Write("\\m[]\\m[magenta]");
				}
				raw.Flush();
				
				WriteNodeRaw((XmlElement)child.FirstChild.NextSibling, method, output);

				output.Flush();
				if (method == "text") {
					raw.Write("\n");
				} else if (method == "groff") {
					raw.Write("\\m[]");
				}
				raw.Flush();
			} else {
				throw new InvalidOperationException("Bad status: " + status);
			}
		}

		if (node.FirstChild is XmlText)
			output.WriteString(node.FirstChild.Value);		
		
		if (node.Name == "Attribute")
			output.WriteEndAttribute();
		else if (node.Name == "Text")
		{}
		else
			output.WriteEndElement();
	}
}

public class XmlNodeInterface : NodeInterface, XmlOutputNodeInterface {
	private const int ElementContentSummaryLength = 20;
	
	public override IList GetChildren(object item) {
		XmlNode node = item as XmlNode;
		
		ArrayList ret = new ArrayList();
		if (node.Attributes != null)
			foreach (XmlNode subnode in node.Attributes)
				ret.Add(subnode);
		foreach (XmlNode subnode in node.ChildNodes)
			ret.Add(subnode);
			
		if (ret.Count == 0)
			return null;
			
		return ret;
	}
	
	public override float Compare(object left, object right, StructuredDiff comparer) {
		XmlNode l = (XmlNode)left;
		XmlNode r = (XmlNode)right;
		
		float ret;
		
		IList cleft = GetChildren(left);
		IList cright = GetChildren(right);

		if (cleft == null || cright == null) {
			if (l.InnerText == r.InnerText) ret = 0;
			else if (l.InnerText.Trim() == r.InnerText.Trim()) ret = .05F;
			else ret = 1;
		} else {		
			ret = comparer.CompareLists(cleft, cright);
		}
		
		if (l is XmlElement || l is XmlAttribute) {
			ret *= .75F;
			if (l.Name != r.Name) 
				ret += .25F;
		}
		
		return ret;
	}
	
	private string ValueSummary(string value) {
		value = value.Replace("\n", "");
		if (value == "") return "...";
		if (value.Length > ElementContentSummaryLength) return value.Substring(0, ElementContentSummaryLength) + "...";
		return value;
	}
	
	public void WriteNodeChildren(object node, XmlWriter output) {
		if (node is XmlElement) {
			foreach (XmlAttribute attr in ((XmlElement)node).Attributes)
				attr.WriteTo(output);
			((XmlNode)node).WriteContentTo(output);
		} else {
			output.WriteString(((XmlNode)node).Value);
		}
	}
	
	private bool IsOkayName(string name) {
		return name != "Attribute" && name != "Text" && name != "CDATA" && name != "EntityReference" && name != "Entity" && name != "ProcessingInstruction" && name != "Comment" && name != "Document" && name != "DocumentType" && name != "DocumentFragment" && name != "Notation" && name != "Whitespace" && name != "SignificantWhitespace" && name != "XmlDeclaration" && name != "Added" && name != "Removed" && name != "Changed";
	}
	
	public void WriteBeginNode(object left, object right, XmlWriter output) {
		string leftname = ((XmlNode)left).Name, rightname = ((XmlNode)right).Name;
		if (left is XmlElement && leftname == rightname && IsOkayName(leftname))
			output.WriteStartElement(leftname);
		else {
			output.WriteStartElement(((XmlNode)left).NodeType.ToString());
			if (!(left is XmlElement || left is XmlAttribute)) {
			} else if (leftname == rightname)
				output.WriteAttributeString("Name", leftname);
			else {
				output.WriteAttributeString("LeftName", leftname);
				output.WriteAttributeString("RightName", rightname);
			}
		}
	}
}

