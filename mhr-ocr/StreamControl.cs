using OpenCvSharp;

namespace mhr_ocr
{
    class StreamControl
    {
        private static VideoCapture capture = new VideoCapture();
        private int _id;

        public void SetId(int id)
        {
            _id = id;
            capture.Release();
        }

        public void Init()
        {
            capture.Open(_id, VideoCaptureAPIs.DSHOW);
            capture.Set(VideoCaptureProperties.FourCC, FourCC.FromString("YUY2"));
            capture.Set(VideoCaptureProperties.FrameWidth, 1920);
            capture.Set(VideoCaptureProperties.FrameHeight, 1080);
            capture.Set(VideoCaptureProperties.Fps, 10);
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
            Cv2.Resize(_m, _m, Size.Zero, f, f);
            Cv2.ImShow("Capture Result " + info, _m);
            capture.Release();
        }

        public Mat GetMat()
        {
            Mat frame = new Mat(360, 640, MatType.CV_8UC1);

            TestInit();

            if (capture.IsOpened())
            {
                capture.Read(frame);
                capture.Read(frame);
            }
            else
            {
                Cv2.PutText(frame, "Error: Failed to open camera", new Point(0, 128), HersheyFonts.HersheyDuplex, 1, Scalar.White);
            }
            return frame;
        }

        public void Release()
        {
            capture.Release();
        }

        public void TestSave(string path)
        {
            Mat m = GetMat();
            Cv2.ImWrite(path, m);
        }

        public Mat TestRead(string path)
        {
            Mat m = Cv2.ImRead(path);
            return m;
        }
    }
}
