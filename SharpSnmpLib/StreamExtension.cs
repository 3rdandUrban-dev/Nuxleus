﻿// Stream extension class.
// Copyright (C) 2008-2010 Malcolm Crowe, Lex Li, and other contributors.
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Collections.Generic;
using System.IO;
using System.Tuples;

namespace Lextm.SharpSnmpLib
{
    /// <summary>
    /// Stream extension class.
    /// </summary>
    public static class StreamExtension
    {
        internal static byte[] WritePayloadLength(this int length) // excluding initial octet
        {
            if (length < 0)
            {
                throw new ArgumentException("length cannot be negative", "length");
            }

            var stream = new MemoryStream();

            if (length < 127)
            {
                stream.WriteByte((byte)length);
                return stream.ToArray();
            }
            
            var c = new byte[16];
            int j = 0;
            while (length > 0)
            {
                c[j++] = (byte)(length & 0xff);
                length = length >> 8;
            }
            
            stream.WriteByte((byte)(0x80 | j));
            while (j > 0)
            {
                int x = c[--j];
                stream.WriteByte((byte)x);
            }

            return stream.ToArray();
        }

        internal static Tuple<int, byte[]> ReadPayloadLength(this Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            var list = new List<byte>();
            int first = stream.ReadByte();
            var firstByte = (byte)first;
            if ((firstByte & 0x80) == 0)
            {
                return new Tuple<int, byte[]>(first, new[] {firstByte});
            }

            list.Add(firstByte);
            
            int result = 0;
            int octets = firstByte & 0x7f;
            for (int j = 0; j < octets; j++)
            {
                int n = stream.ReadByte();
                if (n == -1)
                {
                    throw new SnmpException("BER end of file");
                }

                var nextByte = (byte)n;
                result = (result << 8) + nextByte;
                list.Add(nextByte);
            }
            
            return new Tuple<int, byte[]>(result, list.ToArray());
        }

        internal static void IgnoreBytes(this Stream stream, int length)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            var bytes = new byte[length];
            stream.Read(bytes, 0, length);
        }

        internal static void AppendBytes(this Stream stream, SnmpType typeCode, byte[] length, byte[] raw)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (raw == null)
            {
                throw new ArgumentNullException("raw");
            }

            stream.WriteByte((byte)typeCode);
            if (length == null)
            {
                length = raw.Length.WritePayloadLength();
            }

            stream.Write(length, 0, length.Length);
            stream.Write(raw, 0, raw.Length);
        }
    }
}