using com.sun.org.apache.bcel.@internal.generic;
using HttpPostTest;
using MyThirdCard;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WorkLog;

namespace JSCardKiosks
{
    public partial class MakeCard : Form
    {
        MainForm mainForm;
        CardDataInfo cardInfo = new CardDataInfo();
        string strResult = "";
        int iTime = 10;
       
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }
        public MakeCard()
        {
            InitializeComponent();
        }
        public MakeCard(CardDataInfo cardInfo)
        {
            this.cardInfo = cardInfo;
            InitializeComponent();
        }
        private void MakeCard_Load(object sender, EventArgs e)
        {
            pictureBox2.Image = PublicFuction.GetGifImage("出卡");
            mainForm = (MainForm)this.Owner;
            //mainForm.SetHeadText("社保卡制卡");
            backgroundWorker1.RunWorkerAsync();
            PublicFuction.SoundPlay("您的社保卡在正在制作中请耐心等待.wav");
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (DealAppConfig.GetAppSettingsValue("打印模块") == "51")
                strResult = IssueCard51(cardInfo);
            if (DealAppConfig.GetAppSettingsValue("打印模块") == "70")
                strResult = IssueCard(cardInfo);
        }
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            timer1.Enabled = true;
            if (strResult != "0")
            {
                PublicFuction.SoundPlay("您的社保卡以制作失败，请联系大堂经理.wav");
                string[] paras = { iTime.ToString(), "社保卡发卡失败，"+strResult };
                BlackForm blackForm = new BlackForm(2, "JSCardKiosks.MsgForm", paras);
                blackForm.Show(this);

            }
            else
            {
                if (DealAppConfig.GetAppSettingsValue("打印模块") == "51")
                    IDP51PrintControl.IDPCardReBack(700);
                if (DealAppConfig.GetAppSettingsValue("打印模块") == "70")
                    PrintControl.IDPCardKeep(700);
                    Thread.Sleep(1000);
                PublicFuction.SoundPlay("请取走您的卡.wav");
                #region 清空照片文件
                string strPhotoFile = Application.StartupPath + "\\photo";
                string[] strFileName = Directory.GetFiles(strPhotoFile);
                for (int i = 0; i < strFileName.Length; i++)
                {
                    File.Delete(strFileName[i]);
                }
                #endregion
                panel1.Visible = true;
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            iTime--;
            if (iTime < 0)
            {
                this.Close();
            }
        }
        private string IssueCard(CardDataInfo cardDataInfo)
        {
            int ret = 0;
            string strRet = "";
            IntPtr iHandle = CardCaseHelp.CardCaseOpen();
            if(iHandle!=IntPtr.Zero)
            {
                strRet = CardCaseHelp.CaseCardIn(iHandle);
                if (strRet != "0")
                {
                    WriteWorkLog.WriteLogs("日志", "进卡函数返回", ret.ToString());
                    return "进卡失败";
                }
            }
            else
            {
                return "打开三卡箱端口失败";
            }

            if (cardDataInfo.sex == "男")
                cardDataInfo.sex = "1";
            if (cardDataInfo.sex == "女")
                cardDataInfo.sex = "2";
            List<string> strCardInfo = new List<string>();
            strCardInfo.Add(cardDataInfo.cardDate);
            strCardInfo.Add(cardDataInfo.cardNo);
            strCardInfo.Add(cardDataInfo.idNum);
            strCardInfo.Add(cardDataInfo.name.Trim());
            strCardInfo.Add(cardDataInfo.nameEx);
            strCardInfo.Add(cardDataInfo.sex);
            strCardInfo.Add(cardDataInfo.nation);
            strCardInfo.Add(cardDataInfo.birth);
            cardDataInfo.birthAdrrCode = cardDataInfo.idNum.Substring(0, 6);
            strCardInfo.Add(cardDataInfo.birthAdrrCode);
            strCardInfo.Add(cardDataInfo.personID);
            strCardInfo.Add(cardDataInfo.adminAreaCode);
            string cardIdCode = "";
            ret = CardWriteRead.WriteCard(iHandle,strCardInfo, out cardIdCode);
            if (ret != 0)
            {
                WriteWorkLog.WriteLogs("日志", "写卡函数返回", ret.ToString());
                PrintControl.IDPCardOut();
                CardCaseHelp.CaseCardMoveFromPrinter(iHandle);
                CardCaseHelp.CaseCardRecieve(iHandle);
                return "写卡失败";
            }
            strRet = CardCaseHelp.CaseCardMoveToPrinter(iHandle);
            if (strRet != "0")
            {
                return "进卡到打印机失败";
            }
            string printPhotoPath = AppDomain.CurrentDomain.BaseDirectory + "photo\\" + cardDataInfo.idNum + ".jpg";
            string str4 = cardDataInfo.cardDate.Substring(0, 4) + "    " + cardDataInfo.cardDate.Substring(4, 2);
            ret = PrintControl.IDPCardPrint(cardDataInfo.name ,cardDataInfo.idNum ,cardDataInfo.personID , str4 , printPhotoPath, "PrintConfig.ini");
            if (ret != 0)
            {
                WriteWorkLog.WriteLogs("日志", "打印函数返回", ret.ToString());
                PrintControl.IDPCardOut();
                CardCaseHelp.CaseCardRecieve(iHandle);
                return "打印失败";
            }
            string dataPath = System.Windows.Forms.Application.StartupPath.ToString();
            OleDbConnection conn = new OleDbConnection(Access_SQLHelper.strConn);
            conn.Open();
            string sql = "update E_人员信息 set 制卡状态 ='已制',卡识别码='" + cardIdCode + "' where 身份证号 ='" + cardDataInfo.idNum + "'";
            OleDbCommand comm = null;
            try
            {
                comm = new OleDbCommand(sql, conn);
                comm.ExecuteNonQuery();
                comm.Dispose();
                conn.Close();
            }
            catch (Exception exErr)
            {
                WriteWorkLog.WriteLogs("日志", "本地数据库异常", exErr.ToString());
                comm.Dispose();
                conn.Close();
                PrintControl.IDPCardOut();
                CardCaseHelp.CaseCardMoveFromPrinter(iHandle);
                CardCaseHelp.CaseCardRecieve(iHandle);
                return "更新发卡日期失败";
            }
            string iSupLoad = DealAppConfig.GetAppSettingsValue("是否回传");
            if (iSupLoad == "是")
            {
                strRet = AccessWebInterFace.Interface2_23(cardDataInfo.strBatch, cardDataInfo.personID, cardIdCode);
                if (strRet != "0")
                {
                    PrintControl.IDPCardOut();
                    CardCaseHelp.CaseCardMoveFromPrinter(iHandle);
                    CardCaseHelp.CaseCardRecieve(iHandle);
                    return strRet;
                }
                strRet = AccessWebInterFace.Interface2_49(cardDataInfo.name, cardDataInfo.idNum, cardDataInfo.personID);
                if (strRet != "0")
                {
                    PrintControl.IDPCardOut();
                    CardCaseHelp.CaseCardMoveFromPrinter(iHandle);
                    CardCaseHelp.CaseCardRecieve(iHandle);
                    return strRet;
                }
                PrintControl.IDPCardKeep();
            }
            return "0";
        }
        private string IssueCard51(CardDataInfo cardDataInfo)
        {
            int ret = 0;
            ret = IDP51PrintControl.IDPCardIn();
            if (ret < 0)
            {
                WriteWorkLog.WriteLogs("日志", "进卡函数返回", ret.ToString());
                return "进卡失败";
            }
            //if (cardDataInfo.sex == "男")
            //    cardDataInfo.sex = "1";
            //if (cardDataInfo.sex == "女")
            //    cardDataInfo.sex = "2";
            List<string> strCardInfo = new List<string>();
            strCardInfo.Add(cardDataInfo.cardDate.Replace("-",""));
            strCardInfo.Add(cardDataInfo.cardNo);
            strCardInfo.Add(cardDataInfo.idNum);
            strCardInfo.Add(cardDataInfo.name);
            strCardInfo.Add(cardDataInfo.nameEx);
            strCardInfo.Add(cardDataInfo.sex);
            if(cardDataInfo.nation==null|| cardDataInfo.nation =="")
            {
                cardDataInfo.nation = "01";
            }
            strCardInfo.Add(cardDataInfo.nation);
            if(cardDataInfo.birth==null|| cardDataInfo.birth=="")
            {
                cardDataInfo.birth=  cardDataInfo.idNum.Substring(6, 8);
            }
            strCardInfo.Add(cardDataInfo.birth.Replace("-",""));
            cardDataInfo.birthAdrrCode = cardDataInfo.idNum.Substring(0,6);
            strCardInfo.Add(cardDataInfo.birthAdrrCode);
            strCardInfo.Add(cardDataInfo.personID);
            cardDataInfo.adminAreaCode = "";
            strCardInfo.Add(cardDataInfo.adminAreaCode);
            string cardIdCode = "";
            object[] objPara = new object[2];
            objPara[0] = strCardInfo;
            //byte bPasm = 0X0E;
            //objPara[1] = bPasm;
            //string strDllPath = AppDomain.CurrentDomain.BaseDirectory + "CardWriteReadT10.dll";
            //ret = (int)PublicFuction.RunDllStaticFun(strDllPath, "CardWriteReadT10.CardWriteRead", "WriteCard2", objPara);
            // cardIdCode = objPara[1].ToString();
            string cardVersion = "";
            string strErr = "";
            
            if (DealAppConfig.GetAppSettingsValue("是否写IC") =="Y")
            {
                #region 检测银行卡所属银行
                string chipbankCode = "";
                ret = getBankCodeFromCardChip(out chipbankCode, out cardVersion, out strErr);
                if (ret != 0)
                {
                    WriteWorkLog.WriteLogs("日志", "读银行卡号失败返回", ret.ToString());
                    IDP51PrintControl.IDPCardOut();
                    return "读银行卡号失败";
                }
                else
                {
                    if (chipbankCode != DealAppConfig.GetAppSettingsValue("银行代码"))
                    {
                        strErr = "预制卡银行(" + WebInfeFaceHelp.GetBankName(chipbankCode) + ")不符" ;
                        IDP51PrintControl.IDPCardOut();
                        return strErr;
                    }
                }
                #endregion
                #region 写社保卡信息
                if (cardVersion == "2.00")
                {
                    ret = MyCard.CardWriteReadT10.WriteCard2(strCardInfo, out cardIdCode);
                    if (ret != 0)
                    {
                        WriteWorkLog.WriteLogs("日志", "写卡函数返回", ret.ToString());
                        IDP51PrintControl.IDPCardOut();
                        return "写卡失败";
                    }
                    WriteWorkLog.WriteLogs("日志", "卡识别码：", cardIdCode);
                }
                else if (cardVersion == "3.00")
                {
                    string strPubkey = "";
                    ret = CardWriteReadThirdT10.GetPublicKey(out strPubkey);
                    if (ret != 0)
                    {
                        WriteWorkLog.WriteLogs("日志", "GetPublicKey返回", ret.ToString());
                        IDP51PrintControl.IDPCardOut();
                        return "写卡失败";
                    }
                    CardBaseData cardBaseData = new CardBaseData(cardDataInfo.name, cardDataInfo.idNum);
                    cardBaseData.Aac003 = cardDataInfo.name;
                    cardBaseData.Aac500 = cardDataInfo.cardNo;
                    cardBaseData.Aac002 = cardDataInfo.idNum;
                    cardBaseData.Aab301 = "320100";
                    CARegisterInfo registerInfo = new CARegisterInfo();
                    ret = WebInfeFaceHelp.CARegist(cardBaseData, strPubkey, out registerInfo, out strErr);
                    if (ret != 0)
                    {
                        WriteWorkLog.WriteLogs("日志", "CARegist返回", ret.ToString() + "，" + strErr);
                        IDP51PrintControl.IDPCardOut();
                        return "写卡失败";
                    }
                    WriteWorkLog.WriteLogs("日志", "信息", "CA注册成功");
                    ret = CardWriteReadThirdT10.WriteCACert(registerInfo);
                    if (ret != 0)
                    {
                        WriteWorkLog.WriteLogs("日志", "WriteCACert返回", ret.ToString());
                        IDP51PrintControl.IDPCardOut();
                        return "写卡失败";
                    }
                    WriteWorkLog.WriteLogs("日志", "信息", "CA写入成功");
                    ret = CardWriteReadThirdT10.Write3thCard(strCardInfo, out cardIdCode);
                    if (ret != 0)
                    {
                        WriteWorkLog.WriteLogs("日志", "写卡函数返回", ret.ToString());
                        IDP51PrintControl.IDPCardOut();
                        return "写卡失败";
                    }
                    WriteWorkLog.WriteLogs("日志", "信息", "个人信息写入成功");
                    WriteWorkLog.WriteLogs("日志", "卡识别码：", cardIdCode);
                    //string oldMainkey = "00112233445566778899AABBCCDDEEFF";
                    ////oldMainkey = "31313633393535333536363239393234";
                    //string newMainkey = registerInfo.zkmy;
                    //string newManagePin = registerInfo.glypin;
                    //ret = CardWriteReadThirdT10.UpdateKey(oldMainkey, newMainkey, newManagePin, out strErr);
                    //if (ret != 0)
                    //{
                    //    WriteWorkLog.WriteLogs("日志", "获取卡版本信息失败返回", ret.ToString());
                    //    IDP51PrintControl.IDPCardOut();
                    //    return "写卡失败";
                    //}
                    ///////////////////////////////
                }
                else
                {
                    WriteWorkLog.WriteLogs("日志", "写卡函数返回", ret.ToString());
                    IDP51PrintControl.IDPCardOut();
                    return "写卡失败";
                }
                #endregion
            }
            if (DealAppConfig.GetAppSettingsValue("是否打印")== "Y")
            {
                if (cardVersion == "2.00")
                {
                    string cardDate = cardDataInfo.cardDate.Replace("-", "");
                    string str4 = cardDate.Substring(0, 4) + "    " + cardDate.Substring(4, 2);
                    ret = IDP51PrintControl.IDPCardPrint(cardDataInfo.name + "|" + cardDataInfo.idNum + "|" + cardDataInfo.personID + "|" + str4 + "|" + cardDataInfo.idNum + "|sfgmtv");
                    if (ret != 0)
                    {
                        WriteWorkLog.WriteLogs("日志", "打印函数返回", ret.ToString());
                        IDP51PrintControl.IDPCardOut();
                        return "打印失败";
                    }
                }
                if (cardVersion == "3.00")
                {
                    string cardDate = cardDataInfo.cardDate.Replace("-", "");
                    string str4 = cardDate.Substring(0, 4) + "年" + cardDate.Substring(4, 2) + "月";
                    ret = IDP51PrintControl.IDPCardPrint(cardDataInfo.name + "|" + cardDataInfo.idNum + "|" + cardDataInfo.personID + "|" + str4 + "|" + cardDataInfo.idNum + "|sfgmtv*51PrintConfig3.ini");
                    if (ret != 0)
                    {
                        WriteWorkLog.WriteLogs("日志", "打印函数返回", ret.ToString());
                        IDP51PrintControl.IDPCardOut();
                        return "打印失败";
                    }
                }
            }
            string dataPath = System.Windows.Forms.Application.StartupPath.ToString();
            string iSupLoad = DealAppConfig.GetAppSettingsValue("是否回传");
            if (iSupLoad == "Y")
            { 
                CardBaseData cardBaseData = new CardBaseData(cardDataInfo.name, cardDataInfo.idNum);
                cardBaseData.Baz030 = cardDataInfo.netBatch ;
                cardBaseData.Aaz501 = cardIdCode;
                cardBaseData.Baz805 = cardDataInfo.personID;
                cardBaseData.Aac003 = cardDataInfo.name;
                cardBaseData.Aac147 = cardDataInfo.idNum;
                int iRet = WebInfeFaceHelp.CardDataReBack(cardBaseData, out strErr);
                if (iRet !=  0 )
                {
                    strErr = "回盘失败";
                    IDP51PrintControl.IDPCardOut();
                    return strErr;
                }
                iRet = WebInfeFaceHelp.GetCard(cardBaseData, out strErr);
                if (iRet != 0)
                {
                    strErr = "领卡失败";
                    IDP51PrintControl.IDPCardOut();
                    return strErr;
                }
            }
           
            return "0";
        }
        private int getBankCodeFromCardChip(out string bankCode, out string cardVersion, out string strErr)
        {
            bankCode = "";
            strErr = "";
            System.Collections.Hashtable ht = new System.Collections.Hashtable();
            using (StreamReader sReader = new StreamReader(Application.StartupPath + "\\bankcode.txt", Encoding.UTF8))
            {
                string aLine;
                bool condition = true;
                while (true)
                {
                    aLine = sReader.ReadLine();
                    if (aLine == null)
                    {
                        condition = false;
                    }
                    if (condition)
                    {
                        ht.Add(aLine.Split(',')[0], aLine.Split(',')[1]);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            string bankNo = "";
            cardVersion = "";
            MyCard.CardWriteReadT10.ReadBankNo(out bankNo, out cardVersion, out strErr);
            if (bankNo == "")
            {
                strErr = "读取银行卡号失败";
                return -1;

            }
            string bankNopart = bankNo.Substring(0, 6);
            bankCode = ht[bankNopart].ToString();
            return 0;
        }
        private void MakeCard_FormClosing(object sender, FormClosingEventArgs e)
        {
            mainForm.SetbtnRetmainVisable(false);
            mainForm.SetpalMidVisable(false);
            mainForm.SetHeadText("社会保障卡发卡终端");
            timer1.Stop();
        }
    }
}
