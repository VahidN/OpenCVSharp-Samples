using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.UserInterface;

namespace OpenCVSharpSample06WinForms
{
    public partial class FrmMain : Form
    {
        private PictureBoxIpl _pictureBoxIpl1;
        private BackgroundWorker _worker;

        public FrmMain()
        {
            InitializeComponent();
        }

        private static double getFps(VideoCapture capture)
        {
            using (var image = new Mat())
            {
                while (true)
                {
                    /* start camera */
                    capture.Read(image);
                    if (!image.Empty())
                    {
                        break;
                    }
                }
            }

            using (var image = new Mat())
            {
                double counter = 0;
                double seconds = 0;
                var watch = Stopwatch.StartNew();
                while (true)
                {
                    capture.Read(image);
                    if (image.Empty())
                    {
                        break;
                    }

                    counter++;
                    seconds = watch.ElapsedMilliseconds / (double)1000;
                    if (seconds >= 3)
                    {
                        watch.Stop();
                        break;
                    }
                }
                var fps = counter / seconds;
                return fps;
            }
        }

        private void BtnStart_Click(object sender, System.EventArgs e)
        {
            if (_worker != null && _worker.IsBusy)
            {
                return;
            }

            _worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            if (RadioAvi.Checked)
            {
                _worker.DoWork += workerDoReadVideo;
            }

            if (RadioWebCam.Checked)
            {
                _worker.DoWork += workerDoReadCamera;
            }

            _worker.ProgressChanged += workerProgressChanged;
            _worker.RunWorkerCompleted += workerRunWorkerCompleted;
            _worker.RunWorkerAsync();

            BtnStart.Enabled = false;
        }

        private void BtnStop_Click(object sender, System.EventArgs e)
        {
            if (_worker != null)
            {
                _worker.CancelAsync();
                _worker.Dispose();
            }
            BtnStart.Enabled = true;
        }

        private void FrmMain_Load(object sender, System.EventArgs e)
        {
            _pictureBoxIpl1 = new PictureBoxIpl
            {
                AutoSize = true
            };
            flowLayoutPanel1.Controls.Add(_pictureBoxIpl1);
        }

        private void workerDoReadCamera(object sender, DoWorkEventArgs e)
        {
            using (var capture = new VideoCapture(CaptureDevice.Any, index: 0))
            {
                var fps = getFps(capture);
                capture.Fps = fps;
                var interval = (int)(1000 / fps);

                using (var image = new Mat())
                {
                    while (_worker != null && !_worker.CancellationPending)
                    {
                        capture.Read(image);
                        if (image.Empty())
                            break;

                        _worker.ReportProgress(0, image);
                        Thread.Sleep(interval);
                    }
                }
            }
        }

        private void workerDoReadVideo(object sender, DoWorkEventArgs e)
        {
            using (var capture = new VideoCapture(@"..\..\Videos\drop.avi"))
            {
                var interval = (int)(1000 / capture.Fps);

                using (var image = new Mat())
                {
                    while (_worker != null && !_worker.CancellationPending)
                    {
                        capture.Read(image);
                        if (image.Empty())
                        {
                            break;
                        }

                        _worker.ReportProgress(0, image);
                        Thread.Sleep(interval);
                    }
                }
            }
        }

        private void workerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var image = e.UserState as Mat;
            if (image == null) return;

            //Cv.Not(image, image);
            _pictureBoxIpl1.RefreshIplImage(image);
        }

        private void workerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _worker.Dispose();
            _worker = null;
            BtnStart.Enabled = true;
        }
    }
}