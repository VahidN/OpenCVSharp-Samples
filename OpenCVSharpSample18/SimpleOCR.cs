using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

namespace OpenCVSharpSample18
{
    public class SimpleOCR
    {
        private const double Thresh = 80;
        private const double ThresholdMaxVal = 255;

        public void DoOCR(CvKNearest kNearest, string path)
        {
            var src = Cv2.ImRead(path);
            Cv2.ImShow("Source", src);

            var gray = new Mat();
            Cv2.CvtColor(src, gray, ColorConversion.BgrToGray);

            var threshImage = new Mat();
            Cv2.Threshold(gray, threshImage, Thresh, ThresholdMaxVal, ThresholdType.BinaryInv); // Threshold to find contour


            Point[][] contours;
            HiearchyIndex[] hierarchyIndexes;
            Cv2.FindContours(
                threshImage,
                out contours,
                out hierarchyIndexes,
                mode: ContourRetrieval.CComp,
                method: ContourChain.ApproxSimple);

            if (contours.Length == 0)
            {
                throw new NotSupportedException("Couldn't find any object in the image.");
            }

            //Create input sample by contour finding and cropping
            var dst = new Mat(src.Rows, src.Cols, MatType.CV_8UC3, Scalar.All(0));

            var contourIndex = 0;
            while ((contourIndex >= 0))
            {
                var contour = contours[contourIndex];

                var boundingRect = Cv2.BoundingRect(contour); //Find bounding rect for each contour

                Cv2.Rectangle(src,
                    new Point(boundingRect.X, boundingRect.Y),
                    new Point(boundingRect.X + boundingRect.Width, boundingRect.Y + boundingRect.Height),
                    new Scalar(0, 0, 255),
                    2);

                var roi = new Mat(threshImage, boundingRect); //Crop the image

                var resizedImage = new Mat();
                var resizedImageFloat = new Mat();
                Cv2.Resize(roi, resizedImage, new Size(10, 10)); //resize to 10X10
                resizedImage.ConvertTo(resizedImageFloat, MatType.CV_32FC1); //convert to float
                var result = resizedImageFloat.Reshape(1, 1);


                var results = new Mat();
                var neighborResponses = new Mat();
                var dists = new Mat();
                var detectedClass = (int)kNearest.FindNearest(result, 1, results, neighborResponses, dists);

                //Console.WriteLine("DetectedClass: {0}", detectedClass);
                //Cv2.ImShow("roi", roi);
                //Cv.WaitKey(0);

                //Cv2.ImWrite(string.Format("det_{0}_{1}.png",detectedClass, contourIndex), roi);

                Cv2.PutText(
                    dst,
                    detectedClass.ToString(CultureInfo.InvariantCulture),
                    new Point(boundingRect.X, boundingRect.Y + boundingRect.Height),
                    0,
                    1,
                    new Scalar(0, 255, 0),
                    2);

                contourIndex = hierarchyIndexes[contourIndex].Next;
            }

            Cv2.ImShow("Segmented Source", src);
            Cv2.ImShow("Detected", dst);

            Cv2.ImWrite("dest.jpg", dst);

            Cv2.WaitKey();
        }

        public IList<ImageInfo> ReadTrainingImages(string path, string ext)
        {
            var images = new List<ImageInfo>();

            var imageId = 1;
            foreach (var dir in new DirectoryInfo(path).GetDirectories())
            {
                var groupId = int.Parse(dir.Name);
                foreach (var imageFile in dir.GetFiles(ext))
                {
                    var image = processTrainingImage(new Mat(imageFile.FullName, LoadMode.GrayScale));
                    if (image == null)
                    {
                        continue;
                    }

                    images.Add(new ImageInfo
                    {
                        Image = image,
                        ImageId = imageId++,
                        ImageGroupId = groupId
                    });
                }
            }

            return images;
        }

        public CvKNearest TrainData(IList<ImageInfo> trainingImages)
        {
            var samples = new Mat();
            foreach (var trainingImage in trainingImages)
            {
                samples.PushBack(trainingImage.Image);
            }

            var labels = trainingImages.Select(x => x.ImageGroupId).ToArray();
            var responses = new Mat(labels.Length, 1, MatType.CV_32SC1, labels);
            var tmp = responses.Reshape(1, 1); //make continuous
            var responseFloat = new Mat();
            tmp.ConvertTo(responseFloat, MatType.CV_32FC1); // Convert  to float


            var kNearest = new CvKNearest();
            kNearest.Train(samples, responseFloat); // Train with sample and responses
            return kNearest;
        }

        private static Mat processTrainingImage(Mat gray)
        {
            var threshImage = new Mat();
            Cv2.Threshold(gray, threshImage, Thresh, ThresholdMaxVal, ThresholdType.BinaryInv); // Threshold to find contour

            Point[][] contours;
            HiearchyIndex[] hierarchyIndexes;
            Cv2.FindContours(
                threshImage,
                out contours,
                out hierarchyIndexes,
                mode: ContourRetrieval.CComp,
                method: ContourChain.ApproxSimple);

            if (contours.Length == 0)
            {
                return null;
            }

            Mat result = null;

            var contourIndex = 0;
            while ((contourIndex >= 0))
            {
                var contour = contours[contourIndex];

                var boundingRect = Cv2.BoundingRect(contour); //Find bounding rect for each contour
                var roi = new Mat(threshImage, boundingRect); //Crop the image

                //Cv2.ImShow("src", gray);
                //Cv2.ImShow("roi", roi);
                //Cv.WaitKey(0);

                var resizedImage = new Mat();
                var resizedImageFloat = new Mat();
                Cv2.Resize(roi, resizedImage, new Size(10, 10)); //resize to 10X10
                resizedImage.ConvertTo(resizedImageFloat, MatType.CV_32FC1); //convert to float
                result = resizedImageFloat.Reshape(1, 1);

                contourIndex = hierarchyIndexes[contourIndex].Next;
            }

            return result;
        }
    }
}