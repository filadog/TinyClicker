using System;
using System.Collections.Generic;
using System.Drawing;
using OpenCvSharp;

namespace TinyClicker.Core.Services;

public interface IOpenCvService
{
    Dictionary<string, int> TryFindFirstOnScreen(Image gameScreen);
    (string Key, int Location) TryFindSingle(KeyValuePair<string, Mat> template, Mat reference);
    bool IsImageFound(Enum image, Dictionary<string, Mat>? templates = null, Image? screenshot = null);
    bool IsImageFound(Enum image, out OpenCvSharp.Point location);
    Dictionary<string, Mat> MakeTemplates(Image screenshot);
}