using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WorkLog;

namespace JSCardKiosks
{
    public partial class IdVerification : Form
    {

        public const int CARD_BUSINESSTYPE_RESHOOT = 1;
        public const int CARD_BUSINESSTYPE_NEWADD = 2;
        public const int CW_ERR_CameraNotOpen = 9000;       //摄像头未打开
        public const int CW_ERR_CameraOpenError = 9001;     //摄像头打开失败
        public const int CW_ERR_CameraOpenAdy = 9002;       //摄像头已经打开
        public const int CW_ERR_InitSDK = 9100;     //初始化SDK失败
        //活体状态
        public const int CW_LivenessOK = 0;         //活体并正常获得最佳人脸
        public const int CW_LivenessFailure = 9301;     //未符合活体要求
        public const int CW_LivenessNOFace = 9302;      //未检测到人脸
        public const int CW_LivenessNisNoFace = 9303;       //红外未检测到人脸
        public const int CW_LivenessLostFace = 9304;        //人脸丢失
        public const int CW_LivenessManyFace = 9305;        //检测到多人
        public const int CW_LivenessFaceChange = 9307;      //检测到换人
        public const int CW_GetBestFaceErr = 117;       //获取最佳人脸失败

        public const int EnumLocalNetError = 800;       //网络异常。
        public const int EnumLocalNetErrorUrl = 803;        //URL格式不正确。
        public const int EnumLocalNetErrorSerAddr = 806;        //无法解析人脸识别服务主机地址。
        public const int EnumLocalNetErrorConSer = 807;     //无法连接到人脸识别服务主机。
        public const int EnumLocalNetErrorTimeOut = 828;        //访问人脸识别服务主机超时。
        public const int EnumLocalNetErrorActData = 890;        //无法解析人脸识别服务主机返回数据。

        public const int EnumLocalNoFace = 1100;		//未检测到人脸
        int ir = -1;
        string respMsgRet;
        string strImg;
        int iTime = 30;
        int iTimeVideoStart = 0;
        IdInfo idInfo = new IdInfo();
        string readPhotoPath = "";
        bool bHasVs = false;
        MainForm mainForm;
        //public static void SetDouble(Control cc)
        //{

        //    cc.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance |
        //                 System.Reflection.BindingFlags.NonPublic).SetValue(cc, true, null);

        //}
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        public IdVerification(IdInfo idInfo)
        {
            this.idInfo = idInfo;
            InitializeComponent();
        }
        public IdVerification()
        {
            InitializeComponent();
        }
        private void IdVerification_Load(object sender, EventArgs e)
        {
            newPanel1.Location = new Point(0, 0);
            mainForm = (MainForm)this.Owner;
            pictureBox1.Image = DealGif.GetGifImage("身份证");
            //axCieCloudWalkOCX1.cwUnInit();
            int iret = axCieCloudWalkOCX1.cwInit();
            if (0 != iret)
            {
                WriteWorkLog.WriteLogs("日志", "cwInit ：", iret.ToString());
                return;
            }
            iTimeVideoStart = iTime - 2;
            labName.Text = idInfo.name;
            labId.Text = idInfo.num;
            readPhotoPath = Application.StartupPath + "\\IDPhoto\\" + idInfo.num + "_r.jpg";
        }

