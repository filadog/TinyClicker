using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TinyTowerComputerVisionConsole
{
    public class MouseEvents
    {
        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        public const int WM_LBUTTONDOWN = 0x201;
        public const int WM_LBUTTONUP = 0x202;
        public const int WM_LBUTTONDBLCLK = 0x203;
        public const int WM_RBUTTONDOWN = 0x204;
        public const int WM_RBUTTONUP = 0x205;
        public const int WM_RBUTTONDBLCLK = 0x206;

        //PostMessage(hWnd, WM_LBUTTONDOWN, 1, 0);
        //PostMessage(hWnd, WM_LBUTTONUP, 0, 0);

        [DllImport("User32.dll")]
        public static extern Int32 SendMessage(
        int hWnd,
        int Msg,
        int wParam,
        IntPtr lParam);


        //PostMessage(hWnd, WM_LBUTTONDOWN, 1, MakeLParam(pt.X, pt.Y));
    }

}

