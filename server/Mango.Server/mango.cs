
using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using Mango;
using Mango.Server;


public class T {

	public static bool show_headers;
	public static int connections = 0;

	public static MangoApp app;

	public static void Main (string [] args)
	{
		show_headers = false;

		if (args.Length > 0)
			app = LoadLibrary (args [0]);

		HttpServer server = new HttpServer (HandleRequest);
		server.Bind (8080);
		server.Start ();
		server.IOLoop.Start ();
	}

	public static MangoApp LoadLibrary (string library)
	{
		Assembly a = Assembly.LoadFrom (library);

		foreach (Type t in a.GetTypes ()) {
			if (t.BaseType == typeof (MangoApp)) {
				if (app != null)
					throw new Exception ("Library contains multiple apps.");
				app = (MangoApp) Activator.CreateInstance (t);
			}
		}

		Console.WriteLine ("running app:  {0}", app);
		return app;
	}

	public static void HandleRequest (HttpConnection con)
	{
		string message = String.Format ("You requested {0}\n", con.Request.ResourceUri);

		string fullpath = con.Request.LocalPath;

		if (show_headers) {
			Console.WriteLine ("HEADERS:");
			Console.WriteLine ("===========");
			foreach (string h in con.Request.Headers.Keys) {
				Console.WriteLine ("{0} = {1}", h, con.Request.Headers [h]);
			}
			Console.WriteLine ("===========");
		}

		if (app != null) {
			app.HandleConnection (con);
			return;
		}

		if (fullpath.Length > 0) {
			string path = fullpath.Substring (1);

			int query = path.IndexOf ('?');
			if (query > 0)
				path = path.Substring (0, query);

			if (File.Exists (path)) {
				con.Response.StatusCode = 200;

				if (Path.GetExtension (path) == ".html")
					con.Response.Headers.SetHeader ("Content-Type", "text/html; charset=ISO-8859-4");
				con.Response.SendFile (path);
			} else
				con.Response.StatusCode = 404;
			
		}

		// con.Response.Write (String.Format ("HTTP/1.1 200 OK\r\nContent-Length: {0}\r\n\r\n{1}", Encoding.ASCII.GetBytes (message).Length, message));
		con.Response.Finish ();

		++connections;
		if (connections >= 1000)
			con.IOStream.IOLoop.Stop ();
	}
}

