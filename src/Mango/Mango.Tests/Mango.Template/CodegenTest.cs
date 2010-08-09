
using System;
using NUnit.Framework;

namespace Mango.Templates.Tests
{
	[TestFixture()]
	public class CodegenTest
	{

		[Test()]
		public void TestCase ()
		{
		}
		
		[Test]
		public void TestFullTypeNameForPath ()
		{
			string app_name = "FooBar";
			string path;
			string name;
			
			path = "Tests.html";
			name = Page.FullTypeNameForPath (app_name, path);
			Assert.AreEqual ("FooBar.Templates.Tests", name, "a1");
			
			path = "Mango.Tests.Tests.html";
			name = Page.FullTypeNameForPath (app_name, path);
			Assert.AreEqual ("FooBar.Templates.Mango.Tests.Tests", name, "a2");
			
			path = "mango.tests.html";
			name = Page.FullTypeNameForPath (app_name, path);
			Assert.AreEqual ("FooBar.Templates.Mango.Tests", name, "a3");
			
			path = "Mango/Tests.html";
			name = Page.FullTypeNameForPath (app_name, path);
			Assert.AreEqual ("FooBar.Templates.Mango.Tests", name, "a4");
			
			path = "Mango.Tests/Tests.html";
			name = Page.FullTypeNameForPath (app_name, path);
			Assert.AreEqual ("FooBar.Templates.Mango.Tests.Tests", name, "a5");
			
			path = "mango/tests.html";
			name = Page.FullTypeNameForPath (app_name, path);
			Assert.AreEqual ("FooBar.Templates.Mango.Tests", name, "a6");
		}
		
		[Test]
		public void TestTypeNameForPathWithDoubleDots ()
		{
			string app_name = "FooBar";
			string path;
			
			path = "Mango/tests..html";
			Assert.Throws<ArgumentException> (() => Page.FullTypeNameForPath (app_name, path));
		}
	}
}
