using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace TinyClicker;

public static class InputSimulator
{
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
        WM_SETFOCUS = 0x0007
    }

    [DllImport("User32.dll")]
    public static extern int FindWindow(string strClassName, string strWindowName);

    [DllImport("User32.dll")]
    public static extern int FindWindowEx(int hWndParent, int hWndChildAfter, string strClassName, string strWindowName);

    [DllImport("User32.dll")]
    public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

    [DllImport("user32.dll")]
    private static extern int GetWindowRect(IntPtr hWnd, out Rectangle rect);

    [DllImport("User32.dll")]
    public static extern int PostMessageA(IntPtr hWnd, int Msg, int wParam, int lParam);

    public static Rectangle GetWindowRectangle(IntPtr hWnd)
    {
        GetWindowRect(hWnd, out Rectangle rect);
        return rect;
    }
}