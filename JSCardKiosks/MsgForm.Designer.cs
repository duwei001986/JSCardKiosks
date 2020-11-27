namespace JSCardKiosks
{
    partial class MsgForm
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
            this.labErrText = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.labHead = new System.Windows.Forms.Label();
            this.labRet = new System.Windows.Forms.Label();
            this.labCancel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labErrText
            // 
            this.labErrText.Font = new System.Drawing.Font("黑体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labErrText.ForeColor = System.Drawing.Color.DimGray;
            this.labErrText.Location = new System.Drawing.Point(153, 101);
            this.labErrText.Name = "labErrText";
            this.labErrText.Size = new System.Drawing.Size(396, 128);
            this.labErrText.TabIndex = 0;
            this.labErrText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // labHead
            // 
            this.labHead.AutoSize = true;
            this.labHead.Font = new System.Drawing.Font("黑体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labHead.Location = new System.Drawing.Point(264, 29);
            this.labHead.Name = "labHead";
            this.labHead.Size = new System.Drawing.Size(133, 29);
            this.labHead.TabIndex = 3;
            this.labHead.Text = "信息提示";
            // 
            // labRet
            // 
            this.labRet.BackColor = System.Drawing.Color.Transparent;
            this.labRet.Font = new System.Drawing.Font("黑体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labRet.ForeColor = System.Drawing.Color.OrangeRed;
            this.labRet.Location = new System.Drawing.Point(446, 289);
            this.labRet.Name = "labRet";
            this.labRet.Size = new System.Drawing.Size(148, 75);
            this.labRet.TabIndex = 4;
            this.labRet.Text = "确定";
            this.labRet.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labRet.Click += new System.EventHandler(this.labRet_Click);
            // 
            // labCancel
            // 
            this.labCancel.BackColor = System.Drawing.Color.Transparent;
            this.labCancel.Font = new System.Drawing.Font("黑体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labCancel.Location = new System.Drawing.Point(57, 292);
            this.labCancel.Name = "labCancel";
            this.labCancel.Size = new System.Drawing.Size(155, 68);
            this.labCancel.TabIndex = 5;
            this.labCancel.Text = "取消";
            this.labCancel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labCancel.Click += new System.EventHandler(this.labCancel_Click);
            // 
            // MsgForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = global::JSCardKiosks.Properties.Resources.msgbox2;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(662, 373);
            this.Controls.Add(this.labRet);
            this.Controls.Add(this.labCancel);
            this.Controls.Add(this.labHead);
            this.Controls.Add(this.labErrText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MsgForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form2";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form2_FormClosed);
            this.Load += new System.EventHandler(this.Form2_Load);
            this.Resize += new System.EventHandler(this.Form2_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labErrText;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label labHead;
        private System.Windows.Forms.Label labRet;
        private System.Windows.Forms.Label labCancel;
    }
}