using ImageMagick;
using System.Drawing;

namespace TinyClicker.Core.Services;

public interface IImageToTextService
{
    Image BytesToImage(byte[] bytes);
    (Percentage x, Percentage y) GetScreenDiffPercentageForTemplates(Image? screenshot = null);
    byte[] ImageToBytes(Image image);
    int GetBalanceFromWindow(Image window);
}