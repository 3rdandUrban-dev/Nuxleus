using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using Enyim.Configuration;
using System.Diagnostics;

namespace Enyim.Caching.Memcached
{
	[DebuggerDisplay("[ Address: {EndPoint}, IsAlive = {IsAlive}  ]")]
	internal class MemcachedServer : IDisposable
	{
		private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(MemcachedServer));

		private Stack<PooledSocket> freeItems;
		private Dictionary<Guid, PooledSocket> workingItems;
		private Timer cleanupTimer;

		private bool isDisposed;
		private bool isAlive;
		private DateTime markedAsDeadUtc;

		private int minItems;
		private int maxItems;
		private TimeSpan connectionTimeout;
		private int deadTimeout = 2 * 60;

		private AutoResetEvent itemReleasedEvent;
		private ReaderWriterLock stateLock = new ReaderWriterLock();
		private IPEndPoint endpoint;

		public MemcachedServer(IPEndPoint endpoint, SocketPoolElement config)
		{
			this.isAlive = true;
			this.endpoint = endpoint;

			this.minItems = config.MinPoolSize;
			this.maxItems = config.MaxPoolSize;
			this.connectionTimeout = config.ConnectionTimeout;
			this.deadTimeout = (int)config.DeadTimeout.TotalSeconds;

			if (minItems < 0)
				throw new ArgumentOutOfRangeException("minItems", "minItems must be larger than 0");
			if (maxItems < minItems)
				throw new ArgumentOutOfRangeException("maxItems", "maxItems must be larger than minItems");
			if (connectionTimeout < TimeSpan.Zero)
				throw new ArgumentOutOfRangeException("acquireTimeout", "connectionTimeout must >= TimeSpan.Zero");

			this.freeItems = new Stack<PooledSocket>();
			this.workingItems = new Dictionary<Guid, PooledSocket>();
			this.cleanupTimer = new Timer(new TimerCallback(cleanup_Callback), null, 10 * 60 * 1000, 10 * 60 * 1000); // timer will fire in every 10 minutes
			this.itemReleasedEvent = new AutoResetEvent(false);
			this.InitPool();
		}

		private void InitPool()
		{
			try
			{
				if (this.minItems > 0)
				{
					for (int i = 0; i < this.minItems; i++)
					{
						this.freeItems.Push(this.Create());
					}
				}
			}
			catch (Exception e)
			{
				log.Error("Could not init pool.", e);

				this.MarkAsDead();
			}
		}

		public IPEndPoint EndPoint
		{
			get { return this.endpoint; }
		}

		private PooledSocket Create()
		{
			PooledSocket retval = new PooledSocket(this.endpoint, this.connectionTimeout, this.Release);
			retval.Reset();

			return retval;
		}

		/// <summary>
		/// Gets a value indicating whether the server is working or not. Only returns the state, does not perform expiration checks, does not reinitialize itself.
		/// 
		/// Used by the <see cref="T:ServerPool"/> to quickly check if the server's state is valid.
		/// </summary>
		/// <remarks>To check if the server is alive, and make it try to recover itself use the <see cref="M:MemcachedServer.Ping"/> method.</remarks>
		public bool IsAlive
		{
			get { return this.isAlive; }
		}

		/// <summary>
		/// Gets a value indicating whether the server is working or not.
		/// 
		/// If the server is not working, and the "being dead" timeout has been expired it will reinitialize itself.
		/// </summary>
		/// <remarks>It's possible that the server is still not up &amp; running so the next call to <see cref="M:Acquire"/> could mark the instance as dead again.</remarks>
		/// <returns></returns>
		public bool Ping()
		{
			// TODO maybe we should try to connect to the server to check if it is up&running
			return this.CheckIfAlive();
		}

		private void cleanup_Callback(object state)
		{
			// if there are more free items in the pool than should be then destroy them
			if (this.freeItems.Count > this.minItems)
			{
				lock (this.freeItems)
				{
					while (this.freeItems.Count > this.minItems)
					{
						this.freeItems.Pop().Destroy();
					}
				}
			}
		}

		private bool CheckIfAlive()
		{
			if (this.isAlive)
				return true;

			stateLock.AcquireReaderLock(Timeout.Infinite);

			try
			{
				TimeSpan diff = DateTime.UtcNow - markedAsDeadUtc;

				if (diff.TotalSeconds < deadTimeout)
					return false;

				stateLock.UpgradeToWriterLock(Timeout.Infinite);

				if (isAlive)
					return true; // someone else already reinitialized us while we're waiting for the lock

				lock (this.freeItems) lock (this.workingItems)
					{
						while (this.freeItems.Count > 0)
							this.freeItems.Pop().Destroy();

						this.InitPool();
					}

				this.markedAsDeadUtc = DateTime.MinValue;
				this.isAlive = true;

				return true;
			}
			finally
			{
				stateLock.ReleaseReaderLock();
			}
		}

