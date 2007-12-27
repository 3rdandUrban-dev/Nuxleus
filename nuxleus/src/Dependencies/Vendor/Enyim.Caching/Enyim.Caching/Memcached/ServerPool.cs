using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Configuration;
using Enyim.Configuration;
using System.Net.Sockets;
using System.Threading;
using System.Collections.ObjectModel;

namespace Enyim.Caching.Memcached
{
	internal class ServerPool : IDisposable
	{
		private const int ServerAddressMutations = 30;
		
		private static MemcachedClientSection Settings = ConfigurationManager.GetSection("enyim.com/memcached") as MemcachedClientSection;

		// holds all server keys for mapping an item key to the server consistently
		private uint[] keys;
		// used to lookup a server based on its key
		Dictionary<uint, ServerEntry> servers = new Dictionary<uint, ServerEntry>(new UIntEqualityComparer());
		// holds all dead servers which will be periodically rechecked and put back into the working servers if found alive
		List<ServerEntry> deadServers = new List<ServerEntry>();
		// holds all of the currently working servers
		List<ServerEntry> workingServers = new List<ServerEntry>();

		private ReadOnlyCollection<MemcachedServer> publicWorkingServers;
		private ReadOnlyCollection<MemcachedServer> publicDeadServers;

		// this is a singleton
		public static readonly ServerPool Current = new ServerPool();
		// used to synchronize read/write accesses on the server lists
		private ReaderWriterLock serverAccessLock = new ReaderWriterLock();

		private Timer isAliveTimer;

		private ServerPool()
		{
			if (ServerPool.Settings == null)
				throw new ConfigurationErrorsException("Cache is not configured: missing enyim.com/memcached section.");

			this.isAliveTimer = new Timer(callback_isAliveTimer, null, (int)Settings.SocketPool.DeadTimeout.TotalMilliseconds, (int)Settings.SocketPool.DeadTimeout.TotalMilliseconds);

			this.Initialize();
		}

		private void callback_isAliveTimer(object state)
		{
			this.serverAccessLock.AcquireReaderLock(Timeout.Infinite);

			try
			{
				if (this.deadServers.Count == 0)
					return;

				List<ServerEntry> resurrectList = new List<ServerEntry>();
				foreach (ServerEntry se in this.deadServers)
				{
					if (se.Server.Ping())
						resurrectList.Add(se);
				}

				if (resurrectList.Count > 0)
				{
					this.serverAccessLock.UpgradeToWriterLock(Timeout.Infinite);

					foreach (ServerEntry se in resurrectList)
					{
						this.deadServers.Remove(se);
						this.workingServers.Add(se);
					}

					this.RebuildIndexes();
				}
			}
			finally
			{
				this.serverAccessLock.ReleaseReaderLock();
			}
		}

		/// <summary>
		/// Creates a class which will be used to hash all keys. Only this needs to be changed if another hash algorithm is needed.
		/// </summary>
		/// <returns></returns>
		private static HashAlgorithm CreateHashAlgorithm()
		{
			return new ModifiedFNV();
		}

		private void Initialize()
		{
			if (Settings.Servers.Count == 0)
				throw new ConfigurationErrorsException("Memcached server list cannot be empty.");

			HashAlgorithm hashAlgo = CreateHashAlgorithm();

			foreach (EndPointElement endpoint in Settings.Servers)
			{
				MemcachedServer server = new MemcachedServer(endpoint.ToIPEndPoint(), Settings.SocketPool);
				ServerEntry se = new ServerEntry(server, ServerPool.ServerAddressMutations, hashAlgo);

				this.workingServers.Add(se);

				for (int i = 0; i < se.Keys.Length; i++)
					this.servers[se.Keys[i]] = se; // don't use Add() as it's not guaranteed that our HashAlgorithm is collision-free
			}

			this.RebuildIndexes();
		}

		/// <summary>
		/// Finds a ServerEntry whose key is the neares to the requested item key hash
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		/// <remarks>
		/// Based on the algorith displayed here: http://lists.danga.com/pipermail/memcached/2007-April/003834.html
		/// </remarks>
		private ServerEntry Lookup(uint itemKeyHash)
		{
			this.serverAccessLock.AcquireReaderLock(Timeout.Infinite);
			ServerEntry se;

			try
			{
				if (this.keys.Length == 0)
					return null;

				// get the index of the server assigned to this hash
				int foundIndex = Array.BinarySearch<uint>(this.keys, itemKeyHash);

				// no exact match
				if (foundIndex < 0)
				{
					// this is the nearest server in the list
					foundIndex = ~foundIndex;

					if (foundIndex == 0)
					{
						// it's smaller than everything, so use the last server (with the highest key)
						foundIndex = this.keys.Length - 1;
					}
					else if (foundIndex >= this.keys.Length)
					{
						// the key was larger than all server keys, so return the first server
						foundIndex = 0;
					}
				}

				if (foundIndex < 0 || foundIndex > this.keys.Length)
					return null;

				se = this.servers[this.keys[foundIndex]];
			}
			finally
			{
				this.serverAccessLock.ReleaseReaderLock();
			}

			// oops, server is not working anymore, remove it and do another lookup
			if (!se.Server.IsAlive)
			{
				this.MarkAsDead(se);

				return Lookup(itemKeyHash);
			}

			return se;
		}

