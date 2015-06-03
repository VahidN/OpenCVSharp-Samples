using OpenCvSharp;

namespace OpenCVSharpSample03
{
    class Program
    {
        static void Main(string[] args)
        {
            createAGrayScaleClone1();
            createAGrayScaleClone2();
        }

        private static void createAGrayScaleClone1()
        {
            using (var src = Cv.LoadImage(@"..\..\images\ocv02.jpg", LoadMode.Color))
            using (var dst = Cv.CreateImage(new CvSize(src.Width, src.Height), BitDepth.U8, 1))
            {
                Cv.CvtColor(src, dst, ColorConversion.BgrToGray);

                using (new CvWindow("src", image: src))
                using (new CvWindow("dst", image: dst))
                {
                    Cv.WaitKey();
                }
            }
        }

        private static void createAGrayScaleClone2()
        {
            using (var src = new IplImage(@"..\..\images\ocv02.jpg", LoadMode.Color))
            //using (var dst = new IplImage(new CvSize(src.Width, src.Height), BitDepth.U8, 1))
            using (var dst = new IplImage(src.Size, BitDepth.U8, 1))
            {
                src.CvtColor(dst, ColorConversion.BgrToGray);

                using (new CvWindow("src", image: src))
                using (new CvWindow("dst", image: dst))
                {
                    Cv.WaitKey();
                }
            }
        }
    }
}