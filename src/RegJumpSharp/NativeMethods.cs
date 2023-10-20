// RegJumpSharp | A C# remake of RegJump by SysInternals | Copyright (c) 2023 Vurdalakov | MIT License | https://github.com/vurdalakov/regjumpsharp

namespace Vurdalakov.RegJumpSharp
{
    using System;
    using System.Runtime.InteropServices;

    internal static class NativeMethods
    {
        public const UInt32 SW_SHOWMINIMIZED = 2;
        public const UInt32 SW_RESTORE = 9;

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPLACEMENT
        {
            public Int32 length;
            public UInt32 flags;
            public UInt32 showCmd;
            public Int32 ptMinPositionX; // POINT
            public Int32 ptMinPositionY;
            public Int32 ptMaxPositionX; // POINT
            public Int32 ptMaxPositionY;
            public Int32 rcNormalPositionLeft; // RECT
            public Int32 rcNormalPositionTop;
            public Int32 rcNormalPositionRight;
            public Int32 rcNormalPositionBottom;
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern Boolean GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern Boolean SetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        public static IntPtr HWND_TOP = new IntPtr(0);
        public static UInt32 SWP_NOMOVE = 0x0002;
        public static UInt32 SWP_NOSIZE = 0x0001;
        public static UInt32 SWP_SHOWWINDOW = 0x0040;
        public static UInt32 SWP_NOACTIVATE = 0x0010;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern Boolean SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, Int32 X, Int32 Y, Int32 cx, Int32 cy, UInt32 uFlags);

        [DllImport("user32")]
        public static extern Boolean SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, String lpszClass, String lpszWindow);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern Boolean SetWindowText(IntPtr hwnd, String lpString);

        public const UInt32 VK_RETURN = 0x0D;

        public const UInt32 WM_SETFOCUS = 0x0007;
        public const UInt32 WM_SETTEXT = 0x000C;
        public const UInt32 WM_KEYDOWN = 0x100;
        public const UInt32 WM_KEYUP = 0x101;

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "SendMessageA")]
        public static extern Int32 SendMessageInt32(IntPtr hWnd, UInt32 Msg, UInt32 wParam, Int32 lParam);

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "SendMessageA")]
        public static extern Int32 SendMessageString(IntPtr hWnd, UInt32 Msg, UInt32 wParam, String lParam);

        public const UInt32 CF_TEXT = 1;
        public const UInt32 CF_UNICODETEXT = 13;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern Boolean OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern Boolean CloseClipboard();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetClipboardData(UInt32 uFormat);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern Boolean IsClipboardFormatAvailable(UInt32 format);
    }
}
