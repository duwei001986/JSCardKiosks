namespace JSCardKiosks
{
    partial class InputIDNur
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
            this.labRetTime = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.txtIdNum = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonOk = new System.Windows.Forms.Button();
            this.labErrInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labRetTime
            // 
            this.labRetTime.AutoSize = true;
            this.labRetTime.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(152)))), ((int)(((byte)(204)))));
            this.labRetTime.Font = new System.Drawing.Font("微软雅黑", 16F);
            this.labRetTime.ForeColor = System.Drawing.Color.Red;
            this.labRetTime.Location = new System.Drawing.Point(1247, 40);
            this.labRetTime.Name = "labRetTime";
            this.labRetTime.Size = new System.Drawing.Size(39, 30);
            this.labRetTime.TabIndex = 60;
            this.labRetTime.Text = "60";
            this.labRetTime.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // txtIdNum
            // 
            this.txtIdNum.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtIdNum.Location = new System.Drawing.Point(736, 41);
            this.txtIdNum.Name = "txtIdNum";
            this.txtIdNum.Size = new System.Drawing.Size(354, 35);
            this.txtIdNum.TabIndex = 64;
            this.txtIdNum.Click += new System.EventHandler(this.txtName_Click);
            // 
            // txtName
            // 
            this.txtName.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtName.Location = new System.Drawing.Point(171, 41);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(354, 35);
            this.txtName.TabIndex = 63;
            this.txtName.Click += new System.EventHandler(this.txtName_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(570, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(123, 35);
            this.label2.TabIndex = 62;
            this.label2.Text = "身份证号";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(72, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 35);
            this.label1.TabIndex = 61;
            this.label1.Text = "姓 名";
            // 
            // buttonOk
            // 
            this.buttonOk.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(152)))), ((int)(((byte)(204)))));
            this.buttonOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonOk.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonOk.ForeColor = System.Drawing.Color.White;
            this.buttonOk.Location = new System.Drawing.Point(1150, 32);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(141, 48);
            this.buttonOk.TabIndex = 65;
            this.buttonOk.Text = "提 交";
            this.buttonOk.UseVisualStyleBackColor = false;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // labErrInfo
            // 
            this.labErrInfo.AutoSize = true;
            this.labErrInfo.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labErrInfo.ForeColor = System.Drawing.Color.Red;
            this.labErrInfo.Location = new System.Drawing.Point(823, 100);
            this.labErrInfo.Name = "labErrInfo";
            this.labErrInfo.Size = new System.Drawing.Size(19, 20);
            this.labErrInfo.TabIndex = 66;
            this.labErrInfo.Text = " ";
            // 
            // InputIDNur
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1382, 651);
            this.Controls.Add(this.labErrInfo);
            this.Controls.Add(this.labRetTime);
            this.Controls.Add(this.txtIdNum);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "InputIDNur";
            this.Text = "InputIDNur";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InputIDNur_FormClosing);
            this.Load += new System.EventHandler(this.InputIDNur_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labRetTime;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox txtIdNum;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Label labErrInfo;
    }
}