		/// <summary>
		/// Acquires a new item from the pool
		/// </summary>
		/// <returns>An <see cref="T:PooledSocket"/> instance which is connected to the memcached server, or <value>null</value> if the pool is dead.</returns>
		public PooledSocket Acquire()
		{
			if (log.IsDebugEnabled)
				log.Debug("Acquiring stream from pool.");

			if (!this.CheckIfAlive())
			{
				if (log.IsDebugEnabled)
					log.Debug("Pool is dead, returning null.");

				return null;
			}

			// every release signals the event, so even if the pool becomes full in the meantime
			// the WaitOne will succeed, and more items will be in the pool than allowed,
			// so reset the event when an item is inserted
			this.itemReleasedEvent.Reset();

			PooledSocket retval = null;

			// do we have free items?
			if (this.freeItems.Count > 0)
			{
				lock (this.freeItems)
				{
					// we still have it?
					if (this.freeItems.Count > 0)
					{
						if (log.IsDebugEnabled)
							log.Debug("Has free items.");

						// just get it
						retval = freeItems.Pop();

						try
						{
							retval.Reset();

							if (log.IsDebugEnabled)
								log.Debug("Socket was reset. " + retval.InstanceId);
						}
						catch (Exception e)
						{
							log.Error("Failed to reset an acquired stream.", e);

							this.MarkAsDead();
						}
					}
				}
			}

			// free item pool is empty
			if (retval == null)
			{
				if (log.IsDebugEnabled)
					log.Debug("Could not get a stream from the pool.");

				// we ar not allowed to create more, so wait for an item to be released back into the pool
				if (this.workingItems.Count >= this.maxItems)
				{
					if (log.IsDebugEnabled)
						log.Debug("Pool is full, wait for a release.");

					// wait on the event
					if (!itemReleasedEvent.WaitOne(this.connectionTimeout, false))
					{
						if (log.IsDebugEnabled)
							log.Debug("Pool is still full, timeouting.");

						// everyone is working
						throw new TimeoutException();
					}
				}

				if (log.IsDebugEnabled)
					log.Debug("Creating a new item.");

				try
				{
					// okay, create the new item
					retval = this.Create();
				}
				catch (Exception e)
				{
					log.Error("Failed to create socket.", e);
					this.MarkAsDead();

					return null;
				}
			}

			// register as a working item
			lock (workingItems)
			{
				if (log.IsDebugEnabled)
					log.Debug("Register working item.");

				workingItems[retval.InstanceId] = retval;
			}

			// commit all writes
			Thread.MemoryBarrier();

			if (log.IsDebugEnabled)
				log.Debug("Done.");

			return retval;
		}

		private void MarkAsDead()
		{
			if (log.IsWarnEnabled)
				log.WarnFormat("Marking pool {0} as dead", this.EndPoint);

			this.isAlive = false;
			this.markedAsDeadUtc = DateTime.UtcNow;
		}

		/// <summary>
		/// Releases an item back into the pool
		/// </summary>
		/// <param name="socket"></param>
		private void Release(PooledSocket socket)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("Releasing socket " + socket.InstanceId);
				log.Debug("Are we alive? " + this.isAlive);
			}

			// remove the item from the working items' list
			lock (this.workingItems)
				this.workingItems.Remove(socket.InstanceId);

			if (this.isAlive)
			{
				// is it still working (i.e. the server is still connected)
				if (socket.IsAlive)
				{
					// mark the item as free
					lock (freeItems)
						this.freeItems.Push(socket);
				}
				else
				{
					// kill this item
					socket.Destroy();

					//mark ourselves as not working for a while
					this.MarkAsDead();
				}
			}
			else
			{
				// one of our previous sockets has died, so probably all of them are dead
				// kill the socket thus clearing the pool, and after we become alive
				// we'll fill the pool with working sockets
				socket.Destroy();
			}

			// signal the event so if someone is waiting it can reuse this item
			this.itemReleasedEvent.Set();
		}

		/// <summary>
		/// Releases all resources allocated by this instance
		/// </summary>
		public void Dispose()
		{
			// this is not a graceful shutdown
			// if someone uses a pooled item then 99% that an exception will be thrown
			// somewhere. But since the dispose is mostly used when eeveryone else is finished
			// this should not kill any kitten
			lock (this)
			{
				this.CheckDisposed();

				this.isDisposed = true;

				using(this.itemReleasedEvent)
					this.itemReleasedEvent.Reset();

				this.itemReleasedEvent = null;

				lock (this.freeItems) lock (this.workingItems)
					{
						while (this.freeItems.Count > 0)
						{
							try { this.freeItems.Pop().Destroy(); }
							catch { }
						}

						foreach (PooledSocket ps in this.workingItems.Values)
						{
							try { ps.Destroy(); }
							catch { }
						}

						this.cleanupTimer.Dispose();

						this.workingItems = null;
						this.freeItems = null;
						this.cleanupTimer = null;
					}
			}
		}

		private void CheckDisposed()
		{
			if (this.isDisposed)
				throw new ObjectDisposedException("pool");
		}

		void IDisposable.Dispose()
		{
			this.Dispose();
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
