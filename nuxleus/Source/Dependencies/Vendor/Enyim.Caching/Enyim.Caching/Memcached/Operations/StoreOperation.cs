using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using System.Globalization;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;

namespace Enyim.Caching.Memcached
{
	internal class StoreOperation : ItemOperation
	{
		private const int MaxSeconds = 60 * 60 * 24 * 30;

		private static readonly ArraySegment<byte> DataTerminator = new ArraySegment<byte>(new byte[2] { (byte)'\r', (byte)'\n' });
		private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1);

		private StoreMode mode;
		private object value;

		private long expires;

		private bool hasByteArray;
		private ArraySegment<byte> data;

		internal StoreOperation(StoreMode mode, string key, object value, TimeSpan validFor, DateTime expiresAt)
			: base(key)
		{
			this.mode = mode;
			this.value = value;

			this.expires = GetExpiration(validFor, expiresAt);
		}

		internal StoreOperation(StoreMode mode, string key, ArraySegment<byte> data, TimeSpan validFor, DateTime expiresAt)
			: base(key)
		{
			this.hasByteArray = true;
			this.mode = mode;
			this.data = data;

			this.expires = GetExpiration(validFor, expiresAt);
		}

		private static long GetExpiration(TimeSpan validFor, DateTime expiresAt)
		{
			if (validFor >= TimeSpan.Zero && expiresAt > DateTime.MinValue)
				throw new ArgumentException("You cannot specify both validFor and expiresAt.");

			if (expiresAt > DateTime.MinValue)
			{
				if (expiresAt < UnixEpoch)
					throw new ArgumentOutOfRangeException("expiresAt", "expiresAt must be >= 1970/1/1");

				return (long)(expiresAt.ToUniversalTime() - UnixEpoch).TotalSeconds;
			}

			if (validFor.TotalSeconds >= MaxSeconds || validFor < TimeSpan.Zero)
				throw new ArgumentOutOfRangeException("validFor", "validFor must be < 30 days && >= 0");

			return (long)validFor.TotalSeconds;
		}

		protected override bool ExecuteAction()
		{
			if (this.hasByteArray)
			{
				return this.Store(DataTypeCode.ByteArray, this.data);
			}

			if (this.value is string)
			{
				return this.Store(DataTypeCode.String, Encoding.UTF8.GetBytes((string)value));
			}
			else if (this.value is byte[])
			{
				return this.Store(DataTypeCode.ByteArray, (byte[])value);
			}
			if (this.value is int)
			{
				return this.Store(DataTypeCode.Int32, BitConverter.GetBytes((int)value));
			}
			if (this.value is long)
			{
				return this.Store(DataTypeCode.Int64, BitConverter.GetBytes((long)value));
			}
			if (this.value is uint)
			{
				return this.Store(DataTypeCode.UInt32, BitConverter.GetBytes((uint)value));
			}
			if (this.value is ulong)
			{
				return this.Store(DataTypeCode.UInt64, BitConverter.GetBytes((ulong)value));
			}
			if (this.value is DateTime)
			{
				return this.Store(DataTypeCode.DateTime, BitConverter.GetBytes(((DateTime)value).ToBinary()));
			}
			else
			{
				// ObjectStateFormatter osf = new ObjectStateFormatter();

				using (MemoryStream ms = new MemoryStream())
				{
					new BinaryFormatter().Serialize(ms, this.value);
					// osf.Serialize(ms, this.value);

					return this.Store(DataTypeCode.Object, new ArraySegment<byte>(ms.GetBuffer(), 0, (int)ms.Length));
				}
			}
		}

		private bool Store(DataTypeCode typeCode, byte[] data)
		{
			return this.Store(typeCode, new ArraySegment<byte>(data));
		}

		private bool Store(DataTypeCode typeCode, ArraySegment<byte> data)
		{
			if (data.Array == null) throw new ArgumentNullException("data");

			uint tc = (uint)typeCode;
			StringBuilder sb = new StringBuilder(100);

			switch (this.mode)
			{
				case StoreMode.Add:
					sb.Append("add ");
					break;
				case StoreMode.Replace:
					sb.Append("replace ");
					break;
				case StoreMode.Set:
					sb.Append("set ");
					break;
				default:
					throw new MemcachedClientException(mode + " is not supported.");
			}

			sb.Append(this.HashedKey);
			sb.Append(" ");
			sb.Append(Convert.ToString(tc, CultureInfo.InvariantCulture));
			sb.Append(" ");
			sb.Append(Convert.ToString(this.expires, CultureInfo.InvariantCulture));
			sb.Append(" ");
			sb.Append(Convert.ToString(data.Count - data.Offset, CultureInfo.InvariantCulture));

			ArraySegment<byte> commandBuffer = PooledSocket.GetCommandBuffer(sb.ToString());

			this.Socket.Write(new ArraySegment<byte>[] { commandBuffer, data, StoreOperation.DataTerminator });

			return String.Compare(this.Socket.ReadResponse(), "STORED", StringComparison.Ordinal) == 0;
		}
	}
}

#region [ License information          ]
/* ************************************************************
 *
 * Copyright (c) Attila Kiskó, enyim.com, 2007
 *
 * This source code is subject to terms and conditions of 
 * Microsoft Permissive License (Ms-PL).
 * 
 * A copy of the license can be found in the License.html
 * file at the root of this distribution. If you can not 
 * locate the License, please send an email to a@enyim.com
 * 
 * By using this source code in any fashion, you are 
 * agreeing to be bound by the terms of the Microsoft 
 * Permissive License.
 *
 * You must not remove this notice, or any other, from this
 * software.
 *
 * ************************************************************/
#endregion