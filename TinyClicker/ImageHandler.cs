using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace TinyClicker
{
    internal class ImageHandler
    {
        public static Bitmap CropCurrentBalance(Image window)
        {
            // Crop the image

            Rectangle crop = new Rectangle(16, 602, 75, 20); // 16, 603, 70, 17
            var bitmap = new Bitmap(crop.Width, crop.Height);
            using (var gr = Graphics.FromImage(bitmap))
            {
                gr.DrawImage(window, new Rectangle(0, 0, bitmap.Width, bitmap.Height), crop, GraphicsUnit.Pixel);
            }
            
            // Invert the image

            //Bitmap bitmap = new Bitmap(bitmap);
            //bitmap.Dispose();

            for (int y = 0; (y <= (bitmap.Height - 1)); y++)
            {
                for (int x = 0; (x <= (bitmap.Width - 1)); x++)
                {
                    Color inv = bitmap.GetPixel(x, y);
                    inv = Color.FromArgb(255, (255 - inv.R), (255 - inv.G), (255 - inv.B));
                    bitmap.SetPixel(x, y, inv);
                }
            }
            bitmap = AdjustImage(bitmap);
            //bitmap.Save(Environment.CurrentDirectory + "\\screenshots\\cropped_balance.png", ImageFormat.Png);
            GC.Collect();
            return bitmap;
        }

        static Bitmap AdjustImage(Bitmap bitmap)
        {
            // Adjust brightness of the image to be more readable

            Bitmap adjustedImage = new Bitmap(bitmap);
            Bitmap originalImage = bitmap;

            float brightness = 1.0f; // no change in brightness
            float contrast = 4.0f; // twice the contrast
            float gamma = 1.0f; // no change in gamma
            float adjustedBrightness = brightness - 1.0f;

            // Create matrix that will brighten and contrast the image

            float[][] ptsArray ={
            new float[] {contrast, 0, 0, 0, 0}, // scale red
            new float[] {0, contrast, 0, 0, 0}, // scale green
            new float[] {0, 0, contrast, 0, 0}, // scale blue
            new float[] {0, 0, 0, 1.0f, 0}, // don't scale alpha
            new float[] {adjustedBrightness, adjustedBrightness, adjustedBrightness, 0, 1}};

            ImageAttributes imageAttributes = new ImageAttributes();
            imageAttributes.ClearColorMatrix();
            imageAttributes.SetColorMatrix(new ColorMatrix(ptsArray), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            imageAttributes.SetGamma(gamma, ColorAdjustType.Bitmap);
            Graphics g = Graphics.FromImage(adjustedImage);
            g.DrawImage(originalImage, new Rectangle(0, 0, adjustedImage.Width, adjustedImage.Height) , 0, 0, originalImage.Width, originalImage.Height, GraphicsUnit.Pixel, imageAttributes);
            GC.Collect();
            return adjustedImage;
        }
    }
}
