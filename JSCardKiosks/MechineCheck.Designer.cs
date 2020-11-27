namespace JSCardKiosks
{
    partial class MechineCheck
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
            this.btnSure = new System.Windows.Forms.Button();
            this.listHardwareCheck = new System.Windows.Forms.ListBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // btnSure
            // 
            this.btnSure.BackColor = System.Drawing.Color.Transparent;
            this.btnSure.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSure.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkBlue;
            this.btnSure.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Navy;
            this.btnSure.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSure.Font = new System.Drawing.Font("微软雅黑", 16F, System.Drawing.FontStyle.Bold);
            this.btnSure.ForeColor = System.Drawing.Color.White;
            this.btnSure.Location = new System.Drawing.Point(615, 596);
            this.btnSure.Margin = new System.Windows.Forms.Padding(0);
            this.btnSure.Name = "btnSure";
            this.btnSure.Size = new System.Drawing.Size(181, 52);
            this.btnSure.TabIndex = 50;
            this.btnSure.Text = "确     定";
            this.btnSure.UseVisualStyleBackColor = false;
            this.btnSure.Click += new System.EventHandler(this.btnSure_Click);
            // 
            // listHardwareCheck
            // 
            this.listHardwareCheck.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.listHardwareCheck.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listHardwareCheck.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.listHardwareCheck.FormattingEnabled = true;
            this.listHardwareCheck.ItemHeight = 31;
            this.listHardwareCheck.Location = new System.Drawing.Point(0, 0);
            this.listHardwareCheck.Name = "listHardwareCheck";
            this.listHardwareCheck.Size = new System.Drawing.Size(1382, 593);
            this.listHardwareCheck.TabIndex = 0;
            this.listHardwareCheck.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listHardwareCheck_DrawItem);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 200;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // MechineCheck
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::JSCardKiosks.Properties.Resources.主页;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1382, 651);
            this.Controls.Add(this.btnSure);
            this.Controls.Add(this.listHardwareCheck);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MechineCheck";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MechineCheck_FormClosing);
            this.Load += new System.EventHandler(this.MechineCheck_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSure;
        private System.Windows.Forms.ListBox listHardwareCheck;
        private System.Windows.Forms.Timer timer1;
    }
}