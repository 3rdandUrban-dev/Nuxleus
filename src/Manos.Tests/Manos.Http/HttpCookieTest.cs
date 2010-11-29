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
using NUnit.Framework;

using Manos.ShouldExt;

namespace Manos.Http.Tests
{
	[TestFixture()]
	public class HttpCookieTest
	{
		[Test]
		public void Ctor_NullName_Throws ()
		{
			Should.Throw<ArgumentNullException> (() => new HttpCookie (null, "value"));
		}
		
		[Test]
		public void Ctor_NullValue_Throws ()
		{
			Should.Throw<ArgumentNullException> (() => new HttpCookie ("name", null));
		}
		
		[Test]
		public void Ctor_ValidValues_AddsPair ()
		{
			var name = "foobar";
			var value = "the value";
			
			var cookie = new HttpCookie (name, value);
			Assert.AreEqual (1, cookie.Values.Count);
		}
		
		[Test]
		public void Ctor_ValidValues_AddsName ()
		{
			var name = "foobar";
			var value = "the value";
			
			var cookie = new HttpCookie (name, value);
			
			bool contains = cookie.Values.ContainsKey ("foobar");
			Assert.IsTrue (contains);
		}
		
		[Test]
		public void Ctor_ValidValues_SetsValue ()
		{
			var name = "foobar";
			var value = "the value";
			
			var cookie = new HttpCookie (name, value);
			Assert.AreEqual ("the value", cookie.Values [name]);
		}
		
		[Test]
		public void ToHeaderString_SingleValueNoWhiteSpace_Formatting()
		{
			var name = "foobar";
			var value = "value";
			var cookie = new HttpCookie (name, value);
			
			var header = cookie.ToHeaderString ();
			Assert.AreEqual ("Set-Cookie: foobar=value\r\n", header);
		}
		
		[Test]
		public void ToHeaderString_SingleValueWithWhiteSpace_Formatting()
		{
			var name = "foobar";
			var value = "the value";
			var cookie = new HttpCookie (name, value);
			
			var header = cookie.ToHeaderString ();
			Assert.AreEqual ("Set-Cookie: foobar=\"the value\"\r\n", header);
		}
		
		[Test]
		public void ToHeaderString_SingleValueWithSemiColon_Formatting()
		{
			var name = "foobar";
			var value = "value;";
			var cookie = new HttpCookie (name, value);
			
			var header = cookie.ToHeaderString ();
			Assert.AreEqual ("Set-Cookie: foobar=\"value;\"\r\n", header);
		}
		
		[Test]
		public void ToHeaderString_SingleValueWithComma_Formatting()
		{
			var name = "foobar";
			var value = "val,ue";
			var cookie = new HttpCookie (name, value);
			
			var header = cookie.ToHeaderString ();
			Assert.AreEqual ("Set-Cookie: foobar=\"val,ue\"\r\n", header);
		}
		
		[Test]
		public void ToHeaderString_NameWithComma_Formatting()
		{
			var name = "foo,bar";
			var value = "value";
			var cookie = new HttpCookie (name, value);
			
			var header = cookie.ToHeaderString ();
			Assert.AreEqual ("Set-Cookie: \"foo,bar\"=value\r\n", header);
		}
		
		[Test]
		public void ToHeaderString_NameValueAndDomain_Formatting ()
		{
			var name = "foobar";
			var value = "value";
			var cookie = new HttpCookie (name, value);
			
			cookie.Domain = "http://manos-de-mono.com";
			
			var header = cookie.ToHeaderString ();
			Assert.AreEqual ("Set-Cookie: foobar=value; domain=http://manos-de-mono.com\r\n", header);
		}
		
		[Test]
		public void ToHeaderString_NameValueAndPath_Formatting ()
		{
			var name = "foobar";
			var value = "value";
			var cookie = new HttpCookie (name, value);
			
			cookie.Path = "/foobar/";
			
			var header = cookie.ToHeaderString ();
			Assert.AreEqual ("Set-Cookie: foobar=value; path=/foobar/\r\n", header);
		}
		
