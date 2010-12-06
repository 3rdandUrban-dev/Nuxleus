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


using Manos;

using System;
using System.IO;


namespace HelloWorld {

	public class HelloWorld : ManosApp {

		public HelloWorld ()
		{
			Route ("/", ctx => ctx.Response.End ("Hello, World"));

			Route ("/shutdown", ctx => System.Environment.Exit (0));

			Route ("/transactions", ctx => {
				ctx.Response.Write ("Number of transactions: '{0}'", ctx.Server.Transactions.Count);

				foreach (var t in ctx.Server.Transactions) {
					ctx.Response.WriteLine ("{0}", t.Request.LocalPath);
				}

				ctx.Response.End ();
			});

			Route ("/timeout", ctx => {
				ctx.Response.WriteLine ("Hello");
				AddTimeout (TimeSpan.FromSeconds (2), (app, data) => {
					Console.WriteLine ("writing world.");
					ctx.Response.WriteLine ("World");
					ctx.Response.End ();
				});
			});

			Route ("/encoding", ctx => {
				ctx.Response.ContentEncoding = System.Text.Encoding.UTF8;
				ctx.Response.SetHeader ("Content-Type","text/html; charset=UTF-8");
				ctx.Response.Write ("À");
				Console.WriteLine ("Writing: '{0}'", "À");

				ctx.Response.End ();
			});

			Get ("/upload", ctx => {
				ctx.Response.Write ("<html><head></head><body>");
				ctx.Response.Write ("<form method=\"POST\" enctype=\"multipart/form-data\">");
				ctx.Response.Write ("<input type=\"text\" name=\"some_name\"><br>");
				ctx.Response.Write ("<input type=\"file\" name=\"the_file\" >");
				ctx.Response.Write ("<input type=\"submit\">");
				ctx.Response.Write ("</form>");
				ctx.Response.Write ("</body></html>");

				ctx.Response.End ();
			});

			Post ("/upload", ctx => {
				ctx.Response.End ("handled upload!");
			});
		}
	}
}
