using System;
using System.Drawing;
using System.IO;

namespace TinyClicker.Tests.ImageProcessing;

internal static class TestHelper
{
    public static Image LoadBalanceSample(string imageName)
    {
        string fileName = $@".\Screenshots\{imageName}.png";
        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException($"Could not find the image at: {fileName}");
        }

        return Image.FromFile(fileName);
    }

    public static Image LoadGameScreenshot(string screenshotName)
    {
        string fileName = $@".\Screenshots\{screenshotName}.png";
        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException();
        }

        return Image.FromFile(fileName);
    }
}
