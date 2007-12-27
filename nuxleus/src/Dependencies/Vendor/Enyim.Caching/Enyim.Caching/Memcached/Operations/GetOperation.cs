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
	internal class GetOperation : ItemOperation
	{
		private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(GetOperation));

		private object result;

		internal GetOperation(string key)
			: base(key)
		{
		}

		public object Result
		{
			get { return this.result; }
		}

		protected override bool ExecuteAction()
		{
			this.Socket.SendCommand("get " + this.HashedKey);

			this.result = this.DeserializeCurrent();
			if (this.result != null)
				this.FinishCurrent();

			return true;
		}

		private object DeserializeCurrent()
		{
			SerializedData data = this.ReadValue();
			if (data.IsEmpty)
				return null;

			switch (data.TypeCode)
			{
				case DataTypeCode.ByteArray:
					return data.Data.Array;
				case DataTypeCode.DateTime:
					return DateTime.FromBinary(BitConverter.ToInt64(data.Data.Array, 0));
				case DataTypeCode.Int32:
					return BitConverter.ToInt32(data.Data.Array, 0);
				case DataTypeCode.Int64:
					return BitConverter.ToInt64(data.Data.Array, 0);
				case DataTypeCode.UInt32:
					return BitConverter.ToUInt32(data.Data.Array, 0);
				case DataTypeCode.UInt64:
					return BitConverter.ToUInt64(data.Data.Array, 0);
				case DataTypeCode.String:
					return Encoding.UTF8.GetString(data.Data.Array);
				case DataTypeCode.Object:
					using (MemoryStream ms = new MemoryStream(data.Data.Array, data.Data.Offset, data.Data.Count, false))
					{
						return new BinaryFormatter().Deserialize(ms);
						// return new ObjectStateFormatter().Deserialize(ms);
					}
				default:
					throw new MemcachedClientException("Invalid TypeCode returned: " + (int)data.TypeCode);
			}
		}

		private void FinishCurrent()
		{
			string response = this.Socket.ReadResponse();

			if (String.Compare(response, "END", StringComparison.Ordinal) != 0)
				throw new MemcachedClientException("No END was received.");
		}

		private SerializedData ReadValue()
		{
			string description = this.Socket.ReadResponse();

			if (String.Compare(description, "END", StringComparison.Ordinal) == 0)
				return SerializedData.Empty;

			if (description.Length < 6 || String.Compare(description, 0, "VALUE ", 0, 6, StringComparison.Ordinal) != 0)
				throw new MemcachedClientException("No VALUE response received.\r\n" + description);

			string[] parts = description.Split(' ');
			if (parts.Length != 4)
				throw new MemcachedClientException("Invalid VALUE response received.\r\n" + description);

			uint flags = UInt32.Parse(parts[2], CultureInfo.InvariantCulture);
			int length = Int32.Parse(parts[3], CultureInfo.InvariantCulture);

			byte[] allData = new byte[length];
			byte[] eod = new byte[2];

			this.Socket.Read(allData, 0, length);
			this.Socket.Read(eod, 0, 2); // data is terminated by \r\n

			SerializedData data = new SerializedData(flags, allData);

			if (log.IsDebugEnabled)
				log.DebugFormat("Received value. Data type: {0}, size: {1}.", data.TypeCode, data.Data.Count);

			return data;
		}

		#region [ T:SerializedData               ]
		private struct SerializedData
		{
			public SerializedData(uint flags, byte[] data)
				: this(flags, data, 0, data.Length)
			{
			}

			public SerializedData(uint flags, byte[] data, int offset, int count)
			{
				this.TypeCode = (DataTypeCode)(flags & 0xff);
				this.Data = new ArraySegment<byte>(data, offset, count);
			}

			public bool IsEmpty
			{
				get { return this.TypeCode == DataTypeCode.Unknown; }
			}

			public readonly DataTypeCode TypeCode;
			public readonly ArraySegment<byte> Data;

			public static readonly SerializedData Empty = new SerializedData();
		}
		#endregion
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