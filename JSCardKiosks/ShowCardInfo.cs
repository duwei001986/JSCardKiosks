using Microsoft.Office.Interop.Excel;
using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using WorkLog;


namespace JSCardKiosks
{

    public partial class ShowCardInfo : Form
    {
        CardDataInfo cardInfo = new CardDataInfo();
        MainForm mainForm;
        int iTime = 10;
        bool bErr = false;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }
        public ShowCardInfo()
        {
            InitializeComponent();
        }
        public ShowCardInfo(CardBaseData cardBaseData)
        {
            cardInfo.name = cardBaseData.Aac003;
            cardInfo.idNum = cardBaseData.Aac147;
            cardInfo.cardDate = cardBaseData.Aaz503;
            cardInfo.cardNo = cardBaseData.Aac500;
            cardInfo.sex = cardBaseData.Aac004;
            cardInfo.nation = cardBaseData.Aac005;
            cardInfo.birth = cardBaseData.Aac006;
            cardInfo.nameEx = cardBaseData.Baz103;
            cardInfo.birthAdrrCode = cardBaseData.Aac025;
            cardInfo.personID = cardBaseData.Baz805;
            cardInfo.photo = cardBaseData.Image;
           //cardInfo.adminAreaCode = dt.Rows[0]["行政区划"].ToString();
           cardInfo.strBatch = cardBaseData.Baz033;
            cardInfo.netBatch = cardBaseData.Baz030;
            InitializeComponent();
        }

        private void ShowCardInfo_Load(object sender, EventArgs e)
        {
            mainForm = (MainForm)this.Owner;
            mainForm.SetHeadText("社保卡信息");
            string strRet = "";
            panel1.Visible = true;
            labName.Text = cardInfo.name;
            labIDCode.Text = cardInfo.idNum;
            labPerID.Text = cardInfo.personID;
            Byte[] filedate = Convert.FromBase64String(cardInfo.photo);
            MemoryStream ms = new MemoryStream(filedate);
            Image image = System.Drawing.Image.FromStream(ms);
            image.Save(System.Windows.Forms.Application.StartupPath + "\\photo\\" + cardInfo.idNum + ".jpg");
            picPhoto.Image = image;
        }

        private void labCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void labSubmit_Click(object sender, EventArgs e)
        {
            labSubmit.Enabled = false;
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            iTime--;
            if (bErr == false)
            {
                btnlabSubmit.Text = iTime.ToString().PadLeft(2, '0');
                if (iTime <= 0)
                {
                    object[] objPara = new object[1];
                    objPara[0] = cardInfo;
                    mainForm.ShowChildForm("JSCardKiosks.MakeCard", 1, objPara);
                    timer1.Stop();
                    timer1.Dispose();
                    this.Dispose();
                }
            }
            else
            {
                if (iTime <= 0)
                {
                    this.Close();
                }
            }
        }

        private void ShowCardInfo_FormClosing(object sender, FormClosingEventArgs e)
        {

            mainForm.SetbtnRetmainVisable(false);
            mainForm.SetpalMidVisable(false);
            mainForm.SetHeadText("社会保障卡发卡终端");
            timer1.Stop();
            timer1.Dispose();
            this.Dispose();
        }

        private void labPalSubmit_Click(object sender, EventArgs e)
        {
            btnlabSubmit.Enabled = false;
            object[] objPara = new object[1];
            objPara[0] = cardInfo;
            mainForm.ShowChildForm("JSCardKiosks.MakeCard", 1, objPara);
            timer1.Stop();
            timer1.Dispose();
            this.Dispose();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
