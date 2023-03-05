using ImageMagick;
using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using Tesseract;

namespace TinyClicker.Core.Services;

public class ImageService : IImageService
{
    private readonly TesseractEngine _tesseractEngine;
    public ImageService(TesseractEngine tesseractEngine)
    {
        _tesseractEngine = tesseractEngine;
    }

    public int ParseBalance(Image window)
    {
        var sourceImage = GetAdjustedBalanceImage(window);
        string result;
        try
        {
            using (var page = _tesseractEngine.Process(sourceImage, PageSegMode.SingleLine))
            {
                result = page.GetText().Trim();
                sourceImage.Dispose();

                return ResultToBalance(result);
            }
        }
        catch (Exception)
        {
            return -1;
        }
    }

    private int ResultToBalance(string result)
    {
        if (result.Contains('M'))
        {
            int endIndex = result.IndexOf('M');
            result = result[..endIndex];
            int dotIndex = result.IndexOf('.');
            if (dotIndex == 1)
            {
                result = TrimWithRegex(result);
                result += "000";
            }
            else if (dotIndex == 2)
            {
                result = TrimWithRegex(result);
                result += "0000";
            }
            else if (dotIndex == 3)
            {
                result = TrimWithRegex(result);
                result += "00000";
            }

            return Convert.ToInt32(result);
        }
        else if (result.Contains(' '))
        {
            int endIndex = result.IndexOf(' ');
            result = result[..endIndex];

            return Convert.ToInt32(TrimWithRegex(result));
        }
        else
        {
            return Convert.ToInt32(TrimWithRegex(result));
        }
    }

    private string TrimWithRegex(string str)
    {
        return Regex.Replace(str, "[^0-9]", "").Trim();
    }

    public Bitmap GetAdjustedBalanceImage(Image gameWindow)
    {
        // resize image to default size
        var percentage = GetScreenDiffPercentageForBalance(gameWindow);
        using (var imageOld = new MagickImage(ImageToBytes(gameWindow), MagickFormat.Png))
        {
            imageOld.Resize(percentage.x, percentage.y);
            imageOld.BrightnessContrast(new Percentage(-40), new Percentage(100));

            var image = BytesToImage(imageOld.ToByteArray());
            var result = CropCurrentBalance(image);

            //Uncomment to save the balance image for manual checking
            //string filename = @"./screenshots/balance.png";
            //WindowToImage.SaveScreenshot(result, filename);

            return result;
        }
    }

    private Bitmap CropCurrentBalance(Image window)
    {
        // Crop the image
        Rectangle crop = new Rectangle(20, 541, 70, 20);
        var bitmap = new Bitmap(crop.Width, crop.Height);
        using (var gr = Graphics.FromImage(bitmap))
        {
            gr.DrawImage(window, new Rectangle(0, 0, bitmap.Width, bitmap.Height), crop, GraphicsUnit.Pixel);
        }

        //Invert the image
        for (int y = 0; y <= bitmap.Height - 1; y++)
        {
            for (int x = 0; x <= bitmap.Width - 1; x++)
            {
                Color inv = bitmap.GetPixel(x, y);
                inv = Color.FromArgb(255, 255 - inv.R, 255 - inv.G, 255 - inv.B);
                bitmap.SetPixel(x, y, inv);
            }
        }

        return bitmap;
    }

    private (Percentage x, Percentage y) GetScreenDiffPercentageForBalance(Image? screenshot = null)
    {
        var x = new Percentage(100 * 333 / (float)screenshot.Width);
        var y = new Percentage(100 * 592 / (float)screenshot.Height);

        return (x, y);
    }

    public (Percentage x, Percentage y) GetScreenDiffPercentageForTemplates(Image? screenshot = null)
    {
        var x = new Percentage((float)screenshot.Width * 100 / 333);
        var y = new Percentage((float)screenshot.Height * 100 / 592);

        return (x, y);
    }

    public byte[] ImageToBytes(Image image)
    {
        var imageConverter = new ImageConverter();
        var result = (byte[])imageConverter.ConvertTo(image, typeof(byte[]));

        return result;
    }

    public Image BytesToImage(byte[] bytes)
    {
        using var ms = new MemoryStream(bytes);
        return Image.FromStream(ms);
    }
}
