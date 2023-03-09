using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TinyClicker.Core.Logging;
using System.Linq;

namespace TinyClicker.Core.Services;

public class WindowsApiService : IWindowsApiService
{
    private readonly ILogger _logger;
    private readonly ConfigService _configService;

    private const string _ldPlayerProcName = "dnplayer";
    private const string _blueStacksProcName = "HD-Player";

    private Process? _process;
    private IntPtr _childHandle;
    Rectangle _screenRect;

    public WindowsApiService(ConfigService configService, ILogger logger)
    {
        _configService = configService;
        _logger = logger;
    }

    public enum KeyCodes
    {
        WM_LBUTTON = 0x01,
        WM_RBUTTON = 0x02,
        WM_KEYDOWN = 0x100,
        WM_KEYUP = 0x101,
        WM_COMMAND = 0x111,
        WM_LBUTTONDOWN = 0x201,
        WM_LBUTTONUP = 0x202,
        WM_LBUTTONDBLCLK = 0x203,
        WM_RBUTTONDOWN = 0x204,
        WM_RBUTTONUP = 0x205,
        WM_RBUTTONDBLCLK = 0x206,
        VK_ESCAPE = 0x1B,
        WM_MOUSEACTIVATE = 0x0021,
        WM_NCHITTEST = 0x0084,
        WM_MOUSEMOVE = 0x0200,
        WM_SETCURSOR = 0x0020,
        WM_ACTIVATE = 0x0006,
        WM_NCACTIVATE = 0x0086,
        WM_CAPTURECHANGED = 0x0215,
        WM_SETFOCUS = 0x0007,
        WM_KILLFOCUS = 0x0008
    }

    [DllImport("User32.dll")]
    private static extern int FindWindow(string strClassName, string strWindowName);

    [DllImport("User32.dll")]
    private static extern int FindWindowEx(int hWndParent, int hWndChildAfter, string strClassName, string strWindowName);

    [DllImport("User32.dll")]
    private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

    [DllImport("user32.dll")]
    private static extern int GetWindowRect(IntPtr hWnd, out Rectangle rect);

    [DllImport("User32.dll")]
    private static extern int PostMessageA(IntPtr hWnd, int Msg, int wParam, int lParam);

    private static Rectangle GetWindowRectangle(IntPtr hWnd)
    {
        GetWindowRect(hWnd, out Rectangle rect);
        return rect;
    }

    private Process GetEmulatorProcess()
    {
        var processes = new string[] { _blueStacksProcName, _ldPlayerProcName };
        var processlist = Process.GetProcesses();
        var process = processlist.Select(x => x).Where(x => !string.IsNullOrEmpty(x.MainWindowTitle) && processes.Contains(x.ProcessName)).FirstOrDefault();

        if (process == null)
        {
            throw new InvalidOperationException("Emulator process not found");
        }

        return process;
    }

    public Rectangle GetWindowRectangle()
    {
        return GetWindowRectangle(_childHandle);
    }

    public void SendClick(int location)
    {
        if (_childHandle != IntPtr.Zero)
        {
            if (_configService.Config.IsBluestacks)
            {
                // Bluestacks input simulation
                SendMessage(_process.MainWindowHandle, (int)KeyCodes.WM_SETFOCUS, 0, 0);
                PostMessageA(_childHandle, (int)KeyCodes.WM_LBUTTONDOWN, 0x0001, location);
                PostMessageA(_childHandle, (int)KeyCodes.WM_LBUTTONUP, 0x0001, location);
                SendMessage(_process.MainWindowHandle, (int)KeyCodes.WM_KILLFOCUS, 0, 0);
            }
            else
            {
                // LDPlayer input simulation
                SendMessage(_childHandle, (int)KeyCodes.WM_LBUTTONDOWN, 1, location);
                Task.Delay(1).Wait();
                SendMessage(_childHandle, (int)KeyCodes.WM_LBUTTONUP, 0, location);
            }
        }
    }

    public void SendClick(int x, int y)
    {
        SendClick(GetRelativeCoordinates(x, y));
    }

