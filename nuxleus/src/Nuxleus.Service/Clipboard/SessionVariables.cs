using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Extf.Net.IO;
using com.amazon.s3;

namespace X5 {
    public partial class GlobalClip {
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

        private int AppMode {
            get {
                return this._Mode;
            }
            set {
                this._Mode = value;
            }
        }

        private Formatter_Base64 provider = new Formatter_Base64();

        private Stream _Output;
        private ListBucketResponse _List;

    }
}
