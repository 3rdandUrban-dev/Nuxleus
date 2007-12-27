using System;
using System.Collections;
using System.IO;
using System.Reflection;

using Algorithm.Diff;

using Mono.GetOptions;

[assembly: AssemblyTitle("diff - A C# Diffing Tool")]
[assembly: AssemblyCopyright("Copyright (c) 2004 Joshua Tauberer <tauberer@for.net>.")]
[assembly: AssemblyDescription("A tool for performing diffs.")]
[assembly: Mono.UsageComplement("")]

class Opts : Options {
	[Option("The left source file.")]
	public string left;

	[Option("The right source file.")]
	public string right;
	
	[Option("The comparison method: chars or lines (default).")]
	public string method = "lines";
	
	[Option("Whether to perform a case-insensitive comparison.  (Default no.)")]
	public bool caseinsensitive = false;

	[Option("Prompt before each change made, and output a revised document excepting any denied changes.")]
	public bool interactive = false;
}


public class Driver {
	public static void Main(string[] args) {
		Opts opts = new Opts();
		opts.ProcessArgs(args);
		
		if (opts.left == null || opts.right == null) {
			Console.Error.WriteLine("The --left and --right arguments are required.");
			return;
		}
		
		IList file1, file2;
		
		if (opts.method == "lines") {
			file1 = LoadFileLines(opts.left);
			file2 = LoadFileLines(opts.right);
		} else if (opts.method == "chars") {
			file1 = LoadFileChars(opts.left);
			file2 = LoadFileChars(opts.right);
		} else {
			Console.Error.WriteLine("The method option must be either lines or chars.");
			return;
		}
		
		Diff diff = new Diff(file1, file2, 
			!opts.caseinsensitive ? null : CaseInsensitiveComparer.Default,
			!opts.caseinsensitive ? null : CaseInsensitiveHashCodeProvider.Default);
		
		if (!opts.interactive) {
			UnifiedDiff.WriteUnifiedDiff(diff, Console.Out);
		} else {
			foreach (Diff.Hunk hunk in diff) {
				if (hunk.Same) {
					WriteBlock(hunk.Left, opts);
				} else {
					Console.Error.WriteLine(hunk);
					Console.Error.Write("Enter 'r' to reject, or hit enter to accept. ");
					Console.Error.WriteLine();
					string choice = Console.In.ReadLine();
					if (choice == "r")
						WriteBlock(hunk.Left, opts);
					else
						WriteBlock(hunk.Right, opts);
				}
			}
		}
	}

	private static void WriteBlock(Range range, Opts opts) {
		foreach (object item in range) {
			Console.Out.Write(item);
			if (opts.method == "lines") Console.Out.WriteLine();
		}
	}
	
	public static string[] LoadFileLines(string file) {
		ArrayList lines = new ArrayList();
		using (StreamReader reader = new StreamReader(file)) {
			string s;
			while ((s = reader.ReadLine()) != null)
				lines.Add(s);
		}
		return (string[])lines.ToArray(typeof(string));
	}
	
	public static char[] LoadFileChars(string file) {
		using (StreamReader reader = new StreamReader(file)) {
			string s = reader.ReadToEnd();
			return s.ToCharArray();
		}
	}
}
