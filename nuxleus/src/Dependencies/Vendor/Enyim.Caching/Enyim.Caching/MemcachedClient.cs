using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using System.Globalization;
using System.Threading;
using Enyim.Caching.Memcached;

namespace Enyim.Caching
{
	public sealed class MemcachedClient
	{
		public static readonly TimeSpan Infinite = TimeSpan.Zero;

		/// <summary>
		/// Removes the specified item from the cache.
		/// </summary>
		/// <param name="key">The identifier for the item to delete.</param>
		public void Remove(string key)
		{
			using (DeleteOperation d = new DeleteOperation(key))
				d.Execute();
		}

		/// <summary>
		/// Retrieves the specified item from the cache.
		/// </summary>
		/// <param name="key">The identifier for the item to retrieve.</param>
		/// <returns>The retrieved item, or <value>null</value> if the key was not found.</returns>
		public object Get(string key)
		{
			using (GetOperation g = new GetOperation(key))
			{
				g.Execute();

				return g.Result;
			}
		}

		/// <summary>
		/// Retrieves the specified item from the cache.
		/// </summary>
		/// <param name="key">The identifier for the item to retrieve.</param>
		/// <returns>The retrieved item, or <value>null</value> if the key was not found.</returns>
		public T Get<T>(string key)
		{
			object retval = this.Get(key);

			if (retval == null || !(retval is T))
				return default(T);

			return (T)retval;
		}

		/// <summary>
		/// Increments the value of the specified key by the given amount. The operation is atomic and happens on the server.
		/// </summary>
		/// <param name="key">The identifier for the item to increment.</param>
		/// <param name="amount">The amount by which the client wants to increase the item.</param>
		/// <returns>The new value of the item or -1 if not found.</returns>
		/// <remarks>The item must be inserted into the cache before it can be changed. The item must be inserted as a <see cref="T:System.String"/>. The operation only works with <see cref="System.UInt32"/> values, so -1 always indicates that the item was not found.</remarks>
		public long Increment(string key, uint amount)
		{
			using (IncrementOperation i = new IncrementOperation(key, amount))
			{
				i.Execute();

				return i.Success ? (long)i.Result : -1;
			}
		}

		/// <summary>
		/// Increments the value of the specified key by the given amount. The operation is atomic and happens on the server.
		/// </summary>
		/// <param name="key">The identifier for the item to increment.</param>
		/// <param name="amount">The amount by which the client wants to decrease the item.</param>
		/// <returns>The new value of the item or -1 if not found.</returns>
		/// <remarks>The item must be inserted into the cache before it can be changed. The item must be inserted as a <see cref="T:System.String"/>. The operation only works with <see cref="System.UInt32"/> values, so -1 always indicates that the item was not found.</remarks>
		public long Decrement(string key, uint amount)
		{
			using (DecrementOperation d = new DecrementOperation(key, amount))
			{
				d.Execute();

				return d.Success ? (long)d.Result : -1;
			}
		}

		/// <summary>
		/// Inserts an item into the cache with a cache key to reference its location.
		/// </summary>
		/// <param name="mode">Defines how the item is stored in the cache.</param>
		/// <param name="key">The key used to reference the item.</param>
		/// <param name="value">The object to be inserted into the cache.</param>
		/// <remarks>The item does not expire unless it is removed due memory pressure.</remarks>
		public void Store(StoreMode mode, string key, object value)
		{
			MemcachedClient.Store(mode, key, value, MemcachedClient.Infinite, DateTime.MinValue);
		}

		/// <summary>
		/// Inserts a range of bytes (usually memory area or serialized data) into the cache with a cache key to reference its location.
		/// </summary>
		/// <param name="mode">Defines how the item is stored in the cache.</param>
		/// <param name="key">The key used to reference the item.</param>
		/// <param name="value">The data to be stored.</param>
		/// <param name="offset">A 32 bit integer that represents the index of the first byte to store.</param>
		/// <param name="length">A 32 bit integer that represents the number of bytes to store.</param>
		/// <remarks>The item does not expire unless it is removed due memory pressure.</remarks>
		public void Store(StoreMode mode, string key, byte[] value, int offset, int length)
		{
			MemcachedClient.Store(mode, key, value, offset, length, MemcachedClient.Infinite, DateTime.MinValue);
		}

