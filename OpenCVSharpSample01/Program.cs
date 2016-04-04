using OpenCvSharp;

namespace OpenCVSharpSample01
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var src = new Mat(new Size(128, 128), MatType.CV_8U, Scalar.All(255)))
            using (var dst = new Mat())
            {
                for (var y = 0; y < src.Height; y++)
                {
                    for (var x = 0; x < src.Width; x++)
                    {
                        var color = src.Get<Vec3b>(y, x);
                        var temp = color.Item0;
                        color.Item0 = color.Item2; // B <- R
                        color.Item2 = temp;        // R <- B
                        src.Set(y, x, color);
                    }
                }

                src.CopyTo(dst);

                using (var window = new Window("window", image: dst, flags: WindowMode.AutoSize))
                {
                    Cv2.WaitKey();
                }
            }
        }

        /*private static void version2xDeprecated()
        {
            var img = Cv.CreateImage(new CvSize(128, 128), BitDepth.U8, 1);

            for (var y = 0; y < img.Height; y++)
            {
                for (var x = 0; x < img.Width; x++)
                {
                    Cv.Set2D(img, y, x, x + y);
                }
            }

            Cv.NamedWindow("window");
            Cv.ShowImage("window", img);
            Cv.WaitKey();
            Cv.DestroyWindow("window");

            Cv.ReleaseImage(img);
        }*/
    }
}