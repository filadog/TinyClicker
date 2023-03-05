using ImageMagick;
using System.Drawing;

namespace TinyClicker.Core.Services;

public interface IImageService
{
    Image BytesToImage(byte[] bytes);
    Bitmap GetAdjustedBalanceImage(Image gameWindow);
    (Percentage x, Percentage y) GetScreenDiffPercentageForTemplates(Image? screenshot = null);
    byte[] ImageToBytes(Image image);
    int ParseBalance(Image window);
}