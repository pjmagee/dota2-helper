using System;
using System.Runtime.InteropServices;
using Avalonia.Controls;

namespace Dota2Helper.ViewModels;

internal static class WindowExtensions
{
    const int GWL_STYLE = -16, GWL_EXSTYLE = -20;
    const int WS_MAXIMIZEBOX = 0x10000, WS_MINIMIZEBOX = 0x20000, WS_SYSMENU = 0x80000;
    const int WS_EX_DLGMODALFRAME = 0x0001;

    [DllImport("user32.dll")]
    static extern int GetWindowLong(IntPtr hwnd, int index);

    [DllImport("user32.dll")]
    static extern int SetWindowLong(IntPtr hwnd, int index, int value);

    [DllImport("user32.dll")]
    static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    const uint SWP_NOSIZE = 0x0001;
    const uint SWP_NOMOVE = 0x0002;
    const uint SWP_NOZORDER = 0x0004;
    const uint SWP_FRAMECHANGED = 0x0020;

    internal static void HideMinimizeAndMaximizeButtons(this Window window)
    {
        var platformHandle = window.TryGetPlatformHandle();
        if (platformHandle is not null)
        {
            var hwnd = platformHandle.Handle;
            var currentStyle = GetWindowLong(hwnd, GWL_STYLE);
            var currentExStyle = GetWindowLong(hwnd, GWL_EXSTYLE);

            SetWindowLong(hwnd, GWL_STYLE, (currentStyle & ~WS_MAXIMIZEBOX & ~WS_MINIMIZEBOX));
            SetWindowLong(hwnd, GWL_EXSTYLE, (currentExStyle | WS_EX_DLGMODALFRAME));

            SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOZORDER | SWP_FRAMECHANGED);
        }
    }
}