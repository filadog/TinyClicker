using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Vanara.PInvoke;

namespace TinyClicker.Core.Services;

public class WindowHandleInfo
{
    private readonly nint _mainHandle;

    private WindowHandleInfo(nint handle)
    {
        _mainHandle = handle;
    }

    private List<nint> GetAllChildHandles()
    {
        var childHandles = new List<nint>();

        var gcChildHandlesList = GCHandle.Alloc(childHandles);
        var pointerChildHandlesList = GCHandle.ToIntPtr(gcChildHandlesList);

        try
        {
            User32.EnumChildWindows(_mainHandle, EnumWindow, pointerChildHandlesList);
        }
        finally
        {
            gcChildHandlesList.Free();
        }

        return childHandles;
    }

    private static bool EnumWindow(HWND hWnd, nint lParam)
    {
        var gcChildHandlesList = GCHandle.FromIntPtr(lParam);

        if (gcChildHandlesList.Target == null)
        {
            return false;
        }

        if (gcChildHandlesList.Target is not List<nint> childHandles)
        {
            throw new InvalidOperationException("Child handles list is null");
        }

        childHandles.Add((nint)hWnd);
        return true;
    }

    public static List<nint> GetChildrenHandles(string processName)
    {
        var processes = Process.GetProcessesByName(processName);
        if (processes.Any())
        {
            return new WindowHandleInfo(processes[0].MainWindowHandle).GetAllChildHandles();
        }

        throw new InvalidOperationException($"There is no process with {processName} name");
    }
}
