using System;
using OpenCvSharp;

namespace OpenCVSharpSample14
{
    class Program
    {
        static void Main(string[] args)
        {
            var srcImage = new Mat(@"..\..\Images\cvlbl.png");
            Cv2.ImShow("Source", srcImage);
            Cv2.WaitKey(1); // do events


            var binaryImage = new Mat(srcImage.Size(), MatType.CV_8UC1);

            Cv2.CvtColor(srcImage, binaryImage, ColorConversionCodes.BGRA2GRAY);
            Cv2.Threshold(binaryImage, binaryImage, thresh: 100, maxval: 255, type: ThresholdTypes.Binary);

            var detectorParams = new SimpleBlobDetector.Params
            {
                //MinDistBetweenBlobs = 10, // 10 pixels between blobs
                //MinRepeatability = 1,

                //MinThreshold = 100,
                //MaxThreshold = 255,
                //ThresholdStep = 5,

                FilterByArea = false,
                //FilterByArea = true,
                //MinArea = 0.001f, // 10 pixels squared
                //MaxArea = 500,

                FilterByCircularity = false,
                //FilterByCircularity = true,
                //MinCircularity = 0.001f,

                FilterByConvexity = false,
                //FilterByConvexity = true,
                //MinConvexity = 0.001f,
                //MaxConvexity = 10,

                FilterByInertia = false,
                //FilterByInertia = true,
                //MinInertiaRatio = 0.001f,

                FilterByColor = false
                //FilterByColor = true,
                //BlobColor = 255 // to extract light blobs
            };
            var simpleBlobDetector = SimpleBlobDetector.Create(detectorParams);
            var keyPoints = simpleBlobDetector.Detect(binaryImage);

            Console.WriteLine("keyPoints: {0}", keyPoints.Length);
            foreach (var keyPoint in keyPoints)
            {
                Console.WriteLine("X: {0}, Y: {1}", keyPoint.Pt.X, keyPoint.Pt.Y);
            }

            var imageWithKeyPoints = new Mat();
            Cv2.DrawKeypoints(
                    image: binaryImage,
                    keypoints: keyPoints,
                    outImage: imageWithKeyPoints,
                    color: Scalar.FromRgb(255, 0, 0),
                    flags: DrawMatchesFlags.DrawRichKeypoints);


            Cv2.ImShow("Key Points", imageWithKeyPoints);
            Cv2.WaitKey(1); // do events


            Cv2.WaitKey(0);

            Cv2.DestroyAllWindows();
            srcImage.Dispose();
            imageWithKeyPoints.Dispose();
        }
    }
}