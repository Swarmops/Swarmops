using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swarmops.Logic.Support
{
    public class DocumentImage
    {
        public DocumentImage(Document parent)
        {
            this.Parent = parent;
        }

        private Image GetImage()
        {
            Image image = null;
            using (StreamReader docReader = Parent.GetReader())
            {
                image = Image.FromStream(docReader.BaseStream);
            }

            return image;
        }

        private static Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            double ratio = (double)maxWidth / image.Width;

            if (maxHeight > 0)
            {
                double ratioY = (double) maxHeight/image.Height;
                if (ratioY < ratio)
                {
                    ratio = ratioY;
                }
            }

            int newWidth = (int) (image.Width*ratio);
            int newHeight = (int) (image.Height*ratio);

            Image newImage = new Bitmap(newWidth, newHeight);

            using (Graphics g = Graphics.FromImage(newImage))
            {
                g.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return newImage;
            
        }

        public string GetBase64(int width, int minHeight = 0)
        {
            string cacheKey = String.Format("DocumentImage-Base64-{0}-{1}-{2}", Parent.Identity, width, minHeight);

            // Do we have the requested item in cache?

            string base64 = (string) Cache.GuidCache.Get(cacheKey);
            if (!string.IsNullOrEmpty(base64))
            {
                return base64;
            }

            // No, so construct it

            using (Image image = ScaleImage(GetImage(), width, minHeight))
            {
                MemoryStream stream = new MemoryStream();
                image.Save(stream, ImageFormat.Png);
                base64 = Convert.ToBase64String(stream.ToArray());
            }

            Cache.GuidCache.Set(cacheKey, base64);
            return base64;
        }

        public Document Parent { get; private set; }
    }
}
