﻿using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using Point = OpenCvSharp.Point;

namespace TinyClicker.Core.Services;

public interface IOpenCvService
{
    bool TryFindFirstImageOnScreen(Image gameScreen, out (string ItemName, int Location) result);
    bool IsImageOnScreen(Enum image, Dictionary<string, Mat>? templates = null, Image? screenshot = null);
    bool TryFindOnScreen(Enum image, out Point location);
    Dictionary<string, Mat> MakeTemplatesFromSamples(Image screenshot);
}