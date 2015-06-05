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
            using (var iplImage = new IplImage(@"..\..\Images\Penguin.png", LoadMode.Color))
            {
                Cv.Dilate(iplImage, iplImage);

                Image1.Source = iplImage.ToWriteableBitmap(PixelFormats.Bgr24);
            }
        }
    }
}