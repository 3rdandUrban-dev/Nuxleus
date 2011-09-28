using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Hosting;

using ICSharpCode.SharpZipLib.Zip;
using System.IO;

namespace Clarius.Samples.Web.VirtualPathProvider
{
    class ZipVirtualFile : VirtualFile
    {
        ZipFile _zipFile;

        public ZipVirtualFile (String virtualPath, ZipFile zipFile)
            : base(virtualPath) {
            _zipFile = zipFile;
        }

        public override System.IO.Stream Open () {
            ZipEntry entry = _zipFile.GetEntry(Util.ConvertVirtualPathToZipPath(base.VirtualPath,true));
            using (Stream st = _zipFile.GetInputStream(entry))
            {
                MemoryStream ms = new MemoryStream();
                ms.SetLength(entry.Size);
                byte[] buf = new byte[2048];
                while (true)
                {
                    int r = st.Read(buf, 0, 2048);
                    if (r == 0)
                        break;
                    ms.Write(buf, 0, r);
                }
                ms.Seek(0, SeekOrigin.Begin);
                return ms;
            }
        }
    }
}
