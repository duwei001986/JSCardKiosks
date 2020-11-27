using CZIDCardReader;
using HttpPostTest;
using javax.smartcardio;
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
using System.Windows.Forms;
using WorkLog;

namespace JSCardKiosks
{
    public partial class IdReadForm : Form
    {
        public const int CARD_BUSINESSTYPE_RESHOOT = 1;
        public const int CARD_BUSINESSTYPE_NEWADD = 2;
        IdInfo idInfo = new IdInfo();
        int iTime = 30;
        int iBeginTime = 0;
        int iRet = 0;
        string strPath2;
        bool bReaderOpen = false;
        MainForm mainForm;
        bool bRun;
        CardInfo IDInfo;
        string strNextFormName = "";
        public IdReadForm()
        {
            InitializeComponent();
        }
        public IdReadForm(string strNextFormName)
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
            player.SoundLocation = Application.StartupPath + "\\请把您的身份证插入到身份证阅读器中.wav";
            player.Load();
            player.Play();
            pictureBox1.Image = DealGif.GetGifImage("身份证");
            strPath2 = AppDomain.CurrentDomain.BaseDirectory + "temp";
            if (!Directory.Exists(strPath2))
            {
                Directory.CreateDirectory(strPath2);
            }
            iRet = ReadIDBadge.IDReaderOpen(strPath2);
            iBeginTime = iTime - 2;
            if (iRet != 0)
            {
                WriteWorkLog.WriteLogs("日志", "错误", "身份证阅读器打开失败");
                bReaderOpen = true;

                return;
            }
            ReadIDBadge.CardInEnable();
            bRun = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            iTime--;
            labRetTime.Text = iTime.ToString().PadLeft(2, '0');
            if (iTime == iBeginTime && !backgroundWorker1.IsBusy)
            {
                backgroundWorker1.RunWorkerAsync();
            }
            if (iTime == 1)
            {
                ReadIDBadge.CardInDisable();
            }
            if (iTime <= 0)
            {
                this.Close();
                //mainForm.ShowChildForm("JSCardKiosks.MainPal", 0, new string[] { "", "" });
            }

        }

