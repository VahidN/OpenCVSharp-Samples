using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCvSharp;
using OpenCvSharp.ML;

namespace PrereleaseTests
{
    /// <summary>
    /// Using PM> Install-Package OpenCvSharp3-AnyCPU -Source https://ci.appveyor.com/nuget/opencvsharp -IncludePrerelease
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            //RunKNearest();

            using (var src = new Mat(@"..\..\Images\cvmorph.Png", ImreadModes.AnyDepth | ImreadModes.AnyColor))
            using (var dst = new Mat())
            {
                src.CopyTo(dst);

                var elementShape = MorphShapes.Rect;
                var maxIterations = 10;

                var openCloseWindow = new Window("Open/Close", image: dst);
                var openCloseTrackbar = openCloseWindow.CreateTrackbar(
                    name: "Iterations", value: 0, max: maxIterations * 2 + 1,
                    callback: (pos, obj) =>
                    {
                        var n = pos - maxIterations;
                        var an = n > 0 ? n : -n;
                        var element = Cv2.GetStructuringElement(
                                elementShape,
                                new Size(an * 2 + 1, an * 2 + 1),
                                new Point(an, an));

                        if (n < 0)
                        {
                            Cv2.MorphologyEx(src, dst, MorphTypes.Open, element);
                        }
                        else
                        {
                            Cv2.MorphologyEx(src, dst, MorphTypes.Close, element);
                        }

                        Cv2.PutText(dst, (n < 0) ?
                            string.Format("Open/Erosion followed by Dilation[{0}]", elementShape)
                            : string.Format("Close/Dilation followed by Erosion[{0}]", elementShape),
                            new Point(10, 15), HersheyFonts.HersheyPlain, 1, Scalar.Black);
                        openCloseWindow.Image = dst;
                    });


                var erodeDilateWindow = new Window("Erode/Dilate", image: dst);
                var erodeDilateTrackbar = erodeDilateWindow.CreateTrackbar(
                    name: "Iterations", value: 0, max: maxIterations * 2 + 1,
                    callback: (pos, obj) =>
                    {
                        var n = pos - maxIterations;
                        var an = n > 0 ? n : -n;
                        var element = Cv2.GetStructuringElement(
                                elementShape,
                                new Size(an * 2 + 1, an * 2 + 1),
                                new Point(an, an));
                        if (n < 0)
                        {
                            Cv2.Erode(src, dst, element);
                        }
                        else
                        {
                            Cv2.Dilate(src, dst, element);
                        }

                        Cv2.PutText(dst, (n < 0) ?
                            string.Format("Erode[{0}]", elementShape) :
                            string.Format("Dilate[{0}]", elementShape),
                            new Point(10, 15), HersheyFonts.HersheyPlain, 1, Scalar.Black);
                        erodeDilateWindow.Image = dst;
                    });


                for (;;)
                {
                    //openCloseTrackbar.Callback.DynamicInvoke(0);
                    //erodeDilateTrackbar.Callback.DynamicInvoke(0);

                    var key = Cv2.WaitKey();

                    if ((char)key == 27) // ESC
                        break;

                    switch ((char)key)
                    {
                        case 'e':
                            elementShape = MorphShapes.Ellipse;
                            break;
                        case 'r':
                            elementShape = MorphShapes.Rect;
                            break;
                        case 'c':
                            elementShape = MorphShapes.Cross;
                            break;
                    }
                }

                openCloseWindow.Dispose();
                erodeDilateWindow.Dispose();
            }
        }

        public static void RunKNearest()
        {
            float[] trainFeaturesData =
            {
                 2,2,2,2,
                 3,3,3,3,
                 4,4,4,4,
                 5,5,5,5,
                 6,6,6,6,
                 7,7,7,7
            };
            var trainFeatures = new Mat(6, 4, MatType.CV_32F, trainFeaturesData);

            int[] trainLabelsData = { 2, 3, 4, 5, 6, 7 };
            var trainLabels = new Mat(1, 6, MatType.CV_32F, trainLabelsData);

            var kNearest = KNearest.Create();
            kNearest.Train(trainFeatures, SampleTypes.RowSample, trainLabels);

            float[] testFeatureData = { 3, 3, 3, 3 };
            var testFeature = new Mat(1, 4, MatType.CV_32F, testFeatureData);

            const int k = 1;
            var results = new Mat();
            var neighborResponses = new Mat();
            var dists = new Mat();
            var detectedClass = (int)kNearest.FindNearest(testFeature, k, results, neighborResponses, dists);
            /*
            K=1
            [3]
            [0]

            K=4:
            [3, 2, 4, 5]
            [0, 4, 4, 16]
            */
            Console.WriteLine("results: {0},{1}", results.Get<int>(0,0), results.Get<int>(0, 1));
            Console.WriteLine("DetectedClass: {0}", detectedClass);
            Console.WriteLine("neighborResponses: {0}", neighborResponses);
            Console.WriteLine("dists: {0}", dists);
        }

    }
}
