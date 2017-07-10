using System;
using OpenCvSharp;
using OpenCvSharp.ML;

namespace OpenCVSharpSample18.KNearestTests
{
    public static class KNearestSample
    {
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
            Console.WriteLine("DetectedClass: {0}", detectedClass);
            Console.WriteLine("neighborResponses: {0}", neighborResponses);
            Console.WriteLine("dists: {0}", dists);
        }
    }
}
