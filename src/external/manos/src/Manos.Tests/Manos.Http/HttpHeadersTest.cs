//
// Copyright (C) 2010 Jackson Harper (jackson@manosdemono.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//


using System;
using System.IO;

using NUnit.Framework;
using Manos.ShouldExt;



namespace Manos.Http.Tests
{
	[TestFixture()]
	public class HttpHeadersTest
	{

		[Test()]
		public void TestCtor ()
		{
			HttpHeaders headers = new HttpHeaders ();
			
			Assert.IsNull (headers.ContentLength, "a1");
		}
		
		[Test()]
		public void TestSingleValueParse ()
		{
			HttpHeaders headers = new HttpHeaders ();
			
			string str = "Key: Value";
			
			headers.Parse (new StringReader (str));
			
			Assert.AreEqual ("Value", headers ["Key"], "a1");
			Assert.AreEqual (1, headers.Count, "a2");
		}
		
		[Test()]
		public void TestSingleValueParseTrailingWhiteSpace ()
		{
			HttpHeaders headers = new HttpHeaders ();
			
			string str = "Key: Value      ";
			headers.Parse (new StringReader (str));
			Assert.AreEqual ("Value", headers ["Key"], "a1");
			Assert.AreEqual (1, headers.Count, "a2");
			
			str = "Key: Value\t";
			headers.Parse (new StringReader (str));
			Assert.AreEqual ("Value", headers ["Key"], "a1");
			Assert.AreEqual (1, headers.Count, "a2");
			
			str = "Key: Value ";
			headers.Parse (new StringReader (str));
			Assert.AreEqual ("Value", headers ["Key"], "a1");
			Assert.AreEqual (1, headers.Count, "a2");
		}
		
		[Test()]
		public void TestValueIsJustWhiteSpace ()
		{
			HttpHeaders headers = new HttpHeaders ();
			
			string str = "Key: ";
			
			Should.Throw<HttpException> (() => headers.Parse (new StringReader (str)));
			Assert.AreEqual (0, headers.Count, "a2");
		}
		
		[Test()]
		public void TestWhiteSpaceStartsFirstLine ()
		{
			HttpHeaders headers = new HttpHeaders ();
			
			string str = " Key: Value";
			
			Should.Throw<HttpException> (() => headers.Parse (new StringReader (str)));
			Assert.AreEqual (0, headers.Count, "a2");
		}
		
		[Test()]
		public void TestMultipleValueParse ()
		{
			HttpHeaders headers = new HttpHeaders ();
			
			string str = "Key1: Value1\nKey2: Value2\nKey3: Value3";
			
			headers.Parse (new StringReader (str));
			
			Assert.AreEqual ("Value1", headers ["Key1"], "a1");
			Assert.AreEqual ("Value2", headers ["Key2"], "a2");
			Assert.AreEqual ("Value3", headers ["Key3"], "a3");
			Assert.AreEqual (3, headers.Count, "a4");
		}
		
		[Test()]
		public void TestMultilineParse ()
		{
			//
			// multiline values are acceptable if the next 
			// line starts with spaces
			//
			string header = @"HeaderName: Some multiline
  								value";
		
			HttpHeaders headers = new HttpHeaders ();
			
			headers.Parse (new StringReader (header));
			
			Assert.AreEqual ("Some multiline value", headers ["HeaderName"], "a1");
			
			header = @"HeaderName: Some multiline
  								value
	that spans
	a bunch of lines";
			
			headers = new HttpHeaders ();
			headers.Parse (new StringReader (header));
			
			Assert.AreEqual ("Some multiline value that spans a bunch of lines", headers ["HeaderName"], "a2");
		}
		
		[Test]
		public void TestParseNoValue ()
		{
			HttpHeaders headers = new HttpHeaders ();
			
			string str = "Key:\n";
			
			Should.Throw<HttpException> (() => headers.Parse (new StringReader (str)));
			Assert.AreEqual (0, headers.Count, "a2");
			Assert.IsNull (headers.ContentLength, "a3");
		}
		
