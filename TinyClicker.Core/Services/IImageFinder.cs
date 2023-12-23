using System;
using System.Drawing;
using Point = OpenCvSharp.Point;

namespace TinyClicker.Core.Services;

public interface IImageFinder
{
    bool TryFindFirstImageOnScreen(Bitmap gameScreen, out (string ItemName, int Location) result);
    bool IsImageOnScreen(Enum image, Bitmap? gameScreen = null);
    bool TryFindOnScreen(Enum image, out Point location, Bitmap? gameScreen = null);
}
