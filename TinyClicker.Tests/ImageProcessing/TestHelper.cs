using System.Drawing;
using System.IO;

namespace TinyClicker.Tests.ImageProcessing;

internal static class TestHelper
{
    public static Bitmap LoadGameScreenshot(string screenshotName)
    {
        var fileName = $@".\Screenshots\{screenshotName}.png";
        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException();
        }

        return new Bitmap(Image.FromFile(fileName));
    }
}
