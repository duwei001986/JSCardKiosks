using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JSCardKiosks
{
    public partial class SearchCardInfo : Form
    {
        IdInfo idInfo = new IdInfo();
        int iTime = 60;
        MainForm mainForm;
        public SearchCardInfo(IdInfo idInfo)
        {
            this.idInfo = idInfo;
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            iTime--;
            labRetMain.Text = iTime.ToString() + "S后返回主界面";
            if(iTime<0)
            {
                this.Close();
            }
        }

        private void SearchCardInfo_Load(object sender, EventArgs e)
        {
            mainForm = (MainForm)this.Owner;
            labRetMain.Text = iTime.ToString()+ "S后返回主界面";
            string strRet = "";
            labName.Text = idInfo.name;
            labIDNum.Text = idInfo.num;
            strRet = AccessWebInterFace.Interface3_52(idInfo.num, idInfo.name);
            if(strRet.Substring(0,2) =="-1")
            {
                string[] paras1 = { iTime.ToString(), "网路连接异常，请联系工作人员"  };
                BlackForm blackForm1 = new BlackForm(2, "JSCardKiosks.MsgForm", paras1);
                blackForm1.Show(this);
                return;
            }
            labCardStatus.Text = strRet;
            string netName = "";
            string netAddr = ""; ;
            if (labCardStatus.Text != "查无此人"&&labCardStatus.Text!="未发卡")
            {
                strRet = AccessWebInterFace.Interface3_44(idInfo.num, idInfo.name,out netName,out netAddr);
                if (strRet != "0")
                {
                    string[] paras1 = { iTime.ToString(), "网路连接异常，请联系工作人员" };
                    BlackForm blackForm1 = new BlackForm(2, "JSCardKiosks.MsgForm", paras1);
                    blackForm1.Show(this);
                    return;
                }
            }
            labNetName.Text = netName;
            labNetAddr.Text = netAddr;
            string bankCode = "";
            string bankCardNo = "";
            string hasOpenCount = "";
            string hasActivate = "";
            if (labCardStatus.Text != "查无此人" && labCardStatus.Text != "未发卡")
            {
                strRet = AccessWebInterFace.Interface3_3(idInfo.num, idInfo.name, out bankCode, out bankCardNo, out hasOpenCount, out hasActivate);
                if (strRet != "0")
                {
                    string[] paras1 = { iTime.ToString(), strRet.Substring(3) };
                    BlackForm blackForm1 = new BlackForm(2, "JSCardKiosks.MsgForm", paras1);
                    blackForm1.Show(this);
                    return;
                }
            }
            
            labBankCardNo.Text = bankCardNo;
            labBankName.Text = AccessWebInterFace.GetBankName(bankCode);
            labFinance.Text = hasActivate;
            labOpenCount.Text = hasOpenCount;
            string cardProgress ="";
            strRet = AccessWebInterFace.Interface3_7(idInfo.num, idInfo.name,  out cardProgress);
            if (strRet != "0")
            {
                string[] paras1 = { iTime.ToString(), strRet.Substring(3) };
                BlackForm blackForm1 = new BlackForm(2, "JSCardKiosks.MsgForm", paras1);
                blackForm1.Show(this);
                return;
            }
            setProgressPic(Convert.ToInt32(cardProgress)+1);
        }
        public void setProgressPic(int CurPos)
        {
            for (int i = 1; i <= CurPos; i++)
            {
                string labelName = "label" + Convert.ToString(i);
                string btnName = "button" + Convert.ToString(i);
                string labelName2 = "lab" + Convert.ToString(i);
                foreach (Control item in this.Controls)
                {
                    if (item.Name == labelName)
                    {
                        if (i <= CurPos)
                            item.ForeColor = Color.Lime;
                    }
                    if (item.Name == btnName)
                    {
                        if (i <= CurPos)
                            item.BackgroundImage = Properties.Resources.green;
                    }
                    if (item.Name == labelName2)
                    {
                        if (i <= CurPos-1)
                            item.ForeColor = Color.Lime;
                    }
                }
            }


        }

        private void SearchCardInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            mainForm.SetbtnRetmainVisable(false);
            mainForm.SetpalMidVisable(false);
            mainForm.SetHeadText("社会保障卡发卡终端");
            timer1.Stop();
            this.Dispose();
        }
    }
}
