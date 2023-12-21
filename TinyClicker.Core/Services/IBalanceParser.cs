using System.Drawing;
using ImageMagick;

namespace TinyClicker.Core.Services;

public interface IBalanceParser
{
    Image BytesToImage(byte[] bytes);
    (Percentage x, Percentage y) GetScreenDiffPercentageForTemplates(Image? screenshot = null);
    byte[] ImageToBytes(Image image);
    int GetBalanceFromWindow(Image window);
}
