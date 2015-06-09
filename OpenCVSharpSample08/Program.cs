using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

namespace OpenCVSharpSample08
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var src = new Mat(@"..\..\Images\cvmorph.Png", LoadMode.AnyDepth | LoadMode.AnyColor))
            using (var dst = new Mat())
            {
                src.CopyTo(dst);

                var elementShape = StructuringElementShape.Rect;
                var maxIterations = 10;

                var openCloseWindow = new Window("Open/Close", image: dst);
                var openCloseTrackbar = openCloseWindow.CreateTrackbar(
                    name: "Iterations", value: 0, max: maxIterations * 2 + 1,
                    callback: pos =>
                    {
                        var n = pos - maxIterations;
                        var an = n > 0 ? n : -n;
                        var element = Cv2.GetStructuringElement(
                                elementShape,
                                new Size(an * 2 + 1, an * 2 + 1),
                                new Point(an, an));

                        if (n < 0)
                        {
                            Cv2.MorphologyEx(src, dst, MorphologyOperation.Open, element);
                        }
                        else
                        {
                            Cv2.MorphologyEx(src, dst, MorphologyOperation.Close, element);
                        }

                        Cv2.PutText(dst, (n < 0) ?
                            string.Format("Open/Erosion followed by Dilation[{0}]", elementShape)
                            : string.Format("Close/Dilation followed by Erosion[{0}]", elementShape),
                            new Point(10, 15), FontFace.HersheyPlain, 1, Scalar.Black);
                        openCloseWindow.Image = dst;
                    });


                var erodeDilateWindow = new Window("Erode/Dilate", image: dst);
                var erodeDilateTrackbar = erodeDilateWindow.CreateTrackbar(
                    name: "Iterations", value: 0, max: maxIterations * 2 + 1,
                    callback: pos =>
                    {
                        var n = pos - maxIterations;
                        var an = n > 0 ? n : -n;
                        var element = Cv2.GetStructuringElement(
                                elementShape,
                                new Size(an * 2 + 1, an * 2 + 1),
                                new Point(an, an));
                        if (n < 0)
                        {
                            Cv2.Erode(src, dst, element);
                        }
                        else
                        {
                            Cv2.Dilate(src, dst, element);
                        }

                        Cv2.PutText(dst, (n < 0) ?
                            string.Format("Erode[{0}]", elementShape) :
                            string.Format("Dilate[{0}]", elementShape),
                            new Point(10, 15), FontFace.HersheyPlain, 1, Scalar.Black);
                        erodeDilateWindow.Image = dst;
                    });


                for (; ; )
                {
                    openCloseTrackbar.Callback.DynamicInvoke(0);
                    erodeDilateTrackbar.Callback.DynamicInvoke(0);

                    var key = Cv2.WaitKey();

                    if ((char)key == 27) // ESC
                        break;

                    switch ((char)key)
                    {
                        case 'e':
                            elementShape = StructuringElementShape.Ellipse;
                            break;
                        case 'r':
                            elementShape = StructuringElementShape.Rect;
                            break;
                        case 'c':
                            elementShape = StructuringElementShape.Cross;
                            break;
                    }
                }

                openCloseWindow.Dispose();
                erodeDilateWindow.Dispose();
            }
        }
    }
}