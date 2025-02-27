using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace autoClicker
{
    public class WindowFinder
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);


        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        public static RECT? GetWindowPosition(string appName)
        {
            foreach (var handle in GetWindowHandles())
            {
                if (!IsWindowVisible(handle)) continue;

                string title = GetWindowTitle(handle);
                if (title == appName)
                {
                    if (GetWindowRect(handle, out RECT rect))
                    {
                        return rect;
                    }
                }
            }
            return null;
        }

        private static IEnumerable<IntPtr> GetWindowHandles()
        {
            List<IntPtr> handles = new List<IntPtr>();
            GCHandle gcHandle = GCHandle.Alloc(handles);

            try
            {
                EnumWindowsProc callback = (hWnd, lParam) =>
                {
                    GCHandle target = GCHandle.FromIntPtr(lParam);
                    if (target.Target is List<IntPtr> list) list.Add(hWnd);
                    return true;
                };

                EnumWindows(callback, GCHandle.ToIntPtr(gcHandle));
            }
            finally
            {
                gcHandle.Free();
            }

            return handles;
        }

        private static string GetWindowTitle(IntPtr handle)
        {
            int length = GetWindowTextLength(handle);
            if (length == 0) return string.Empty;

            StringBuilder builder = new StringBuilder(length + 1);
            GetWindowText(handle, builder, builder.Capacity);
            return builder.ToString();
        }

        public static Point SetWindowToMatchAppPosition(string appName)
        {
            var position = WindowPositionFinder.GetWindowPositionByTitle(appName);
            if (!position.HasValue)
            {
                return new Point(0, 0);
            }
            return new Point(position.Value.Left, position.Value.Top);


        }

        internal static ClickParameters CreateWindowParam(string appName)
        {
            WindowFinder.RECT? cords = WindowFinder.GetWindowPosition(appName);
            return new ClickParameters()
            {
                Start = new Point(cords.Value.Right - cords.Value.Left, cords.Value.Bottom - cords.Value.Top), //Witdth, Height of AppName
                End = null,
                Slide = false,
                WaitDurationTime = 0,
                WaitRNDDurationTime = 0,
                WaitAfterTime = 0,
                WaitRNDAfterTime = 0
            };
        }

        internal static void ForceAppWindowSize(string appName, Point? savedSize)
        {
            Point currentSize = CreateWindowParam(appName).Start.Value;
            if (currentSize.X == savedSize.Value.X && currentSize.Y == savedSize.Value.Y)
            {
                return;
            }
            SetWindowSize(appName, currentSize);
        }

        public static void SetWindowSize(string appTitle, Point size)
        {
            IntPtr hWnd = FindWindow(null, appTitle);
            if (hWnd == IntPtr.Zero)
            {
                throw new Exception("Nie znaleziono okna o podanym tytule.");
            }

            RECT rect;
            if (!GetWindowRect(hWnd, out rect))
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
            }

            if (!MoveWindow(hWnd, rect.Left, rect.Top, size.X, size.Y, true))
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        public static string GetActiveWindowTitle()
        {
            IntPtr handle = GetForegroundWindow();
            if (handle == IntPtr.Zero)
            {
                return string.Empty;
            }

            int length = GetWindowTextLength(handle);
            if (length == 0)
            {
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder(length + 1);
            GetWindowText(handle, builder, builder.Capacity);
            return builder.ToString();
        }
    }
}
