using OpenCvSharp;

namespace OpenCVSharpSample01
{
    class Program
    {
        static void Main(string[] args)
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
        }
    }
}