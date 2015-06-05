using System;
using System.Drawing;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace OpenCVSharpSample04WinForms
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            showImageUsingPictureBox();
            showImageUsingPictureBoxIpl();
        }

        private void showImageUsingPictureBoxIpl()
        {
            using (var iplImage = new IplImage(@"..\..\Images\Penguin.png", LoadMode.Color))
            {
                Cv.Dilate(iplImage, iplImage);

                var pictureBoxIpl = new OpenCvSharp.UserInterface.PictureBoxIpl
                {
                    ImageIpl = iplImage,
                    AutoSize = true
                };
                flowLayoutPanel1.Controls.Add(pictureBoxIpl);


                //How to redraw:
                //pictureBoxIpl.RefreshIplImage(iplImage);
            }
        }

        private void showImageUsingPictureBox()
        {
            Bitmap bitmap;
            using (var iplImage = new IplImage(@"..\..\Images\Penguin.png", LoadMode.Color))
            {
                bitmap = iplImage.ToBitmap(); // BitmapConverter.ToBitmap()
            }

            var pictureBox = new PictureBox
            {
                Image = bitmap,
                ClientSize = bitmap.Size
            };

            //How to redraw:
            //iplImage.ToBitmap(dst: (Bitmap)pictureBox.Image);

            flowLayoutPanel1.Controls.Add(pictureBox);
        }
    }
}