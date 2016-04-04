using OpenCvSharp;

namespace OpenCVSharpSample09
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var src = new Mat(@"..\..\Images\Penguin.Png", ImreadModes.AnyDepth | ImreadModes.AnyColor))
            using (var dst = new Mat())
            {
                src.CopyTo(dst);

                using (var window = new Window("Resize/Rotate/Blur",
                                                image: dst, flags: WindowMode.AutoSize))
                {
                    var angle = 0.0;
                    var scale = 0.7;

                    var angleTrackbar = window.CreateTrackbar(
                        name: "Angle", value: 0, max: 180,
                        callback: (pos, obj) =>
                        {
                            angle = pos;
                            rotateImage(angle, scale, src, dst);
                            window.Image = dst;
                        });

                    var scaleTrackbar = window.CreateTrackbar(
                        name: "Scale", value: 1, max: 10,
                        callback: (pos, obj) =>
                        {
                            scale = pos / 10f;
                            rotateImage(angle, scale, src, dst);
                            window.Image = dst;
                        });

                    var resizeTrackbar = window.CreateTrackbar(
                        name: "Resize", value: 1, max: 100,
                        callback: (pos, obj) =>
                        {
                            rotateImage(angle, scale, src, dst);
                            Cv2.Resize(dst, dst,
                                new Size(src.Width + pos, src.Height + pos),
                                interpolation: InterpolationFlags.Cubic);
                            window.Image = dst;
                        });

                    var blurTrackbar = window.CreateTrackbar(
                       name: "Blur", value: 1, max: 100,
                       callback: (pos, obj) =>
                       {
                           if (pos % 2 == 0) pos++;

                           rotateImage(angle, scale, src, dst);
                           Cv2.GaussianBlur(dst, dst, new Size(pos, pos), sigmaX: 0);
                           window.Image = dst;
                       });

                    angleTrackbar.Callback.DynamicInvoke(0);
                    scaleTrackbar.Callback.DynamicInvoke(1);
                    resizeTrackbar.Callback.DynamicInvoke(1);
                    blurTrackbar.Callback.DynamicInvoke(1);

                    Cv2.WaitKey();
                }
            }
        }

        private static void rotateImage(double angle, double scale, Mat src, Mat dst)
        {
            var imageCenter = new Point2f(src.Cols / 2f, src.Rows / 2f);
            var rotationMat = Cv2.GetRotationMatrix2D(imageCenter, angle, scale);
            Cv2.WarpAffine(src, dst, rotationMat, src.Size());
        }
    }
}