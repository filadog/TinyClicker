﻿using System.Drawing;

namespace TinyClicker.Core.Services;

public interface IWindowsApiService
{
    int MakeLParam(int x, int y);
    Bitmap GetGameScreenshot();
    void SendClick(int location);
    void SendClick(int x, int y);
}
