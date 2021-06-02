using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace mhr_ocr
{
    // 用于实现截图功能的窗口
    // 弃用
    public partial class Scanner : Window
    {
        private static double outerWidth = 310;
        private static double outerHeight = 340;
        private static double innerWidth = 300;
        private static double innerHeigth = 300;

        private static double widthGutter = outerWidth - innerWidth;
        private static double heightGutter = outerHeight - innerHeigth;

        private MainWindow _owner;

        public Scanner()
        {
            InitializeComponent();
        }

        public Scanner(MainWindow owner)
        {
            _owner = owner;
            InitializeComponent();
        }

        public System.Drawing.Point GetTopLeft()
        {
            System.Windows.Point wp = Rs_TopLeft.PointToScreen(new System.Windows.Point(0, 0));
            System.Drawing.Point dp = new System.Drawing.Point((int)wp.X, (int)wp.Y);
            return dp;
        }

        public System.Drawing.Point GetBottomRight()
        {
            System.Windows.Point wp = Rs_BottomRight.PointToScreen(new System.Windows.Point(0, 0));
            System.Drawing.Point dp = new System.Drawing.Point((int)wp.X, (int)wp.Y);
            return dp;
        }

        public int GetWidth()
        {
            return GetBottomRight().X - GetTopLeft().X;
        }

        public int GetHeight()
        {
            return GetBottomRight().Y - GetTopLeft().Y;
        }

        public System.Drawing.Size GetSize()
        {
            return new System.Drawing.Size(GetWidth(), GetHeight());
        }

        public Bitmap GetScreenShot()
        {
            Bitmap bitmap = new Bitmap(GetWidth(), GetHeight());
            Graphics g = Graphics.FromImage(bitmap);
            g.CopyFromScreen(GetTopLeft(), System.Drawing.Point.Empty, GetSize());
            return bitmap;
        }

        private bool ResizeInProcess = false;
        private void Resize_Init(object sender, MouseButtonEventArgs e)
        {
            Rectangle senderRect = sender as Rectangle;
            if (senderRect != null)
            {
                ResizeInProcess = true;
                senderRect.CaptureMouse();
                Update_Magnifier();
                Magnifier_Border.Visibility = Visibility.Visible;
            }
        }

        private void Resize_End(object sender, MouseButtonEventArgs e)
        {
            Rectangle senderRect = sender as Rectangle;
            if (senderRect != null)
            {
                ResizeInProcess = false; ;
                senderRect.ReleaseMouseCapture();
                Magnifier_Border.Visibility = Visibility.Hidden;
            }
        }

        private void Resizeing_Form(object sender, MouseEventArgs e)
        {
            if (ResizeInProcess)
            {
                Rectangle senderRect = sender as Rectangle;
                Grid resizeGrid = ResizeGrid;
                Window mainWindow = this;
                if (senderRect != null)
                {
                    double width = e.GetPosition(resizeGrid).X;
                    double height = e.GetPosition(resizeGrid).Y;
                    senderRect.CaptureMouse();
                    if (senderRect.Name.ToLower().Contains("right"))
                    {
                        if (width > resizeGrid.MinWidth)
                        {
                            resizeGrid.Width = width;
                            mainWindow.Width = widthGutter + width;
                        }
                        else
                        {
                            resizeGrid.Width = resizeGrid.MinWidth;
                            mainWindow.Width = mainWindow.MinWidth;

                        }
                    }
                    if (senderRect.Name.ToLower().Contains("left"))
                    {
                        double old_right = mainWindow.Left + mainWindow.Width;

                        width = resizeGrid.Width - width;
                        if (width > resizeGrid.MinWidth)
                        {
                            resizeGrid.Width = width;
                            mainWindow.Width = widthGutter + width;
                            mainWindow.Left = old_right - mainWindow.Width;
                        }
                        else
                        {
                            resizeGrid.Width = resizeGrid.MinWidth;
                            mainWindow.Width = mainWindow.MinWidth;
                            mainWindow.Left = old_right - mainWindow.Width;
                        }
                    }
                    if (senderRect.Name.ToLower().Contains("bottom"))
                    {
                        if (height > resizeGrid.MinHeight)
                        {
                            resizeGrid.Height = height;
                            mainWindow.Height = heightGutter + height;
                        }
                        else
                        {
                            resizeGrid.Height = resizeGrid.MinHeight;
                            mainWindow.Height = mainWindow.MinHeight;
                        }
                    }
                    if (senderRect.Name.ToLower().Contains("top"))
                    {
                        double old_bottom = mainWindow.Top + mainWindow.Height;

                        height = resizeGrid.Height - height;
                        if (height > resizeGrid.MinHeight)
                        {
                            resizeGrid.Height = height;

                            mainWindow.Height = heightGutter + height;
                            mainWindow.Top = old_bottom - mainWindow.Height;
                        }
                        else
                        {
                            resizeGrid.Height = resizeGrid.MinHeight;
                            mainWindow.Height = mainWindow.MinHeight;
                            mainWindow.Top = old_bottom - mainWindow.Height;
                        }
                    }

                    Update_Magnifier();
                }
            }
        }

        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("User32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator System.Drawing.Point(POINT point)
            {
                return new System.Drawing.Point(point.X, point.Y);
            }
        }
        private void Resizeing_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (ResizeInProcess)
            {
                GetCursorPos(out POINT p);
                switch (e.Key)
                {
                    case Key.Up:
                        SetCursorPos(p.X, p.Y - 1);
                        break;
                    case Key.Right:
                        SetCursorPos(p.X + 1, p.Y);
                        break;
                    case Key.Down:
                        SetCursorPos(p.X, p.Y + 1);
                        break;
                    case Key.Left:
                        SetCursorPos(p.X - 1, p.Y);
                        break;
                    default:
                        break;
                }
            }
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
        private void Update_Magnifier()
        {
            int s = 31;

            System.Windows.Point wp = PointToScreen(Mouse.GetPosition(this));
            System.Drawing.Point dp = new System.Drawing.Point((int)wp.X - (s / 2), (int)wp.Y - (s / 2));
            using (Bitmap bitmap = new Bitmap(s, s))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(dp, System.Drawing.Point.Empty, new System.Drawing.Size(s, s));
                    Magnifier.Source = BitmapToImageSource(bitmap);
                }
            }
        }

        private void Effect_Fade(UIElement ele, double from, double to, int duration)
        {
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(duration);
            DoubleAnimation doubleAnimation = new DoubleAnimation(from, to, new Duration(timeSpan));
            ele.BeginAnimation(OpacityProperty, doubleAnimation);
        }

        private void Function_Button_MouseEnter(object sender, MouseEventArgs e)
        {
            Effect_Fade(sender as Button, 0.1, 1, 200);
        }

        private void Function_Button_MouseLeave(object sender, MouseEventArgs e)
        {
            Effect_Fade(sender as Button, 1, 0.1, 200);
        }

        private void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            //if (_owner != null)
            //{
            //    _owner.Close_Scanner();
            //}
            //else
            //{
            //    Close();
            //}
            Close();
        }

        private void Move_Button_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
