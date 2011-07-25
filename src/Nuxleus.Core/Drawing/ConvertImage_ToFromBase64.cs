using System;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace Nuxleus.Core
{
    public partial class ConvertImage
    {
        public static string BitmapToBase64 (Bitmap bitmap)
        {
            Byte[] data = null;
            using (MemoryStream memorystream = new MemoryStream()) {
                bitmap.Save (memorystream, ImageFormat.Png);
                data = memorystream.GetBuffer ();
            }
            return Convert.ToBase64String (data, Base64FormattingOptions.InsertLineBreaks);
        }

        public static Bitmap Base64ToBitmap (string base64String)
        {
            Bitmap bitmap = null;
            Byte[] data = Convert.FromBase64String (base64String);
            using (MemoryStream memorystream = new MemoryStream(data)) {
                bitmap = new Bitmap (memorystream);
            }
            return bitmap;
        }
    }
}

