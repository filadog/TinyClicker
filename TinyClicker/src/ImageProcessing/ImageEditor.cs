using OpenCvSharp;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime;

namespace TinyClicker;

internal class ImageEditor
{
    Rectangle _screenRect;
    Rectangle _balanceRect;
    readonly ClickerActionsRepo _actionsRepo;
    bool _isBalanceLocationFound;

    public ImageEditor(Rectangle screenRect, ClickerActionsRepo actionsRepo)
    {
        _screenRect = screenRect;
        _actionsRepo = actionsRepo;
        _isBalanceLocationFound = false;
    }

    public Bitmap GetBalanceImageAdjusted(Image window)
    {
        if (!_isBalanceLocationFound)
        {
            _balanceRect = GetBalanceRect(_screenRect);
        }
        Bitmap result = AdjustImage(CropCurrentBalance(window));
        // Save the result for manual checking
        //string filename = Environment.CurrentDirectory + @"/screenshots/balance.png";
        //ScreenshotManager.SaveScreenshot(result, filename);

        return result;
    }

    public Bitmap CropCurrentBalance(Image window)
    {
        // Crop the image
        var cropRect = _balanceRect;
        var bitmap = new Bitmap(cropRect.Width, cropRect.Height);
        using (var gr = Graphics.FromImage(bitmap))
        {
            gr.DrawImage(window, new Rectangle(0, 0, bitmap.Width, bitmap.Height), cropRect, GraphicsUnit.Pixel);
        }
        
        // Invert the image
        for (int y = 0; y <= (bitmap.Height - 1); y++)
        {
            for (int x = 0; x <= (bitmap.Width - 1); x++)
            {
                Color inv = bitmap.GetPixel(x, y);
                inv = Color.FromArgb(255, (255 - inv.R), (255 - inv.G), (255 - inv.B));
                bitmap.SetPixel(x, y, inv);
            }
        }

        bitmap = AdjustImage(bitmap);

        return bitmap;
    }

    Bitmap AdjustImage(Bitmap bitmap)
    {
        // Adjust the brightness of the image to be more readable for Tesseract
        Bitmap adjustedImage = new Bitmap(bitmap);
        Bitmap originalImage = bitmap;

        float brightness = 1.0f;
        float contrast = 4.0f;
        float gamma = 1.0f;
        float adjustedBrightness = brightness - 1.0f;

        // Create a matrix that will brighten and change the contrast of the image
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

        return adjustedImage;
    }

    Rectangle GetBalanceRect(Rectangle screenRect)
    {
        int posX = 0;
        int posY = 0;

        // Check for balance coordinates on the screen
        if (!_isBalanceLocationFound)
        {
            bool found = _actionsRepo.IsImageFound("balanceCoin", out OpenCvSharp.Point location);
            posX = location.X + 14;
            posY = location.Y - 2;
            if (found)
            {
                _isBalanceLocationFound = true;
            }
        }

        // Adjust coordinates to the actual window size
        int rectX = Math.Abs(_screenRect.Width - _screenRect.Left);
        int rectY = Math.Abs(_screenRect.Height - _screenRect.Top);
        float x1 = ((float)posX * 100 / 333) / 100;
        float y1 = ((float)posY * 100 / 592) / 100;
        int x2 = (int)(rectX * x1);
        int y2 = (int)(rectY * y1);

        return new Rectangle(x2, y2, 74, 25);
    }
}
