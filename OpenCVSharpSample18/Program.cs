using System;
using OpenCVSharpSample18.KNearestTests;

namespace OpenCVSharpSample18
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //KNearestSample.RunKNearest();
            runSimpleOCR();
        }

        private static void runSimpleOCR()
        {
            var simpleOCR = new SimpleOCR();

            Console.WriteLine("ReadTrainingImages");
            //var trainingImages = simpleOCR.ReadTrainingImages(@"..\..\HandWritingNumbers", "*.pbm");
            var trainingImages = simpleOCR.ReadTrainingImages(@"..\..\Numbers", "*.png");

            Console.WriteLine("TrainData");
            var knn = simpleOCR.TrainData(trainingImages);

            Console.WriteLine("DoOCR");
            simpleOCR.DoOCR(knn, @"..\..\Samples\sample1.png");
        }
    }
}