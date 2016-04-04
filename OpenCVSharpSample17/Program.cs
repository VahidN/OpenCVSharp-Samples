using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenCvSharp;
using OpenCvSharp.Face;

namespace OpenCVSharpSample17
{
    public class ImageInfo
    {
        public Mat Image { set; get; }
        public int ImageGroupId { set; get; }
        public int ImageId { set; get; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var images = new List<ImageInfo>();

            var imageId = 0;
            foreach (var dir in new DirectoryInfo(@"..\..\Images").GetDirectories())
            {
                var groupId = int.Parse(dir.Name.Replace("s", string.Empty)) - 1;
                foreach (var imageFile in dir.GetFiles("*.pgm"))
                {
                    images.Add(new ImageInfo
                    {
                        Image = new Mat(imageFile.FullName, ImreadModes.GrayScale),
                        ImageId = imageId++,
                        ImageGroupId = groupId
                    });
                }
            }

            var model = FaceRecognizer.CreateFisherFaceRecognizer();
            model.Train(images.Select(x => x.Image), images.Select(x => x.ImageGroupId));

            var rnd = new Random();
            var randomImageId = rnd.Next(0, images.Count - 1);
            var testSample = images[randomImageId];

            Console.WriteLine("Actual group: {0}", testSample.ImageGroupId);
            Cv2.ImShow("actual", testSample.Image);

            var predictedGroupId = model.Predict(testSample.Image);
            Console.WriteLine("Predicted group: {0}", predictedGroupId);

            Cv2.WaitKey(0);
        }
    }
}