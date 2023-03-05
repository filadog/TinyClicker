using System.Drawing;
using System.Drawing.Imaging;
using TinyClicker.Core.Logic;

namespace TinyClicker.Core.Services;

public interface IWindowsApiService
{
    Image CaptureScreen();
    Image CaptureWindow(nint handle);
    void CaptureWindowToFile(nint handle, string filename, ImageFormat format);
    nint GetChildHandle(string processName);
    int GetRelativeCoordinates(int x, int y);
    Rectangle GetWindowRectangle();
    int MakeLParam(int x, int y);
    Image MakeScreenshot();
    void SendClick(int location);
    void SendClick(int x, int y);
    void SendEscapeButton();
}