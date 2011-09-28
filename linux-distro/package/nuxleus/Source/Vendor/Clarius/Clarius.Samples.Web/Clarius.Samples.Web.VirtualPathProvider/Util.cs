using System;
using System.Collections.Generic;
using System.Text;

namespace Clarius.Samples.Web.VirtualPathProvider
{
	internal static class Util
	{
		internal static string ConvertVirtualPathToZipPath (String virtualPath, bool isFile)
		{
			if (virtualPath[0] == '/')
			{
				if (!isFile)
					return virtualPath.Substring (1) + "/";
				else
					return virtualPath.Substring (1);
			}
			else
				return virtualPath;
		}

		internal static string ConvertZipPathToVirtualPath (String zipPath)
		{
			return "/" + zipPath;
		}
	}
}
