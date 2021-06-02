using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace mhr_ocr
{
    class StreamControl
    {
        private static VideoCapture capture = new VideoCapture();
        private int _id;

        public void SetId(int id)
        {
            _id = id;
        }

        public void Init()
        {
            capture.Open(_id, VideoCaptureAPIs.DSHOW);
            capture.Set(VideoCaptureProperties.FourCC, FourCC.FromString("YUY2"));
            capture.Set(VideoCaptureProperties.FrameWidth, 1920);
            capture.Set(VideoCaptureProperties.FrameHeight, 1080);
            capture.Set(VideoCaptureProperties.Fps, 30);
        }

        public void Init(int id)
        {
            _id = id;
            Init();
        }

        private void TestInit()
        {
            if (!capture.IsOpened())
            {
                Init();
            }
        }

        public void TestCapture()
        {
            TestInit();
            Mat _m = GetMat();
            string info = "" + _m.Width + "x" + _m.Height;
            double f = 640.0 / _m.Width;
            Cv2.Resize(_m, _m, OpenCvSharp.Size.Zero, f, f);
            Cv2.ImShow("Capture Result " + info, _m);
            capture.Release();
        }

        public Mat GetMat()
        {
            Mat frame = new Mat();

            TestInit();

            if (capture.IsOpened())
            {
                capture.Read(frame);
            }
            return frame;
        }

        public void Release()
        {
            capture.Release();
        }
    }
}
