using System;
using OpenCvSharp;

namespace OpenCVSharpSample04
{
    class Program
    {
        private static void Main(string[] args)
        {
            applyLinearFilter();
            testBuiltinFilters();
        }

        private static void applyLinearFilter()
        {
            using (var src = new IplImage(@"..\..\Images\Penguin.Png", LoadMode.AnyDepth | LoadMode.AnyColor))
            using (var dst = new IplImage(src.Size, src.Depth, src.NChannels))
            {
                float[] data = { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
                var kernel = new CvMat(rows: 1, cols: 21, type: MatrixType.F32C1, elements: data);

                Cv.Normalize(src: kernel, dst: kernel, a: 1.0, b: 0, normType: NormType.L1);

                double sum = 0;
                foreach (var item in data)
                {
                    sum += Math.Abs(item);
                }
                Console.WriteLine(sum); // => .999999970197678

                Cv.Filter2D(src, dst, kernel, anchor: new CvPoint(0, 0));

                using (new CvWindow("src", image: src))
                using (new CvWindow("dst", image: dst))
                {
                    Cv.WaitKey(0);
                }
            }
        }

        private static void testBuiltinFilters()
        {
            using (var src = new IplImage(@"..\..\Images\Car.jpg", LoadMode.AnyDepth | LoadMode.AnyColor))
            {
                using (var dst = new IplImage(src.Size, src.Depth, src.NChannels))
                {
                    using (new CvWindow("src", image: src))
                    {
                        Cv.Erode(src, dst);
                        using (new CvWindow("Erode", image: dst))
                        {
                            Cv.Dilate(src, dst);
                            using (new CvWindow("Dilate", image: dst))
                            {
                                Cv.Not(src, dst);
                                using (new CvWindow("Invert", image: dst))
                                {
                                    Cv.WaitKey(0);
                                }
                            }
                        }
                    }
                }
            }
        }

        /*
        #include <cv.h>
        #include <highgui.h>
        #include <stdio.h>

        int main (int argc, char **argv)
        {
          IplImage *src_img = 0, *dst_img;
          float data[] = { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1
          };
          CvMat kernel = cvMat (1, 21, CV_32F, data);

          if (argc >= 2)
            src_img = cvLoadImage (argv[1], CV_LOAD_IMAGE_ANYDEPTH | CV_LOAD_IMAGE_ANYCOLOR);
          if (src_img == 0)
            exit (-1);

          dst_img = cvCreateImage (cvGetSize (src_img), src_img->depth, src_img->nChannels);

          cvNormalize (&kernel, &kernel, 1.0, 0, CV_L1);
          cvFilter2D (src_img, dst_img, &kernel, cvPoint (0, 0));

          cvNamedWindow ("Filter2D", CV_WINDOW_AUTOSIZE);
          cvShowImage ("Filter2D", dst_img);
          cvWaitKey (0);

          cvDestroyWindow ("Filter2D");
          cvReleaseImage (&src_img);
          cvReleaseImage (&dst_img);

          return 0;
        }
        */
    }
}