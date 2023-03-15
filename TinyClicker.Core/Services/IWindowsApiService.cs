using System.Drawing;

namespace TinyClicker.Core.Services;

public interface IWindowsApiService
{
    int MakeLParam(int x, int y);
    Image MakeScreenshot();
    void SendClick(int location);
    void SendClick(int x, int y);
    void SendEscapeButton();
}