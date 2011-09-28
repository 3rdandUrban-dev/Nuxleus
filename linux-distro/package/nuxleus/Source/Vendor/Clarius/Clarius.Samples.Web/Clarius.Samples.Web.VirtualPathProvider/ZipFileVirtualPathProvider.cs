using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Hosting;

using ICSharpCode.SharpZipLib.Zip;

namespace Clarius.Samples.Web.VirtualPathProvider
{
    public class ZipFileVirtualPathProvider : System.Web.Hosting.VirtualPathProvider
    {
        ZipFile _zipFile;

        public ZipFileVirtualPathProvider (string zipFilename)
            : base () {
            _zipFile = new ZipFile (zipFilename);
        }

        ~ZipFileVirtualPathProvider () {
            _zipFile.Close ();
        }

        public override bool FileExists (string virtualPath)
		{
			string zipPath = Util.ConvertVirtualPathToZipPath (virtualPath, true);
			ZipEntry zipEntry = _zipFile.GetEntry (zipPath);

			if (zipEntry != null)
			{
				return !zipEntry.IsDirectory;
			}
			else
			{
                // Here you may want to return Previous.FileExists(virtualPath) instead of false
                // if you want to give the previously registered provider a process to serve the file
				return false;
			}
		}

		public override bool DirectoryExists (string virtualDir)
		{
			string zipPath = Util.ConvertVirtualPathToZipPath (virtualDir, false);
			ZipEntry zipEntry = _zipFile.GetEntry (zipPath);

			if (zipEntry != null)
			{
				return zipEntry.IsDirectory;
			}
			else
			{
                // Here you may want to return Previous.DirectoryExists(virtualDir) instead of false
                // if you want to give the previously registered provider a chance to process the directory
                return false;
			}
		}

        public override VirtualFile GetFile (string virtualPath) {
            return new ZipVirtualFile (virtualPath, _zipFile);
        }

		public override VirtualDirectory GetDirectory (string virtualDir)
		{
			return new ZipVirtualDirectory (virtualDir, _zipFile);
		}

        public override string GetFileHash(string virtualPath, System.Collections.IEnumerable virtualPathDependencies)
        {
            return null;
        }

        public override System.Web.Caching.CacheDependency GetCacheDependency(String virtualPath, System.Collections.IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            return null;
        }
    }
}
