using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace Swarmops.Logic.ExtensionMethods
{
    public static partial class DummyContainer {

        public static Bitmap AdjustColor (this Bitmap sourceBitmap, double redTint, double greenTint, double blueTint)
        {
            BitmapData sourceData = sourceBitmap.LockBits (new Rectangle (0, 0,
                sourceBitmap.Width, sourceBitmap.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);


            byte[] pixelBuffer = new byte[sourceData.Stride*sourceData.Height];


            Marshal.Copy (sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);


            sourceBitmap.UnlockBits (sourceData);


            double blue = 0;
            double green = 0;
            double red = 0;


            for (int k = 0; k + 4 < pixelBuffer.Length; k += 4)
            {
                blue = pixelBuffer[k] + (255 - pixelBuffer[k])*blueTint;
                green = pixelBuffer[k + 1] + (255 - pixelBuffer[k + 1])*greenTint;
                red = pixelBuffer[k + 2] + (255 - pixelBuffer[k + 2])*redTint;


                if (blue > 255)
                {
                    blue = 255;
                }


                if (green > 255)
                {
                    green = 255;
                }


                if (red > 255)
                {
                    red = 255;
                }


                pixelBuffer[k] = (byte) blue;
                pixelBuffer[k + 1] = (byte) green;
                pixelBuffer[k + 2] = (byte) red;


            }


            Bitmap resultBitmap = new Bitmap (sourceBitmap.Width, sourceBitmap.Height);


            BitmapData resultData = resultBitmap.LockBits (new Rectangle (0, 0,
                resultBitmap.Width, resultBitmap.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);


            Marshal.Copy (pixelBuffer, 0, resultData.Scan0, pixelBuffer.Length);
            resultBitmap.UnlockBits (resultData);


            return resultBitmap;
        }

        public static Bitmap AdjustSaturation(this Bitmap sourceBitmap, double saturation)
        {
            BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0,
                sourceBitmap.Width, sourceBitmap.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);

            sourceBitmap.UnlockBits(sourceData);

            double blue = 0;
            double green = 0;
            double red = 0;


            for (int k = 0; k + 4 < pixelBuffer.Length; k += 4)
            {
                double luminance = pixelBuffer[k + 2]*0.2126 + pixelBuffer[k + 1]*0.7152 + pixelBuffer[k]*0.0722;

                blue = pixelBuffer[k]*saturation + (1-saturation) * luminance;
                green = pixelBuffer[k + 1] * saturation + (1 - saturation) * luminance;
                red = pixelBuffer[k + 2] * saturation + (1 - saturation) * luminance;


                if (blue > 255)
                {
                    blue = 255;
                }


                if (green > 255)
                {
                    green = 255;
                }


                if (red > 255)
                {
                    red = 255;
                }


                pixelBuffer[k] = (byte)blue;
                pixelBuffer[k + 1] = (byte)green;
                pixelBuffer[k + 2] = (byte)red;


            }


            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);


            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0,
                resultBitmap.Width, resultBitmap.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);


            Marshal.Copy(pixelBuffer, 0, resultData.Scan0, pixelBuffer.Length);
            resultBitmap.UnlockBits(resultData);


            return resultBitmap;
        }


        public static Bitmap AdjustBrightness(this Bitmap sourceBitmap, double brightness)
        {
            BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0,
                sourceBitmap.Width, sourceBitmap.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);


            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];


            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);


            sourceBitmap.UnlockBits(sourceData);


            double blue = 0;
            double green = 0;
            double red = 0;


            for (int k = 0; k + 4 < pixelBuffer.Length; k += 4)
            {
                blue = pixelBuffer[k] * brightness;
                green = pixelBuffer[k + 1] * brightness;
                red = pixelBuffer[k + 2] * brightness;


                if (blue > 255)
                {
                    blue = 255;
                }


                if (green > 255)
                {
                    green = 255;
                }


                if (red > 255)
                {
                    red = 255;
                }


                pixelBuffer[k] = (byte)blue;
                pixelBuffer[k + 1] = (byte)green;
                pixelBuffer[k + 2] = (byte)red;


            }


            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);


            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0,
                resultBitmap.Width, resultBitmap.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);


            Marshal.Copy(pixelBuffer, 0, resultData.Scan0, pixelBuffer.Length);
            resultBitmap.UnlockBits(resultData);


            return resultBitmap;
        }


        public static Bitmap Blur(this Bitmap image, Int32 blurSize)
        {
            Bitmap blurred = new Bitmap(image.Width, image.Height);

            // make an exact copy of the bitmap provided
            using (Graphics graphics = Graphics.FromImage(blurred))
                graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                    new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);

            // look at every pixel in the image
            for (Int32 xx = 0; xx < image.Width; xx++)
            {
                for (Int32 yy = 0; yy < image.Height; yy++)
                {
                    Int32 avgR = 0, avgG = 0, avgB = 0, avgA = 0;
                    Int32 blurPixelCount = 0;

                    // average the color of the red, green and blue for each pixel in the
                    // blur size while making sure you don't go outside the image bounds
                    for (Int32 x = xx; (x < xx + blurSize && x < image.Width); x++)
                    {
                        for (Int32 y = yy; (y < yy + blurSize && y < image.Height); y++)
                        {
                            Color pixel = blurred.GetPixel(x, y);

                            avgR += pixel.R;
                            avgG += pixel.G;
                            avgB += pixel.B;
                            avgA += pixel.A;

                            blurPixelCount++;
                        }
                    }

                    avgR = avgR / blurPixelCount;
                    avgG = avgG / blurPixelCount;
                    avgB = avgB / blurPixelCount;
                    avgA = avgA / blurPixelCount;

                    // now that we know the average for the blur size, set each pixel to that color
                    for (Int32 x = xx; x < xx + blurSize && x < image.Width && x < image.Width; x++)
                        for (Int32 y = yy; y < yy + blurSize && y < image.Height && y < image.Height; y++)
                            blurred.SetPixel(x, y, Color.FromArgb(avgA, avgR, avgG, avgB));
                }
            }

            return blurred;
        }
    } 
}
