using CZIDCardReader;
using IDCardReader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WorkLog;

namespace JSCardKiosks
{
    public partial class IdReadForm1 : Form
    {
        public const int CARD_BUSINESSTYPE_RESHOOT = 1;
        public const int CARD_BUSINESSTYPE_NEWADD = 2;
        IdInfo idInfo = new IdInfo();
        int iTime = 30;
        int iBeginTime = 0;
        int iRet = 0;
        MainForm mainForm;
        bool bRun;
        string strNextFormName = "";
        public IdReadForm1()
        {
            InitializeComponent();
        }
        public IdReadForm1(string strNextFormName)
        {
            this.strNextFormName = strNextFormName;
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
        private void IDReadForm_Load(object sender, EventArgs e)
        {
            labRetTime.Text = iTime.ToString().PadLeft(2, '0');
            mainForm = (MainForm)this.Owner;
            SoundPlayer player = new SoundPlayer();
            player.SoundLocation = Application.StartupPath + "\\请将身份证放到身份证阅读区.wav";
            player.Load();
            player.Play();
            iBeginTime = iTime - 2;
            bRun = true;
            pictureBox1.Image =PublicFuction.GetGifImage("身份证");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            iTime--;
            labRetTime.Text = iTime.ToString().PadLeft(2, '0');
            if (iTime == iBeginTime && !backgroundWorker1.IsBusy)
            {
                backgroundWorker1.RunWorkerAsync();
            }
            if (iTime <= 0)
            {
                this.Close();
            }
        }

        private void IDReadForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            mainForm.SetbtnRetmainVisable(false);
            mainForm.SetpalMidVisable(false);
            mainForm.SetHeadText("社会保障卡发卡终端");
            bRun = false;
            //ReadIDBadge.CardInDisable();
            //iRet = ReadIDBadge.Uninit();
            timer1.Stop();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (bRun)
            {
                System.Threading.Thread.Sleep(200);
                try
                {
                    iRet = SysIDCardRead(out idInfo);
                }
                catch (Exception ex)
                {
                    WriteWorkLog.WriteLogs("日志", "异常", "读取证件失败" + ex.ToString());
                }
                if (iRet != 0)
                {
                    WriteWorkLog.WriteLogs("日志", "错误", "读取证件失败" + iRet.ToString());
                    idInfo.name = "";
                }
                else
                {
                    bRun = false;
                    backgroundWorker1.ReportProgress(0, idInfo);
                }
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
           IdInfo idInfo = (IdInfo)e.UserState;
            string strErrInfo = "";
            if (idInfo.name != "")
            {
                SoundPlayer player = new SoundPlayer();
                player.SoundLocation = Application.StartupPath + "\\身份证读取成功.wav";
                player.Load();
                player.Play();
                backgroundWorker1.CancelAsync();
                if (!backgroundWorker1.CancellationPending)
                    backgroundWorker1.CancelAsync();
                object[] objPara = new object[1];
                objPara[0] = idInfo;
                
                //有人脸比对
                // mainForm.ShowChildForm("JSSYCardKiosks.IdVerification", 1, objPara);
                mainForm.SetbtnRetmainVisable(true);
                if (strNextFormName == "SearchCardInfo")
                {
                    mainForm.SetHeadText("社保卡信息查询");
                    mainForm.ShowChildForm("JSCardKiosks.SearchCardInfo", 1, objPara);
                    timer1.Stop();
                    timer1.Dispose();
                    this.Dispose();
                    return;
                }

                string strRet = "";
                try
                {
                    strRet = AccessWebInterFace.downLoadCardData(idInfo.num, idInfo.name);
                }
                catch (Exception ex)
                {

                    WriteWorkLog.WriteLogs("日志", "错误", "downLoadCardData:" + ex.ToString());
                    if (iTime < 10)
                    {
                        iTime = 10;
                    }
                    string[] paras = { iTime.ToString(), "下载社保数据失败，" + ex.ToString() };
                    BlackForm blackForm = new BlackForm(2, "JSCardKiosks.MsgForm", paras);
                    blackForm.Show(this);
                }
                switch (strRet)
                {
                    case "0$0":
                        timer1.Stop();
                        mainForm.SetHeadText("社保卡信息显示");
                        mainForm.ShowChildForm("JSCardKiosks.ShowCardInfo", 1, objPara);
                        break;
                    case "0$1":
                        timer1.Stop();
                        mainForm.SetHeadText("社保卡信息显示");
                        idInfo.BusinessType = CARD_BUSINESSTYPE_RESHOOT;
                        objPara[0] = idInfo;
                        mainForm.ShowChildForm("JSCardKiosks.TakePhoto", 1, objPara);
                        break;

                    case "0$2":
                        timer1.Stop();
                        mainForm.SetHeadText("个人信息确认");
                        idInfo.BusinessType = CARD_BUSINESSTYPE_NEWADD;
                        objPara[0] = idInfo;
                        mainForm.ShowChildForm("JSCardKiosks.TakePhoto", 1, objPara);
                        break;
                    default:
                        strErrInfo = strRet.Split('$')[1];
                        WriteWorkLog.WriteLogs("日志", "错误", "downLoadCardData:" + strErrInfo);
                        if (iTime < 10)
                        {
                            iTime = 10;
                        }
                        string[] paras = { iTime.ToString(), "下载社保数据失败，" + strErrInfo };
                        BlackForm blackForm = new BlackForm(2, "JSCardKiosks.MsgForm", paras);
                        blackForm.Show(this);
                        break;
                }
                bRun = false;
                timer1.Stop();
                timer1.Dispose();
                this.Dispose();
            }
            else
            {
                WriteWorkLog.WriteLogs("日志", "信息", "无法识别该证件");
                if (iTime < 5)
                    iTime = 5;
                string[] paras2 = { iTime.ToString(), "系统无法识别您的身份证, 请您确认证件是否正确的或者联系工作人员。" };
                BlackForm blackForm2= new BlackForm(2, "JSSYCardKiosks.MsgForm", paras2);
                blackForm2.Show(this);
            }
        }
        private int SysIDCardRead(out IdInfo idInfo)
        {
            idInfo = new IdInfo();
            int nRet;
            byte[] pucIIN = new byte[4];
            byte[] pucSN = new byte[8];
            IDCardData idcardData = new IDCardData();
            byte[] ctmp = new byte[255];
            int nPort = 1001;
            try
            {
                ReadCardAPI.Syn_SetPhotoPath(1, ref ctmp);
                ReadCardAPI.Syn_SetPhotoName(2);
                ReadCardAPI.Syn_SetPhotoType(1);
                nRet = ReadCardAPI.Syn_OpenPort(nPort);
                if (nRet == 0)
                {
                    nRet = ReadCardAPI.Syn_StartFindIDCard(nPort, ref pucIIN, 0);
                    if (nRet != 0 && nRet != 128)
                    {
                        ReadCardAPI.Syn_ClosePort(nPort);
                        return -2;
                    }
                    Thread.Sleep(20);
                    nRet = ReadCardAPI.Syn_SelectIDCard(nPort, ref pucSN, 0);
                    if (nRet != 0)
                    {
                        ReadCardAPI.Syn_ClosePort(nPort);
                        return -3;
                    }
                    Thread.Sleep(20);
                    nRet = ReadCardAPI.Syn_ReadMsg(nPort, 0, ref idcardData);
                    if (nRet == 0)
                    {
                        idInfo.num = idcardData.IDCardNo.Trim();
                        idInfo.name = idcardData.Name.Trim();
                        idInfo.sex = idcardData.Sex.Trim();
                        idInfo.nation = idcardData.Nation.Trim();
                        idInfo.birth = idcardData.Born.Trim();
                        idInfo.address = idcardData.Address.Trim();
                        if (File.Exists(Application.StartupPath + "\\test.txt"))
                        {
                            //  bTestMode = true;
                            StreamReader sr = new StreamReader((Application.StartupPath + "\\test.txt"), Encoding.GetEncoding("utf-8"));
                            idInfo.name = sr.ReadLine();
                            idInfo.num = sr.ReadLine();
                        }
                        WriteWorkLog.WriteLogs("日志", "信息", "strID ：" + idInfo.num);
                        WriteWorkLog.WriteLogs("日志", "信息", "strName ：" + idInfo.name);
                        ReadCardAPI.Syn_ClosePort(nPort);
                        return 0;
                    }
                    else
                    {
                        WriteWorkLog.WriteLogs("日志", "错误", "未检测到身份证");
                        ReadCardAPI.Syn_ClosePort(nPort);
                        return -4;
                    }
                }
                else
                {
                    WriteWorkLog.WriteLogs("日志", "错误", "打开身份证阅读器端口失败");
                    return -1;
                }
            }
            catch (Exception exErr)
            {
                WriteWorkLog.WriteLogs("日志", "异常", "打开身份证阅读器失败" + exErr.ToString());
                return -9;
            }
        }
    }
}
