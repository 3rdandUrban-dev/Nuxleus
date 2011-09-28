using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Hosting;

using ICSharpCode.SharpZipLib.Zip;

namespace Clarius.Samples.Web.VirtualPathProvider
{
    class ZipVirtualDirectory : VirtualDirectory
    {
        ZipFile _zipFile;

        public ZipVirtualDirectory (String virtualDir, ZipFile file)
            : base (virtualDir) {
            _zipFile = file;
        }

        public override System.Collections.IEnumerable Children
        {
            get {
                return new ZipVirtualPathCollection (base.VirtualPath, VirtualPathType.All, _zipFile);
            }
        }

        public override System.Collections.IEnumerable Directories
        {
            get {
                return new ZipVirtualPathCollection (base.VirtualPath, VirtualPathType.Directories, _zipFile);
            }
        }

        public override System.Collections.IEnumerable Files
        {
            get {
                return new ZipVirtualPathCollection (base.VirtualPath, VirtualPathType.Files, _zipFile);
            }
        }
    }
}
