using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace autoClicker
{
    public class Mouse
    {

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool ClientToScreen(IntPtr hWnd, ref System.Drawing.Point lpPoint);


        // Stałe
        private const uint WM_LBUTTONDOWN = 0x0201;
        private const uint WM_LBUTTONUP = 0x0202;
        private const int MK_LBUTTON = 0x0001;

        // Struktura do przechowywania pozycji kursora
        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }
        string AppName;
        public Mouse(string appName)
        {
            this.AppName = appName;
        }
        public Point? GetMousePosition()
        {
            IntPtr hWnd = WindowFinder.FindWindow(null, AppName);
            if (hWnd == IntPtr.Zero)
            {
                MessageBox.Show("Nie znaleziono okna.");
                return null;
            }

            POINT cursorPos;
            if (!GetCursorPos(out cursorPos))
            {
                MessageBox.Show("Błąd odczytu pozycji kursora.");
                return null;
            }

            // Konwersja do współrzędnych klienta
            if (!ScreenToClient(hWnd, ref cursorPos))
            {
                MessageBox.Show("Błąd konwersji współrzędnych.");
                return null;
            }

            return new Point(cursorPos.X, cursorPos.Y);
        }

        public Point? GetNonRelativeMousePosition()
        {
            POINT cursorPos;
            if (!GetCursorPos(out cursorPos))
            {
                MessageBox.Show("Błąd odczytu pozycji kursora.");
                return null;
            }

            return new Point(cursorPos.X, cursorPos.Y);
        }

        public bool SimulateClick(Point? clientPos)
        {
            if(clientPos == null) {
                return false;
            }
            IntPtr hWnd = WindowFinder.FindWindow(null, AppName);
            if (hWnd == IntPtr.Zero)
            {
                MessageBox.Show("Nie znaleziono okna.");
                return false;
            }

            // Przygotowanie parametrów współrzędnych
            int lParam = (clientPos.Value.Y << 16) | (clientPos.Value.X & 0xFFFF);

            // Wysłanie wiadomości do okna
            SendMessage(hWnd, WM_LBUTTONDOWN, (IntPtr)MK_LBUTTON, (IntPtr)lParam);
            SendMessage(hWnd, WM_LBUTTONUP, IntPtr.Zero, (IntPtr)lParam);

            return true;
        }

        public void SimulateClickDown(Point? clientPos, IntPtr hWnd)
        {
            int lParam = (clientPos.Value.Y << 16) | (clientPos.Value.X & 0xFFFF);
            SendMessage(hWnd, WM_LBUTTONDOWN, (IntPtr)MK_LBUTTON, (IntPtr)lParam);
        }


        public void SimulateClickUp(Point? clientPos, IntPtr hWnd)
        {
            int lParam = (clientPos.Value.Y << 16) | (clientPos.Value.X & 0xFFFF);
            SendMessage(hWnd, WM_LBUTTONUP, IntPtr.Zero, (IntPtr)lParam);
        }


        public bool IsLpmPressed()
        {
            return (GetAsyncKeyState(0x01) & 0x8000) != 0;
        }

        public void MoveMouse(ref System.Drawing.Point point)
        {
            IntPtr hWnd = WindowFinder.FindWindow(null, AppName);
            if (hWnd == IntPtr.Zero)
            {
                throw new Exception("Nie znaleziono okna o podanej nazwie.");
            }

            if (!SetForegroundWindow(hWnd))
            {
                throw new Exception("Uwaga: Nie udało się aktywować okna.");
            }

            if (!ClientToScreen(hWnd, ref point))
            {
                throw new Exception("Błąd konwersji współrzędnych.");
            }

            if (!SetCursorPos(point.X, point.Y))
            {
                throw new Exception("Nie udało się ustawić pozycji kursora.");
            }
        }

        public static void MoveNonRelativeMouse(System.Drawing.Point point)
        {
            if (!SetCursorPos(point.X, point.Y))
            {
                throw new Exception("Nie udało się ustawić pozycji kursora.");
            }
        }
    }
}
