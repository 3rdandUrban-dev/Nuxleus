using System;
using System.IO;

namespace Mvp.Xml.Tests
{
	/// <summary>
	/// Loads test documents.
	/// </summary>
	public sealed class Globals
	{
		public const string MvpNamespace = "mvp-xml";
		public const string MvpPrefix = "mvp";

		/// <summary>
		/// The resource name for sample data from a library (<see cref="http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpguide/html/cpconXslTransformClassImplementsXSLTProcessor.asp?frame=true&hidetoc=true"/>.
		/// </summary>
		public const string LibraryResource = "Mvp.Xml.Tests.library.xml";

		/// <summary>
		/// The resource name for sample data from Pubs database.
		/// </summary>
		public const string PubsResource = "Mvp.Xml.Tests.pubs.xml";

		/// <summary>
		/// The resource name for sample data from Pubs database with xmlns="mvp-xml".
		/// </summary>
		public const string PubsNsResource = "Mvp.Xml.Tests.pubsNs.xml";

		/// <summary>
		/// The resource name for schema for the resource <see cref="PubsNsResource"/>.
		/// </summary>
		public const string PubsNsSchemaResource = "Mvp.Xml.Tests.pubsNs.xsd";

		/// <summary>
		/// The resource name for sample data from Pubs database.
		/// </summary>
		public const string NorthwindResource = "Mvp.Xml.Tests.northwind.xml";

		public static Stream GetResource(string name)
		{
			return typeof(Globals).Assembly.GetManifestResourceStream(
				name);
		}
	}
}
