using DirectShowLib;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace mhr_ocr
{
    public partial class MainWindow : System.Windows.Window
    {
        private StreamControl streamControl = new StreamControl();

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

        private void Scan(object sender, RoutedEventArgs e)
        {
            //if (!Has_Scanner())
            //{
            //    return;
            //}

            //Bitmap bitmap = scanner.Get_ScreenShot();
            //Ocr o = new Ocr(bitmap);

            //Ocr o = new Ocr(@"E:\Users\YueLeng_M\Desktop\test\123.png");

            //ContentLabel.Content = o.res["rare"] + "\n";
            //ContentLabel.Content += o.res["slot"] + "\n";
            //ContentLabel.Content += o.res["s1"] + "\n";
            //ContentLabel.Content += o.res["l1"] + "\n";
            //ContentLabel.Content += o.res["s2"] + "\n";
            //ContentLabel.Content += o.res["l2"] + "\n";

            //StreamTest s = new StreamTest(2);

            //Cv2.ImShow("1", s.GetBitmap());

            //StreamTest s = new StreamTest(2);

            //Cv2.ImShow("1", s.Get_Mat());
            //Console.WriteLine("");
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
    }
}

