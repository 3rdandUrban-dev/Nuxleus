using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Nuxleus.Utility.S3;
using Nuxleus.Utility.Format;

namespace Nuxleus.IO
{
    public partial class GlobalClip
    {
        private AWSAuthConnection _Connect;
        private string _BaseHost;
        private string _StorageBase;
        private string _FilePrefix;
        private string _SessionId;
        private string _DateHash;
        private string _KeyPrefix;

        private ClipboardCollection<ClipItem> _ClipCopy;
        private ClipboardCollection<ClipItem> _ClipPaste;

        private string _GC_PUBLIC_KEY;
        private string _GC_PRIVATE_KEY;

        public enum Mode { AspNet, Console, WinApp, Dynamic };

        private int _Mode = (int)Mode.AspNet;

        private int AppMode
        {
            get
            {
                return this._Mode;
            }
            set
            {
                this._Mode = value;
            }
        }

        private Base64FormatProvider provider = new Base64FormatProvider();

        private Stream _Output;
        private ListBucketResponse _List;

    }
}