		[Test]
		public void TestParseNoColon ()
		{
			HttpHeaders headers = new HttpHeaders ();
			
			string str = "Key value";
			
			Should.Throw<HttpException> (() => headers.Parse (new StringReader (str)));
			Assert.AreEqual (0, headers.Count, "a2");
			Assert.IsNull (headers.ContentLength, "a3");
		}
		
		[Test()]
		public void TestNormalizeNoDash ()
		{	
			Assert.AreEqual ("Foo", HttpHeaders.NormalizeName ("foo"));
			Assert.AreEqual ("Foo", HttpHeaders.NormalizeName ("FOO"));
			Assert.AreEqual ("Foo", HttpHeaders.NormalizeName ("FOo"));
			Assert.AreEqual ("Foo", HttpHeaders.NormalizeName ("foO"));
		}
		
		[Test()]
		public void TestNormalizeDashedName ()
		{
			Assert.AreEqual ("Foo-Bar", HttpHeaders.NormalizeName ("foo-bar"));
			Assert.AreEqual ("Foo-Bar", HttpHeaders.NormalizeName ("FOO-BAR"));
			Assert.AreEqual ("Foo-Bar", HttpHeaders.NormalizeName ("Foo-bar"));
			Assert.AreEqual ("Foo-Bar", HttpHeaders.NormalizeName ("foo-BAR"));
			Assert.AreEqual ("Foo-Bar", HttpHeaders.NormalizeName ("Foo-BaR"));
		}
		
		[Test()]
		public void TestNormalizeDoubleDash ()
		{
			Assert.AreEqual ("Foo--Bar", HttpHeaders.NormalizeName ("foo--bar"));
			Assert.AreEqual ("Foo--Bar", HttpHeaders.NormalizeName ("FOO--BAR"));
			Assert.AreEqual ("Foo--Bar", HttpHeaders.NormalizeName ("Foo--bar"));
			Assert.AreEqual ("Foo--Bar", HttpHeaders.NormalizeName ("foo--BAR"));
			Assert.AreEqual ("Foo--Bar", HttpHeaders.NormalizeName ("Foo--BaR"));
		}
		
		[Test()]
		public void TestNormalizeStartDash ()
		{
			Assert.AreEqual ("-Foo-Bar", HttpHeaders.NormalizeName ("-foo-bar"));
			Assert.AreEqual ("-Foo-Bar", HttpHeaders.NormalizeName ("-FOO-BAR"));
			Assert.AreEqual ("-Foo-Bar", HttpHeaders.NormalizeName ("-Foo-bar"));
			Assert.AreEqual ("-Foo-Bar", HttpHeaders.NormalizeName ("-foo-BAR"));
			Assert.AreEqual ("-Foo-Bar", HttpHeaders.NormalizeName ("-Foo-BaR"));
		}
		
		[Test()]
		public void TestNormalizeEndDash ()
		{
			Assert.AreEqual ("Foo-Bar-", HttpHeaders.NormalizeName ("foo-bar-"));
			Assert.AreEqual ("Foo-Bar-", HttpHeaders.NormalizeName ("FOO-BAR-"));
			Assert.AreEqual ("Foo-Bar-", HttpHeaders.NormalizeName ("Foo-bar-"));
			Assert.AreEqual ("Foo-Bar-", HttpHeaders.NormalizeName ("foo-BAR-"));
			Assert.AreEqual ("Foo-Bar-", HttpHeaders.NormalizeName ("Foo-BaR-"));
		}
		
		[Test]
		public void TestSetValue ()
		{
			HttpHeaders headers = new HttpHeaders ();
			
			headers.SetHeader ("Foo", "Bar");
			Assert.AreEqual ("Bar", headers ["Foo"], "a1");
			
			// Keys should get normalized
			headers.SetHeader ("foo", "bar");
			Assert.AreEqual ("bar", headers ["foo"], "a2");
			Assert.AreEqual ("bar", headers ["Foo"], "a3");
		}
		
