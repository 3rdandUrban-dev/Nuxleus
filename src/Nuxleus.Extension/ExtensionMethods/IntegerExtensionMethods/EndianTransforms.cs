using System;

namespace Nuxleus
{
	public static partial class IntegerExtensionMethods
	{
		public static short ToBigEndianByteOrder (this short v)
		{
			return (short)(((v & 0xff) << 8) | ((v >> 8) & 0xff));
		}

		public static int ToBigEndianByteOrder (this int v)
		{
			return (int)(((ToBigEndianByteOrder ((short)v) & 0xffff) << 0x10) |
                          (ToBigEndianByteOrder ((short)(v >> 0x10)) & 0xffff));
		}
	}
}