		private void MarkAsDead(ServerEntry server)
		{
			this.serverAccessLock.AcquireWriterLock(Timeout.Infinite);

			try
			{
				this.workingServers.Remove(server);
				this.deadServers.Add(server);

				this.RebuildIndexes();
			}
			finally
			{
				this.serverAccessLock.ReleaseWriterLock();
			}
		}

		private void RebuildIndexes()
		{
			// this should only be called with appropriate lockings
			List<uint> tmp = new List<uint>();

			// store all working servers' keys in a sorted array
			foreach (ServerEntry se in this.workingServers)
				tmp.AddRange(se.Keys);

			tmp.Sort();

			this.keys = tmp.ToArray();
			this.publicWorkingServers = null;
			this.publicDeadServers = null;
		}

		public PooledSocket Acquire(string itemKey)
		{
			if (this.serverAccessLock == null)
				throw new ObjectDisposedException("ServerPool");

			uint k = BitConverter.ToUInt32(ServerPool.CreateHashAlgorithm().ComputeHash(Encoding.Unicode.GetBytes(itemKey)), 0);

			ServerEntry se = this.Lookup(k);
			if (se == null)
				return null;

			return se.Server.Acquire();
		}

		public ReadOnlyCollection<MemcachedServer> WorkingServers
		{
			get
			{
				if (this.publicWorkingServers == null)
				{
					this.publicWorkingServers = new ReadOnlyCollection<MemcachedServer>(this.workingServers.ConvertAll<MemcachedServer>(delegate(ServerEntry e) { return e.Server; }));
				}

				return this.publicWorkingServers;
			}
		}

		public ReadOnlyCollection<MemcachedServer> DeadServers
		{
			get
			{
				if (this.publicDeadServers == null)
				{
					this.publicDeadServers = new ReadOnlyCollection<MemcachedServer>(this.deadServers.ConvertAll<MemcachedServer>(delegate(ServerEntry e) { return e.Server; }));
				}

				return this.publicDeadServers;
			}
		}

		#region [ T:ServerEntry                ]
		/// <summary>
		/// This used to group together a server and all of its keys for easier pool management
		/// </summary>
		private class ServerEntry
		{
			public readonly uint[] Keys;
			public readonly MemcachedServer Server;

			public ServerEntry(MemcachedServer server, int numberOfKeys, HashAlgorithm hashAlgo)
			{
				this.Server = server;

				const int KeyLength = 4;
				int partCount = (hashAlgo.HashSize / 8) / KeyLength; // HashSize is in bits, uint is 4 byte long

				if (partCount < 1)
					throw new ArgumentOutOfRangeException("The hash algorithm must provide at least 32 bits long hashes");

				List<uint> k = new List<uint>(partCount * numberOfKeys);

				// every server is registered numberOfKeys times
				// using UInt32s generated from the different parts of the hash
				// i.e. hash is 64 bit:
				// 00 00 aa bb 00 00 cc dd
				// server will be stored with keys 0x0000aabb & 0x0000ccdd
				// (or a bit differently based on the little/big indianness of the host)
				string address = server.EndPoint.ToString();

				for (int i = 0; i < numberOfKeys; i++)
				{
					byte[] data = hashAlgo.ComputeHash(Encoding.ASCII.GetBytes(i + "::" + address));

					for (int h = 0; h < partCount; h++)
					{
						k.Add(BitConverter.ToUInt32(data, h * KeyLength));
					}
				}

				this.Keys = k.ToArray();
			}
		}
		#endregion
		#region [ IDisposable                  ]
		void IDisposable.Dispose()
		{
			ReaderWriterLock rwl = this.serverAccessLock;

			if (rwl == null)
				return;

			this.serverAccessLock = null;

			try
			{
				rwl.AcquireWriterLock(Timeout.Infinite);

				this.deadServers.ForEach(delegate(ServerEntry se) { se.Server.Dispose(); });
				this.workingServers.ForEach(delegate(ServerEntry se) { se.Server.Dispose(); });

				this.deadServers.Clear();
				this.workingServers.Clear();
				this.servers.Clear();

				this.isAliveTimer.Dispose();
				this.isAliveTimer = null;
			}
			finally
			{
				rwl.ReleaseLock();
			}
		}
		#endregion
		#region [ UIntEqualityComparer         ]
		// faster than Comparer.Default
		private class UIntEqualityComparer : IEqualityComparer<uint>
		{
			bool IEqualityComparer<uint>.Equals(uint x, uint y)
			{
				return x == y;
			}

			int IEqualityComparer<uint>.GetHashCode(uint value)
			{
				unchecked
				{
					return (int)value;
				}
			}
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