		/// <summary>
		/// Inserts an item into the cache with a cache key to reference its location.
		/// </summary>
		/// <param name="mode">Defines how the item is stored in the cache.</param>
		/// <param name="key">The key used to reference the item.</param>
		/// <param name="value">The object to be inserted into the cache.</param>
		/// <param name="validFor">The interval after the item is invalidated in the cache.</param>
		public void Store(StoreMode mode, string key, object value, TimeSpan validFor)
		{
			MemcachedClient.Store(mode, key, value, validFor, DateTime.MinValue);
		}

		/// <summary>
		/// Inserts an item into the cache with a cache key to reference its location.
		/// </summary>
		/// <param name="mode">Defines how the item is stored in the cache.</param>
		/// <param name="key">The key used to reference the item.</param>
		/// <param name="value">The object to be inserted into the cache.</param>
		/// <param name="expiresAt">The time when the item is invalidated in the cache.</param>
		public void Store(StoreMode mode, string key, object value, DateTime expiresAt)
		{
			MemcachedClient.Store(mode, key, value, TimeSpan.MinValue, expiresAt);
		}

		/// <summary>
		/// Inserts a range of bytes (usually memory area or serialized data) into the cache with a cache key to reference its location.
		/// </summary>
		/// <param name="mode">Defines how the item is stored in the cache.</param>
		/// <param name="key">The key used to reference the item.</param>
		/// <param name="value">The data to be stored.</param>
		/// <param name="offset">A 32 bit integer that represents the index of the first byte to store.</param>
		/// <param name="length">A 32 bit integer that represents the number of bytes to store.</param>
		/// <param name="validFor">The interval after the item is invalidated in the cache.</param>
		public void Store(StoreMode mode, string key, byte[] value, int offset, int length, TimeSpan validFor)
		{
			MemcachedClient.Store(mode, key, value, offset, length, validFor, DateTime.MinValue);
		}

		/// <summary>
		/// Inserts a range of bytes (usually memory area or serialized data) into the cache with a cache key to reference its location.
		/// </summary>
		/// <param name="mode">Defines how the item is stored in the cache.</param>
		/// <param name="key">The key used to reference the item.</param>
		/// <param name="value">The data to be stored.</param>
		/// <param name="offset">A 32 bit integer that represents the index of the first byte to store.</param>
		/// <param name="length">A 32 bit integer that represents the number of bytes to store.</param>
		/// <param name="expiresAt">The time when the item is invalidated in the cache.</param>
		public void Store(StoreMode mode, string key, byte[] value, int offset, int length, DateTime expiresAt)
		{
			MemcachedClient.Store(mode, key, value, offset, length, TimeSpan.MinValue, expiresAt);
		}

		public void FlushAll()
		{
			using (FlushOperation f = new FlushOperation())
			{
				f.Execute();
			}
		}
		public ServerStats Stats()
		{
			using (StatsOperation s = new StatsOperation())
			{
				s.Execute();

				return s.Results;
			}
		}

		#region [ Store                        ]
		private static void Store(StoreMode mode, string key, object value, TimeSpan validFor, DateTime expiresAt)
		{
			if (value == null)
				return;

			using (StoreOperation s = new StoreOperation(mode, key, value, validFor, expiresAt))
				s.Execute();
		}

		private static void Store(StoreMode mode, string key, byte[] value, int offset, int length, TimeSpan validFor, DateTime expiresAt)
		{
			if (value == null)
				return;

			using (StoreOperation s = new StoreOperation(mode, key, new ArraySegment<byte>(value, offset, length), validFor, expiresAt))
				s.Execute();
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