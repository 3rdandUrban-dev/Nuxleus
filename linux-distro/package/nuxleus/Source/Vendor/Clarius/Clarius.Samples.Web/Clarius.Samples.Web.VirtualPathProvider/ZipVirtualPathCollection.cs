using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using ICSharpCode.SharpZipLib.Zip;

namespace Clarius.Samples.Web.VirtualPathProvider
{
    class ZipVirtualPathCollection : MarshalByRefObject, IEnumerable
    {
        ZipFile _zipFile;
        ArrayList _paths;
        String _virtualPath;
        VirtualPathType _requestType;

        public ZipVirtualPathCollection (String virtualPath, VirtualPathType requestType, ZipFile zipFile) {
            _paths = new ArrayList();
            _virtualPath = virtualPath;
            _requestType = requestType;
            _zipFile = zipFile;

			PerformEnumeration ();
		}

		private void PerformEnumeration ()
		{
			String zipPath = Util.ConvertVirtualPathToZipPath (_virtualPath, false);

            if (zipPath[zipPath.Length - 1] != '/') {
                ZipEntry entry = _zipFile.GetEntry (zipPath);
                if (entry != null)
					_paths.Add (new ZipVirtualFile (zipPath, _zipFile));
                return;
            }
            else {
                foreach (ZipEntry entry in _zipFile) {
                    Console.WriteLine (entry.Name);
                    if (entry.Name == zipPath)
                        continue;
                    if (entry.Name.StartsWith (zipPath)) {
                        // if we're looking for files and current entry is a directory, skip it
						if (_requestType == VirtualPathType.Files && entry.IsDirectory)
                            continue;
                        // if we're looking for directories and current entry its not one, skip it
						if (_requestType == VirtualPathType.Directories && !entry.IsDirectory)
                            continue;

                        int pos = entry.Name.IndexOf ('/', zipPath.Length);
                        if (pos != -1) {
                            if (entry.Name.Length > pos + 1)
                                continue;
                        }
                        //    continue;
                        if (entry.IsDirectory)
							_paths.Add (new ZipVirtualDirectory (Util.ConvertZipPathToVirtualPath (entry.Name), _zipFile));
                        else
							_paths.Add (new ZipVirtualFile (Util.ConvertZipPathToVirtualPath (entry.Name), _zipFile));
                    }
                }
            }
        }

        public override object InitializeLifetimeService () {
            return null;
        }

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator () {
            return _paths.GetEnumerator ();
        }

        #endregion
    }
}
