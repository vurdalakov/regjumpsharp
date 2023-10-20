// RegJumpSharp | A C# remake of RegJump by SysInternals | Copyright (c) 2023 Vurdalakov | MIT License | https://github.com/vurdalakov/regjumpsharp

namespace Vurdalakov.RegJumpSharp
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Principal;

    internal static class NativeHelpers
    {
        public static Boolean ActivateWindow(IntPtr windowHandle)
        {
            var windowPlacement = new NativeMethods.WINDOWPLACEMENT();
            windowPlacement.length = Marshal.SizeOf(windowPlacement);
            if (NativeMethods.GetWindowPlacement(windowHandle, ref windowPlacement) && (NativeMethods.SW_SHOWMINIMIZED == windowPlacement.showCmd))
            {
                windowPlacement.showCmd = NativeMethods.SW_RESTORE;
                NativeMethods.SetWindowPlacement(windowHandle, ref windowPlacement);
            }

            NativeMethods.SetWindowPos(windowHandle, NativeMethods.HWND_TOP, 0, 0, 0, 0, NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_SHOWWINDOW | NativeMethods.SWP_NOACTIVATE);

            return NativeMethods.SetForegroundWindow(windowHandle) || (NativeMethods.SetActiveWindow(windowHandle) != IntPtr.Zero);
        }

        public static Boolean TryGetClipboardText(out String text)
        {
            text = null;

            if (!NativeMethods.OpenClipboard(IntPtr.Zero))
            {
                return false;
            }

            try
            {
                if (TryGetClipbardData(NativeMethods.CF_UNICODETEXT, out var data))
                {
                    text = Marshal.PtrToStringUni(data);
                    return true;
                }

                if (TryGetClipbardData(NativeMethods.CF_TEXT, out data))
                {
                    text = Marshal.PtrToStringAnsi(data);
                    return true;
                }

                return false;
            }
            finally
            {
                NativeMethods.CloseClipboard();
            }

            Boolean TryGetClipbardData(UInt32 clipboardFormat, out IntPtr clipboardData)
            {
                if (!NativeMethods.IsClipboardFormatAvailable(clipboardFormat))
                {
                    clipboardData = IntPtr.Zero;
                    return false;
                }
                else
                {
                    clipboardData = NativeMethods.GetClipboardData(clipboardFormat);
                    return clipboardData != IntPtr.Zero;
                }
            }
        }

        public static Boolean IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
