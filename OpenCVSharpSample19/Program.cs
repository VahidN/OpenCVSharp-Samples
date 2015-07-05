using System;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp.Extensions;
using ZXing;
using ZXing.Common;

namespace OpenCVSharpSample19
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            detectBarcode(@"..\..\images\ghabz01_resize.jpg", thresh: 210, debug: true);

            detectBarcode(@"..\..\images\book2.jpg", thresh: 170);

            detectBarcode(@"..\..\images\book1.jpg", thresh: 170);

            detectBarcode(@"..\..\images\barcode5.jpg", thresh: 170);

            detectBarcode(@"..\..\images\ghabz.jpg", thresh: 150, rotation: -2, debug: false);

            detectBarcode(@"..\..\images\bargh.png", thresh: 170);

            detectBarcode(@"..\..\images\gass1.jpg", thresh: 175);

            detectBarcode(@"..\..\images\b_code128.png", thresh: 80);

            detectBarcode(@"..\..\images\128c.png", thresh: 120, debug: false);

            detectBarcode(@"..\..\images\ghabz_bar01_128C.png", thresh: 120, debug: false);

            detectBarcode(@"..\..\images\ghabz05.jpg", thresh: 203, rotation: -90, debug: false);

            detectBarcode(@"..\..\images\msbooklet.png", thresh: 120);

            detectBarcode(@"..\..\images\newspaper.jpg", thresh: 120);

            detectBarcode(@"..\..\images\Code128test-CodeC.png", thresh: 75);

            detectBarcode(@"..\..\images\Code128test-CodeB.png", thresh: 80);

            detectBarcode(@"..\..\images\ghabz04.jpg", thresh: 207, rotation: -90, debug: false);

            detectBarcode(@"..\..\images\ghabz03.jpg", thresh: 215);

            detectBarcode(@"..\..\images\ghabz02.jpg", thresh: 214);

            detectBarcode(@"..\..\images\ghabz01.jpg", thresh: 184);

            detectBarcode(@"..\..\images\barcode3.jpg", thresh: 80);

            detectBarcode(@"..\..\images\EAN-13-ISBN-13.png", thresh: 82);

            detectBarcode(@"..\..\images\barcode1.jpg", thresh: 225);

            decodeBarcodeText((System.Drawing.Bitmap)System.Drawing.Bitmap.FromFile(@"..\..\images\Sample_PDF417.png"));
            detectBarcode(@"..\..\images\Sample_PDF417.png", thresh: 30);

            detectBarcode(@"..\..\images\image.png", thresh: 82);

            detectBarcode(@"..\..\images\test.jpg", thresh: 210);

            detectBarcode(@"..\..\images\isbn.png", thresh: 82);

            decodeBarcodeText((System.Drawing.Bitmap)System.Drawing.Bitmap.FromFile(@"..\..\images\b_ean8.png"));
            detectBarcode(@"..\..\images\b_ean8.png", thresh: 80);

            detectBarcode(@"..\..\images\barcode2.png", thresh: 80);

            detectBarcode(@"..\..\images\barcode4.jpg", thresh: 170);
        }

        private static void rotateImage(Mat src, Mat dst, double angle, double scale)
        {
            var imageCenter = new Point2f(src.Cols / 2f, src.Rows / 2f);
            var rotationMat = Cv2.GetRotationMatrix2D(imageCenter, angle, scale);
            Cv2.WarpAffine(src, dst, rotationMat, src.Size());
        }

        private static string detectBarcode(string fileName, double thresh, bool debug = false, double rotation = 0)
        {
            Console.WriteLine("\nProcessing: {0}", fileName);

            // load the image and convert it to grayscale
            var image = new Mat(fileName);

            if (rotation != 0)
            {
                rotateImage(image, image, rotation, 1);
            }

            if (debug)
            {
                Cv2.ImShow("Source", image);
                Cv2.WaitKey(1); // do events
            }

            var gray = new Mat();
            var channels = image.Channels();
            if (channels > 1)
            {
                Cv2.CvtColor(image, gray, ColorConversion.BgrToGray);
            }
            else
            {
                image.CopyTo(gray);
            }


            // compute the Scharr gradient magnitude representation of the images
            // in both the x and y direction
            var gradX = new Mat();
            Cv2.Sobel(gray, gradX, MatType.CV_32F, xorder: 1, yorder: 0, ksize: -1);
            //Cv2.Scharr(gray, gradX, MatType.CV_32F, xorder: 1, yorder: 0);

            var gradY = new Mat();
            Cv2.Sobel(gray, gradY, MatType.CV_32F, xorder: 0, yorder: 1, ksize: -1);
            //Cv2.Scharr(gray, gradY, MatType.CV_32F, xorder: 0, yorder: 1);

            // subtract the y-gradient from the x-gradient
            var gradient = new Mat();
            Cv2.Subtract(gradX, gradY, gradient);
            Cv2.ConvertScaleAbs(gradient, gradient);

            if (debug)
            {
                Cv2.ImShow("Gradient", gradient);
                Cv2.WaitKey(1); // do events
            }


            // blur and threshold the image
            var blurred = new Mat();
            Cv2.Blur(gradient, blurred, new Size(9, 9));

            var threshImage = new Mat();
            Cv2.Threshold(blurred, threshImage, thresh, 255, ThresholdType.Binary);

            if (debug)
            {
                Cv2.ImShow("Thresh", threshImage);
                Cv2.WaitKey(1); // do events
            }


            // construct a closing kernel and apply it to the thresholded image
            var kernel = Cv2.GetStructuringElement(StructuringElementShape.Rect, new Size(21, 7));
            var closed = new Mat();
            Cv2.MorphologyEx(threshImage, closed, MorphologyOperation.Close, kernel);

            if (debug)
            {
                Cv2.ImShow("Closed", closed);
                Cv2.WaitKey(1); // do events
            }


            // perform a series of erosions and dilations
            Cv2.Erode(closed, closed, null, iterations: 4);
            Cv2.Dilate(closed, closed, null, iterations: 4);

            if (debug)
            {
                Cv2.ImShow("Erode & Dilate", closed);
                Cv2.WaitKey(1); // do events
            }


            //find the contours in the thresholded image, then sort the contours
            //by their area, keeping only the largest one

            Point[][] contours;
            HiearchyIndex[] hierarchyIndexes;
            Cv2.FindContours(
                closed,
                out contours,
                out hierarchyIndexes,
                mode: ContourRetrieval.CComp,
                method: ContourChain.ApproxSimple);

            if (contours.Length == 0)
            {
                throw new NotSupportedException("Couldn't find any object in the image.");
            }

            var contourIndex = 0;
            var previousArea = 0;
            var biggestContourRect = Cv2.BoundingRect(contours[0]);
            while ((contourIndex >= 0))
            {
                var contour = contours[contourIndex];

                var boundingRect = Cv2.BoundingRect(contour); //Find bounding rect for each contour
                var boundingRectArea = boundingRect.Width * boundingRect.Height;
                if (boundingRectArea > previousArea)
                {
                    biggestContourRect = boundingRect;
                    previousArea = boundingRectArea;
                }

                contourIndex = hierarchyIndexes[contourIndex].Next;
            }


            /*biggestContourRect.Width += 10;
            biggestContourRect.Height += 10;
            biggestContourRect.Left -= 5;
            biggestContourRect.Top -= 5;*/


            var barcode = new Mat(image, biggestContourRect); //Crop the image
            Cv2.CvtColor(barcode, barcode, ColorConversion.BgrToGray);

            Cv2.ImShow("Barcode", barcode);
            Cv2.WaitKey(1); // do events

            var barcodeClone = barcode.Clone();
            var barcodeText = getBarcodeText(barcodeClone);

            if (string.IsNullOrWhiteSpace(barcodeText))
            {
                Console.WriteLine("Enhancing the barcode...");
                //Cv2.AdaptiveThreshold(barcode, barcode, 255,
                    //AdaptiveThresholdType.GaussianC, ThresholdType.Binary, 9, 1);
                //var th = 119;
                var th = 100;
                Cv2.Threshold(barcode, barcode, th, 255, ThresholdType.ToZero);
                Cv2.Threshold(barcode, barcode, th, 255, ThresholdType.Binary);
                barcodeText = getBarcodeText(barcode);
            }

            Cv2.Rectangle(image,
                new Point(biggestContourRect.X, biggestContourRect.Y),
                new Point(biggestContourRect.X + biggestContourRect.Width, biggestContourRect.Y + biggestContourRect.Height),
                new Scalar(0, 255, 0),
                2);

            if (debug)
            {
                Cv2.ImShow("Segmented Source", image);
                Cv2.WaitKey(1); // do events
            }

            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();

            return barcodeText;
        }

        private static string getBarcodeText(Mat barcode)
        {
            // `ZXing.Net` needs a white space around the barcode
            var barcodeWithWhiteSpace = new Mat(new Size(barcode.Width + 30, barcode.Height + 30), MatType.CV_8U, Scalar.White);
            var drawingRect = new Rect(new Point(15, 15), new Size(barcode.Width, barcode.Height));
            var roi = barcodeWithWhiteSpace[drawingRect];
            barcode.CopyTo(roi);

            Cv2.ImShow("Enhanced Barcode", barcodeWithWhiteSpace);
            Cv2.WaitKey(1); // do events

            return decodeBarcodeText(barcodeWithWhiteSpace.ToBitmap());
        }

        private static string decodeBarcodeText(System.Drawing.Bitmap barcodeBitmap)
        {
            var source = new BitmapLuminanceSource(barcodeBitmap);

            // using http://zxingnet.codeplex.com/
            // PM> Install-Package ZXing.Net
            var reader = new BarcodeReader(null, null, ls => new GlobalHistogramBinarizer(ls))
            {
                AutoRotate = true,
                TryInverted = true,
                Options = new DecodingOptions
                {
                    TryHarder = true,
                    //PureBarcode = true,
                    /*PossibleFormats = new List<BarcodeFormat>
                    {
                        BarcodeFormat.CODE_128
                        //BarcodeFormat.EAN_8,
                        //BarcodeFormat.CODE_39,
                        //BarcodeFormat.UPC_A
                    }*/
                }
            };

            //var newhint = new KeyValuePair<DecodeHintType, object>(DecodeHintType.ALLOWED_EAN_EXTENSIONS, new Object());
            //reader.Options.Hints.Add(newhint);

            var result = reader.Decode(source);
            if (result == null)
            {
                Console.WriteLine("Decode failed.");
                return string.Empty;
            }

            Console.WriteLine("BarcodeFormat: {0}", result.BarcodeFormat);
            Console.WriteLine("Result: {0}", result.Text);


            var writer = new BarcodeWriter
            {
                Format = result.BarcodeFormat,
                Options = { Width = 200, Height = 50, Margin = 4},
                Renderer = new ZXing.Rendering.BitmapRenderer()
            };
            var barcodeImage = writer.Write(result.Text);
            Cv2.ImShow("BarcodeWriter", barcodeImage.ToMat());

            return result.Text;
        }
    }
}