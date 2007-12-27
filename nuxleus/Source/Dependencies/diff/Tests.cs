using System;
using System.Collections;

using Algorithm.Diff;

public class Test {

	public static void Main(string[] args) {
		
		string s1 = "This is a test of the emergency broadcast system.";
		string s2 = "This is another test of the emergency broadcast system.";
		string s3 = "This is a test of the new emergency broadcast system.";
		string s4 = "This is the third test of the emergency broadcast mechanism.";
		
		DoDiff(s1, s1);
		DoDiff(s1, s2);
		DoDiff(s1, s3);
		DoDiff(s1, s4);
		DoDiff(s2, s3);
		DoDiff(s2, s4);
		DoDiff(s3, s4);
		
		DoMerge(s1, s2, s3);
		DoMerge(s1, s2, s4);
		DoMerge(s2, s3, s4);
	}
	
	public static void DoDiff(string s1, string s2) {
		
		IDiff d = new TextDiff(s1, s2); 
			//new Diff(s1.ToCharArray(), s2.ToCharArray(), null, null);
		
		Console.WriteLine("Left:  " + s1);
		Console.WriteLine("Right: " + s2);

		Console.Write("Diff: ");
		
		foreach (Diff.Hunk hunk in d) {
			if (hunk.Same) {
				WriteRange(hunk.Left);
			} else if (hunk.Left.Count == 0) {
				Console.Write("<+");
				WriteRange(hunk.Right);
				Console.Write(">");
			} else if (hunk.Right.Count == 0) {
				Console.Write("<-");
				WriteRange(hunk.Left);
				Console.Write(">");
			} else {
				Console.Write("<");
				WriteRange(hunk.Left);
				Console.Write("|");
				WriteRange(hunk.Right);
				Console.Write(">");
			}
		}
		Console.WriteLine();
		Console.WriteLine();
	}
	
	public static void DoMerge(string s1, string s2, string s3) {
		Merge m = new Merge(s1, new string[] { s2, s3 }, null);
		
		Console.WriteLine("Base: " + s1);
		Console.WriteLine("Left: " + s2);
		Console.WriteLine("Right: " + s3);
		
		Console.Write("Merge: ");
		
		foreach (Merge.Hunk hunk in m) {
			if (hunk.Same) {
				WriteRange(hunk.Original());
			} else if (!hunk.Conflict) {
				int idx = hunk.ChangedIndex();
				Console.Write("<");
				Console.Write((idx == 0 ? "L" : "R"));
				if (hunk.Original().Count == 0) {
					Console.Write("+");
					WriteRange(hunk.Changes(idx));
				} else if (hunk.Changes(idx).Count == 0) {
					Console.Write("-");
					WriteRange(hunk.Original());
				} else {
					Console.Write(":");
					WriteRange(hunk.Original());
					Console.Write("|");
					WriteRange(hunk.Changes(idx));
				}
				Console.Write(">");
			} else {
				Console.Write("<");
				WriteRange(hunk.Original());
				Console.Write("|");
				WriteRange(hunk.Changes(0));
				Console.Write("|");
				WriteRange(hunk.Changes(1));
				Console.Write(">");
			}
		}
		Console.WriteLine();
		Console.WriteLine();
	}
	
	public static void WriteRange(Range r) {
		foreach (char c in r)
			Console.Write(c);
	}
	
}