		[Test]
		public void ToHeaderString_NameValueAndExpires_Formatting ()
		{
			var name = "foobar";
			var value = "value";
			var cookie = new HttpCookie (name, value);
			
			cookie.Expires = new DateTime (2010, 8, 4, 10, 11, 12, DateTimeKind.Utc);
			
			var header = cookie.ToHeaderString ();
			Assert.AreEqual ("Set-Cookie: foobar=value; expires=Wed, 04 Aug 2010 10:11:12 GMT\r\n", header);
		}
		
		[Test]
		public void ToHeaderString_NameValueAndSecure_Formatting ()
		{
			var name = "foobar";
			var value = "value";
			var cookie = new HttpCookie (name, value);
			
			cookie.Secure = true;
			
			var header = cookie.ToHeaderString ();
			Assert.AreEqual ("Set-Cookie: foobar=value; secure\r\n", header);
		}
		
		[Test]
		public void ToHeaderString_NameValueAndHttpOnly_Formatting ()
		{
			var name = "foobar";
			var value = "value";
			var cookie = new HttpCookie (name, value);
			
			cookie.HttpOnly = true;
			
			var header = cookie.ToHeaderString ();
			Assert.AreEqual ("Set-Cookie: foobar=value; HttpOnly\r\n", header);
		}
		
		[Test]
		public void FromHeader_SingleValue_SetsValue ()
		{
			var header = "foo=bar";
			var dict = HttpCookie.FromHeader (header);
			
			var value = dict ["foo"];
			Assert.AreEqual ("bar", value);
		}
		
		[Test]
		public void FromHeader_SingleValueSpaceBeforeEquals_SetsValueNoSpaces ()
		{
			var header = "foo =bar";
			var dict = HttpCookie.FromHeader (header);
			
			var value = dict ["foo"];
			Assert.AreEqual ("bar", value);
		}
		
		[Test]
		public void FromHeader_SingleValueSpaceAfterEquals_SetsValueNoSpaces ()
		{
			var header = "foo= bar";
			var dict = HttpCookie.FromHeader (header);
			
			var value = dict ["foo"];
			Assert.AreEqual ("bar", value);
		}
		
		[Test]
		public void FromHeader_TwoValues_SetsFirstValue ()
		{
			var header = "foo=bar;blah=bra";
			var dict = HttpCookie.FromHeader (header);
			
			var value = dict ["foo"];
			Assert.AreEqual ("bar", value);
		}
		
		[Test]
		public void FromHeader_TwoValues_SetsSecondValue ()
		{
			var header = "blah=bra;foo=bar";
			var dict = HttpCookie.FromHeader (header);
			
			var value = dict ["foo"];
			Assert.AreEqual ("bar", value);
		}
		
		[Test]
		public void FromHeader_TwoValuesWhiteSpaceBetween_SetsFirstValue ()
		{
			var header = "foo=bar ; blah=bra";
			var dict = HttpCookie.FromHeader (header);
			
			var value = dict ["foo"];
			Assert.AreEqual ("bar", value);
		}
		
		[Test]
		public void FromHeader_TwoValuesWhiteSpaceBetween_SetsSecondValue ()
		{
			var header = "blah=bra ; foo=bar";
			var dict = HttpCookie.FromHeader (header);
			
			var value = dict ["foo"];
			Assert.AreEqual ("bar", value);
		}
		
		[Test]
		public void FromHeader_EmptyValue_DoesNotThrow ()
		{
			var header = " ; ";
			var dict = HttpCookie.FromHeader (header);
			
			Should.NotThrow (() => HttpCookie.FromHeader (header));
		}
		
		[Test]
		public void FromHeader_EmptyValue_ReturnsEmptyDict ()
		{
			var header = " ; ";
			var dict = HttpCookie.FromHeader (header);
			
			HttpCookie.FromHeader (header);
			
			Assert.AreEqual (0, dict.Count);
		}
		
		[Test]
		public void FromHeader_KeyNoValue_DoesNotThrow ()
		{
			var header = "foobar";
			var dict = HttpCookie.FromHeader (header);
			
			Should.NotThrow (() => HttpCookie.FromHeader (header));
		}
		
		[Test]
		public void FromHeader_KeyNoValue_ReturnsEmptyDict ()
		{
			var header = "foobar";
			var dict = HttpCookie.FromHeader (header);
			
			HttpCookie.FromHeader (header);
			
			Assert.AreEqual (0, dict.Count);
		}
	}
}

