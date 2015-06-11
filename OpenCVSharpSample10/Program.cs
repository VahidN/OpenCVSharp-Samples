using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

namespace OpenCVSharpSample10
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var src = new Mat(@"..\..\Images\Penguin.Png", LoadMode.AnyDepth | LoadMode.AnyColor))
            {
                using (var sourceWindow = new Window("Source", image: src,
                       flags: WindowMode.AutoSize | WindowMode.FreeRatio))
                {
                    using (var histogramWindow = new Window("Histogram",
                           flags: WindowMode.AutoSize | WindowMode.FreeRatio))
                    {
                        var brightness = 100;
                        var contrast = 100;

                        var brightnessTrackbar = sourceWindow.CreateTrackbar(
                                name: "Brightness", value: brightness, max: 200,
                                callback: pos =>
                                {
                                    brightness = pos;
                                    updateImageCalculateHistogram(sourceWindow, histogramWindow, src, brightness, contrast);
                                });

                        var contrastTrackbar = sourceWindow.CreateTrackbar(
                            name: "Contrast", value: contrast, max: 200,
                            callback: pos =>
                            {
                                contrast = pos;
                                updateImageCalculateHistogram(sourceWindow, histogramWindow, src, brightness, contrast);
                            });


                        brightnessTrackbar.Callback.DynamicInvoke(brightness);
                        contrastTrackbar.Callback.DynamicInvoke(contrast);

                        Cv2.WaitKey();
                    }
                }
            }
        }

        private static void updateImageCalculateHistogram(Window sourceWindow, Window histogramWindow, Mat src, int brightness, int contrast)
        {
            using (var modifiedSrc = new Mat())
            {
                updateBrightnessContrast(src, modifiedSrc, brightness, contrast);
                sourceWindow.Image = modifiedSrc;

                calculateHistogram2(histogramWindow, src, modifiedSrc);
            }
        }

        private static void updateBrightnessContrast(Mat src, Mat modifiedSrc, int brightness, int contrast)
        {
            brightness = brightness - 100;
            contrast = contrast - 100;

            double alpha, beta;
            if (contrast > 0)
            {
                double delta = 127f * contrast / 100f;
                alpha = 255f / (255f - delta * 2);
                beta = alpha * (brightness - delta);
            }
            else
            {
                double delta = -128f * contrast / 100;
                alpha = (256f - delta * 2) / 255f;
                beta = alpha * brightness + delta;
            }
            src.ConvertTo(modifiedSrc, MatType.CV_8UC3, alpha, beta);
        }


        private static void calculateHistogram1(Window histogramWindow, Mat src, Mat modifiedSrc)
        {
            const int histogramSize = 64;//from 0 to 63
            int[] dimensions = { histogramSize }; // Histogram size for each dimension
            Rangef[] ranges = { new Rangef(0, histogramSize) }; // min/max

            using (var histogram = new Mat())
            {
                Cv2.CalcHist(
                    images: new[] { modifiedSrc },
                    channels: new[] { 0 }, //The channel (dim) to be measured. In this case it is just the intensity (each array is single-channel) so we just write 0.
                    mask: null,
                    hist: histogram,
                    dims: 1, //The histogram dimensionality.
                    histSize: dimensions,
                    ranges: ranges);

                using (var histogramImage = (Mat)(Mat.Ones(rows: src.Rows, cols: src.Cols, type: MatType.CV_8U) * 255))
                {
                    // Scales and draws histogram

                    Cv2.Normalize(histogram, histogram, 0, histogramImage.Rows, NormType.MinMax);
                    var binW = Cv.Round((double)histogramImage.Cols / histogramSize);

                    var color = Scalar.All(100);

                    for (var i = 0; i < histogramSize; i++)
                    {
                        Cv2.Rectangle(histogramImage,
                            new Point(i * binW, histogramImage.Rows),
                            new Point((i + 1) * binW, histogramImage.Rows - Cv.Round(histogram.Get<float>(i))),
                            color,
                            -1);
                    }

                    histogramWindow.Image = histogramImage;
                }
            }
        }

        private static void calculateHistogram2(Window histogramWindow, Mat src, Mat modifiedSrc)
        {
            const int histogramSize = 64;//from 0 to 63
            using (var histogram = new Mat())
            {
                int[] dimensions = { histogramSize }; // Histogram size for each dimension
                Rangef[] ranges = { new Rangef(0, histogramSize) }; // min/max
                Cv2.CalcHist(
                    images: new[] { modifiedSrc },
                    channels: new[] { 0 }, //The channel (dim) to be measured. In this case it is just the intensity (each array is single-channel) so we just write 0.
                    mask: null,
                    hist: histogram,
                    dims: 1,
                    histSize: dimensions,
                    ranges: ranges);

                // Get the max value of histogram
                double minVal, maxVal;
                Cv2.MinMaxLoc(histogram, out minVal, out maxVal);

                var color = Scalar.All(100);

                // Scales and draws histogram
                var scaledHistogram = (Mat)(histogram * (maxVal != 0 ? src.Rows / maxVal : 0.0));

                using (var histogramImage = new Mat(new Size(src.Cols, src.Rows), MatType.CV_8UC3, Scalar.All(255)))
                {
                    var binW = (int)((double)src.Cols / histogramSize);
                    for (var j = 0; j < histogramSize; j++)
                    {
                        histogramImage.Rectangle(
                            new Point(j * binW, histogramImage.Rows),
                            new Point((j + 1) * binW, histogramImage.Rows - (int)(scaledHistogram.Get<float>(j))),
                            color,
                            -1);
                    }

                    histogramWindow.Image = histogramImage;
                }
            }
        }
    }
}