using ImageMagick;
using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using Tesseract;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace TinyClicker.Core.Services;

public class ImageToTextService : IImageToTextService
{
    private readonly TesseractEngine _tesseractEngine;

    public ImageToTextService(TesseractEngine tesseractEngine)
    {
        _tesseractEngine = tesseractEngine;
    }

    public int GetBalanceFromWindow(Image window)
    {
        using var sourceImage = GetAdjustedBalanceImage(window);
        using var page = _tesseractEngine.Process(sourceImage, PageSegMode.SingleLine);

        var result = page.GetText().Trim();

        window.Dispose();
        return StringToBalance(result);
    }

    private static int StringToBalance(string result)
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

        var parsed = int.TryParse(TrimWithRegex(result), out var value);
        return parsed ? value : -1;
    }

    private static string TrimWithRegex(string input)
    {
        return Regex.Replace(input, "[^0-9]", "").Trim();
    }

    private Bitmap GetAdjustedBalanceImage(Image gameWindow)
    {
        // resize image to default size
        var percentage = GetScreenDiffPercentageForBalance(gameWindow);
        using var imageOld = new MagickImage(ImageToBytes(gameWindow), MagickFormat.Png);

        imageOld.Resize(percentage.x, percentage.y);

        using var image = BytesToImage(imageOld.ToByteArray());

        //var filename = @"./screenshots/window.png";
        //SaveScreenshot(result, filename);

        return CropCurrentBalance(image);
    }

    private static Bitmap CropCurrentBalance(Image window)
    {
        // Crop the image
        var cropRectangle = new Rectangle(20, 541, 65, 20);
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

    private static (Percentage x, Percentage y) GetScreenDiffPercentageForBalance(Image? screenshot = null)
    {
        if (screenshot == null)
        {
            throw new ArgumentNullException(nameof(screenshot));
        }

        return (new Percentage(100 * 333 / (float)screenshot.Width), new Percentage(100 * 592 / (float)screenshot.Height));
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

        return result ?? throw new InvalidOperationException("Cannot convert samples to images");
    }

    public Image BytesToImage(byte[] bytes)
    {
        using var ms = new MemoryStream(bytes);
        return Image.FromStream(ms);
    }

    private static void SaveScreenshot(Image screenshot, string filename)
    {
        if (!Directory.Exists(Environment.CurrentDirectory + @"/screenshots"))
        {
            Directory.CreateDirectory(Environment.CurrentDirectory + @"/screenshots");
        }

        screenshot.Save(filename, ImageFormat.Png);
    }
}
