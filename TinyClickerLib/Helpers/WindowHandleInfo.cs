﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TinyClicker;

public class WindowHandleInfo
{
    private delegate bool EnumWindowProc(IntPtr hwnd, IntPtr lParam);

    [DllImport("user32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback, IntPtr lParam);

    private IntPtr _mainHandle;

    public WindowHandleInfo(IntPtr handle)
    {
        _mainHandle = handle;
    }

    public List<IntPtr> GetAllChildHandles()
    {
        List<IntPtr> childHandles = new List<IntPtr>();

        GCHandle gcChildhandlesList = GCHandle.Alloc(childHandles);
        IntPtr pointerChildHandlesList = GCHandle.ToIntPtr(gcChildhandlesList);

        try
        {
            EnumWindowProc childProc = new EnumWindowProc(EnumWindow);
            EnumChildWindows(_mainHandle, childProc, pointerChildHandlesList);
        }
        finally
        {
            gcChildhandlesList.Free();
        }

        return childHandles;
    }

    private bool EnumWindow(IntPtr hWnd, IntPtr lParam)
    {
        GCHandle gcChildhandlesList = GCHandle.FromIntPtr(lParam);

        if (gcChildhandlesList.Target == null)
        {
            return false;
        }

        List<IntPtr> childHandles = gcChildhandlesList.Target as List<IntPtr>;
        if (childHandles is null)
        {
            throw new InvalidOperationException("Child handles list is null");
        }

        childHandles.Add(hWnd);
        return true;
    }

    [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
    public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

    public static List<IntPtr>? GetChildrenHandles(string processName)
    {
        Process[] processes = Process.GetProcessesByName(processName);
        if (processes[0] != null)
        {
            var allChildWindows = new WindowHandleInfo(processes[0].MainWindowHandle).GetAllChildHandles();
            return allChildWindows;
        }
        else
        {
            throw new Exception($"There is no process with {processName} name");
        }
    }
}
