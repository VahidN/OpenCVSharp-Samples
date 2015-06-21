using System;
using System.IO;
using System.Text;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

namespace OpenCVSharpSample16
{
    class Program
    {
        static void Main(string[] args)
        {
            createCarImagesFile();
            createNegativeImagesFile();
            useTrainedData();
        }

        private static void useTrainedData()
        {
            var srcImage = new Mat(@"..\..\CarData\CarData\TestImages_Scale\test-1.pgm");
            Cv2.ImShow("Source", srcImage);
            Cv2.WaitKey(1); // do events

            var grayImage = new Mat();
            Cv2.CvtColor(srcImage, grayImage, ColorConversion.BgrToGray);
            Cv2.EqualizeHist(grayImage, grayImage);

            var cascade = new CascadeClassifier(@"..\..\CarsInfo\data\cascade.xml");

            var cars = cascade.DetectMultiScale(
                image: grayImage,
                scaleFactor: 1.1,
                minNeighbors: 2,
                flags: HaarDetectionType.Zero | HaarDetectionType.ScaleImage,
                minSize: new Size(30, 30)
                );

            Console.WriteLine("Detected cars: {0}", cars.Length);

            var rnd = new Random();
            var count = 1;
            foreach (var carRect in cars)
            {
                var detectedFaceImage = new Mat(srcImage, carRect);
                Cv2.ImShow(string.Format("Car {0}", count), detectedFaceImage);
                Cv2.WaitKey(1); // do events

                var color = Scalar.FromRgb(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
                Cv2.Rectangle(srcImage, carRect, color, 3);


                var detectedFaceGrayImage = new Mat();
                Cv2.CvtColor(detectedFaceImage, detectedFaceGrayImage, ColorConversion.BgrToGray);

                count++;
            }

            Cv2.ImShow("Haar Detection", srcImage);
            Cv2.WaitKey(1); // do events


            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
            srcImage.Dispose();
        }

        private static void createCarImagesFile()
        {
            var sb = new StringBuilder();
            foreach (var file in new DirectoryInfo(@"..\..\CarData\CarData\TrainImages").GetFiles("*pos-*.pgm"))
            {
                sb.AppendFormat("{0} {1} {2} {3} {4} {5}{6}", file.FullName, 1, 0, 0, 100, 40, Environment.NewLine);
            }
            File.WriteAllText(@"..\..\CarsInfo\carImages.txt", sb.ToString());
        }

        private static void createNegativeImagesFile()
        {
            var sb = new StringBuilder();
            foreach (var file in new DirectoryInfo(@"..\..\CarData\CarData\TrainImages").GetFiles("*neg-*.pgm"))
            {
                sb.AppendFormat("{0}{1}", file.FullName,Environment.NewLine);
            }
            File.WriteAllText(@"..\..\CarsInfo\negativeImages.txt", sb.ToString());
        }
    }
}