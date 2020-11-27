namespace JSCardKiosks
{
    partial class MainPal
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainPal));
            this.lbtnIDCard = new System.Windows.Forms.Label();
            this.labQrRead = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbtnIDCard
            // 
            this.lbtnIDCard.BackColor = System.Drawing.Color.Transparent;
            this.lbtnIDCard.Location = new System.Drawing.Point(552, 226);
            this.lbtnIDCard.Name = "lbtnIDCard";
            this.lbtnIDCard.Size = new System.Drawing.Size(401, 500);
            this.lbtnIDCard.TabIndex = 0;
            this.lbtnIDCard.Click += new System.EventHandler(this.lbtnIDCard_Click);
            // 
            // labQrRead
            // 
            this.labQrRead.BackColor = System.Drawing.Color.Transparent;
            this.labQrRead.Location = new System.Drawing.Point(3, 226);
            this.labQrRead.Name = "labQrRead";
            this.labQrRead.Size = new System.Drawing.Size(518, 239);
            this.labQrRead.TabIndex = 1;
            this.labQrRead.Click += new System.EventHandler(this.labQrRead_Click);
            // 
            // MainPal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(1389, 850);
            this.Controls.Add(this.labQrRead);
            this.Controls.Add(this.lbtnIDCard);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MainPal";
            this.Text = "StartForm";
            this.Load += new System.EventHandler(this.MainPal_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbtnIDCard;
        private System.Windows.Forms.Label labQrRead;
    }
}