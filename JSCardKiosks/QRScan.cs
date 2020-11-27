
using HttpPostTest;
using QrCodeScan;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WorkLog;

namespace JSCardKiosks
{
    public partial class QRScan : Form
    {
        MainForm mainForm;
        private DeviceControlHelper m_device;
        CardDataInfo cardDataInfo;
        int iTime = 30;
        bool bhasRead = false;
        public QRScan()
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

        private void QRScan_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = PublicFuction.GetGifImage("二维码");
            mainForm = (MainForm)this.Owner;
            m_device = new DeviceControlHelper();
            cardDataInfo = new CardDataInfo();
            PublicFuction.SoundPlay("请将条码或二维码放入扫描区.wav");
            try
            {
                string strPort = DealAppConfig.GetAppSettingsValue("二维码扫描器端口");
                if (!m_device.OpenDevice(strPort))
                {
                    WriteWorkLog.WriteLogs("日志", "错误", "二维码端口打开失败");
                    return;
                }
                else
                {
                    WriteWorkLog.WriteLogs("日志", "二维码端口打开", strPort);
                    //return true;
                }
            }
            catch (Exception exErr)
            {
                WriteWorkLog.WriteLogs("日志", "异常", "二维码端口异常" + exErr.ToString());
                return;
            }

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            iTime--;
            labRetTime.Text = iTime.ToString().PadLeft(2, '0');
            if (iTime <= 0)
            {

                this.Close();
            }
            if (m_device.PortState)
            {
                CardBaseData cardBaseData = new CardBaseData("", "");
                string strErr = "";
                if (m_device.ScanQrCode() && !bhasRead)
                {
                    bhasRead = true;
                    string strTemp = m_device.strResult;
                    string codeInfo = strTemp.Replace("$","");
                    codeInfo = codeInfo.Replace("\r", "");
                    WriteWorkLog.WriteLogs("日志", "信息", "二维码：" + codeInfo);
                    int iRet = WebInfeFaceHelp.QRCodeCardRequest(codeInfo, cardBaseData, out strErr);
                    if (iRet!= 0)
                    {
                        string[] paras = { iTime.ToString(), "二维码返回制卡批次失败，" + strErr };
                        BlackForm blackForm = new BlackForm(2, "JSCardKiosks.MsgForm", paras);
                        blackForm.Show(this);
                        if (m_device.PortState)
                        {
                            m_device.CloseDevice();
                        }
                    }
                    else
                    {
                        iRet = WebInfeFaceHelp.CardDataDownload(cardBaseData, out strErr);
                        if (iRet == -1)
                        {
                            string[] paras = { iTime.ToString(), "二维码制卡下载数据失败，"+ strErr };
                            BlackForm blackForm = new BlackForm(2, "JSCardKiosks.MsgForm", paras);
                            blackForm.Show(this);
                            if (m_device.PortState)
                            {
                                m_device.CloseDevice();
                            }
                        }
                        else
                        {
                            object[] objPara = new object[1];
                            objPara[0] = cardBaseData;
                            timer1.Stop();
                            if (m_device.PortState)
                            {
                                m_device.CloseDevice();
                            }
                            mainForm.SetHeadText("社保卡信息显示");
                            mainForm.ShowChildForm("JSCardKiosks.ShowCardInfo", 1, objPara);
                        }
                    }
                   // WriteWorkLog.WriteLogs("日志", "信息", "二维码：" + m_device.strResult);
                   // this.Close();
                }
            }
        }

        private void QRScan_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_device.PortState)
            {
                m_device.CloseDevice();
            }
            mainForm.SetbtnRetmainVisable(false);
            mainForm.SetpalMidVisable(false);
            mainForm.SetHeadText("社会保障卡发卡终端");
            timer1.Stop();
        }
    }
}
