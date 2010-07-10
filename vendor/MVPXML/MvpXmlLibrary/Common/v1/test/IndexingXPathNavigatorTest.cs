using System;
using System.Xml;
using System.Xml.XPath;

using Mvp.Xml.Common.XPath;
using NUnit.Framework;

namespace Mvp.Xml.Tests
{
	/// <summary>
	/// Test class for IndexingXPathNavigator
	/// </summary>
	[TestFixture]
	public class IndexingXPathNavigatorTest
	{
		[Test]
		public void RunTests()
		{
			Main(new string[0]);
		}

		[STAThread]
		static void Main(string[] args)
		{
			PerfTest perf = new PerfTest();
			int repeat = 1000;
			perf.Start();
			XPathDocument doc = new XPathDocument(Globals.GetResource(Globals.NorthwindResource));
			//XmlDocument doc = new XmlDocument();
			//doc.Load("test/northwind.xml");
			Console.WriteLine("Loading XML document: {0, 6:f2} ms", perf.Stop());
			XPathNavigator nav = doc.CreateNavigator();      
			XPathExpression expr = nav.Compile("/ROOT/CustomerIDs/OrderIDs/Item[OrderID=' 10330']/ShipAddress");
      
			Console.WriteLine("Regular selection, warming...");
			SelectNodes(nav, repeat, perf, expr);
			Console.WriteLine("Regular selection, testing...");
			SelectNodes(nav, repeat, perf, expr);
   
      
			perf.Start();
			IndexingXPathNavigator inav = new IndexingXPathNavigator(
				doc.CreateNavigator());
			Console.WriteLine("Building IndexingXPathNavigator: {0, 6:f2} ms", perf.Stop());
			perf.Start();
			inav.AddKey("orderKey", "OrderIDs/Item", "OrderID");
			Console.WriteLine("Adding keys: {0, 6:f2} ms", perf.Stop());    
			XPathExpression expr2 = inav.Compile("key('orderKey', ' 10330')/ShipAddress");
			perf.Start();
			inav.BuildIndexes();
			Console.WriteLine("Indexing: {0, 6:f2} ms", perf.Stop());    
      
			Console.WriteLine("Indexed selection, warming...");
			SelectIndexedNodes(inav, repeat, perf, expr2);
			Console.WriteLine("Indexed selection, testing...");
			SelectIndexedNodes(inav, repeat, perf, expr2);      
		}

		private static void SelectNodes(XPathNavigator nav, int repeat, PerfTest perf, XPathExpression expr) 
		{
			int counter = 0;
			perf.Start();
			for (int i=0; i<repeat; i++) 
			{
				XPathNodeIterator ni =  nav.Select(expr);
				while (ni.MoveNext())
					counter++;
			}
			Console.WriteLine("Regular selection: {0} times, total time {1, 6:f2} ms, {2} nodes selected", repeat,
				perf.Stop(), counter);
		}

		private static void SelectIndexedNodes(XPathNavigator nav, int repeat, PerfTest perf, XPathExpression expr) 
		{
			int counter = 0;
			perf.Start();
			for (int i=0; i<repeat; i++) 
			{
				XPathNodeIterator ni =  nav.Select(expr);
				while (ni.MoveNext())
					counter++;
			}
			Console.WriteLine("Indexed selection: {0} times, total time {1, 6:f2} ms, {2} nodes selected", repeat, 
				perf.Stop(), counter);
		}
	}
}