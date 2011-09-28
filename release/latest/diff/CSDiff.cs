using System;
using System.IO;
using System.Collections;
using System.Xml;
using System.Xml.Xsl;

using ASTNode = antlr.BaseAST;

using DDW.CSharp.Parse;
using DDW.CSharp.Dom;
using DDW.CSharp.Walk;
using DDW.CSharp.Gen;

using Algorithm.Diff;

using Mono.GetOptions;

using System.Reflection;
[assembly: AssemblyTitle("csdiff - Compare Two C# Files")]
[assembly: AssemblyCopyright("Copyright (c) 2004 Joshua Tauberer <tauberer@for.net>, released under the GPL.")]
[assembly: AssemblyDescription("A tool like GNU diff, but for C# files.")]

[assembly: Mono.UsageComplement("")]

public class CSDiff {

	public static void Main(string[] args) {
		if (args.Length != 2)
			return;
		
		// Read in the output rules files
		
		XmlDocument stylesheet = new XmlDocument();
		
		XmlElement stylesheet_root = stylesheet.CreateElement("xsl:stylesheet", "http://www.w3.org/1999/XSL/Transform");
		stylesheet_root.SetAttribute("version", "1.0");
		stylesheet.AppendChild(stylesheet_root);

		/*XmlElement outputnode = stylesheet.CreateElement("xsl:output", "http://www.w3.org/1999/XSL/Transform");
		outputnode.SetAttribute("method", "text");
		stylesheet_root.AppendChild(outputnode);*/

		XmlElement basetemplate = stylesheet.CreateElement("xsl:template", "http://www.w3.org/1999/XSL/Transform");
		basetemplate.SetAttribute("match", "*");
		basetemplate.InnerXml = "[<xsl:value-of xmlns:xsl='http://www.w3.org/1999/XSL/Transform' select=\"name()\"/>:"
			+ "<xsl:for-each select='*' xmlns:xsl='http://www.w3.org/1999/XSL/Transform'>"
			+ "[<xsl:value-of select='name(.)'/>]"
			+ "</xsl:for-each>]";
		stylesheet_root.AppendChild(basetemplate);
		
		StreamReader csgen = new StreamReader("csgen.txt");
		System.Text.RegularExpressions.Regex SubstParam = new System.Text.RegularExpressions.Regex(@"@([\w\*]+)(\[[^\]]+\])?");
		string csgen_line;
		while ((csgen_line = csgen.ReadLine()) != null) {
			if (csgen_line == "" || csgen_line[0] == '#') { continue; }
			int sp = csgen_line.IndexOf(" ");
			if (sp == -1) { continue; }
			string rule = csgen_line.Substring(sp+1);
			
			int priority = -1;
			
			if (csgen_line.StartsWith("*"))
				priority = 10;
			if (csgen_line.StartsWith("!")) {
				priority = 20;
				csgen_line = csgen_line.Substring(1);
			}
			
			XmlElement template = stylesheet.CreateElement("xsl:template", "http://www.w3.org/1999/XSL/Transform");
			template.SetAttribute("match", csgen_line.Substring(0, sp));
			template.SetAttribute("xml:space", "preserve");

			if (priority != -1)
				template.SetAttribute("priority", priority.ToString());
			if (rule == "none") {
				template.InnerXml = "<xsl:apply-templates xmlns:xsl='http://www.w3.org/1999/XSL/Transform' select='*'/>";
			} else if (rule == "text") {
				template.InnerXml = "<xsl:value-of xmlns:xsl='http://www.w3.org/1999/XSL/Transform' select='@Text'/>";
			} else {
				rule = rule.Replace("|", "\n");
				if (!rule.EndsWith("\\")) rule += "\n"; else rule = rule.Substring(0, rule.Length-1);

				rule = rule.Replace("@@1", "<xsl:apply-templates xmlns:xsl='http://www.w3.org/1999/XSL/Transform' select='*[position()&gt;1]'/>");
				rule = rule.Replace("@@", "<xsl:apply-templates xmlns:xsl='http://www.w3.org/1999/XSL/Transform'/>");
				
				foreach (System.Text.RegularExpressions.Match match in SubstParam.Matches(rule)) {
					string type = match.Result("$1");
					string sep = match.Result("$2");
					if (sep != "") sep = sep.Substring(1, sep.Length-2);
					if (type == "text") {
						rule = rule.Replace(match.Value, "<xsl:value-of xmlns:xsl='http://www.w3.org/1999/XSL/Transform' select='@Text'/>");
					} else {
						if (char.IsDigit(type[0]))
							type = "*[position()=" + type + "]";
						if (sep == "")
							rule = rule.Replace(match.Value, "<xsl:apply-templates xmlns:xsl='http://www.w3.org/1999/XSL/Transform' select='" + type + "'/>");
						else
							rule = rule.Replace(match.Value,
							"<xsl:for-each xmlns:xsl='http://www.w3.org/1999/XSL/Transform' select='" + type + "'>" 
							+ "<xsl:if test='not(position()=1)'>" + sep + "</xsl:if>"
							+ "<xsl:apply-templates select='.'/>"
							+ "</xsl:for-each>");
					}
				}

				template.InnerXml = rule;
			}
			stylesheet_root.AppendChild(template);
		}
		
		CSharpAST
			left = GetAST(args[0]),
			right = GetAST(args[1]);
		
		StringWriter buffer = new StringWriter();
		XmlTextWriter output = new XmlTextWriter(buffer);
		//output.Formatting = Formatting.Indented;
		StructuredDiff test = new XmlOutputStructuredDiff(output, null);
		test.AddInterface(typeof(ASTNode), new ASTNodeInterface());		
		test.Compare(left, right);		
		output.Close();
		
		XmlDocument diff = new XmlDocument();
		diff.LoadXml(buffer.ToString());
		
		XslTransform transform = new XslTransform();
		transform.Load(stylesheet);
		
		XmlReader diffcontent = transform.Transform(diff, null);
		
		throw new Exception(diffcontent.BaseURI == null ? "null" : "not null");
		
		XmlDocument diffdoc = new XmlDocument();
		diffdoc.Load(diffcontent);
		diffdoc.Normalize();
		
		WriteChildren(diffdoc.DocumentElement, 0, ' ');

		
		Console.WriteLine();
	}
	
