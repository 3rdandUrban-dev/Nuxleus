using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Web.Configuration;

namespace Enyim.Configuration
{
	public sealed class MemcachedClientSection : ConfigurationSection
	{
		[ConfigurationProperty("servers", IsRequired = true)]
		public EndPointElementCollection Servers
		{
			get { return (EndPointElementCollection)base["servers"]; }
		}

		[ConfigurationProperty("socketPool", IsRequired = false)]
		public SocketPoolElement SocketPool
		{
			get { return (SocketPoolElement)base["socketPool"]; }
			set { base["socketPool"] = value; }
		}

		//[ConfigurationProperty("hashKeys", IsRequired = false, DefaultValue = true)]
		//public bool HashKeys
		//{
		//    get { return (bool)base["hashKeys"]; }
		//    set { base["hashKeys"] = value; }
		//}

		//[ConfigurationProperty("keyHashAlgo", IsRequired = false)]
		//public string KeyHashAlgo
		//{
		//    get { return (string)base["keyHashAlgo"]; }
		//    set { base["keyHashAlgo"] = value; }
		//}

		protected override void PostDeserialize()
		{
			WebContext hostingContext = base.EvaluationContext.HostingContext as WebContext;

			if (hostingContext != null && hostingContext.ApplicationLevel == WebApplicationLevel.BelowApplication)
			{
				throw new InvalidOperationException("The enyim.com/memcached section cannot be defined below the application level.");
			}
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