using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using ImageMagick;
using Tesseract;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace TinyClicker.Core.Services;

public class BalanceParser : IBalanceParser
{
    private readonly TesseractEngine _tesseractEngine;
    private readonly Rectangle _cropRectangle = new(20, 541, 65, 20);

    public BalanceParser(TesseractEngine tesseractEngine)
    {
        _tesseractEngine = tesseractEngine;
    }

    public int GetBalanceFromWindow(Image window)
    {
        using var balanceImage = CropBalanceImage(window);
        using var balancePage = _tesseractEngine.Process(balanceImage, PageSegMode.SingleLine);

        //var filename = @"./screenshots/croppedWindow.png";
        //SaveDebugScreenshot(balanceImage, filename);

        var balance = balancePage.GetText().Trim();

        return ParseBalanceFromString(balance);
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
        var result = imageConverter.ConvertTo(image, typeof(byte[])) as byte[];

        return result ?? throw new InvalidOperationException("Cannot convert samples to images");
    }

    public Image BytesToImage(byte[] bytes)
    {
        using var ms = new MemoryStream(bytes);
        return Image.FromStream(ms);
    }

    private static int ParseBalanceFromString(string result)
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

        return int.TryParse(TrimWithRegex(result), out var value) ? value : -1;
    }

    private static string TrimWithRegex(string input)
    {
        return Regex.Replace(input, "[^0-9]", "").Trim();
    }

    private Bitmap CropBalanceImage(Image gameWindow)
    {
        // Resize image to default size
        var percentage = GetScreenDiffPercentageForBalance(gameWindow);
        using var imageOld = new MagickImage(ImageToBytes(gameWindow), MagickFormat.Png);

        imageOld.Resize(percentage.x, percentage.y);
        imageOld.WhiteThreshold(new Percentage(50));
        imageOld.Threshold(new Percentage(60));

        using var image = BytesToImage(imageOld.ToByteArray());

        //var filename = @"./screenshots/window.png";
        //SaveDebugScreenshot(image, filename);

        return CropCurrentBalance(image);
    }

    private Bitmap CropCurrentBalance(Image window)
    {
        // crop the image
        var bitmap = new Bitmap(_cropRectangle.Width, _cropRectangle.Height);
        using (var graphics = Graphics.FromImage(bitmap))
        {
            graphics.DrawImage(window, new Rectangle(0, 0, bitmap.Width, bitmap.Height), _cropRectangle, GraphicsUnit.Pixel);
        }

        // invert the image
        for (var y = 0; y <= bitmap.Height - 1; y++)
        {
            for (var x = 0; x <= bitmap.Width - 1; x++)
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

    // ReSharper disable once UnusedMember.Local
    private static void SaveDebugScreenshot(Image screenshot, string filename)
    {
        if (!Directory.Exists(Environment.CurrentDirectory + "/screenshots"))
        {
            Directory.CreateDirectory(Environment.CurrentDirectory + "/screenshots");
        }

        screenshot.Save(filename, ImageFormat.Png);
    }
}
