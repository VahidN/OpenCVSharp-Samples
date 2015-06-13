using System;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

namespace OpenCVSharpSample11
{
    class Program
    {
        static void Main(string[] args)
        {
            example02();
        }

        private static void example02()
        {
            var src = new Mat(@"..\..\Images\fruits.jpg", LoadMode.AnyDepth | LoadMode.AnyColor);
            Cv2.ImShow("Source", src);
            Cv2.WaitKey(1); // do events

            Cv2.Blur(src, src, new Size(15, 15));
            Cv2.ImShow("Blurred Image", src);
            Cv2.WaitKey(1); // do events

            // Converts the MxNx3 image into a Kx3 matrix where K=MxN and
            // each row is now a vector in the 3-D space of RGB.
            // change to a Mx3 column vector (M is number of pixels in image)
            var columnVector = src.Reshape(cn: 3, rows: src.Rows * src.Cols);

            // convert to floating point, it is a requirement of the k-means method of OpenCV.
            var samples = new Mat();
            columnVector.ConvertTo(samples, MatType.CV_32FC3);

            for (var clustersCount = 2; clustersCount <= 8; clustersCount += 2)
            {
                var bestLabels = new Mat();
                var centers = new Mat();
                Cv2.Kmeans(
                    data: samples,
                    k: clustersCount,
                    bestLabels: bestLabels,
                    criteria:
                        new TermCriteria(type: CriteriaType.Epsilon | CriteriaType.Iteration, maxCount: 10, epsilon: 1.0),
                    attempts: 3,
                    flags: KMeansFlag.PpCenters,
                    centers: centers);


                var clusteredImage = new Mat(src.Rows, src.Cols, src.Type());
                for (var size = 0; size < src.Cols * src.Rows; size++)
                {
                    var clusterIndex = bestLabels.At<int>(0, size);
                    var newPixel = new Vec3b
                    {
                        Item0 = (byte)(centers.At<float>(clusterIndex, 0)), // B
                        Item1 = (byte)(centers.At<float>(clusterIndex, 1)), // G
                        Item2 = (byte)(centers.At<float>(clusterIndex, 2)) // R
                    };
                    clusteredImage.Set(size / src.Cols, size % src.Cols, newPixel);
                }

                Cv2.ImShow(string.Format("Clustered Image [k:{0}]", clustersCount), clusteredImage);
                Cv2.WaitKey(1); // do events
            }

            Cv2.WaitKey();
            Cv2.DestroyAllWindows();
        }

        /// <summary>
        /// https://github.com/Itseez/opencv_extra/blob/master/learning_opencv_v2/ch13_ex13_1.cpp
        /// </summary>
        private static void example01()
        {
            using (var window = new Window("Clusters", flags: WindowMode.AutoSize | WindowMode.FreeRatio))
            {
                const int maxClusters = 5;
                var rng = new RNG(state: (ulong)DateTime.Now.Ticks);

                for (; ; )
                {
                    var clustersCount = rng.Uniform(a: 2, b: maxClusters + 1);
                    var samplesCount = rng.Uniform(a: 1, b: 1001);

                    var points = new Mat(rows: samplesCount, cols: 1, type: MatType.CV_32FC2);
                    clustersCount = Math.Min(clustersCount, samplesCount);

                    var img = new Mat(rows: 500, cols: 500, type: MatType.CV_8UC3, s: Scalar.All(0));

                    // generate random sample from multi-gaussian distribution
                    for (var k = 0; k < clustersCount; k++)
                    {
                        var pointChunk = points.RowRange(
                                startRow: k * samplesCount / clustersCount,
                                endRow: (k == clustersCount - 1)
                                    ? samplesCount
                                    : (k + 1) * samplesCount / clustersCount);

                        var center = new Point
                        {
                            X = rng.Uniform(a: 0, b: img.Cols),
                            Y = rng.Uniform(a: 0, b: img.Rows)
                        };
                        rng.Fill(
                            mat: pointChunk,
                            distType: DistributionType.Normal,
                            a: new Scalar(center.X, center.Y),
                            b: new Scalar(img.Cols * 0.05f, img.Rows * 0.05f));
                    }

                    Cv2.RandShuffle(dst: points, iterFactor: 1, rng: rng);

                    var labels = new Mat();
                    var centers = new Mat(rows: clustersCount, cols: 1, type: points.Type());
                    Cv2.Kmeans(
                        data: points,
                        k: clustersCount,
                        bestLabels: labels,
                        criteria: new TermCriteria(CriteriaType.Epsilon | CriteriaType.Iteration, 10, 1.0),
                        attempts: 3,
                        flags: KMeansFlag.PpCenters,
                        centers: centers);


                    Scalar[] colors =
                    {
                       new Scalar(0, 0, 255),
                       new Scalar(0, 255, 0),
                       new Scalar(255, 100, 100),
                       new Scalar(255, 0, 255),
                       new Scalar(0, 255, 255)
                    };

                    for (var i = 0; i < samplesCount; i++)
                    {
                        var clusterIdx = labels.At<int>(i);
                        Point ipt = points.At<Point2f>(i);
                        Cv2.Circle(
                            img: img,
                            center: ipt,
                            radius: 2,
                            color: colors[clusterIdx],
                            lineType: LineType.AntiAlias,
                            thickness: Cv.FILLED);
                    }

                    window.Image = img;

                    var key = (char)Cv2.WaitKey();
                    if (key == 27 || key == 'q' || key == 'Q') // 'ESC'
                    {
                        break;
                    }
                }
            }
        }
    }
}