namespace JSCardKiosks
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.labFormhead = new System.Windows.Forms.Label();
            this.labTime = new System.Windows.Forms.Label();
            this.labDate = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnRetMainPal = new System.Windows.Forms.Button();
            this.labclose1 = new System.Windows.Forms.Label();
            this.labclose2 = new System.Windows.Forms.Label();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.labQrRead = new System.Windows.Forms.Label();
            this.lbtnIDCard = new System.Windows.Forms.Label();
            this.labManage = new System.Windows.Forms.Label();
            this.labSearch = new System.Windows.Forms.Label();
            this.labNotice = new System.Windows.Forms.Label();
            this.palMidShow = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // labFormhead
            // 
            this.labFormhead.BackColor = System.Drawing.Color.Transparent;
            this.labFormhead.Font = new System.Drawing.Font("微软雅黑", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labFormhead.ForeColor = System.Drawing.Color.White;
            this.labFormhead.Location = new System.Drawing.Point(-1, 126);
            this.labFormhead.Name = "labFormhead";
            this.labFormhead.Size = new System.Drawing.Size(1920, 64);
            this.labFormhead.TabIndex = 1;
            this.labFormhead.Text = "发 卡 终 端 件 自 检";
            this.labFormhead.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.labFormhead.Click += new System.EventHandler(this.labFormhead_Click);
            // 
            // labTime
            // 
            this.labTime.BackColor = System.Drawing.Color.Transparent;
            this.labTime.Font = new System.Drawing.Font("微软雅黑", 30F);
            this.labTime.ForeColor = System.Drawing.Color.White;
            this.labTime.Location = new System.Drawing.Point(1732, 23);
            this.labTime.Name = "labTime";
            this.labTime.Size = new System.Drawing.Size(143, 48);
            this.labTime.TabIndex = 2;
            this.labTime.Text = "20:08";
            this.labTime.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labDate
            // 
            this.labDate.BackColor = System.Drawing.Color.Transparent;
            this.labDate.Font = new System.Drawing.Font("微软雅黑", 20F);
            this.labDate.ForeColor = System.Drawing.Color.White;
            this.labDate.Location = new System.Drawing.Point(1570, 72);
            this.labDate.Name = "labDate";
            this.labDate.Size = new System.Drawing.Size(301, 47);
            this.labDate.TabIndex = 3;
            this.labDate.Text = "2020";
            this.labDate.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnRetMainPal
            // 
            this.btnRetMainPal.BackColor = System.Drawing.Color.Transparent;
            this.btnRetMainPal.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnRetMainPal.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkBlue;
            this.btnRetMainPal.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Navy;
            this.btnRetMainPal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRetMainPal.Font = new System.Drawing.Font("微软雅黑", 16F, System.Drawing.FontStyle.Bold);
            this.btnRetMainPal.ForeColor = System.Drawing.Color.White;
            this.btnRetMainPal.Location = new System.Drawing.Point(1734, 996);
            this.btnRetMainPal.Margin = new System.Windows.Forms.Padding(0);
            this.btnRetMainPal.Name = "btnRetMainPal";
            this.btnRetMainPal.Size = new System.Drawing.Size(141, 49);
            this.btnRetMainPal.TabIndex = 51;
            this.btnRetMainPal.Text = "返回主页";
            this.btnRetMainPal.UseVisualStyleBackColor = false;
            this.btnRetMainPal.Visible = false;
            this.btnRetMainPal.Click += new System.EventHandler(this.btnRetMainPal_Click);
            // 
            // labclose1
            // 
            this.labclose1.BackColor = System.Drawing.Color.Transparent;
            this.labclose1.Location = new System.Drawing.Point(-1, 1016);
            this.labclose1.Name = "labclose1";
            this.labclose1.Size = new System.Drawing.Size(64, 63);
            this.labclose1.TabIndex = 52;
            this.labclose1.Click += new System.EventHandler(this.labclose1_Click);
            // 
            // labclose2
            // 
            this.labclose2.BackColor = System.Drawing.Color.Transparent;
            this.labclose2.Location = new System.Drawing.Point(916, 60);
            this.labclose2.Name = "labclose2";
            this.labclose2.Size = new System.Drawing.Size(90, 63);
            this.labclose2.TabIndex = 53;
            this.labclose2.DoubleClick += new System.EventHandler(this.labclose2_DoubleClick);
            // 
            // picLogo
            // 
            this.picLogo.BackColor = System.Drawing.Color.Transparent;
            this.picLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.picLogo.Location = new System.Drawing.Point(1, 1);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(646, 118);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLogo.TabIndex = 54;
            this.picLogo.TabStop = false;
            // 
            // labQrRead
            // 
            this.labQrRead.BackColor = System.Drawing.Color.Transparent;
            this.labQrRead.Location = new System.Drawing.Point(269, 358);
            this.labQrRead.Name = "labQrRead";
            this.labQrRead.Size = new System.Drawing.Size(518, 239);
            this.labQrRead.TabIndex = 3;
            this.labQrRead.Click += new System.EventHandler(this.labQrRead_Click);
            // 
            // lbtnIDCard
            // 
            this.lbtnIDCard.BackColor = System.Drawing.Color.Transparent;
            this.lbtnIDCard.Location = new System.Drawing.Point(809, 358);
            this.lbtnIDCard.Name = "lbtnIDCard";
            this.lbtnIDCard.Size = new System.Drawing.Size(401, 500);
            this.lbtnIDCard.TabIndex = 2;
            this.lbtnIDCard.Click += new System.EventHandler(this.lbtnIDCard_Click);
            // 
            // labManage
            // 
            this.labManage.BackColor = System.Drawing.Color.Transparent;
            this.labManage.Location = new System.Drawing.Point(269, 610);
            this.labManage.Name = "labManage";
            this.labManage.Size = new System.Drawing.Size(518, 239);
            this.labManage.TabIndex = 55;
            this.labManage.Click += new System.EventHandler(this.labManage_Click);
            // 
            // labSearch
            // 
            this.labSearch.BackColor = System.Drawing.Color.Transparent;
            this.labSearch.Location = new System.Drawing.Point(1248, 358);
            this.labSearch.Name = "labSearch";
            this.labSearch.Size = new System.Drawing.Size(399, 239);
            this.labSearch.TabIndex = 3;
            this.labSearch.Click += new System.EventHandler(this.labSearch_Click);
            // 
            // labNotice
            // 
            this.labNotice.BackColor = System.Drawing.Color.Transparent;
            this.labNotice.Location = new System.Drawing.Point(1248, 610);
            this.labNotice.Name = "labNotice";
            this.labNotice.Size = new System.Drawing.Size(399, 239);
            this.labNotice.TabIndex = 55;
            this.labNotice.Click += new System.EventHandler(this.labNotice_Click);
            // 
            // palMidShow
            // 
            this.palMidShow.BackColor = System.Drawing.Color.White;
            this.palMidShow.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.palMidShow.Location = new System.Drawing.Point(46, 325);
            this.palMidShow.Name = "palMidShow";
            this.palMidShow.Size = new System.Drawing.Size(1382, 651);
            this.palMidShow.TabIndex = 56;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::JSCardKiosks.Properties.Resources.mainformnj;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1455, 880);
            this.Controls.Add(this.palMidShow);
            this.Controls.Add(this.labNotice);
            this.Controls.Add(this.labManage);
            this.Controls.Add(this.labSearch);
            this.Controls.Add(this.labQrRead);
            this.Controls.Add(this.lbtnIDCard);
            this.Controls.Add(this.labclose2);
            this.Controls.Add(this.labclose1);
            this.Controls.Add(this.btnRetMainPal);
            this.Controls.Add(this.labDate);
            this.Controls.Add(this.labTime);
            this.Controls.Add(this.labFormhead);
            this.Controls.Add(this.picLogo);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labFormhead;
        private System.Windows.Forms.Label labTime;
        private System.Windows.Forms.Label labDate;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnRetMainPal;
        private System.Windows.Forms.Label labclose1;
        private System.Windows.Forms.Label labclose2;
        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.Label labQrRead;
        private System.Windows.Forms.Label lbtnIDCard;
        private System.Windows.Forms.Label labManage;
        private System.Windows.Forms.Label labSearch;
        private System.Windows.Forms.Label labNotice;
        private System.Windows.Forms.Panel palMidShow;
    }
}

