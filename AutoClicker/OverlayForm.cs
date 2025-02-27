using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace autoClicker
{
    public partial class OverlayForm : Form
    {
        string AppName;
        private Bitmap canvas;
        private bool showWarningOnce;
        Point appPosition;
        WindowFinder.RECT? cords;
        private readonly Pen drawingPointPen = new Pen(Color.Red, 2);
        private readonly Pen drawingMovePen = new Pen(Color.Green, 1);
        private readonly Pen drawingRNDClick = new Pen(Color.Blue, 2);
        private static readonly int SIZE_OF_CROSS = 5;
        public int Xcorrection { get; set; }
        public int Ycorrection { get; set; }

        public OverlayForm(string appName)
        {
            this.ShowInTaskbar = false;
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.DoubleBuffered = true;

            this.BackColor = Color.Magenta;
            this.TransparencyKey = Color.Magenta;

            this.AppName = appName;
            WindowFinder.RECT? cords = WindowFinder.GetWindowPosition(AppName);
            if (cords != null)
            {
                this.Top = 0;
                this.Left = 0;
                this.Width = cords.Value.Right - cords.Value.Left;
                this.Height = cords.Value.Bottom - cords.Value.Top;
            }
            InitializeCanvas();
            showWarningOnce = true;
        }

        private void InitializeCanvas()
        {
            pictureBox.Left = 0;
            pictureBox.Top = 0;
            pictureBox.BackColor = Color.Transparent;

            pictureBox.Width = ClientSize.Width;
            pictureBox.Height = ClientSize.Height;
            canvas = new Bitmap(ClientSize.Width, ClientSize.Height);
            using (var g = Graphics.FromImage(canvas))
            {
                g.Clear(Color.Transparent);
            }
            Invalidate();
            pictureBox.Image = canvas;
        }
        public void UpdateLocation()
        {
            Point loc = WindowFinder.SetWindowToMatchAppPosition(AppName);
            this.Top = loc.Y + Ycorrection;
            this.Left = loc.X + Xcorrection;
        }

        public void drawPoint(List<ClickParameters> listParameters)
        {
            if (!timer.Enabled) return;

            this.Clean();
            this.Show();
            UpdateLocation();
            if (listParameters.Count == 0)
            {
                return;
            }

            using (var g = Graphics.FromImage(canvas))
            {
                int n = 1;
                foreach (ClickParameters param in listParameters)
                {
                    drawClickParameters(param, g, n);
                    n++;
                }
            }
            pictureBox.Image = canvas;
            Refresh();
        }

        private void drawClickParameters(ClickParameters param, Graphics g, int n)
        {

            g.DrawLine(drawingMovePen, param.Start.Value.X, param.Start.Value.Y, param.End.Value.X, param.End.Value.Y);

            if (param.Start.Value.X == param.End.Value.X && param.Start.Value.Y == param.End.Value.Y)
            {
                g.DrawEllipse(drawingRNDClick, param.Start.Value.X - param.RNDPoint, param.Start.Value.Y - param.RNDPoint, param.RNDPoint * 2, param.RNDPoint * 2);
                g.DrawLine(drawingPointPen, param.Start.Value.X, param.Start.Value.Y - SIZE_OF_CROSS, param.Start.Value.X, param.Start.Value.Y + SIZE_OF_CROSS);
                g.DrawLine(drawingPointPen, param.Start.Value.X - SIZE_OF_CROSS, param.Start.Value.Y, param.Start.Value.X + SIZE_OF_CROSS, param.Start.Value.Y);

                g.DrawString(n.ToString() + "SE", new Font("Arial", 12), Brushes.Red, param.Start.Value.X, param.Start.Value.Y);
            }
            else
            {
                g.DrawEllipse(drawingRNDClick, param.Start.Value.X - param.RNDPoint, param.Start.Value.Y - param.RNDPoint, param.RNDPoint * 2, param.RNDPoint * 2);
                g.DrawLine(drawingPointPen, param.Start.Value.X, param.Start.Value.Y - SIZE_OF_CROSS, param.Start.Value.X, param.Start.Value.Y + SIZE_OF_CROSS);
                g.DrawLine(drawingPointPen, param.Start.Value.X - SIZE_OF_CROSS, param.Start.Value.Y, param.Start.Value.X + SIZE_OF_CROSS, param.Start.Value.Y);

                g.DrawEllipse(drawingRNDClick, param.End.Value.X - param.RNDPoint, param.End.Value.Y - param.RNDPoint, param.RNDPoint * 2, param.RNDPoint * 2);
                g.DrawLine(drawingPointPen, param.End.Value.X, param.End.Value.Y - SIZE_OF_CROSS, param.End.Value.X, param.End.Value.Y + SIZE_OF_CROSS);
                g.DrawLine(drawingPointPen, param.End.Value.X - SIZE_OF_CROSS, param.End.Value.Y, param.End.Value.X + SIZE_OF_CROSS, param.End.Value.Y);

                g.DrawString(n.ToString() + "S", new Font("Arial", 12), Brushes.Red, param.Start.Value.X, param.Start.Value.Y);
                g.DrawString(n.ToString() + "E", new Font("Arial", 12), Brushes.Red, param.End.Value.X, param.End.Value.Y);
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (WindowFinder.GetActiveWindowTitle() == AppName)
            {
                appPosition = WindowFinder.SetWindowToMatchAppPosition(AppName);
                cords = WindowFinder.GetWindowPosition(AppName);

                if (this.Top == appPosition.Y + Ycorrection &&
                    this.Left == appPosition.X + Xcorrection &&
                    this.Width == cords.Value.Right - cords.Value.Left &&
                    this.Height == cords.Value.Bottom - cords.Value.Top)
                {
                    this.Show();
                }
                else if ((this.Top != appPosition.Y + +Ycorrection ||
                    this.Left != appPosition.X + Xcorrection) &&
                    (this.Width == cords.Value.Right - cords.Value.Left &&
                    this.Height == cords.Value.Bottom - cords.Value.Top))
                {
                    this.Top = appPosition.Y + Ycorrection;
                    this.Left = appPosition.X + Xcorrection;
                    this.Show();
                }
                else if (this.Width != cords.Value.Right - cords.Value.Left ||
                    this.Height != cords.Value.Bottom - cords.Value.Top)
                {
                    if (showWarningOnce)
                    {
                        this.Hide();
                        showWarningOnce = false;
                        MessageBox.Show("Application was resized. You need to update a click points.");
                    }
                }
            }
            else
            {
                this.Hide();
            }

        }

        internal void Clean()
        {
            canvas = new Bitmap(ClientSize.Width, ClientSize.Height);
            using (var g = Graphics.FromImage(canvas))
            {
                g.Clear(Color.Transparent);
            }
            Invalidate();
            pictureBox.Image = canvas;
        }

        internal void resetComponent()
        {
            showWarningOnce = true;
            WindowFinder.RECT? cords = WindowFinder.GetWindowPosition(AppName);
            if (cords != null)
            {
                this.Top = 0;
                this.Left = 0;
                this.Width = cords.Value.Right - cords.Value.Left;
                this.Height = cords.Value.Bottom - cords.Value.Top;
            }
            InitializeCanvas();
        }

        public void changeTimer(bool enabled)
        {
            timer.Enabled = enabled;
            if (timer.Enabled)
            {
                this.Show();
            }
            else
            {
                this.Hide();
            }
        }
    }
}
