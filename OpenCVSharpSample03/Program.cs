using OpenCvSharp;

namespace OpenCVSharpSample03
{
    class Program
    {
        static void Main(string[] args)
        {
            createAGrayScaleClone();
        }

        private static void createAGrayScaleClone()
        {
            using (var src = new Mat(@"..\..\images\ocv02.jpg", ImreadModes.AnyDepth | ImreadModes.AnyColor))
            {
                using (var srcCopy = new Mat())
                {
                    src.CopyTo(srcCopy);
                    Cv2.CvtColor(srcCopy, srcCopy, ColorConversionCodes.BGRA2GRAY);

                    using (var sourceWindow = new Window("A GrayScale Clone", image: srcCopy, flags: WindowMode.AutoSize))
                    {
                        Cv2.WaitKey();
                    }
                }
            }
        }

        /*private static void createAGrayScaleClone1_version2xDeprecated()
        {
            using (var src = Cv.LoadImage(@"..\..\images\ocv02.jpg", LoadMode.Color))
            using (var dst = Cv.CreateImage(new CvSize(src.Width, src.Height), BitDepth.U8, 1))
            {
                Cv.CvtColor(src, dst, ColorConversionCodes.BGRA2GRAY);

                using (new CvWindow("src", image: src))
                using (new CvWindow("dst", image: dst))
                {
                    Cv.WaitKey();
                }
            }
        }

        private static void createAGrayScaleClone2_version2xDeprecated()
        {
            using (var src = new IplImage(@"..\..\images\ocv02.jpg", LoadMode.Color))
            //using (var dst = new IplImage(new CvSize(src.Width, src.Height), BitDepth.U8, 1))
            using (var dst = new IplImage(src.Size, BitDepth.U8, 1))
            {
                src.CvtColor(dst, ColorConversionCodes.BGRA2GRAY);

                using (new CvWindow("src", image: src))
                using (new CvWindow("dst", image: dst))
                {
                    Cv.WaitKey();
                }
            }
        }*/
    }
}