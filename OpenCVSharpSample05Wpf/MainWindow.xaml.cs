using System.Windows.Media;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace OpenCVSharpSample05Wpf
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            loadImage();
        }

        private void loadImage()
        {
            using (var iplImage = new Mat(@"..\..\Images\Penguin.png", ImreadModes.AnyDepth | ImreadModes.AnyColor))
            {
                Cv2.Dilate(iplImage, iplImage, new Mat());

                Image1.Source = iplImage.ToWriteableBitmap(PixelFormats.Bgr24);
            }
        }
    }
}