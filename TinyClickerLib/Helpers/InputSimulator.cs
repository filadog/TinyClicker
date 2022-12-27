using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace TinyClicker;

public class InputSimulator
{
    private readonly Logger _logger;
    private ScreenScanner _screenScanner;
    private readonly WindowToImage _windowToImage;

    private const string _ldPlayerProcName = "dnplayer";
    private const string _blueStacksProcName = "HD-Player";

    private Process? _process;
    private int _processId;
    private IntPtr _childHandle;
    private string _curProcName;
    Rectangle _screenRect;

    public InputSimulator(WindowToImage windowToImage, Logger logger)
    {
        _logger = logger;
        _windowToImage = windowToImage;
    }

    public void Init(ScreenScanner screenScanner)
    {
        _screenScanner = screenScanner;
        _process = GetEmulatorProcess();
        _processId = _process.Id;
        _childHandle = GetChildHandle();
        _screenRect = GetWindowRectangle();
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
        //string curProcName = _screenScanner._isBluestacks ? _blueStacksProcName : _ldPlayerProcName;
        var processes = new string[] { "HD-Player", "dnplayer" };

        var processlist = Process.GetProcesses();
        var process = processlist.Select(x => x).Where(x => !string.IsNullOrEmpty(x.MainWindowTitle) && processes.Contains(x.ProcessName)).SingleOrDefault();

        if (process is null)
        {
            throw new Exception("Emulator process not found");
        }

        _curProcName = process.ProcessName;

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
            if (_screenScanner._isBluestacks)
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
        SendClick(GetRelativeCoords(x, y));
    }

    public void SendEscapeButton()
    {
        if (_childHandle != IntPtr.Zero)
        {
            if (_screenScanner._isBluestacks)
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

    public IntPtr GetChildHandle()
    {
        if (WindowHandleInfo.GetChildrenHandles(_curProcName) != null)
        {
            List<IntPtr> childProcesses = WindowHandleInfo.GetChildrenHandles(_curProcName);
            if (childProcesses != null)
            {
                return childProcesses[0];
            }
        }
        _logger.Log("Emulator process not found - TinyClicker function is not possible. Launch emulator and restart the app.");
        throw new Exception("Emulator child handle not found");
    }

    public int GetRelativeCoords(int x, int y)
    {
        int rectX = Math.Abs(_screenRect.Width - _screenRect.Left);
        int rectY = Math.Abs(_screenRect.Height - _screenRect.Top);
        float x1 = ((float)x * 100 / 333) / 100;
        float y1 = ((float)y * 100 / 592) / 100;

        int x2 = (int)(rectX * x1);
        int y2 = (int)(rectY * y1);
        return MakeLParam(x2, y2);
    }

    public Image MakeScreenshot()
    {
        if (_processId != -1)
        {
            IntPtr handle = Process.GetProcessById(_processId).MainWindowHandle;
            Image img = _windowToImage.CaptureWindow(handle);
            return img;
        }
        else
        {
            throw new Exception("No emulator process found");
        }
    }

    public void SaveScreenshot()
    {
        if (_processId != -1)
        {
            if (!Directory.Exists($"./screenshots"))
            {
                Directory.CreateDirectory($"./screenshots");
            }

            IntPtr handle = Process.GetProcessById(_processId).MainWindowHandle;
            // Captures screenshot of a window and saves it to the screenshots folder
            _windowToImage.CaptureWindowToFile(handle, $"./screenshots/window.png", ImageFormat.Png);
            _logger.Log($"Made a screenshot. Screenshots can be found inside TinyClicker/screenshots folder");
        }
    }

    public int MakeLParam(int x, int y) => (y << 16) | (x & 0xFFFF); // Generate coordinates within the game screen
}