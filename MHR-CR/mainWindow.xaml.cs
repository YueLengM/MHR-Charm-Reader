using DirectShowLib;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;


namespace MHR_CR
{
    public partial class MainWindow : System.Windows.Window
    {
        private StreamControl streamControl = new StreamControl();
        private Ocr ocr = new Ocr();

        public MainWindow()
        {
            InitializeComponent();
            RefreshInput();
        }

        public List<string> GetCameraNameList()
        {
            List<DsDevice> devices = new List<DsDevice>(DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice));
            List<string> cameraNames = new List<string>();
            foreach (DsDevice device in devices)
            {
                cameraNames.Add(device.Name);
            }
            return cameraNames;
        }

        public void RefreshInput()
        {
            InputComboBox.ItemsSource = GetCameraNameList();
            InputComboBox.SelectedIndex = 0;
        }

        private void RefreshInput(object sender, RoutedEventArgs e)
        {
            RefreshInput();
        }

        private void TestCamare(object sender, RoutedEventArgs e)
        {
            streamControl.TestCapture();
        }

        private void InputComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            streamControl.SetId(InputComboBox.SelectedIndex);
        }

        private void Scan(object sender, RoutedEventArgs e)
        {
            Scan();
        }
        private void Scan()
        {
            using (Mat m = streamControl.GetMat())
            {
                ocr.Proc(m);
            }
            ResultTextBox.Text = string.Join("\n", ocr.res.Values);
            if (AutoCopy.IsChecked == true)
            {
                Dictionary<string, string> tRes = new Dictionary<string, string>(ocr.res)
                {
                    ["slot"] = string.Join("\t", ocr.res["slot"].ToCharArray()).Replace("\t0", "\t")
                };
                Clipboard.SetText(string.Join("\t", tRes.Values));
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void Copy_Button_Click(object sender, RoutedEventArgs e)
        {
            string[] t = ResultTextBox.Text.Split('\n');
            t[1] = string.Join("\t", t[1].ToCharArray()).Replace("\t0", "\t");
            Clipboard.SetText(string.Join("\t", t));
        }

        [DllImport("User32.dll")]
        private static extern bool RegisterHotKey([In] IntPtr hWnd, [In] int id, [In] uint fsModifiers, [In] uint vk);

        [DllImport("User32.dll")]
        private static extern bool UnregisterHotKey([In] IntPtr hWnd, [In] int id);

        private HwndSource _source;
        private const int HOTKEY_ID = 9010;

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            _source = HwndSource.FromHwnd(helper.Handle);
            _source.AddHook(HwndHook);
            RegisterHotKey();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _source.RemoveHook(HwndHook);
            _source = null;
            UnregisterHotKey();
        }

        private void RegisterHotKey()
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            //const uint MOD_ALT = 0x0001;
            const uint MOD_CTRL = 0x0002;
            if (!RegisterHotKey(helper.Handle, HOTKEY_ID, MOD_CTRL, (uint)KeyInterop.VirtualKeyFromKey(Key.NumPad0)))
            {
                // handle error
            }
        }

        private void UnregisterHotKey()
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            UnregisterHotKey(helper.Handle, HOTKEY_ID);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_ID:
                            OnHotKeyPressed();
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        private void OnHotKeyPressed()
        {
            Scan();
        }
    }
}

