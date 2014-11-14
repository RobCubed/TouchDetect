using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MouseKeyboardActivityMonitor.WinApi;
using MouseKeyboardActivityMonitor;

namespace TouchDetect
{
    public partial class Form1 : Form
    {

        private readonly MouseHookListener m_MouseHookManager;
        private Graphics g;
        Rectangle rEllipse;

        Brush brush; //= new SolidBrush(Color.FromArgb(128, 40, 40, 40));
        Pen outline; //= new Pen(Color.Red);
        public Form1()
        {
            InitializeComponent();

            m_MouseHookManager = new MouseHookListener(new GlobalHooker());
            m_MouseHookManager.Enabled = true;
            m_MouseHookManager.MouseMove += HookManager_MouseMove;

            brush = new SolidBrush(Color.FromArgb(128, 40, 40, 40));
            outline = new Pen(Color.Red);

            this.TopMost = true; // make the form always on top
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None; // hidden border
            //this.AutoScaleMode = AutoScaleMode.Dpi;
            this.WindowState = FormWindowState.Maximized; // maximized
            int height = Screen.PrimaryScreen.Bounds.Height;
            int width = Screen.PrimaryScreen.Bounds.Width;
            //Console.WriteLine(height + " " + width);
            this.Width = width;
            this.Height = height;
            this.MinimizeBox = this.MaximizeBox = false; // not allowed to be minimized
            this.MinimumSize = this.MaximumSize = this.Size; // not allowed to be resized
            this.TransparencyKey = this.BackColor = Color.Red; // the color key to transparent, choose a color that you don't use
            g = this.CreateGraphics();
            this.Opacity = 0.30;
            Console.WriteLine("Press Ctrl+C To Exit");
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // Set the form click-through
                cp.ExStyle |= 0x80000 /* WS_EX_LAYERED */ | 0x20 /* WS_EX_TRANSPARENT */;
                return cp;
            }
        }

        private void HookManager_MouseMove(object sender, MouseEventArgs e)
        {

            g.Clear(Color.Red);
            label1.Text = string.Format("x={0:0000}; y={1:0000}", e.X, e.Y);
            rEllipse = new Rectangle();
            float scalingFactor = getScalingFactor();
            float x = e.X / scalingFactor;
            float y = e.Y / scalingFactor;
            rEllipse.Width = 50;
            rEllipse.Height = 50;
            rEllipse.X = (int)x - (rEllipse.Width / 2);
            rEllipse.Y = (int)y - (rEllipse.Height / 2);
            g.DrawEllipse(outline, rEllipse);
            g.FillEllipse(brush, rEllipse);
            //Console.WriteLine(e.X + ", " + e.Y + " : " + this.Width + ", " + this.Height + " " + getScalingFactor());
            //label1.Location = new Point(e.X, e.Y);
        }

        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        public enum DeviceCap
        {
            VERTRES = 10,
            DESKTOPVERTRES = 117,

            // http://pinvoke.net/default.aspx/gdi32/GetDeviceCaps.html
        }

        private float getScalingFactor()
        {
            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();
            int LogicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
            int PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);

            float ScreenScalingFactor = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;

            return ScreenScalingFactor; // 1.25 = 125%
        }

    }
    
}