        private void IDReadForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            mainForm.SetbtnRetmainVisable(false);
            mainForm.SetpalMidVisable(false);
            mainForm.SetHeadText("社会保障卡发卡终端");
            bRun = false;
            ReadIDBadge.CardInDisable();
            iRet = ReadIDBadge.Uninit();
            timer1.Stop();
            timer1.Dispose();
            this.Dispose();

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (bRun)
            {
                System.Threading.Thread.Sleep(200);
                _IDS_STATUS_RESULT pResult = new _IDS_STATUS_RESULT();
                int iRet = ReadIDBadge.GetStatus(_IDS_DEV_STATUS_TYPE._IDS_DEV_STATUS_TYPE_DS, out pResult);
                if (pResult._Sensor.ulCOVER == 12)
                {
                    CardInfo IDInfo = new CardInfo();
                    backgroundWorker1.ReportProgress(0, IDInfo);
                    if (iTime < 10)
                        iTime = 10;
                    iRet = ReadIDBadge.IDCardRead(strPath2, out IDInfo);
                    if (iRet == 0 && IDInfo.name != "")
                    {
                        //try
                        //{
                        //    iRet = ReadIDBadge.IDCardScan();
                        //}
                        //catch (Exception ex)
                        //{

                        //    WriteWorkLog.WriteLogs("日志", "异常", "扫描证件失败" + ex.ToString());
                        //    IDInfo.name = "";
                        //}

                    }
                    if (iRet != 0)
                    {
                        WriteWorkLog.WriteLogs("日志", "错误", "扫描证件失败" + iRet.ToString());
                        IDInfo.name = "";
                    }
                    backgroundWorker1.ReportProgress(1, IDInfo);

                }
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 0)
            {
                mainForm.SetbtnRetmainVisable(false);
                return;
            }
            IDInfo = (CardInfo)e.UserState;
            idInfo.name = IDInfo.name.Trim();
            idInfo.sex = IDInfo.Gender.Trim();
            idInfo.nation = IDInfo.Nation.Trim();
            idInfo.birth = IDInfo.Birth.Trim();
            idInfo.address = IDInfo.Address.Trim();
            idInfo.num = IDInfo.IDNumber.Trim();
            idInfo.start = IDInfo.DateBegin.Trim();
            idInfo.end = IDInfo.DateEnd.Trim();
            if (IDInfo.name != "")
            {
                SoundPlayer player = new SoundPlayer();
                player.SoundLocation = Application.StartupPath + "\\身份证读取成功.wav";
                player.Load();
                player.Play();
            }
            int iRet = ReadIDBadge.CardEject();
            if (iRet != 0)
            {
                WriteWorkLog.WriteLogs("日志", "错误", "身份证阅读器出卡失败" + iRet.ToString());
            }
            CardBaseData cardBaseData = new CardBaseData(idInfo.name, idInfo.num);
            cardBaseData.Aae008 = DealAppConfig.GetAppSettingsValue("银行代码");
            cardBaseData.Aac059 = idInfo.end;
            cardBaseData.Aac006 = idInfo.birth;
            WriteWorkLog.WriteLogs("日志", "信息", "姓名：" + IDInfo.name.Trim());
            WriteWorkLog.WriteLogs("日志", "信息", "身份证号：" + IDInfo.IDNumber.Trim());
            if (IDInfo.name == "")
            {
                mainForm.SetbtnRetmainVisable(true);
                WriteWorkLog.WriteLogs("日志", "信息", "无法识别该证件");
                if (iTime < 5)
                    iTime = 5;
                string[] paras = { iTime.ToString(), "系统无法识别您的身份证, 请您确认是否插入正确的证件或者联系工作人员。" };
                BlackForm blackForm = new BlackForm(2, "JSCardKiosks.MsgForm", paras);
                blackForm.Show(this);
            }
            else
            {
                backgroundWorker1.CancelAsync();
                if (!backgroundWorker1.CancellationPending)
                    backgroundWorker1.CancelAsync();
                //string strPath = AppDomain.CurrentDomain.BaseDirectory + "IDPhoto\\" + IDInfo.IDNumber.Trim() + "_r.jpg";
                //PublicFuction.CompressImage(strPath2 + "\\front.bmp", strPath2 + "\\" + IDInfo.IDNumber.Trim() + "front2.bmp", 30, 100, true);
                //PublicFuction.CompressImage(strPath2 + "\\back.bmp", strPath2 + "\\" + IDInfo.IDNumber.Trim() + "back2.bmp", 30, 100, true);
                if (File.Exists(Application.StartupPath + "\\test.txt"))
                {
                    //  bTestMode = true;
                    StreamReader sr = new StreamReader((Application.StartupPath + "\\test.txt"), Encoding.GetEncoding("utf-8"));

                    cardBaseData.Aac003 = sr.ReadLine();
                    cardBaseData.Aac147 = sr.ReadLine();
                    //cardBaseData.Baz805 = sr.ReadLine();
                    //cardBaseData.Aac500 = sr.ReadLine();
                    //cardBaseData.Image = sr.ReadToEnd();
                    //cardBaseData.Aac004 = (idInfo.sex == "男") ? "1" : "2";
                    //cardBaseData.Aac005 = DataHaseTable.GetNationCode(idInfo.nation+"族");
                    //cardBaseData.Aac006 = idInfo.birth;
                    //cardBaseData.Baz103 = "";
                    //cardBaseData.Aae005 = "15210379537";
                    //cardBaseData.Bac025 = "15";
                    //cardBaseData.Aaz503 = "20201020";
                    //cardBaseData.Baz030 = "20201020";
                }
                mainForm.SetbtnRetmainVisable(true);
                object[] objPara = new object[1];
                objPara[0] = idInfo;

                if (strNextFormName == "SearchCardInfo")
                {
                    mainForm.SetHeadText("社保卡信息查询");
                    mainForm.ShowChildForm("JSCardKiosks.SearchCardInfo", 1, objPara);
                    timer1.Stop();
                    timer1.Dispose();
                    this.Dispose();
                    ReadIDBadge.CardInDisable();
                    iRet = ReadIDBadge.Uninit();
                    bRun = false;
                    return;
                }
                if (strNextFormName == "ReportCardLoss")
                {
                    string[] paras = { iTime.ToString(), "请确认是否要挂失您的原有市民卡或者社保卡？" , "1" };
                    BlackForm blackForm = new BlackForm(3, "JSCardKiosks.MsgForm", paras);
                    if (blackForm.ShowDialog(this) == DialogResult.Cancel)
                    {
                        int iMsgRet = MsgForm.iRet;
                        if (iMsgRet == 0)
                        {
                            this.Close();
                            timer1.Stop();
                            timer1.Dispose();
                            this.Dispose();
                            ReadIDBadge.CardInDisable();
                            iRet = ReadIDBadge.Uninit();
                            bRun = false;
                            return;
                        }
                    }
                    #region 挂失
                    string strErr = "";
                    string strInfo = "";
                    iRet = WebInfeFaceHelp.ReportCardLoss(cardBaseData,out strErr);
                    if(iRet==0)
                    {
                        strInfo = "您的社保卡挂失成功";
                    }
                    else
                    {
                        strInfo = "您的社保卡挂失失败，" + strErr;
                    }
                    #endregion
                    string[] paras1 = { iTime.ToString(), strInfo };
                    BlackForm blackForm1 = new BlackForm(2, "JSCardKiosks.MsgForm", paras1);
                    blackForm1.Show(this);
                    timer1.Stop();
                    timer1.Dispose();
                    this.Dispose();
                    ReadIDBadge.CardInDisable();
                    iRet = ReadIDBadge.Uninit();
                    bRun = false;
                    return;
                }
                bool bInputPhoneNur = true;
                if (bInputPhoneNur)
                {
                    iRet = WebInfeFaceHelp. StatusCheck(cardBaseData);
                    if(iRet==0)
                    {
                        string[] paras = { iTime.ToString(), "您的卡状态正常，如果需要新制卡需要挂失原有社保卡，请确认是否要挂失您的原有市民卡或者社保卡？", "1" };
                        BlackForm blackForm = new BlackForm(3, "JSCardKiosks.MsgForm", paras);
                        if (blackForm.ShowDialog(this) == DialogResult.Cancel)
                        {
                            int iMsgRet = MsgForm.iRet;
                            if (iMsgRet == 0)
                            {
                                this.Close();
                                timer1.Stop();
                                timer1.Dispose();
                                this.Dispose();
                                ReadIDBadge.CardInDisable();
                                iRet = ReadIDBadge.Uninit();
                                bRun = false;
                                return;
                            }
                        }
                        #region 挂失
                        string strErr = "";
                        string strInfo = "";
                        iRet = WebInfeFaceHelp.ReportCardLoss(cardBaseData, out strErr);
                        if (iRet == 0)
                        {
                            strInfo = "您的社保卡挂失成功";
                        }
                        else
                        {
                            strInfo = "您的社保卡挂失失败，" + strErr;
                            string[] paras1 = { iTime.ToString(), strInfo };
                            BlackForm blackForm1 = new BlackForm(2, "JSCardKiosks.MsgForm", paras1);
                            blackForm1.Show(this);
                            timer1.Stop();
                            timer1.Dispose();
                            this.Dispose();
                            ReadIDBadge.CardInDisable();
                            iRet = ReadIDBadge.Uninit();
                            bRun = false;
                            return;
                        }
                        #endregion
                       
                    }
                    try
                    {
                        int iret = 0;
                        string strErr = "";
                        iret = WebInfeFaceHelp.GetDataFromWeb(cardBaseData, out strErr);
                        cardBaseData.Aac005 = DataHaseTable.GetNationCode(idInfo.nation + "族");

                        //if (File.Exists(Application.StartupPath + "\\test.txt"))
                        //{
                        //    iret = 0;
                        //}
                        if (iret == 0)
                        {
                            objPara[0] = cardBaseData;
                            timer1.Stop();
                            mainForm.SetHeadText("社保卡信息显示");
                            mainForm.ShowChildForm("JSCardKiosks.ShowCardInfo", 1, objPara);
                        }
                        if (iret == -1)
                        {
                            if (iTime < 10)
                            {
                                iTime = 10;
                            }
                            string[] paras = { iTime.ToString(), "下载社保数据失败，" + strErr };
                            BlackForm blackForm = new BlackForm(2, "JSCardKiosks.MsgForm", paras);
                            blackForm.Show(this);
                        }
                        if (iret == 1)
                        {
                            //cardBaseData.Aac059 = idInfo.end;
                            //cardBaseData.Aae008 = DealAppConfig.GetAppSettingsValue("银行代码");
                            // cardBaseData.Bac025 = "21";
                            //cardBaseData.Aac006 = idInfo.birth;

                            objPara[0] = cardBaseData;
                            timer1.Stop();
                            mainForm.SetHeadText("个人信息录入");
                            mainForm.ShowChildForm("JSCardKiosks.InputPhoneNur", 1, objPara);
                        }

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
                }
                else
                {
                    try
                    {
                        int iret = 0;
                        string strErr = "";
                        iret = WebInfeFaceHelp.GetDataFromWeb(cardBaseData, out strErr);
                        cardBaseData.Aac005 = DataHaseTable.GetNationCode(idInfo.nation + "族");

                        //if (File.Exists(Application.StartupPath + "\\test.txt"))
                        //{
                        //    iret = 0;
                        //}
                        if (iret == 0)
                        {
                            objPara[0] = cardBaseData;
                            timer1.Stop();
                            cardBaseData.Aac059 = idInfo.end;
                            cardBaseData.Aae008 = DealAppConfig.GetAppSettingsValue("银行代码");
                            cardBaseData.Bac025 = "21";//old
                            cardBaseData.Aac006 = idInfo.birth;
                            mainForm.SetHeadText("社保卡信息显示");
                            mainForm.ShowChildForm("JSCardKiosks.ShowCardInfo", 1, objPara);
                        }
                        if (iret == -1)
                        {
                            if (iTime < 10)
                            {
                                iTime = 10;
                            }
                            string[] paras = { iTime.ToString(), "下载社保数据失败，" + strErr };
                            BlackForm blackForm = new BlackForm(2, "JSCardKiosks.MsgForm", paras);
                            blackForm.Show(this);
                        }
                        if (iret == 2)
                        {
                            cardBaseData.Aac059 = idInfo.end;
                            cardBaseData.Aae008 = DealAppConfig.GetAppSettingsValue("银行代码");
                            cardBaseData.Bac025 = "21";//old
                            cardBaseData.Aac006 = idInfo.birth;
                            iret = WebInfeFaceHelp.GetDataFromWeb(cardBaseData, out strErr);
                            if (iret == -1)
                            {
                                if (iTime < 10)
                                {
                                    iTime = 10;
                                }
                                string[] paras = { iTime.ToString(), "下载社保数据失败，" + strErr };
                                BlackForm blackForm = new BlackForm(2, "JSCardKiosks.MsgForm", paras);
                                blackForm.Show(this);
                            }
                            else
                            {
                                objPara[0] = cardBaseData;
                                timer1.Stop();
                                mainForm.SetHeadText("社保卡信息显示");
                                mainForm.ShowChildForm("JSCardKiosks.ShowCardInfo", 1, objPara);
                            }
                        }
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
                }

                // mainForm.ShowChildForm("JSCardKiosks.IdVerification", 1, objPara);
                bRun = false;
                ReadIDBadge.CardInDisable();
                iRet = ReadIDBadge.Uninit();

            }
        }
    }
}
