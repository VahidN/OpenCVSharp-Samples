using System.Drawing.Imaging;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp.Extensions;

namespace OpenCVSharpSample07
{
    class Program
    {
        static void Main(string[] args)
        {
            usingCInterface();
            usingCppInterface1();
            usingCppInterface2();
        }

        private static void usingCInterface()
        {
            using (var src = new IplImage(@"..\..\Images\Penguin.Png", LoadMode.AnyDepth | LoadMode.AnyColor))
            using (var dst = new IplImage(src.Size, src.Depth, src.NChannels))
            {
                for (var y = 0; y < src.Height; y++)
                {
                    for (var x = 0; x < src.Width; x++)
                    {
                        CvColor pixel = src[y, x];
                        dst[y, x] = new CvColor
                        {
                            B = (byte)(255 - pixel.B),
                            G = (byte)(255 - pixel.G),
                            R = (byte)(255 - pixel.R)
                        };
                    }
                }

                // [C] Accessing Pixel
                // https://github.com/shimat/opencvsharp/wiki/%5BC%5D-Accessing-Pixel

                using (new CvWindow("C Interface: Src", image: src))
                using (new CvWindow("C Interface: Dst", image: dst))
                {
                    Cv.WaitKey(0);
                }
            }
        }

        private static void usingCppInterface1()
        {
            // Cv2.ImRead
            using (var src = new Mat(@"..\..\Images\Penguin.Png", LoadMode.AnyDepth | LoadMode.AnyColor))
            using (var dst = new Mat())
            {
                src.CopyTo(dst);

                for (var y = 0; y < src.Height; y++)
                {
                    for (var x = 0; x < src.Width; x++)
                    {
                        var pixel = src.Get<Vec3b>(y, x);
                        var newPixel = new Vec3b
                        {
                            Item0 = (byte)(255 - pixel.Item0), // B
                            Item1 = (byte)(255 - pixel.Item1), // G
                            Item2 = (byte)(255 - pixel.Item2) // R
                        };
                        dst.Set(y, x, newPixel);
                    }
                }

                // [Cpp] Accessing Pixel
                // https://github.com/shimat/opencvsharp/wiki/%5BCpp%5D-Accessing-Pixel

                //Cv2.NamedWindow();
                //Cv2.ImShow();
                using (new Window("C++ Interface: Src", image: src))
                using (new Window("C++ Interface: Dst", image: dst))
                {
                    Cv2.WaitKey(0);
                }
            }
        }

        private static void usingCppInterface2()
        {
            // Cv2.ImRead
            using (var src = new Mat(@"..\..\Images\Penguin.Png", LoadMode.AnyDepth | LoadMode.AnyColor))
            using (var dst = new Mat())
            {
                Cv2.CvtColor(src, dst, ColorConversion.BgrToGray);

                // How to export
                using (var bitmap = dst.ToBitmap()) // => OpenCvSharp.Extensions.BitmapConverter.ToBitmap(dst)
                {
                    bitmap.Save("gray.png", ImageFormat.Png);
                }

                using (new Window("BgrToGray C++: src", image: src))
                using (new Window("BgrToGray C++: dst", image: dst))
                {
                    Cv2.WaitKey();
                }
            }
        }
    }
}