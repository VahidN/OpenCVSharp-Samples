using System.ComponentModel;
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
            _worker.DoWork += workerDoWork;
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

        private void workerDoWork(object sender, DoWorkEventArgs e)
        {
            using (var capture = new CvCapture(@"..\..\Videos\drop.avi"))
            {
                var interval = (int)(1000 / capture.Fps);

                IplImage image;
                while ((image = capture.QueryFrame()) != null &&
                        _worker != null && !_worker.CancellationPending)
                {
                    _worker.ReportProgress(0, image);
                    Thread.Sleep(interval);
                }
            }
        }

        private void workerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var image = e.UserState as IplImage;
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