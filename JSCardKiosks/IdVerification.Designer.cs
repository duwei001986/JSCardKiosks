namespace JSCardKiosks
{
    partial class IdVerification
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IdVerification));
            this.picGif = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.axCieCloudWalkOCX1 = new AxCieCloudWalkOCXLib.AxCieCloudWalkOCX();
            this.labName = new System.Windows.Forms.Label();
            this.labId = new System.Windows.Forms.Label();
            this.labRetTime = new System.Windows.Forms.Label();
            this.picPhoto = new System.Windows.Forms.PictureBox();
            this.labValidate = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.newPanel1 = new JSCardKiosks.NewPanel();
            ((System.ComponentModel.ISupportInitialize)(this.picGif)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axCieCloudWalkOCX1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPhoto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.newPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // picGif
            // 
            this.picGif.BackColor = System.Drawing.Color.White;
            this.picGif.Image = ((System.Drawing.Image)(resources.GetObject("picGif.Image")));
            this.picGif.Location = new System.Drawing.Point(963, 187);
            this.picGif.Name = "picGif";
            this.picGif.Size = new System.Drawing.Size(176, 173);
            this.picGif.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picGif.TabIndex = 0;
            this.picGif.TabStop = false;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // axCieCloudWalkOCX1
            // 
            this.axCieCloudWalkOCX1.Enabled = true;
            this.axCieCloudWalkOCX1.Location = new System.Drawing.Point(28, 56);
            this.axCieCloudWalkOCX1.Name = "axCieCloudWalkOCX1";
            this.axCieCloudWalkOCX1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axCieCloudWalkOCX1.OcxState")));
            this.axCieCloudWalkOCX1.Size = new System.Drawing.Size(640, 480);
            this.axCieCloudWalkOCX1.TabIndex = 5;
            this.axCieCloudWalkOCX1.Visible = false;
            this.axCieCloudWalkOCX1.cwLivesInfoCallBack += new AxCieCloudWalkOCXLib._DCieCloudWalkOCXEvents_cwLivesInfoCallBackEventHandler(this.axCieCloudWalkOCX1_cwLivesInfoCallBack);
            // 
            // labName
            // 
            this.labName.AutoSize = true;
            this.labName.BackColor = System.Drawing.Color.Transparent;
            this.labName.Font = new System.Drawing.Font("微软雅黑", 15F);
            this.labName.Location = new System.Drawing.Point(999, 451);
            this.labName.Name = "labName";
            this.labName.Size = new System.Drawing.Size(0, 27);
            this.labName.TabIndex = 6;
            // 
            // labId
            // 
            this.labId.AutoSize = true;
            this.labId.BackColor = System.Drawing.Color.Transparent;
            this.labId.Font = new System.Drawing.Font("微软雅黑", 15F);
            this.labId.Location = new System.Drawing.Point(999, 509);
            this.labId.Name = "labId";
            this.labId.Size = new System.Drawing.Size(0, 27);
            this.labId.TabIndex = 7;
            // 
            // labRetTime
            // 
            this.labRetTime.AutoSize = true;
            this.labRetTime.BackColor = System.Drawing.Color.Transparent;
            this.labRetTime.Font = new System.Drawing.Font("微软雅黑", 20F);
            this.labRetTime.ForeColor = System.Drawing.Color.Red;
            this.labRetTime.Location = new System.Drawing.Point(655, 598);
            this.labRetTime.Name = "labRetTime";
            this.labRetTime.Size = new System.Drawing.Size(47, 35);
            this.labRetTime.TabIndex = 8;
            this.labRetTime.Text = "30";
            this.labRetTime.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // picPhoto
            // 
            this.picPhoto.BackColor = System.Drawing.Color.White;
            this.picPhoto.Location = new System.Drawing.Point(28, 56);
            this.picPhoto.Name = "picPhoto";
            this.picPhoto.Size = new System.Drawing.Size(640, 480);
            this.picPhoto.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picPhoto.TabIndex = 9;
            this.picPhoto.TabStop = false;
            this.picPhoto.Visible = false;
            // 
            // labValidate
            // 
            this.labValidate.AutoSize = true;
            this.labValidate.BackColor = System.Drawing.Color.Transparent;
            this.labValidate.Font = new System.Drawing.Font("微软雅黑", 24F, System.Drawing.FontStyle.Bold);
            this.labValidate.ForeColor = System.Drawing.Color.Green;
            this.labValidate.Location = new System.Drawing.Point(976, 106);
            this.labValidate.Name = "labValidate";
            this.labValidate.Size = new System.Drawing.Size(146, 42);
            this.labValidate.TabIndex = 10;
            this.labValidate.Text = "核验结果";
            this.labValidate.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(240, 109);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(902, 424);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 20F);
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(688, 599);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 35);
            this.label1.TabIndex = 1;
            this.label1.Text = "01";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // newPanel1
            // 
            this.newPanel1.BackColor = System.Drawing.Color.White;
            this.newPanel1.BackgroundImage = global::JSCardKiosks.Properties.Resources.插入身份证;
            this.newPanel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.newPanel1.Controls.Add(this.pictureBox1);
            this.newPanel1.Controls.Add(this.label1);
            this.newPanel1.Location = new System.Drawing.Point(0, 0);
            this.newPanel1.Name = "newPanel1";
            this.newPanel1.Size = new System.Drawing.Size(1382, 651);
            this.newPanel1.TabIndex = 3;
            // 
            // IdVerification
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1382, 651);
            this.Controls.Add(this.newPanel1);
            this.Controls.Add(this.labRetTime);
            this.Controls.Add(this.labValidate);
            this.Controls.Add(this.labId);
            this.Controls.Add(this.labName);
            this.Controls.Add(this.picGif);
            this.Controls.Add(this.picPhoto);
            this.Controls.Add(this.axCieCloudWalkOCX1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "IdVerification";
            this.Text = "Form3";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.IdVerification_FormClosing);
            this.Load += new System.EventHandler(this.IdVerification_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picGif)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axCieCloudWalkOCX1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPhoto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.newPanel1.ResumeLayout(false);
            this.newPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picGif;
        private System.Windows.Forms.Timer timer1;
        private AxCieCloudWalkOCXLib.AxCieCloudWalkOCX axCieCloudWalkOCX1;
        private System.Windows.Forms.Label labName;
        private System.Windows.Forms.Label labId;
        private System.Windows.Forms.Label labRetTime;
        private System.Windows.Forms.PictureBox picPhoto;
        private System.Windows.Forms.Label labValidate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private NewPanel newPanel1;
    }
}