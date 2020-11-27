using JSCardKiosks;
using CZIDCardReader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace JSCardKiosks
{

    public struct IdInfo
    {
        public string name;
        public string sex;
        public string nation;
        public string birth;
        public string address;
        public string num;
        public string start;
        public string end;
        public string phoneNur;
        public int BusinessType;
    }
    public struct CardDataInfo
    {
        public string name;
        public string idNum;
        public string cardDate;
        public string cardNo;
        public string sex;
        public string nation;
        public string birth;
        public string nameEx;
        public string birthAdrrCode;
        public string personID;
        public string adminAreaCode;
        public string strBatch;
        public string photo;
        public string job;
        public string netBatch;
    }
    public partial class MainForm : Form
    {
        public const int TYPE_MAINFORM = 1;
        public const int TYPE_CHILDFORM = 2;
        [DllImport("user32.dll")]
        static extern bool LockWindowUpdate(IntPtr hWndLock);
        bool btnDown = false;
        double iWidthRate;
        double iHeigthRate;
        public MainForm()
        {
            InitializeComponent();
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime dtime = DateTime.Now;
            string strWeek = dtime.ToString("dddd", new System.Globalization.CultureInfo("zh-cn"));
            string strDate = string.Format("{0:yyyy/MM/dd}", dtime);
            string strTime = string.Format("{0:t}", dtime);
            labDate.Text = strDate + " " + strWeek;
            labTime.Text = strTime;
        }
        public void CreateImg()
        {
            StreamReader sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "sysword.dat");
            string s = sr.ReadToEnd();
            sr.Close();
            byte[] buf = Convert.FromBase64String(s);//把字符串读到字节数组中
            MemoryStream ms = new MemoryStream(buf);
            System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
            picLogo.Image = img;
            ms.Close();
            ms.Dispose();
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            GetSreenDPI getDpi = new GetSreenDPI();
            iWidthRate = getDpi.DWidthRate;
            iHeigthRate = getDpi.DHeightRate;
            this.Width = (int)(1920 * iWidthRate);
            this.Height = (int)(1080 * iHeigthRate);
            int oldPalWidth = palMidShow.Width;
            int oldPalHeight = palMidShow.Height;
            int oldpalY = palMidShow.Location.Y;
            setSreenDPI(iWidthRate, iHeigthRate, this, TYPE_MAINFORM);
            palMidShow.Width = Convert.ToInt32(oldPalWidth * (iWidthRate + iHeigthRate) / 2);
            palMidShow.Height = Convert.ToInt32(oldPalHeight * (iWidthRate + iHeigthRate) / 2);
            palMidShow.Location = new Point((this.Width - palMidShow.Width) / 2, palMidShow.Location.Y);
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "sysword.dat"))
            {
                CreateImg();
            }
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "OutService.dll"))
            {
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + "OutService.dll");
            }
            SetHeadText("发卡终端件自检");
            ShowChildForm("JSCardKiosks.MechineCheck", 0, new string[] { "", "" });


        }
        public void ShowChildForm(string formName, int paramcount, object[] paras)
        {
            foreach (Control control in palMidShow.Controls)//
            {
                if (control.GetType().ToString() == "System.Windows.Forms.Form")
                {
                    (control as Form).Close();
                }
            }
            LockWindowUpdate(palMidShow.Handle);
            this.palMidShow.Controls.Clear();
            Assembly assembly = Assembly.GetExecutingAssembly();
            dynamic obj;
            if (paramcount == 0)
            {
                obj = assembly.CreateInstance(formName);
            }
            else
            {
                obj = assembly.CreateInstance(formName, true, System.Reflection.BindingFlags.Default, null, paras, null, null);// 创建类的实例 
            }
            obj.Owner = this;
            obj.TopLevel = false;
            obj.Dock = System.Windows.Forms.DockStyle.Fill;
            palMidShow.Controls.Add(obj);
            setSreenDPI(iWidthRate, iHeigthRate, obj, TYPE_CHILDFORM);

            obj.Show();
            LockWindowUpdate(IntPtr.Zero);
            palMidShow.Visible = true;
        }
        public void setSreenDPI(double iWidthRate, double iHeigthRate, Control formControl, int iType)
        {
            if (iType == TYPE_CHILDFORM)
            {
                iWidthRate = iHeigthRate = (iWidthRate + iHeigthRate) / 2;
            }
            foreach (Control control in formControl.Controls)
            {

                control.Width = Convert.ToInt32((control.Width) * iWidthRate);
                control.Height = Convert.ToInt32((control.Height) * iHeigthRate);
                control.Location = new Point(Convert.ToInt32(control.Location.X * iWidthRate), Convert.ToInt32(control.Location.Y * iHeigthRate));
                // control.Font  = new Font(control.Font.FontFamily, Convert.ToInt32((control.Font.Size * (iWidthRate + iHeigthRate) / 2)), control.Font.Style);
                if (control is Button)
                {
                    Button button = (Button)control;
                    button.Font = new Font(button.Font.FontFamily, Convert.ToInt32((button.Font.Size * (iWidthRate + iHeigthRate) / 2)), button.Font.Style);
                }
                if (control is Label)
                {
                    Label label = (Label)control;
                    label.Font = new Font(label.Font.FontFamily, Convert.ToInt32((label.Font.Size * (iWidthRate + iHeigthRate) / 2)), label.Font.Style);
                }
                if (control is TextBox)
                {
                    TextBox label = (TextBox)control;
                    label.Font = new Font(label.Font.FontFamily, Convert.ToInt32((label.Font.Size * (iWidthRate + iHeigthRate) / 2)), label.Font.Style);
                }
                if (control is ListBox)
                {
                    ListBox label = (ListBox)control;
                    label.Font = new Font(label.Font.FontFamily, Convert.ToInt32((label.Font.Size * (iWidthRate + iHeigthRate) / 2)), label.Font.Style);
                }
                if (control is Panel)
                {
                    Panel panel = (Panel)control;
                    foreach (Control control2 in panel.Controls)
                    {
                        control2.Width = Convert.ToInt32((control2.Width) * iWidthRate);
                        control2.Height = Convert.ToInt32((control2.Height) * iHeigthRate);
                        control2.Location = new Point(Convert.ToInt32(control2.Location.X * iWidthRate), Convert.ToInt32(control2.Location.Y * iHeigthRate));
                        if (control2 is Label)
                        {
                            Label label = (Label)control2;
                            label.Font = new Font(label.Font.FontFamily, Convert.ToInt32((label.Font.Size * (iWidthRate + iHeigthRate) / 2)), label.Font.Style);
                        }
                    }
                }
            }
        }

        private void btnRetMainPal_Click(object sender, EventArgs e)
        {
            SetHeadText("社会保障卡发卡终端");
            foreach (Control control in palMidShow.Controls)//
            {
                if (control is Form)
                {
                    Form form = (Form)control;
                    form.Close();
                }
            }
            palMidShow.Controls.Clear();
            palMidShow.Visible = false;
            btnRetMainPal.Visible = false;
        }
        public void SetbtnRetmainVisable(bool bVisable)
        {
            btnRetMainPal.Visible = bVisable;
        }
        public void SetpalMidVisable(bool bVisable)
        {
            palMidShow.Visible = bVisable;
        }
        public void SetHeadText(string strText)
        {
            strText = strText.Trim();
            strText = Regex.Replace(strText, "(?=)", " ");
            labFormhead.Text = strText; ;
        }
        private void labclose1_Click(object sender, EventArgs e)
        {
            btnDown = true;
        }

        private void labclose2_DoubleClick(object sender, EventArgs e)
        {
            if (btnDown)
            {
                string[] paras = { "", };
                BlackForm blackForm = new BlackForm(0, "JSCardKiosks.InputPwdForm", paras);
                blackForm.Show(this);
            }
            btnDown = false;
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
        private void labQrRead_Click(object sender, EventArgs e)
        {
            SetHeadText("扫描二维码制卡");
            SetbtnRetmainVisable(true);
            ShowChildForm("JSCardKiosks.QRScan", 0, new string[] { "", "" });
        }
        private void labManage_Click(object sender, EventArgs e)
        {

            SetHeadText("身份证读取");
            SetbtnRetmainVisable(true);
            object[] objPara = new object[1];
            objPara[0] = "ReportCardLoss";
            if (DealAppConfig.GetAppSettingsValue("身份证阅读器") == "插入式")
            {
                ShowChildForm("JSCardKiosks.IdReadForm", 1, objPara);
            }
            else if (DealAppConfig.GetAppSettingsValue("身份证阅读器") == "非接式")
            {
                ShowChildForm("JSCardKiosks.IdReadForm1", 1, objPara);
            }
        }
        private void lbtnIDCard_Click(object sender, EventArgs e)
        {
            SetHeadText("读取身份证信息");
            SetbtnRetmainVisable(true);
            if (DealAppConfig.GetAppSettingsValue("身份证阅读器") == "插入式")
            {
                ShowChildForm("JSCardKiosks.IdReadForm", 0, new string[] { "", "" });
            }
            else if (DealAppConfig.GetAppSettingsValue("身份证阅读器") == "非接式")
            {
                ShowChildForm("JSCardKiosks.IdReadForm1", 0, new string[] { "", "" });
            }
        }
        private void labSearch_Click(object sender, EventArgs e)
        {
            SetHeadText("身份证读取");
            SetbtnRetmainVisable(true);
            object[] objPara = new object[1];
            objPara[0] = "SearchCardInfo";
            if (DealAppConfig.GetAppSettingsValue("身份证阅读器") == "插入式")
            {
                ShowChildForm("JSCardKiosks.IdReadForm", 1, objPara);
            }
            else if (DealAppConfig.GetAppSettingsValue("身份证阅读器") == "非接式")
            {
                ShowChildForm("JSCardKiosks.IdReadForm1", 1, objPara);
            }
        }

        private void labNotice_Click(object sender, EventArgs e)
        {

             
            SetHeadText("输入手机号码");
            SetbtnRetmainVisable(true);
            ShowChildForm("JSCardKiosks.InputPhoneNur", 0, new string[] { "", "" });
        }

        private void labFormhead_Click(object sender, EventArgs e)
        {

        }
    }
}
