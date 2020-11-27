using HttpPostTest;
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
    public partial class InputPhoneNur : Form
    {
        string temp = "";
        MainForm mainForm;
        int iTime = 60;
        CardBaseData cardBaseData = new CardBaseData("", "");
        public InputPhoneNur(CardBaseData cardBaseData)
        {
            this.cardBaseData = cardBaseData;
            InitializeComponent();
        }
        public InputPhoneNur()
        {
            InitializeComponent();
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
        private void InputPhoneNur_Click(object sender, EventArgs e)
        {
            labNumErrInfo.Text = "";
            Button button = (Button)sender;
            temp = button.Name.Substring(button.Name.Length - 1);
            textBoxPhone.Text += temp;
            textBoxPhone.SelectionStart = textBoxPhone.Text.Length;
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            labNumErrInfo.Text = "";
            textBoxPhone.Clear();
        }

        private void buttonNX_Click(object sender, EventArgs e)
        {
            labNumErrInfo.Text = "";
            if (textBoxPhone.Text != null)
            {
                try
                {
                    textBoxPhone.Text = textBoxPhone.Text.Substring(0, textBoxPhone.Text.Length - 1);
                }
                catch (Exception)
                {
                    return;
                }
            }
        }
        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (txtWork.Text == "")
            {
                labChooseErr.Text = "请您选择职业!";
                labChooseErr.ForeColor = Color.Red;
                iTime = 30;
                return;
            }
            else
            {
                labNumErrInfo.Text = "";
            }
            #region 输入手机号码
            bool IsRightNur = System.Text.RegularExpressions.Regex.IsMatch(textBoxPhone.Text, @"^1[3456789]\d{9}$");
            if (!IsRightNur)
            {
                labNumErrInfo.Text = "您输入的手机号码不正确!";
                labNumErrInfo.ForeColor = Color.Red;
                textBoxPhone.Focus();
                PublicFuction.SoundPlay("您输入的手机号码不正确请重新输入.wav");
                iTime = 30;
                return;
            }
            else
            {
                labNumErrInfo.Text = "";
            }
            #endregion
            cardBaseData.Aae005 = textBoxPhone.Text;
            cardBaseData.Bac025 =WebInfeFaceHelp. GetJobCode( txtWork.Text);
            string strErr = "";
            int iret = WebInfeFaceHelp.GetDataFromWeb(cardBaseData, out strErr);
            if (iret == -1)
            {
                string[] paras = { iTime.ToString(), "下载社保数据失败，" + strErr };
                BlackForm blackForm = new BlackForm(2, "JSCardKiosks.MsgForm", paras);
                blackForm.Show(this);
            }
            else
            {
                mainForm.SetbtnRetmainVisable(true);
                object[] objPara = new object[1];
                objPara[0] = cardBaseData;
                mainForm.ShowChildForm("JSCardKiosks.ShowCardInfo", 1, objPara);
                timer1.Stop();
                timer1.Dispose();
                this.Dispose();
            }




        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            iTime--;
            labSure.Text = iTime.ToString().PadLeft(2, '0');
            if (iTime <= 0)
            {
                this.Close();
            }
        }

        private void InputPhoneNur_FormClosing(object sender, FormClosingEventArgs e)
        {
            mainForm.SetbtnRetmainVisable(false);
            mainForm.SetpalMidVisable(false);
            mainForm.SetHeadText("社会保障卡发卡终端");
            timer1.Stop();
            timer1.Dispose();
            this.Dispose();
        }

        private void InputPhoneNur_Load(object sender, EventArgs e)
        {
            mainForm = (MainForm)this.Owner;
            PublicFuction.SoundPlay("请输入您的手机号码.wav");
        }


        private void listBox1_Click(object sender, EventArgs e)
        {
            txtWork.Text = listBox1.SelectedItem.ToString();
        }
    }
}