        private void axCieCloudWalkOCX1_cwLivesInfoCallBack(object sender, AxCieCloudWalkOCXLib._DCieCloudWalkOCXEvents_cwLivesInfoCallBackEvent e)
        {
            string strReceiveInfo = e.strInfo;
            JObject jo = (JObject)JsonConvert.DeserializeObject(strReceiveInfo);
            respMsgRet = jo["result"].ToString();
            strImg = jo["image"].ToString();
            jo.RemoveAll();
        }
        public string ImgToBase64String(string Imagefilename)
        {
            try
            {
                Bitmap bmp = new Bitmap(Imagefilename);
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                string strbase64 = Convert.ToBase64String(arr);
                arr = null;
                bmp.Dispose();
                return strbase64;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private Image GetImageFromPath(string path)
        {
            try
            {
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                BinaryReader br = new BinaryReader(fs);
                MemoryStream ms = new MemoryStream(br.ReadBytes((int)fs.Length));
                Image img = Image.FromStream(ms);
                ms.Close();
                fs.Close();
                br.Close();
                return img;
            }
            catch (Exception ex)
            {
                WriteWorkLog.WriteLogs("日志", "异常", "IdVerification-Line119:" + ex.ToString());
                return null;
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            labRetTime.Text = iTime.ToString().PadLeft(2, '0');
            iTime--;
            if (iTime == iTimeVideoStart)
            {
               // picGif.Visible = false;
                newPanel1.Visible = false;
                ir = axCieCloudWalkOCX1.cwStartCamera();
                if (ir != 0)
                {
                    WriteWorkLog.WriteLogs("日志", "错误", "没有找到摄像头");
                    return;
                }
            }
            if (iTime == iTimeVideoStart )
            {
                Thread.Sleep(200);
                axCieCloudWalkOCX1.Visible = true;
            }
            if (0 == ir && iTime < iTimeVideoStart - 2)
            {
                if (!bHasVs)
                {
                   
                   // Thread.Sleep(200);
                    ir = axCieCloudWalkOCX1.cwStartLiveDetect();
                   
                    if (respMsgRet == "0")
                    {
                        Byte[] filedate = Convert.FromBase64String(strImg);
                        MemoryStream ms = new MemoryStream(filedate);
                        picPhoto.Image = Image.FromStream(ms);
                        ms.Flush();
                        ms.Close();
                        picPhoto.Visible = true;

                        bHasVs = true;
                        picGif.Image = GetImageFromPath(readPhotoPath);
                        filedate = null;
                        try
                        {
                            string baseimg1 = strImg;
                            string baseimg2 = PublicFuction.ImgToBase64String(readPhotoPath);
                            string strReceiveInfo = axCieCloudWalkOCX1.cwFaceCompare(baseimg1, baseimg2);
                            WriteWorkLog.WriteLogs("日志", "人证比对 ：", strReceiveInfo);
                            //string strReceiveInfo = "{\"result\":0,\"source\":0.90189862251281738}";
                            JObject jo = (JObject)JsonConvert.DeserializeObject(strReceiveInfo);
                            string respCode = jo["result"].ToString();
                            string respData = jo["source"].ToString();

                            jo.RemoveAll();
                            baseimg1 = null;
                            baseimg2 = null;
                            axCieCloudWalkOCX1.cwStopCamera();
                            axCieCloudWalkOCX1.cwUnInit();
                            axCieCloudWalkOCX1.EndInit();
                            if (respCode != "0")
                            {

                                WriteWorkLog.WriteLogs("日志", "人证比对失败 ：", strReceiveInfo);
                                labValidate.Text = "核验失败";
                                SoundPlayer player = new SoundPlayer();
                                player.SoundLocation = Application.StartupPath + "\\人证比对失败.wav";
                                player.Load();
                                player.Play();
                                iTime = 3;
                            }
                            else
                            {
                                if (Convert.ToSingle(respData) < 0.60)
                                {
                                    labValidate.Text = "核验失败";
                                    WriteWorkLog.WriteLogs("日志", "信息", "人证比对失败，相似度：" + respData);
                                    SoundPlayer player = new SoundPlayer();
                                    player.SoundLocation = Application.StartupPath + "\\人证比对失败.wav";
                                    player.Load();
                                    player.Play();
                                    iTime = 3;
                                }
                                else
                                {
                                    labValidate.Text = "核验成功";
                                    axCieCloudWalkOCX1.Visible = false;
                                    SoundPlayer player = new SoundPlayer();
                                    player.SoundLocation = Application.StartupPath + "\\人证比对成功.wav";
                                    player.Load();
                                    player.Play();
                                    object[] objPara = new object[1];
                                    if (File.Exists(Application.StartupPath + "\\test.txt"))
                                    {
                                        //  bTestMode = true;
                                        StreamReader sr = new StreamReader((Application.StartupPath + "\\test.txt"), Encoding.GetEncoding("utf-8"));
                                        idInfo.name = sr.ReadLine();
                                        idInfo.num = sr.ReadLine();
                                    }
                                    objPara[0] = idInfo;
                                    string strRet = "";
                                    Thread.Sleep(1000);
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
                                    mainForm.SetbtnRetmainVisable(true);
                                    Thread.Sleep(1000);
                                    switch (strRet)
                                    {
                                        case "0$0":
                                            timer1.Stop();
                                            mainForm.SetHeadText("社保卡信息显示");
                                            mainForm.ShowChildForm("JSCardKiosks.ShowCardInfo", 1, objPara);
                                            this.Dispose();
                                            timer1.Dispose();
                                            return;
                                        case "0$1":
                                            timer1.Stop();
                                            mainForm.SetHeadText("社保卡信息显示");
                                            idInfo.BusinessType = CARD_BUSINESSTYPE_RESHOOT;
                                            objPara[0] = idInfo;
                                            mainForm.ShowChildForm("JSCardKiosks.TakePhoto", 1, objPara);
                                            this.Dispose();
                                            timer1.Dispose();
                                            return;
                                        case "0$2":
                                            timer1.Stop();
                                            mainForm.SetHeadText("个人信息确认");
                                            idInfo.BusinessType = CARD_BUSINESSTYPE_NEWADD;
                                            objPara[0] = idInfo;
                                            mainForm.ShowChildForm("JSCardKiosks.TakePhoto", 1, objPara);
                                            this.Dispose();
                                            timer1.Dispose();
                                            break;
                                        default:
                                            string strErrInfo = strRet.Split('$')[1];
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
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            WriteWorkLog.WriteLogs("日志", "异常", "downLoadCardData:" + ex.ToString());
                            if (iTime < 10)
                            {
                                iTime = 10;
                            }
                            string[] paras = { iTime.ToString(), "下载社保数据失败，" + ex.ToString() };
                            BlackForm blackForm = new BlackForm(2, "JSCardKiosks.MsgForm", paras);
                            blackForm.Show(this);
                        }

                    }
                }

            }
            if (iTime < 0)
            {
                this.Close();
            }
        }

        private void IdVerification_FormClosing(object sender, FormClosingEventArgs e)
        {
            iTime = 0;
            timer1.Stop();
            timer1.Dispose();
            timer1.Enabled = false;
            //if (axCieCloudWalkOCX1 != null)
            //{

            //}
            axCieCloudWalkOCX1.cwStopCamera();
            axCieCloudWalkOCX1.cwUnInit();
            axCieCloudWalkOCX1.EndInit();
            mainForm.SetbtnRetmainVisable(false);
            mainForm.SetpalMidVisable(false);
            mainForm.SetHeadText("社会保障卡发卡终端");
            this.Dispose();
        }
    }
}
