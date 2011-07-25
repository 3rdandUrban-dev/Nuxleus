using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Nuxleus.Drawing.Utility
{
    public partial class ConvertImage
    {

        public static byte[] Crop (byte[] imageFile, int targetW, int targetH, int targetX, int targetY)
        {

            MemoryStream memoryStream = new MemoryStream ();
            using (Image imagePhoto = Image.FromStream(new MemoryStream(imageFile))) {
                using (Bitmap bitmapPhoto = new Bitmap(targetW, targetH, PixelFormat.Format24bppRgb)) {
                    bitmapPhoto.SetResolution (72, 72);
                    using (Graphics photo = Graphics.FromImage(bitmapPhoto)) {
                        photo.SmoothingMode = SmoothingMode.AntiAlias;
                        photo.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        photo.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        photo.DrawImage (imagePhoto, new Rectangle (0, 0, targetW, targetH), targetX, targetY, targetW, targetH, GraphicsUnit.Pixel);
                    }
                    bitmapPhoto.Save (memoryStream, ImageFormat.Jpeg);
                }
            }
            return memoryStream.GetBuffer ();
        }
    }
}
