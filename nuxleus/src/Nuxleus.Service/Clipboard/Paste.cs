using System;
using com.amazon.s3;
using System.Xml;


namespace X5 {
    public partial class GlobalClip {
        public bool Paste () {
            ClipItem item = this.ClipCopy.Pop();
            string pasteKey = KeyPrefix + "-paste" + item.GetHashCode().ToString(provider);

            S3Object oItem = new S3Object(item.Data, item.MetaData);

            try {
                this.Clipboard().put(StorageBase, pasteKey, oItem, null);
                this.ClipPaste.Push(item);
                return true;
            } catch (Exception e) {
                throw;
            }
        }

    }
}