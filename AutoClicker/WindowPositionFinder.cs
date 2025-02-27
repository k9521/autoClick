using System;
using System.Runtime.InteropServices;
using System.Text;

namespace autoClicker
{
    public class WindowPositionFinder
    {
        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public override string ToString() =>
                $"X: {Left}, Y: {Top}, Szerokość: {Right - Left}, Wysokość: {Bottom - Top}";
        }

        public static RECT? GetWindowPositionByTitle(string windowTitle)
        {
            IntPtr foundWindow = IntPtr.Zero;

            EnumWindowsProc callback = (hWnd, lParam) =>
            {
                if (!IsWindowVisible(hWnd))
                    return true;

                int textLength = GetWindowTextLength(hWnd);
                if (textLength == 0)
                    return true;

                StringBuilder windowText = new StringBuilder(textLength + 1);
                GetWindowText(hWnd, windowText, windowText.Capacity);

                if (windowText.ToString() == windowTitle)
                {
                    foundWindow = hWnd;
                    return false; // Przerywa enumerację
                }

                return true;
            };

            EnumWindows(callback, IntPtr.Zero);

            if (foundWindow != IntPtr.Zero && GetWindowRect(foundWindow, out RECT rect))
            {
                return rect;
            }

            return null;
        }
    }
}
