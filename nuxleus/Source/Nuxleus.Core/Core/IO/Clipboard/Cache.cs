using System;

namespace Nuxleus.IO
{
    public partial class GlobalClip
    {
        public ClipboardCollection<ClipItem> ClipCopy
        {
            get
            {
                _ClipCopy = new ClipboardCollection<ClipItem>();
                return this._ClipCopy;
            }
        }

        public ClipboardCollection<ClipItem> ClipPaste
        {
            get
            {
                _ClipPaste = new ClipboardCollection<ClipItem>();
                return this._ClipPaste;
            }
        }
    }
}