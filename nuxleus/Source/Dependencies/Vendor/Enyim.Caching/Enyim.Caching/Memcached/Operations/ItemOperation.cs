﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using System.Globalization;
using System.Threading;
using System.Text.RegularExpressions;

namespace Enyim.Caching.Memcached
{
	internal abstract class ItemOperation : Operation
	{
		private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(ItemOperation));

		private string key;
		private string hashedKey;

		private PooledSocket socket;

		protected ItemOperation(string key)
		{
			this.key = key;
#if (DEBUG)
			// be careful with this one
			//this.hashedKey = Regex.Replace(key, @"[^a-zA-Z0-9<>=+_\-!@#\$%^&\*\(\),\.]", "_");
#endif
		}

		protected string Key
		{
			get { return this.key; }
		}

		/// <summary>
		/// Gets the hashed bersion of the key which should be used as key in communication with memcached
		/// </summary>
		protected string HashedKey
		{
			get { return this.hashedKey ?? (this.hashedKey = Operation.HashValue(this.key)); }
		}

		protected PooledSocket Socket
		{
			get
			{
				if (this.socket == null)
				{
					// get a connection to the server which belongs to "key"
					PooledSocket ps = ServerPool.Current.Acquire(this.key);

					// null was returned, so our server is dead and no one could replace it
					// (probably all of our servers are down)
					if (ps == null)
					{
						return null;
					}

					this.socket = ps;
				}

				return this.socket;
			}
		}

		public override void Dispose()
		{
			if (this.socket != null)
			{
				((IDisposable)this.socket).Dispose();
				this.socket = null;
			}

			base.Dispose();
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