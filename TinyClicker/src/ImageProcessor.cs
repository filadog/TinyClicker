using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace TinyClickerUI
{
    internal class ImageProcessor
    {
        public static Bitmap FetchBalanceImageAdjusted(Image window)
        {
            Bitmap result = AdjustImage(CropCurrentBalance(window));

            // Save the result for manual checking
            //string filename = Environment.CurrentDirectory + @"\screenshots\balance.png";
            //ScreenshotManager.SaveScreenshot(result, filename);

            return result;
        }

        public static Bitmap CropCurrentBalance(Image window)
        {
            // Crops the image
            Rectangle crop = new Rectangle(16, 602, 75, 20);
            var bitmap = new Bitmap(crop.Width, crop.Height);
            using (var gr = Graphics.FromImage(bitmap))
            {
                gr.DrawImage(window, new Rectangle(0, 0, bitmap.Width, bitmap.Height), crop, GraphicsUnit.Pixel);
            }
            
            // Inverts the image
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
            GC.Collect();

            return bitmap;
        }

        static Bitmap AdjustImage(Bitmap bitmap)
        {
            // Adjusts the brightness of the image to be more readable for Tesseract
            Bitmap adjustedImage = new Bitmap(bitmap);
            Bitmap originalImage = bitmap;

            float brightness = 1.0f;
            float contrast = 4.0f;
            float gamma = 1.0f;
            float adjustedBrightness = brightness - 1.0f;

            // Creates a matrix that will brighten and contrast the image
            float[][] ptsArray = {
            new float[] {contrast, 0, 0, 0, 0}, // Red
            new float[] {0, contrast, 0, 0, 0}, // Green
            new float[] {0, 0, contrast, 0, 0}, // Blue
            new float[] {0, 0, 0, 1.0f, 0},     // Alpha
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
