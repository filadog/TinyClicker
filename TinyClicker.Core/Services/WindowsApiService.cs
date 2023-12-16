using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Vanara.PInvoke;

namespace TinyClicker.Core.Services;

public class WindowsApiService : IWindowsApiService
{
    private const string LD_PLAYER_PROCESS = "dnplayer";
    private const string BLUESTACKS_PROCESS = "HD-Player";
    private const string ERROR_MESSAGE = "Emulator window not found. Restart required";

    private readonly IConfigService _configService;

    private Process? _process;
    private nint _childHandle;
    private Rectangle _screenRect;

    public WindowsApiService(IConfigService configService)
    {
        _configService = configService;
    }

    public void SendClick(int location)
    {
        if (_childHandle == nint.Zero || _process == null)
        {
            throw new InvalidOperationException(ERROR_MESSAGE);
        }

        if (_configService.Config.IsBluestacks)
        {
            User32.SendMessage(_process.MainWindowHandle, User32.WindowMessage.WM_SETFOCUS);
            User32.PostMessage(_childHandle, User32.WindowMessage.WM_LBUTTONDOWN, 0x0000, location);
            User32.PostMessage(_childHandle, User32.WindowMessage.WM_LBUTTONUP, 0x0000, location);
            User32.SendMessage(_process.MainWindowHandle, User32.WindowMessage.WM_KILLFOCUS);
        }
        else
        {
            User32.PostMessage(_childHandle, User32.WindowMessage.WM_LBUTTONDOWN, 0x0001, location);
            Task.Delay(1).Wait();
            User32.PostMessage(_childHandle, User32.WindowMessage.WM_LBUTTONUP, 0x0001, location);
        }
    }

    public void SendClick(int x, int y)
    {
        SendClick(GetRelativeCoordinates(x, y));
    }

    public Image GetGameScreenshot()
    {
        if (_childHandle != default)
        {
            MakeScreenshot(_childHandle);
        }

        _process = GetEmulatorProcess();
        _childHandle = GetChildHandle(_process.ProcessName);

        User32.GetWindowRect(_childHandle, out var rect);
        var newRect = new Rectangle(rect.X, rect.Y, rect.right, rect.bottom);
        _screenRect = newRect;

        return MakeScreenshot(_childHandle);
    }

    public int MakeLParam(int x, int y)
    {
        // generate coordinates within the game screen
        return (y << 16) | (x & 0xFFFF);
    }

    private static Process GetEmulatorProcess()
    {
        var processes = new[] { BLUESTACKS_PROCESS, LD_PLAYER_PROCESS };
        var processlist = Process.GetProcesses();
        var process = processlist
            .Select(x => x)
            .FirstOrDefault(x => !string.IsNullOrEmpty(x.MainWindowTitle) && processes.Contains(x.ProcessName));

        return process ?? throw new InvalidOperationException(ERROR_MESSAGE);
    }

    // ReSharper disable once UnusedMember.Global
    public void SendEscapeButton()
    {
        if (_childHandle == nint.Zero || _process == null)
        {
            throw new InvalidOperationException(ERROR_MESSAGE);
        }

        if (_configService.Config.IsBluestacks)
        {
            User32.SendMessage(_process.MainWindowHandle, User32.WindowMessage.WM_SETFOCUS, (nint)0, 0);
            User32.PostMessage(_childHandle, User32.WindowMessage.WM_KEYDOWN, (nint)User32.VK.VK_ESCAPE);
        }
        else
        {
            User32.SendMessage(_childHandle, User32.WindowMessage.WM_KEYDOWN, User32.VK.VK_ESCAPE);
        }
    }

    private static nint GetChildHandle(string processName)
    {
        var childProcesses = WindowHandleInfo.GetChildrenHandles(processName);
        if (childProcesses.Any())
        {
            return childProcesses[0];
        }

        throw new InvalidOperationException(ERROR_MESSAGE);
    }

    private int GetRelativeCoordinates(int x, int y)
    {
        var rectX = Math.Abs(_screenRect.Width - _screenRect.Left);
        var rectY = Math.Abs(_screenRect.Height - _screenRect.Top);

        var x1 = (float)x * 100 / 333 / 100;
        var y1 = (float)y * 100 / 592 / 100;

        var x2 = (int)(rectX * x1);
        var y2 = (int)(rectY * y1);

        return MakeLParam(x2, y2);
    }

    private static Image MakeScreenshot(nint handle)
    {
        var hdcSrc = User32.GetWindowDC(handle);

        User32.GetWindowRect(handle, out var windowRect);

        var width = windowRect.right - windowRect.left;
        var height = windowRect.bottom - windowRect.top;

        var hdcDest = Gdi32.CreateCompatibleDC(hdcSrc);
        var hBitmap = Gdi32.CreateCompatibleBitmap(hdcSrc, width, height);
        var hOld = Gdi32.SelectObject(hdcDest, hBitmap);

        Gdi32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, Gdi32.RasterOperationMode.SRCCOPY);
        Gdi32.SelectObject(hdcDest, hOld);
        Gdi32.DeleteDC(hdcDest);
        User32.ReleaseDC(handle, hdcSrc);

        var image = Image.FromHbitmap(hBitmap.DangerousGetHandle());
        Gdi32.DeleteObject(hBitmap);

        return image;
    }
}
