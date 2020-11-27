using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Windows.Forms;

namespace JSCardKiosks
{
    public partial class MainPal : Form
    {
        MainForm mainForm;
        public MainPal()
        {
            InitializeComponent();
        }
        private void MainPal_Load(object sender, EventArgs e)
        {
            mainForm = (MainForm)this.Owner;
        }
        private void lbtnIDCard_Click(object sender, EventArgs e)
        {
            mainForm.SetbtnRetmainVisable(true);
            mainForm.ShowChildForm("JSCardKiosks.IdReadForm",0, new string[] { "", "" });
        }

        private void labQrRead_Click(object sender, EventArgs e)
        {
           
        }
    }
}