	static bool onNewLine = false;
	
	private static void WriteChildren(XmlNode node, int indentation, char status) {
		foreach (XmlNode child in node)
			WriteNode(child, indentation, status);
	}

	private static void WriteNode(XmlNode node, int indentation, char status) {
		bool groff = true;
		
		if (node.Name == "block") {
			if (!onNewLine) { Console.WriteLine(); onNewLine = true; }
			WriteChildren(node, indentation+1, status);
		} else if (node.Name == "added")
			WriteChildren(node, indentation, '+');
		else if (node.Name == "removed")
			WriteChildren(node, indentation, '-');
		else if (node.Name == "changed") {
			if (node.InnerText.IndexOf("\n") == -1 && !groff) {
				if (!onNewLine)
					Console.WriteLine();

				onNewLine = true;
				WriteChildren(node.FirstChild, indentation, '-');

				if (!onNewLine)
					Console.WriteLine();

				onNewLine = true;
				WriteChildren(node.FirstChild.NextSibling, indentation, '+');

				if (!onNewLine)
					Console.WriteLine();

				onNewLine = true;
			} else {
				WriteChildren(node.FirstChild, indentation, '-');
				WriteChildren(node.FirstChild.NextSibling, indentation, '+');
			}
		} else if (node is XmlText) {
			string text = node.InnerText;
			
			if (groff) {
				if (status == '-')
					Console.Write("\\m[red]");
				if (status == '+')
					Console.Write("\\m[blue]");
				text = text.Replace("\\", "\\\\");
				text = text.Replace(".", "\\.");
			}
			
			if (node.InnerText.IndexOf("\n") >= 0 || (node.NextSibling != null && node.NextSibling.Name == "block")) {
				if (!onNewLine)
					Console.WriteLine();
				if (text.EndsWith("\n")) text = text.Substring(0, text.Length-1);
				string[] lines = text.Split('\n');
				foreach (string line in lines) {
					if (groff) Console.Write(' ');
					Console.Write(status);
					for (int i = 0; i < indentation; i++)
						Console.Write("  ");
					Console.Write(line);
					if (line != lines[lines.Length-1])
						Console.WriteLine();
				}
				if (node.InnerText.EndsWith("\n")) {
					Console.WriteLine();
					onNewLine = true;
				} else {
					onNewLine = false;
				}
			} else {
				if (onNewLine) {
					if (groff) Console.Write(' ');
					Console.Write(status);
					for (int i = 0; i < indentation; i++)
						Console.Write("  ");
				}

				Console.Write(text);
				onNewLine = false;
			}

			if (groff && status != ' ') Console.Write("\\m[]");
		} else {
			Console.WriteLine(node.OuterXml);
		}
	}

	private static CSharpAST GetAST(string filename)
	{
		FileStream s = new FileStream(filename, FileMode.Open, FileAccess.Read);
		CSharpLexer lexer = new CSharpLexer(s);
		lexer.setFilename(filename);
		CSharpParser parser = new CSharpParser(lexer);
		parser.setFilename(filename);
		parser.compilation_unit();
		s.Close();
		CSharpAST antlrTree = (CSharpAST)(parser.getAST());
		antlrTree.FileName = filename;
		return antlrTree;
	}
	
}

public class ASTNodeInterface : NodeInterface, XmlOutputNodeInterface {
	public override IList GetChildren(object item) {
		ASTNode node = (ASTNode)item;
		ArrayList ret = new ArrayList();
		ASTNode child = (ASTNode)node.getFirstChild();
		while (child != null) {
			ret.Add(child);
			child = (ASTNode)child.getNextSibling();
		}
		return ret;
	}
	
	public override float Compare(object left, object right, StructuredDiff comparer) {
		ASTNode l = (ASTNode)left;
		ASTNode r = (ASTNode)right;
		
		if (l.getText() != r.getText()) return 1;
		
		if (l.EqualsTree(r)) return 0;
		
		float ret = comparer.CompareLists(GetChildren(left), GetChildren(right));
		return ret;
	}
	
	public override int GetHashCode(object node) {
		ASTNode n = (ASTNode)node;
		if (n.getText() != "")
			return n.getText().GetHashCode();

		int ret = n.GetType().GetHashCode();
		ASTNode child = (ASTNode)n.getFirstChild();
		while (child != null) {
			ret = unchecked(ret + child.GetType().GetHashCode());
			child = (ASTNode)child.getNextSibling();
		}
		return ret;
	}
	
	public void WriteNodeChildren(object node, XmlWriter output) {
		if (((ASTNode)node).getText() != "")
			output.WriteAttributeString("Text", ((ASTNode)node).getText());

		ASTNode child = (ASTNode)((ASTNode)node).getFirstChild();
		while (child != null) {
			output.WriteStartElement(child.GetType().Name);
			WriteNodeChildren(child, output);
			output.WriteEndElement();
			child = (ASTNode)child.getNextSibling();
		}
	}
	
	public void WriteBeginNode(object left, object right, XmlWriter output) {
		output.WriteStartElement(left.GetType().Name);
	}
}
	
