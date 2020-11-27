namespace JSCardKiosks
{
    partial class TakePhoto
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TakePhoto));
            this.labRetTime2 = new System.Windows.Forms.Label();
            this.vispShoot = new AForge.Controls.VideoSourcePlayer();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.palInfo = new System.Windows.Forms.Panel();
            this.labSure1 = new System.Windows.Forms.Label();
            this.picGif = new System.Windows.Forms.PictureBox();
            this.labInfoTime1 = new System.Windows.Forms.Label();
            this.palChoosePhoto = new System.Windows.Forms.Panel();
            this.labWaitTxt = new System.Windows.Forms.Label();
            this.labsubmitPhoto3 = new System.Windows.Forms.Label();
            this.labCancelBeauty = new System.Windows.Forms.Label();
            this.labRePhoto = new System.Windows.Forms.Label();
            this.labBeauty = new System.Windows.Forms.Label();
            this.btnChoose3 = new System.Windows.Forms.Button();
            this.btnChoose2 = new System.Windows.Forms.Button();
            this.btnChoose1 = new System.Windows.Forms.Button();
            this.photo3 = new AForge.Controls.PictureBox();
            this.photo2 = new AForge.Controls.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.photo1 = new AForge.Controls.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.labSubmit4 = new System.Windows.Forms.Label();
            this.picBlankPhoto = new AForge.Controls.PictureBox();
            this.labRTakePhoto = new System.Windows.Forms.Label();
            this.paldDterminePhoto = new System.Windows.Forms.Panel();
            this.palInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picGif)).BeginInit();
            this.palChoosePhoto.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.photo3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.photo2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.photo1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBlankPhoto)).BeginInit();
            this.paldDterminePhoto.SuspendLayout();
            this.SuspendLayout();
            // 
            // labRetTime2
            // 
            this.labRetTime2.BackColor = System.Drawing.Color.Transparent;
            this.labRetTime2.Font = new System.Drawing.Font("微软雅黑", 20F);
            this.labRetTime2.ForeColor = System.Drawing.Color.Red;
            this.labRetTime2.Location = new System.Drawing.Point(547, 554);
            this.labRetTime2.Name = "labRetTime2";
            this.labRetTime2.Size = new System.Drawing.Size(278, 76);
            this.labRetTime2.TabIndex = 2;
            this.labRetTime2.Text = "30";
            this.labRetTime2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labRetTime2.Click += new System.EventHandler(this.labRetTime_Click);
            // 
            // vispShoot
            // 
            this.vispShoot.BackColor = System.Drawing.Color.White;
            this.vispShoot.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.vispShoot.Location = new System.Drawing.Point(401, 91);
            this.vispShoot.Name = "vispShoot";
            this.vispShoot.Size = new System.Drawing.Size(580, 435);
            this.vispShoot.TabIndex = 21;
            this.vispShoot.Text = "videoSourcePlayer1";
            this.vispShoot.VideoSource = null;
            this.vispShoot.Paint += new System.Windows.Forms.PaintEventHandler(this.vispShoot_Paint);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // palInfo
            // 
            this.palInfo.BackgroundImage = global::JSCardKiosks.Properties.Resources.拍照前的注意事项;
            this.palInfo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.palInfo.Controls.Add(this.labSure1);
            this.palInfo.Controls.Add(this.picGif);
            this.palInfo.Controls.Add(this.labInfoTime1);
            this.palInfo.Location = new System.Drawing.Point(0, 600);
            this.palInfo.Name = "palInfo";
            this.palInfo.Size = new System.Drawing.Size(1382, 651);
            this.palInfo.TabIndex = 23;
            // 
            // labSure1
            // 
            this.labSure1.BackColor = System.Drawing.Color.Transparent;
            this.labSure1.Font = new System.Drawing.Font("微软雅黑", 20F);
            this.labSure1.ForeColor = System.Drawing.Color.DimGray;
            this.labSure1.Location = new System.Drawing.Point(983, 556);
            this.labSure1.Name = "labSure1";
            this.labSure1.Size = new System.Drawing.Size(342, 70);
            this.labSure1.TabIndex = 4;
            this.labSure1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.labSure1.Click += new System.EventHandler(this.labSure_Click);
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
            // labInfoTime1
            // 
            this.labInfoTime1.AutoSize = true;
            this.labInfoTime1.BackColor = System.Drawing.Color.Transparent;
            this.labInfoTime1.Font = new System.Drawing.Font("微软雅黑", 20F);
            this.labInfoTime1.ForeColor = System.Drawing.Color.Red;
            this.labInfoTime1.Location = new System.Drawing.Point(643, 598);
            this.labInfoTime1.Name = "labInfoTime1";
            this.labInfoTime1.Size = new System.Drawing.Size(47, 35);
            this.labInfoTime1.TabIndex = 3;
            this.labInfoTime1.Text = "10";
            this.labInfoTime1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // palChoosePhoto
            // 
            this.palChoosePhoto.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("palChoosePhoto.BackgroundImage")));
            this.palChoosePhoto.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.palChoosePhoto.Controls.Add(this.labWaitTxt);
            this.palChoosePhoto.Controls.Add(this.labsubmitPhoto3);
            this.palChoosePhoto.Controls.Add(this.labCancelBeauty);
            this.palChoosePhoto.Controls.Add(this.labRePhoto);
            this.palChoosePhoto.Controls.Add(this.labBeauty);
            this.palChoosePhoto.Controls.Add(this.btnChoose3);
            this.palChoosePhoto.Controls.Add(this.btnChoose2);
            this.palChoosePhoto.Controls.Add(this.btnChoose1);
            this.palChoosePhoto.Controls.Add(this.photo3);
            this.palChoosePhoto.Controls.Add(this.photo2);
            this.palChoosePhoto.Controls.Add(this.label3);
            this.palChoosePhoto.Controls.Add(this.photo1);
            this.palChoosePhoto.Controls.Add(this.label4);
            this.palChoosePhoto.Location = new System.Drawing.Point(0, 600);
            this.palChoosePhoto.Name = "palChoosePhoto";
            this.palChoosePhoto.Size = new System.Drawing.Size(1382, 651);
            this.palChoosePhoto.TabIndex = 24;
            this.palChoosePhoto.Visible = false;
            // 
            // labWaitTxt
            // 
            this.labWaitTxt.AutoSize = true;
            this.labWaitTxt.BackColor = System.Drawing.Color.Transparent;
            this.labWaitTxt.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labWaitTxt.ForeColor = System.Drawing.Color.Red;
            this.labWaitTxt.Location = new System.Drawing.Point(1046, 598);
            this.labWaitTxt.Name = "labWaitTxt";
            this.labWaitTxt.Size = new System.Drawing.Size(315, 28);
            this.labWaitTxt.TabIndex = 15;
            this.labWaitTxt.Text = "照片背景正在处理中，请稍后......";
            this.labWaitTxt.Visible = false;
            // 
            // labsubmitPhoto3
            // 
            this.labsubmitPhoto3.BackColor = System.Drawing.Color.Transparent;
            this.labsubmitPhoto3.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labsubmitPhoto3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(151)))), ((int)(((byte)(220)))));
            this.labsubmitPhoto3.Location = new System.Drawing.Point(904, 464);
            this.labsubmitPhoto3.Name = "labsubmitPhoto3";
            this.labsubmitPhoto3.Size = new System.Drawing.Size(112, 134);
            this.labsubmitPhoto3.TabIndex = 14;
            this.labsubmitPhoto3.Text = "30";
            this.labsubmitPhoto3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labsubmitPhoto3.Click += new System.EventHandler(this.labsubmitPhoto_Click);
            // 
            // labCancelBeauty
            // 
            this.labCancelBeauty.BackColor = System.Drawing.Color.Transparent;
            this.labCancelBeauty.Location = new System.Drawing.Point(334, 490);
            this.labCancelBeauty.Name = "labCancelBeauty";
            this.labCancelBeauty.Size = new System.Drawing.Size(112, 124);
            this.labCancelBeauty.TabIndex = 13;
            this.labCancelBeauty.Click += new System.EventHandler(this.labCancelBeauty_Click);
            // 
            // labRePhoto
            // 
            this.labRePhoto.BackColor = System.Drawing.Color.Transparent;
            this.labRePhoto.Location = new System.Drawing.Point(714, 490);
            this.labRePhoto.Name = "labRePhoto";
            this.labRePhoto.Size = new System.Drawing.Size(112, 124);
            this.labRePhoto.TabIndex = 11;
            this.labRePhoto.Click += new System.EventHandler(this.labRePhoto_Click);
            // 
            // labBeauty
            // 
            this.labBeauty.BackColor = System.Drawing.Color.Transparent;
            this.labBeauty.Location = new System.Drawing.Point(534, 490);
            this.labBeauty.Name = "labBeauty";
            this.labBeauty.Size = new System.Drawing.Size(112, 124);
            this.labBeauty.TabIndex = 10;
            this.labBeauty.Click += new System.EventHandler(this.labBeauty_Click);
            // 
            // btnChoose3
            // 
            this.btnChoose3.BackColor = System.Drawing.Color.White;
            this.btnChoose3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnChoose3.BackgroundImage")));
            this.btnChoose3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnChoose3.FlatAppearance.BorderSize = 0;
            this.btnChoose3.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.btnChoose3.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.btnChoose3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.btnChoose3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChoose3.Location = new System.Drawing.Point(963, 391);
            this.btnChoose3.Name = "btnChoose3";
            this.btnChoose3.Size = new System.Drawing.Size(112, 70);
            this.btnChoose3.TabIndex = 9;
            this.btnChoose3.UseVisualStyleBackColor = false;
            this.btnChoose3.Visible = false;
            // 
            // btnChoose2
            // 
            this.btnChoose2.BackColor = System.Drawing.Color.White;
            this.btnChoose2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnChoose2.BackgroundImage")));
            this.btnChoose2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnChoose2.FlatAppearance.BorderSize = 0;
            this.btnChoose2.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.btnChoose2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.btnChoose2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.btnChoose2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChoose2.Location = new System.Drawing.Point(631, 392);
            this.btnChoose2.Name = "btnChoose2";
            this.btnChoose2.Size = new System.Drawing.Size(112, 70);
            this.btnChoose2.TabIndex = 9;
            this.btnChoose2.UseVisualStyleBackColor = false;
            this.btnChoose2.Visible = false;
            // 
            // btnChoose1
            // 
            this.btnChoose1.BackColor = System.Drawing.Color.White;
            this.btnChoose1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnChoose1.BackgroundImage")));
            this.btnChoose1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnChoose1.FlatAppearance.BorderSize = 0;
            this.btnChoose1.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.btnChoose1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.btnChoose1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.btnChoose1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChoose1.Location = new System.Drawing.Point(299, 392);
            this.btnChoose1.Name = "btnChoose1";
            this.btnChoose1.Size = new System.Drawing.Size(112, 70);
            this.btnChoose1.TabIndex = 9;
            this.btnChoose1.UseVisualStyleBackColor = false;
            this.btnChoose1.Visible = false;
            // 
            // photo3
            // 
            this.photo3.Image = null;
            this.photo3.Location = new System.Drawing.Point(896, 91);
            this.photo3.Name = "photo3";
            this.photo3.Size = new System.Drawing.Size(240, 294);
            this.photo3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.photo3.TabIndex = 8;
            this.photo3.TabStop = false;
            this.photo3.Click += new System.EventHandler(this.photo_Click);
            // 
            // photo2
            // 
            this.photo2.Image = null;
            this.photo2.Location = new System.Drawing.Point(570, 92);
            this.photo2.Name = "photo2";
            this.photo2.Size = new System.Drawing.Size(240, 294);
            this.photo2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.photo2.TabIndex = 7;
            this.photo2.TabStop = false;
            this.photo2.Click += new System.EventHandler(this.photo_Click);
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 20F);
            this.label3.ForeColor = System.Drawing.Color.DimGray;
            this.label3.Location = new System.Drawing.Point(143, 560);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(264, 66);
            this.label3.TabIndex = 6;
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // photo1
            // 
            this.photo1.Image = null;
            this.photo1.Location = new System.Drawing.Point(244, 92);
            this.photo1.Name = "photo1";
            this.photo1.Size = new System.Drawing.Size(240, 294);
            this.photo1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.photo1.TabIndex = 5;
            this.photo1.TabStop = false;
            this.photo1.Click += new System.EventHandler(this.photo_Click);
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 20F);
            this.label4.ForeColor = System.Drawing.Color.DimGray;
            this.label4.Location = new System.Drawing.Point(983, 556);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(270, 70);
            this.label4.TabIndex = 4;
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labSubmit4
            // 
            this.labSubmit4.BackColor = System.Drawing.Color.Transparent;
            this.labSubmit4.Font = new System.Drawing.Font("微软雅黑", 20F);
            this.labSubmit4.ForeColor = System.Drawing.Color.Red;
            this.labSubmit4.Location = new System.Drawing.Point(983, 556);
            this.labSubmit4.Name = "labSubmit4";
            this.labSubmit4.Size = new System.Drawing.Size(270, 70);
            this.labSubmit4.TabIndex = 4;
            this.labSubmit4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labSubmit4.Click += new System.EventHandler(this.labSubmit4_Click);
            // 
            // picBlankPhoto
            // 
            this.picBlankPhoto.Image = null;
            this.picBlankPhoto.Location = new System.Drawing.Point(114, 106);
            this.picBlankPhoto.Name = "picBlankPhoto";
            this.picBlankPhoto.Size = new System.Drawing.Size(341, 420);
            this.picBlankPhoto.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picBlankPhoto.TabIndex = 5;
            this.picBlankPhoto.TabStop = false;
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
            this.labRTakePhoto.Click += new System.EventHandler(this.labRTakePhoto_Click);
            // 
            // paldDterminePhoto
            // 
            this.paldDterminePhoto.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("paldDterminePhoto.BackgroundImage")));
            this.paldDterminePhoto.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.paldDterminePhoto.Controls.Add(this.labRTakePhoto);
            this.paldDterminePhoto.Controls.Add(this.picBlankPhoto);
            this.paldDterminePhoto.Controls.Add(this.labSubmit4);
            this.paldDterminePhoto.Location = new System.Drawing.Point(0, 600);
            this.paldDterminePhoto.Name = "paldDterminePhoto";
            this.paldDterminePhoto.Size = new System.Drawing.Size(1382, 651);
            this.paldDterminePhoto.TabIndex = 22;
            this.paldDterminePhoto.Visible = false;
            // 
            // TakePhoto
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::JSCardKiosks.Properties.Resources.拍照;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1382, 651);
            this.Controls.Add(this.paldDterminePhoto);
            this.Controls.Add(this.palChoosePhoto);
            this.Controls.Add(this.palInfo);
            this.Controls.Add(this.vispShoot);
            this.Controls.Add(this.labRetTime2);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Location = new System.Drawing.Point(0, 600);
            this.Name = "TakePhoto";
            this.Text = "Form5";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TakePhoto_FormClosing);
            this.Load += new System.EventHandler(this.TakePhoto_Load);
            this.palInfo.ResumeLayout(false);
            this.palInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picGif)).EndInit();
            this.palChoosePhoto.ResumeLayout(false);
            this.palChoosePhoto.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.photo3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.photo2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.photo1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBlankPhoto)).EndInit();
            this.paldDterminePhoto.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label labRetTime2;
        private AForge.Controls.VideoSourcePlayer vispShoot;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel palInfo;
        private System.Windows.Forms.Label labSure1;
        private System.Windows.Forms.PictureBox picGif;
        private System.Windows.Forms.Label labInfoTime1;
        private System.Windows.Forms.Panel palChoosePhoto;
        private System.Windows.Forms.Label label3;
        private AForge.Controls.PictureBox photo1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnChoose3;
        private System.Windows.Forms.Button btnChoose2;
        private System.Windows.Forms.Button btnChoose1;
        private AForge.Controls.PictureBox photo3;
        private AForge.Controls.PictureBox photo2;
        private System.Windows.Forms.Label labRePhoto;
        private System.Windows.Forms.Label labBeauty;
        private System.Windows.Forms.Label labCancelBeauty;
        private System.Windows.Forms.Label labsubmitPhoto3;
        private System.Windows.Forms.Label labSubmit4;
        private AForge.Controls.PictureBox picBlankPhoto;
        private System.Windows.Forms.Label labRTakePhoto;
        private System.Windows.Forms.Panel paldDterminePhoto;
        private System.Windows.Forms.Label labWaitTxt;
    }
}