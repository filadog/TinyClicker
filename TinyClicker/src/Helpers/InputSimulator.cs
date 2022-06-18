using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace TinyClicker;

// Simulates mouse actions

public static class InputSimulator
{
    public const int WM_LBUTTON = 0x01;
    public const int WM_RBUTTON = 0x02;
    public const int WM_KEYDOWN = 0x100;
    public const int WM_KEYUP = 0x101;
    public const int WM_COMMAND = 0x111;
    public const int WM_LBUTTONDOWN = 0x201;
    public const int WM_LBUTTONUP = 0x202;
    public const int WM_LBUTTONDBLCLK = 0x203;
    public const int WM_RBUTTONDOWN = 0x204;
    public const int WM_RBUTTONUP = 0x205;
    public const int WM_RBUTTONDBLCLK = 0x206;
    public const int VK_ESCAPE = 0x1B;
    public const int WM_MOUSEACTIVATE = 0x0021;
    public const int WM_NCHITTEST = 0x0084;
    public const int WM_MOUSEMOVE = 0x0200;
    public const int WM_SETCURSOR = 0x0020;
    public const int WM_ACTIVATE = 0x0006;
    public const int WM_NCACTIVATE = 0x0086;
    public const int WM_CAPTURECHANGED = 0x0215;
    //public const int WM_IME_NOTIFY;

    [DllImport("User32.dll")]
    public static extern int FindWindow(string strClassName, string strWindowName);

    [DllImport("User32.dll")]
    public static extern int FindWindowEx(int hwndParent, int hwndChildAfter, string strClassName, string strWindowName);

    [DllImport("User32.dll")]
    public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

    [DllImport("user32.dll")]
    private static extern int GetWindowRect(IntPtr hwnd, out Rectangle rect);

    public static Rectangle GetWindowRectangle(IntPtr hwnd)
    {
        var rectangle = new Rectangle();
        GetWindowRect(hwnd, out rectangle);
        return rectangle;
    }
}