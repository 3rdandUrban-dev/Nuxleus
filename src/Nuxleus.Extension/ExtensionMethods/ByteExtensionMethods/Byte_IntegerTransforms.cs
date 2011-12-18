using System;

namespace Nuxleus
{
	public static partial class ByteExtensionMethods
	{
		public static int ToBase16Integer (this byte[] byteArray)
		{
			return Convert.ToInt32 (GetStringValue (byteArray), 16);
		}
	}
}

