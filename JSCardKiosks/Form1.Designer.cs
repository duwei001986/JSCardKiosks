namespace JSCardKiosks
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.paldDterminePhoto = new System.Windows.Forms.Panel();
            this.labRTakePhoto = new System.Windows.Forms.Label();
            this.picBlankPhoto = new AForge.Controls.PictureBox();
            this.labSubmit = new System.Windows.Forms.Label();
            this.palInfo = new System.Windows.Forms.Panel();
            this.labSure = new System.Windows.Forms.Label();
            this.picGif = new System.Windows.Forms.PictureBox();
            this.labInfoTime = new System.Windows.Forms.Label();
            this.paldDterminePhoto.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBlankPhoto)).BeginInit();
            this.palInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picGif)).BeginInit();
            this.SuspendLayout();
            // 
            // paldDterminePhoto
            // 
            this.paldDterminePhoto.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("paldDterminePhoto.BackgroundImage")));
            this.paldDterminePhoto.Controls.Add(this.labRTakePhoto);
            this.paldDterminePhoto.Controls.Add(this.picBlankPhoto);
            this.paldDterminePhoto.Controls.Add(this.labSubmit);
            this.paldDterminePhoto.Location = new System.Drawing.Point(8, 8);
            this.paldDterminePhoto.Name = "paldDterminePhoto";
            this.paldDterminePhoto.Size = new System.Drawing.Size(1382, 651);
            this.paldDterminePhoto.TabIndex = 6;
            this.paldDterminePhoto.Visible = false;
            // 
            // labRTakePhoto
            // 
            this.labRTakePhoto.BackColor = System.Drawing.Color.Transparent;
            this.labRTakePhoto.Font = new System.Drawing.Font("微软雅黑", 20F);
            this.labRTakePhoto.ForeColor = System.Drawing.Color.DimGray;
            this.labRTakePhoto.Location = new System.Drawing.Point(143, 560);
            this.labRTakePhoto.Name = "labRTakePhoto";
            this.labRTakePhoto.Size = new System.Drawing.Size(264, 66);
            this.labRTakePhoto.TabIndex = 6;
            this.labRTakePhoto.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // picBlankPhoto
            // 
            this.picBlankPhoto.Image = null;
            this.picBlankPhoto.Location = new System.Drawing.Point(114, 106);
            this.picBlankPhoto.Name = "picBlankPhoto";
            this.picBlankPhoto.Size = new System.Drawing.Size(341, 420);
            this.picBlankPhoto.TabIndex = 5;
            this.picBlankPhoto.TabStop = false;
            // 
            // labSubmit
            // 
            this.labSubmit.BackColor = System.Drawing.Color.Transparent;
            this.labSubmit.Font = new System.Drawing.Font("微软雅黑", 20F);
            this.labSubmit.ForeColor = System.Drawing.Color.DimGray;
            this.labSubmit.Location = new System.Drawing.Point(983, 556);
            this.labSubmit.Name = "labSubmit";
            this.labSubmit.Size = new System.Drawing.Size(270, 70);
            this.labSubmit.TabIndex = 4;
            this.labSubmit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // palInfo
            // 
            this.palInfo.BackgroundImage = global::JSCardKiosks.Properties.Resources.拍照前的注意事项;
            this.palInfo.Controls.Add(this.labSure);
            this.palInfo.Controls.Add(this.picGif);
            this.palInfo.Controls.Add(this.labInfoTime);
            this.palInfo.Location = new System.Drawing.Point(0, 0);
            this.palInfo.Name = "palInfo";
            this.palInfo.Size = new System.Drawing.Size(1382, 651);
            this.palInfo.TabIndex = 7;
            // 
            // labSure
            // 
            this.labSure.BackColor = System.Drawing.Color.Transparent;
            this.labSure.Font = new System.Drawing.Font("微软雅黑", 20F);
            this.labSure.ForeColor = System.Drawing.Color.DimGray;
            this.labSure.Location = new System.Drawing.Point(983, 556);
            this.labSure.Name = "labSure";
            this.labSure.Size = new System.Drawing.Size(342, 70);
            this.labSure.TabIndex = 4;
            this.labSure.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // picGif
            // 
            this.picGif.BackColor = System.Drawing.Color.White;
            this.picGif.Image = global::JSCardKiosks.Properties.Resources.动态;
            this.picGif.Location = new System.Drawing.Point(143, 59);
            this.picGif.Name = "picGif";
            this.picGif.Size = new System.Drawing.Size(1074, 442);
            this.picGif.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picGif.TabIndex = 1;
            this.picGif.TabStop = false;
            // 
            // labInfoTime
            // 
            this.labInfoTime.AutoSize = true;
            this.labInfoTime.BackColor = System.Drawing.Color.Transparent;
            this.labInfoTime.Font = new System.Drawing.Font("微软雅黑", 20F);
            this.labInfoTime.ForeColor = System.Drawing.Color.Red;
            this.labInfoTime.Location = new System.Drawing.Point(643, 598);
            this.labInfoTime.Name = "labInfoTime";
            this.labInfoTime.Size = new System.Drawing.Size(47, 35);
            this.labInfoTime.TabIndex = 3;
            this.labInfoTime.Text = "10";
            this.labInfoTime.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1382, 651);
            this.Controls.Add(this.palInfo);
            this.Controls.Add(this.paldDterminePhoto);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form1";
            this.Text = "Form1";
            this.paldDterminePhoto.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picBlankPhoto)).EndInit();
            this.palInfo.ResumeLayout(false);
            this.palInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picGif)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel paldDterminePhoto;
        private System.Windows.Forms.Label labRTakePhoto;
        private AForge.Controls.PictureBox picBlankPhoto;
        private System.Windows.Forms.Label labSubmit;
        private System.Windows.Forms.Panel palInfo;
        private System.Windows.Forms.Label labSure;
        private System.Windows.Forms.PictureBox picGif;
        private System.Windows.Forms.Label labInfoTime;
    }
}