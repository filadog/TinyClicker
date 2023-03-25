using System;
using System.Collections.Generic;
using System.Drawing;
using OpenCvSharp;

namespace TinyClicker.Core.Services;

public interface IOpenCvService
{
    Dictionary<string, int> TryFindFirstOnScreen(Image gameScreen);
    bool FindOnScreen(Enum image, Dictionary<string, Mat>? templates = null, Image? screenshot = null);
    bool FindOnScreen(Enum image, out OpenCvSharp.Point location);
    Dictionary<string, Mat> MakeTemplates(Image screenshot);
}