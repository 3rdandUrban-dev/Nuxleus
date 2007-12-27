using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Enyim.Caching.Memcached
{
	internal abstract class Operation : IDisposable
	{
		private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(ItemOperation));

		private bool isDisposed;
		private bool success;

		protected Operation() {}

		public void Execute()
		{
			this.success = false;

			try
			{
				if (this.CheckDisposed(false))
					return;

				this.success = this.ExecuteAction();
			}
			catch (Exception e)
			{
				log.Error(e);
			}
		}

		protected abstract bool ExecuteAction();

		/// <summary>
		/// Returns a hashed version of the input string.
		/// </summary>
		/// <param name="value">The value to be hashed.</param>
		/// <returns>The hashed version of the input string</returns>
		/// <remarks>Uses <see cref="Enyim.TigerHash"/> but can be easily changed to use MD5, SHA1, etc. and this change is transparent to the rest of the code.</remarks>
		protected static string HashValue(string value)
		{
			// TigerHash th = new TigerHash();
			HashAlgorithm h = new SHA1Managed();
			
			return Convert.ToBase64String(h.ComputeHash(Encoding.Unicode.GetBytes(value)), Base64FormattingOptions.None);
		}

		protected bool CheckDisposed(bool throwOnError)
		{
			if (throwOnError && this.isDisposed)
				throw new ObjectDisposedException("Operation");

			return this.isDisposed;
		}

		public bool Success
		{
			get { return this.success; }
		}

		#region [ IDisposable                  ]
		public virtual void Dispose()
		{
			this.isDisposed = true;
		}

		void IDisposable.Dispose()
		{
			this.Dispose();
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