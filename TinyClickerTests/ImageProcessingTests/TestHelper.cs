using System;
using System.Drawing;
using System.IO;

namespace TinyClickerTests;

internal static class TestHelper
{
    public static Image LoadBalanceSample(string imageName)
    {
        string fileName = $@".\samples\Tests\BalanceImageSamples\{imageName}.png";
        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException($"Could not find the image at: {fileName}");
        }
        return Image.FromFile(fileName);
    }
}
