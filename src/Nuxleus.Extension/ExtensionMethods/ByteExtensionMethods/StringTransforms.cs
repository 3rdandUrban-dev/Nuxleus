using System;
using System.Text;

namespace Nuxleus
{
	public static partial class ByteExtensionMethods
	{
		public static string GetStringValue (this byte[] byteArray)
		{
			StringBuilder sb = new StringBuilder ();
			foreach (var element in byteArray) {
				sb.Append (element.ToString ("X2"));
			}
			return sb.ToString ();
		}

		public static string GetBase64UrlSafeEncoding (this byte[] byteArray)
		{
			return GetBase64Encoding (byteArray)
				.Replace ("+", "-")
				.Replace ("=", String.Empty)
				.Replace ("/", "_");
		}

		public static string GetBase64Encoding (this byte[] byteArray, bool insertLineBreaks = false)
		{
			return Convert.ToBase64String (byteArray, insertLineBreaks ? 
			Base64FormattingOptions.InsertLineBreaks : Base64FormattingOptions.None);
		}
	}
}