    public void SendEscapeButton()
    {
        if (_childHandle != IntPtr.Zero)
        {
            if (_configService.Config.IsBluestacks)
            {
                // Bluestacks input 
                SendMessage(_process.MainWindowHandle, (int)KeyCodes.WM_SETFOCUS, 0, 0);
                PostMessageA(_childHandle, (int)KeyCodes.WM_KEYDOWN, (int)KeyCodes.VK_ESCAPE, 0);
            }
            else
            {
                // LDPlayer input 
                SendMessage(_childHandle, (int)KeyCodes.WM_KEYDOWN, (int)KeyCodes.VK_ESCAPE, 0);
            }
        }
    }

    public IntPtr GetChildHandle(string processName)
    {
        if (WindowHandleInfo.GetChildrenHandles(processName) != null)
        {
            List<IntPtr> childProcesses = WindowHandleInfo.GetChildrenHandles(processName);
            if (childProcesses != null && childProcesses.Any())
            {
                return childProcesses[0];
            }
        }

        _logger.Log("Emulator process not found - TinyClicker function is not possible. Launch emulator and restart the app.");
        throw new InvalidOperationException("Emulator child handle not found");
    }

    public int GetRelativeCoordinates(int x, int y)
    {
        int rectX = Math.Abs(_screenRect.Width - _screenRect.Left);
        int rectY = Math.Abs(_screenRect.Height - _screenRect.Top);

        float x1 = (float)x * 100 / 333 / 100;
        float y1 = (float)y * 100 / 592 / 100;

        int x2 = (int)(rectX * x1);
        int y2 = (int)(rectY * y1);

        return MakeLParam(x2, y2);
    }

    public Image MakeScreenshot()
    {
        if (_childHandle == default)
        {
            _process = GetEmulatorProcess();
            _childHandle = GetChildHandle(_process.ProcessName);
            _screenRect = GetWindowRectangle();
        }

        var image = CaptureWindow(_childHandle);
        return image;
    }

    public int MakeLParam(int x, int y) => y << 16 | x & 0xFFFF; // Generate coordinates within the game screen

    public Image CaptureScreen()
    {
        return CaptureWindow(User32.GetDesktopWindow());
    }

    public static void SaveScreenshot(Image screenshot, string filename)
    {
        if (!Directory.Exists(Environment.CurrentDirectory + @"/screenshots"))
        {
            Directory.CreateDirectory(Environment.CurrentDirectory + @"/screenshots");
        }
        screenshot.Save(filename, ImageFormat.Png);
    }

    // Creates an Image object containing a screenshot of a specific window
    public Image CaptureWindow(IntPtr handle)
    {
        IntPtr hdcSrc = User32.GetWindowDC(handle);

        User32.RECT windowRect = new User32.RECT();
        User32.GetWindowRect(handle, ref windowRect);

        int width = windowRect.right - windowRect.left;
        int height = windowRect.bottom - windowRect.top;

        IntPtr hdcDest = GDI32.CreateCompatibleDC(hdcSrc);
        IntPtr hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, width, height);
        IntPtr hOld = GDI32.SelectObject(hdcDest, hBitmap);

        GDI32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, GDI32.SRCCOPY);
        GDI32.SelectObject(hdcDest, hOld);
        GDI32.DeleteDC(hdcDest);
        User32.ReleaseDC(handle, hdcSrc);

        Image img = Image.FromHbitmap(hBitmap);
        GDI32.DeleteObject(hBitmap);

        return img;
    }

    // Captures a screenshot of a window and saves it to a file
    public void CaptureWindowToFile(IntPtr handle, string filename, ImageFormat format)
    {
        Image img = CaptureWindow(handle);
        img.Save(filename, format);
    }

    private class GDI32
    {
        public const int SRCCOPY = 0x00CC0020;

        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(
            IntPtr hObject,
            int nXDest,
            int nYDest,
            int nWidth,
            int nHeight,
            IntPtr hObjectSource,
            int nXSrc,
            int nYSrc,
            int dwRop);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(
            IntPtr hDC,
            int nWidth,
            int nHeight);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteDC(IntPtr hDC);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
    }

    // Helper class containing User32 API functions
    private class User32
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);
    }
}