using System;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

namespace OpenCVSharpSample15
{
    class Program
    {
        static void Main(string[] args)
        {
            var srcImage = new Mat(@"..\..\Images\Test.jpg");
            Cv2.ImShow("Source", srcImage);
            Cv2.WaitKey(1); // do events

            var grayImage = new Mat();
            Cv2.CvtColor(srcImage, grayImage, ColorConversion.BgrToGray);
            Cv2.EqualizeHist(grayImage, grayImage);

            var cascade = new CascadeClassifier(@"..\..\Data\haarcascade_frontalface_alt.xml");
            var nestedCascade = new CascadeClassifier(@"..\..\Data\haarcascade_eye_tree_eyeglasses.xml");

            var faces = cascade.DetectMultiScale(
                image: grayImage,
                scaleFactor: 1.1,
                minNeighbors: 2,
                flags: HaarDetectionType.Zero | HaarDetectionType.ScaleImage,
                minSize: new Size(30, 30)
                );

            Console.WriteLine("Detected faces: {0}", faces.Length);

            var rnd = new Random();
            var count = 1;
            foreach (var faceRect in faces)
            {
                var detectedFaceImage = new Mat(srcImage, faceRect);
                Cv2.ImShow(string.Format("Face {0}", count), detectedFaceImage);
                Cv2.WaitKey(1); // do events

                var color = Scalar.FromRgb(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
                Cv2.Rectangle(srcImage, faceRect, color, 3);


                var detectedFaceGrayImage = new Mat();
                Cv2.CvtColor(detectedFaceImage, detectedFaceGrayImage, ColorConversion.BgrToGray);
                var nestedObjects = nestedCascade.DetectMultiScale(
                    image: detectedFaceGrayImage,
                    scaleFactor: 1.1,
                    minNeighbors: 2,
                    flags: HaarDetectionType.Zero | HaarDetectionType.ScaleImage,
                    minSize: new Size(30, 30)
                    );

                Console.WriteLine("Nested Objects[{0}]: {1}", count, nestedObjects.Length);

                foreach (var nestedObject in nestedObjects)
                {
                    var center = new Point
                    {
                        X = Cv.Round(nestedObject.X + nestedObject.Width * 0.5) + faceRect.Left,
                        Y = Cv.Round(nestedObject.Y + nestedObject.Height * 0.5) + faceRect.Top
                    };
                    var radius = Cv.Round((nestedObject.Width + nestedObject.Height) * 0.25);
                    Cv2.Circle(srcImage, center, radius, color, thickness: 3);
                }

                count++;
            }

            Cv2.ImShow("Haar Detection", srcImage);
            Cv2.WaitKey(1); // do events


            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
            srcImage.Dispose();
        }
    }
}