		[Test]
		public void TestSetValueNullRemovesKey ()
		{
			string dummy = null;
			HttpHeaders headers = new HttpHeaders ();
			
			headers.SetHeader ("Foo", "Bar");
			Assert.AreEqual ("Bar", headers ["Foo"], "a1");
			
			headers.SetHeader ("Foo", null);
			Assert.IsFalse (headers.TryGetValue ("Foo", out dummy), "a2");
			Assert.AreEqual (0, headers.Count, "a3");
		}
		
		[Test]
		public void TestSetContentLength ()
		{
			HttpHeaders headers = new HttpHeaders ();
			
			headers.SetContentLength ("100");
			Assert.AreEqual (100, headers.ContentLength, "a1");
			
			headers.SetContentLength ("1");
			Assert.AreEqual (1, headers.ContentLength, "a2");
			
			headers.SetContentLength ("0");
			Assert.AreEqual (0, headers.ContentLength, "a3");
		}
		
		[Test]
		public void TestSetContentLengthNegative ()
		{
			HttpHeaders headers = new HttpHeaders ();
			Should.Throw<ArgumentException> (() => headers.SetContentLength ("-1"));
		}
		
		[Test]
		public void TestSetContentLengthInvalid ()
		{
			HttpHeaders headers = new HttpHeaders ();
			
			Should.Throw<ArgumentException> (() => headers.SetContentLength ("foobar"));
			Should.Throw<ArgumentException> (() => headers.SetContentLength ("-1 foobar"));
			Should.Throw<ArgumentException> (() => headers.SetContentLength ("1foo"));
			Should.Throw<ArgumentException> (() => headers.SetContentLength ("1FA"));
			Should.Throw<ArgumentException> (() => headers.SetContentLength ("1."));
			Should.Throw<ArgumentException> (() => headers.SetContentLength ("1.0"));
		}
		
		[Test]
		public void TestSetContentLengthNullClearsValue ()
		{
			HttpHeaders headers = new HttpHeaders ();
			
			headers.SetContentLength ("100");
			Assert.AreEqual (100, headers.ContentLength, "a1");
			
			headers.SetContentLength (null);
			Assert.IsNull (headers.ContentLength, "a2");
		}
		
		[Test]
		public void TestSetContentLengthProperty ()
		{
			string cl;
			HttpHeaders headers = new HttpHeaders ();
			
			headers.ContentLength = 10;
			Assert.AreEqual (10, headers.ContentLength, "a1");
			Assert.IsTrue (headers.TryGetValue ("Content-Length", out cl), "a2");
			Assert.AreEqual ("10", cl, "a3");
			
			headers.ContentLength = 100;
			Assert.AreEqual (100, headers.ContentLength, "a4");
			Assert.IsTrue (headers.TryGetValue ("Content-Length", out cl), "a5");
			Assert.AreEqual ("100", cl, "a6");
			
			headers.ContentLength = 0;
			Assert.AreEqual (0, headers.ContentLength, "a7");
			Assert.IsTrue (headers.TryGetValue ("Content-Length", out cl), "a8");
			Assert.AreEqual ("0", cl, "a9");
		}
		
		[Test]
		public void TestSetContentLengthPropertyNull ()
		{
			string dummy = null;
			HttpHeaders headers = new HttpHeaders ();
			
			headers.ContentLength = 100;
			Assert.AreEqual (100, headers.ContentLength, "a1");
			
			headers.ContentLength = null;
			Assert.IsNull (headers.ContentLength, "a2");
			Assert.IsFalse (headers.TryGetValue ("Content-Length", out dummy), "a3");
			Assert.IsNull (dummy, "a4");
			
		}
		
		[Test]
		public void TestSetContentLengthPropertyNegative ()
		{
			string dummy = null;
			HttpHeaders headers = new HttpHeaders ();
			
			Should.Throw<ArgumentException> (() => headers.ContentLength = -1, "a1");
			Assert.IsNull (headers.ContentLength, "a2");
			Assert.IsFalse (headers.TryGetValue ("Content-Length", out dummy), "a3");
			Assert.IsNull (dummy, "a4");
		}
	}
}
