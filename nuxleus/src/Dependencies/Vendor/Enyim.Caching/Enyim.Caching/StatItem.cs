using System;
using System.Collections.Generic;
using System.Text;

namespace Enyim.Caching
{
	public enum StatItem : int
	{
		/// <summary>
		/// The number of seconds the server has been running.
		/// </summary>
		Uptime = 0,
		/// <summary>
		/// Current time according to the server.
		/// </summary>
		ServerTime,
		/// <summary>
		/// The version of the server.
		/// </summary>
		Version,

		ItemCount,
		TotalItems,
		ConnectionCount,
		TotalConnections,
		ConnectionStructures,

		GetCount,
		SetCount,
		GetHits,
		GetMisses,

		UsedBytes,
		BytesRead,
		BytesWritten,
		MaxBytes
	}
}

#region [ License information          ]
/* ************************************************************
 *
 * Copyright (c) Attila Kisk�, enyim.com, 2007
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