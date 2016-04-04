using OpenCvSharp;

namespace OpenCVSharpSample02
{
    class Program
    {
        static void Main(string[] args)
        {
            readImage1();
            readImage2();
        }

        private static void readImage2()
        {
            using (var src = new Mat(@"..\..\Images\ocv02.jpg", ImreadModes.Unchanged))
            {
                using (var window = new Window("window", image: src, flags: WindowMode.AutoSize))
                {
                    Cv2.WaitKey();
                }
            }
        }

        private static void readImage1()
        {
            using (var src = new Mat(@"..\..\Images\ocv02.jpg", ImreadModes.GrayScale))
            {
                using (var window = new Window("window", image: src, flags: WindowMode.AutoSize))
                {
                    Cv2.WaitKey();
                }
            }
        }

        /*private static void readImage1_version2xDeprecated()
        {
            var img = Cv.LoadImage(@"..\..\images\ocv02.jpg", ImreadModes.GrayScale);

            Cv.NamedWindow("window");
            Cv.ShowImage("window", img);

            Cv.WaitKey();

            Cv.DestroyWindow("window");

            Cv.ReleaseImage(img);
        }

        private static void readImage2_version2xDeprecated()
        {
            using (var img = new IplImage(@"..\..\images\ocv02.jpg", LoadMode.Unchanged))
            {
                using (var window = new CvWindow("window"))
                {
                    window.Image = img;
                    Cv.WaitKey();
                }
            }
        }

        private static void readImage3_version2xDeprecated()
        {
            // it uses `System.Drawing.Bitmap` behind the scene.
            using (var img = IplImage.FromFile(@"..\..\images\ocv02.jpg", LoadMode.Unchanged))
            {
                using (var window = new CvWindow("window"))
                {
                    window.Image = img;
                    Cv.WaitKey();
                }
            }
        }*/
    }
}