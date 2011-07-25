using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections;
using System.IO;

namespace Nuxleus.Drawing.Utility
{

    public enum ResizeType
    {
        HeightProportionalToWidth,
        WidthProportionalToHeight,
        HeightProportionalToWidth_HeightCropped,
        HeightProportionalToWidth_WidthCropped,
        WidthProportionalToHeight_HeightCropped,
        WidthProportionalToHeight_WidthCropped
    }

    public partial class ConvertImage
    {
        public class JpegImageResize
        {

            Width_XYOffset[] m_imageSizeProportionalToWidth_XYOffsetArray;
            Dictionary<string, Size_XYOffset> m_imageSize;
            Dictionary<string, MemoryStream> m_imageResultDictionary;
            Image m_originalImage;
            int m_originalWidth;
            int m_originalHeight;

            public JpegImageResize (Stream imageStream)
            {
                m_originalImage = Image.FromStream (imageStream);
                m_originalImage.InitializeLifetimeService ();
                m_originalWidth = m_originalImage.Width;
                m_originalHeight = m_originalImage.Height;
                m_imageSize = new Dictionary<string, Size_XYOffset> ();
                m_imageResultDictionary = new Dictionary<string, MemoryStream> ();
                m_imageSize ["100"] = validateAndReturnImageSize_XYOffset (100, 100, 0, 0, m_originalWidth, m_originalHeight);
                m_imageSize ["320"] = validateAndReturnImageSize_XYOffset (320, 240, 0, 0, m_originalWidth, m_originalHeight);
                m_imageSizeProportionalToWidth_XYOffsetArray = new Width_XYOffset[] { 
                new Width_XYOffset(150, 0, 0),
                new Width_XYOffset(200, 0, 0),
                new Width_XYOffset(500, 0, 0),
            };
                try {
                    IEnumerator imageWidthEnumerator = m_imageSizeProportionalToWidth_XYOffsetArray.GetEnumerator ();
                    while (imageWidthEnumerator.MoveNext()) {
                        AddImageSizeProportionalToWidth_XYOffset ((Width_XYOffset)imageWidthEnumerator.Current);
                    }

                } catch (Exception e) {
                    Console.WriteLine (e.Message);
                }
            }

            private static Size_XYOffset validateAndReturnImageSize_XYOffset (int width, int height, int xOffset, int yOffset, int originalWidth, int originalHeight)
            {
                if (originalWidth > width && originalHeight > height) {
                    return new Size_XYOffset (width, height, xOffset, xOffset);
                } else {
                    return new Size_XYOffset (originalWidth, originalHeight, xOffset, xOffset);
                }
            }

            public Image OriginalImage { get { return m_originalImage; } set { m_originalImage = value; } }

            public Dictionary<string, MemoryStream> ResultDictionary { get { return m_imageResultDictionary; } set { m_imageResultDictionary = value; } }

            public Dictionary<string, MemoryStream> InvokeProcess ()
            {
                Console.WriteLine ("Invoking Process");
                return GenerateProportionallyReducedImages ();
            }

            private Dictionary<string, MemoryStream> GenerateProportionallyReducedImages ()
            {
                try {
                    IEnumerator imageSizeEnumerator = m_imageSize.GetEnumerator ();
                    Console.WriteLine ("Image Size Offset: {0}", m_imageSize.Count);
                    using (m_originalImage) {
                        while (imageSizeEnumerator.MoveNext()) {
                            KeyValuePair<String, Size_XYOffset > keyValuePair = (KeyValuePair<String, Size_XYOffset>)imageSizeEnumerator.Current;
                            Size_XYOffset newImageSize = keyValuePair.Value;
                            Console.WriteLine ("Processing Image Width: {0}, Height: {1}", newImageSize.Size.Width, newImageSize.Size.Height);
                            Image reducedImage = GenerateImage (newImageSize);
                            MemoryStream memoryStream = new MemoryStream ();
                            reducedImage.Save (memoryStream, ImageFormat.Jpeg);
                            m_imageResultDictionary.Add (keyValuePair.Key, memoryStream);
                        }
                    }
                } catch (Exception e) {
                    Console.WriteLine (e.Message);
                }
                Console.WriteLine ("Image Result Dictionary has {0} entries.", m_imageResultDictionary.Count);
                return this.ResultDictionary;
            }

            private Image GenerateImage (Size_XYOffset size)
            {
                Image reducedImage = new Bitmap (m_originalImage, size.Size);
                try {
                    using (Graphics graphic = Graphics.FromImage(reducedImage)) {
                        graphic.CompositingQuality = CompositingQuality.HighQuality;
                        graphic.SmoothingMode = SmoothingMode.AntiAlias;
                        graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        graphic.DrawImage (reducedImage, new Rectangle (size.XOffset, size.YOffset, size.Size.Width, size.Size.Height));
                    }

                } catch (Exception e) {
                    Console.WriteLine (e.Message);
                }
                return reducedImage;
            }

            public void AddImageSizeProportionalToWidth_XYOffset (Width_XYOffset widthXYOffset)
            {
                m_imageSize.Add (widthXYOffset.Width.ToString (), getImageSizeProportionalToWidth (widthXYOffset.Width, widthXYOffset.XOffset, widthXYOffset.YOffset));
            }

            private Size_XYOffset getImageSizeProportionalToWidth (int width, int x, int y)
            {

                decimal height = (decimal)m_originalHeight;
                decimal n_width = (decimal)width;
                decimal originalWidth = (decimal)m_originalWidth;
                decimal originalHeight = (decimal)m_originalHeight;
                decimal factor = Decimal.Divide (originalWidth, width);

                if (n_width < originalWidth) {
                    height = Math.Floor (Decimal.Divide (originalHeight, factor));
                } else {
                    n_width = originalWidth;
                    height = originalHeight;
                }
                return new Size_XYOffset ((int)n_width, (int)height, x, y);
            }
        }

        public struct Width_XYOffset
        {

            int m_x;
            int m_y;
            int m_width;

            public Width_XYOffset (int width)
            : this(width, 0, 0)
            {
            }

            public Width_XYOffset (int width, int x, int y)
            {
                m_width = width;
                m_x = x;
                m_y = y;
            }

            public int Width { get { return m_width; } set { m_width = value; } }

            public int XOffset { get { return m_x; } set { m_x = value; } }

            public int YOffset { get { return m_y; } set { m_y = value; } }

        }

        public struct Size_XYOffset
        {

            int m_x;
            int m_y;
            Size m_size;

            public Size_XYOffset (int width, int height, int x, int y)
            : this(new Size(width, height), x, y)
            {
            }

            public Size_XYOffset (Size size, int x, int y)
            {
                m_size = size;
                m_x = x;
                m_y = y;
            }

            public Size Size { get { return m_size; } set { m_size = value; } }

            public int XOffset { get { return m_x; } set { m_x = value; } }

            public int YOffset { get { return m_y; } set { m_y = value; } }

        }
    }
}
