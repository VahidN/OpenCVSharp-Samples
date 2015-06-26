namespace OpenCVSharpSample06WinForms
{
    partial class FrmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.BtnStart = new System.Windows.Forms.Button();
            this.BtnStop = new System.Windows.Forms.Button();
            this.RadioAvi = new System.Windows.Forms.RadioButton();
            this.RadioWebCam = new System.Windows.Forms.RadioButton();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.BtnStart);
            this.flowLayoutPanel1.Controls.Add(this.BtnStop);
            this.flowLayoutPanel1.Controls.Add(this.RadioAvi);
            this.flowLayoutPanel1.Controls.Add(this.RadioWebCam);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(448, 328);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // BtnStart
            // 
            this.BtnStart.Location = new System.Drawing.Point(3, 3);
            this.BtnStart.Name = "BtnStart";
            this.BtnStart.Size = new System.Drawing.Size(75, 23);
            this.BtnStart.TabIndex = 0;
            this.BtnStart.Text = " Start";
            this.BtnStart.UseVisualStyleBackColor = true;
            this.BtnStart.Click += new System.EventHandler(this.BtnStart_Click);
            // 
            // BtnStop
            // 
            this.BtnStop.Location = new System.Drawing.Point(84, 3);
            this.BtnStop.Name = "BtnStop";
            this.BtnStop.Size = new System.Drawing.Size(75, 23);
            this.BtnStop.TabIndex = 1;
            this.BtnStop.Text = "Stop";
            this.BtnStop.UseVisualStyleBackColor = true;
            this.BtnStop.Click += new System.EventHandler(this.BtnStop_Click);
            // 
            // RadioAvi
            // 
            this.RadioAvi.AutoSize = true;
            this.RadioAvi.Checked = true;
            this.RadioAvi.Location = new System.Drawing.Point(165, 3);
            this.RadioAvi.Name = "RadioAvi";
            this.RadioAvi.Size = new System.Drawing.Size(58, 17);
            this.RadioAvi.TabIndex = 2;
            this.RadioAvi.TabStop = true;
            this.RadioAvi.Text = "AVI file";
            this.RadioAvi.UseVisualStyleBackColor = true;
            // 
            // RadioWebCam
            // 
            this.RadioWebCam.AutoSize = true;
            this.RadioWebCam.Location = new System.Drawing.Point(229, 3);
            this.RadioWebCam.Name = "RadioWebCam";
            this.RadioWebCam.Size = new System.Drawing.Size(69, 17);
            this.RadioWebCam.TabIndex = 3;
            this.RadioWebCam.Text = "WebCam";
            this.RadioWebCam.UseVisualStyleBackColor = true;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(448, 328);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "FrmMain";
            this.Text = "OpenCV Sample";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button BtnStart;
        private System.Windows.Forms.Button BtnStop;
        private System.Windows.Forms.RadioButton RadioAvi;
        private System.Windows.Forms.RadioButton RadioWebCam;
    }
}

