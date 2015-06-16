using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

namespace OpenCVSharpSample13
{
    class Program
    {
        static void Main(string[] args)
        {
            var img1 = new Mat(@"..\..\Images\left.png", LoadMode.GrayScale);
            Cv2.ImShow("Left", img1);
            Cv2.WaitKey(1); // do events

            var img2 = new Mat(@"..\..\Images\right.png", LoadMode.GrayScale);
            Cv2.ImShow("Right", img2);
            Cv2.WaitKey(1); // do events


            // detecting keypoints
            // FastFeatureDetector, StarDetector, SIFT, SURF, ORB, BRISK, MSER, GFTTDetector, DenseFeatureDetector, SimpleBlobDetector
            // SURF = Speeded Up Robust Features
            var detector = new SURF(hessianThreshold: 400); //A good default value could be from 300 to 500, depending from the image contrast.
            var keypoints1 = detector.Detect(img1);
            var keypoints2 = detector.Detect(img2);

            // computing descriptors, BRIEF, FREAK
            // BRIEF = Binary Robust Independent Elementary Features
            var extractor = new BriefDescriptorExtractor();
            var descriptors1 = new Mat();
            var descriptors2 = new Mat();
            extractor.Compute(img1, ref keypoints1, descriptors1);
            extractor.Compute(img2, ref keypoints2, descriptors2);

            // matching descriptors
            var matcher = new BFMatcher();
            var matches = matcher.Match(descriptors1, descriptors2);

            // drawing the results
            var imgMatches = new Mat();
            Cv2.DrawMatches(img1, keypoints1, img2, keypoints2, matches, imgMatches);
            Cv2.ImShow("Matches", imgMatches);
            Cv2.WaitKey(1); // do events


            Cv2.WaitKey(0);

            Cv2.DestroyAllWindows();
            img1.Dispose();
            img2.Dispose();
        }
    }
}