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

    public int GetBalanceFromWindow(Image window)
    {
        var sourceImage = GetAdjustedBalanceImage(window);
        using (var page = _tesseractEngine.Process(sourceImage, PageSegMode.SingleLine))
        {
            var result = page.GetText().Trim();
            sourceImage.Dispose();

            return StringToBalance(result);
        }
    }

    private int StringToBalance(string result)
    {
        if (result.Contains('M'))
        {
            var endIndex = result.IndexOf('M');
            result = result[..endIndex];

            var dotIndex = result.IndexOf('.');
            var zeroes = new string('0', dotIndex + 2);

            result = TrimWithRegex(result);
            result += zeroes;
        }
        else if (result.Contains(' '))
        {
            var endIndex = result.IndexOf(' ');
            result = result[..endIndex];
        }

        var success = int.TryParse(TrimWithRegex(result), out int value);
        return success ? value : -1;
    }

    private string TrimWithRegex(string input)
    {
        return Regex.Replace(input, "[^0-9]", "").Trim();
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
        var cropRectangle = new Rectangle(20, 541, 70, 20);
        var bitmap = new Bitmap(cropRectangle.Width, cropRectangle.Height);
        using (var gr = Graphics.FromImage(bitmap))
        {
            gr.DrawImage(window, new Rectangle(0, 0, bitmap.Width, bitmap.Height), cropRectangle, GraphicsUnit.Pixel);
        }

        //Invert the image
        for (int y = 0; y <= bitmap.Height - 1; y++)
        {
            for (int x = 0; x <= bitmap.Width - 1; x++)
            {
                var color = bitmap.GetPixel(x, y);
                color = Color.FromArgb(255, 255 - color.R, 255 - color.G, 255 - color.B);
                bitmap.SetPixel(x, y, color);
            }
        }

        return bitmap;
    }

    private (Percentage x, Percentage y) GetScreenDiffPercentageForBalance(Image? screenshot = null)
    {
        if (screenshot == null)
        {
            throw new ArgumentNullException(nameof(screenshot));
        }

        var x = new Percentage(100 * 333 / (float)screenshot.Width);
        var y = new Percentage(100 * 592 / (float)screenshot.Height);

        return (x, y);
    }

    public (Percentage x, Percentage y) GetScreenDiffPercentageForTemplates(Image? screenshot = null)
    {
        if (screenshot == null)
        {
            throw new ArgumentNullException(nameof(screenshot));
        }

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
