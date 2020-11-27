using JSCardKiosks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JSCardKiosks
{
    public partial class MechineCheck : Form
    {
        int pnRemain = 0;
        int iTime = 9;
        int iCardCase = 0;
        bool bPrintReady = false;
        int hasPass = 1;
        string strIsPass = "已连接";
        string strCaseStatus = "";
        string strRecieveStatus = "";
        List<string> strDevName = new List<string>();
        MainForm mainForm;
        public MechineCheck()
        {
            InitializeComponent();
        }

        private void MechineCheck_Load(object sender, EventArgs e)
        {
            mainForm = (MainForm)this.Owner;
            listHardwareCheck.Items.Add(" 设备开始自检......");
            strDevName.Add(" 连接身份证阅读器" + "                                                                      ");
            strDevName.Add(" 连接二维码阅读器" + "                                                                      ");
            strDevName.Add(" 连接"+ DealAppConfig.GetAppSettingsValue("拍照摄像头") + "   " + "                                                                      ");
            strDevName.Add(" 连接入卡箱          " + "                                                                      ");
            strDevName.Add(" 连接打印模块       " + "                                                                      ");
            strDevName.Add(" 连接社保网          " + "                                                                      ");
            strDevName.Add(" 入卡箱卡量         " + "                                                                      ");
            strDevName.Add(" 废卡箱卡量        " + "                                                                      ");
            strDevName.Add(" 色带剩余量           " + "                                                                      ");
            listHardwareCheck.DrawMode = DrawMode.OwnerDrawFixed;
        }

        private void btnSure_Click(object sender, EventArgs e)
        {
            mainForm.SetpalMidVisable(false);
            mainForm.SetHeadText("社会保障卡发卡终端");//社会保障卡发卡终端
            this.Close();
           
        }

        private void listHardwareCheck_DrawItem(object sender, DrawItemEventArgs e)
        {
            string s = listHardwareCheck.Items[e.Index].ToString();
            if (s.Contains("已连接"))
            {
                e.Graphics.DrawString(s, listHardwareCheck.Font, Brushes.Green, e.Bounds);
            }
            else if (s.Contains("未通过"))
            {
                hasPass = -1;
                e.Graphics.DrawString(s, listHardwareCheck.Font, Brushes.Red, e.Bounds);
            }
            else if (s.Contains("入卡箱卡量"))
            {
                if (s.Contains("卡足"))
                {
                    e.Graphics.DrawString(s, listHardwareCheck.Font, Brushes.Green, e.Bounds);
                }
                else if (s.Contains("卡少"))
                {
                    if (hasPass == 1)
                        hasPass = 0;
                    e.Graphics.DrawString(s, listHardwareCheck.Font, new SolidBrush(Color.FromArgb(244, 214, 11)), e.Bounds);
                }
                else
                {
                    e.Graphics.DrawString(s, listHardwareCheck.Font, Brushes.Red, e.Bounds);
                }
            }
            else if (s.Contains("废卡箱卡量"))
            {
                if (s.Contains("无卡或未满"))
                {
                    e.Graphics.DrawString(s, listHardwareCheck.Font, Brushes.Green, e.Bounds);
                }
                else if (s.Contains("有卡"))
                {
                    hasPass = 0;
                    e.Graphics.DrawString(s, listHardwareCheck.Font, new SolidBrush(Color.FromArgb(244, 214, 11)), e.Bounds);
                }
                else
                {
                    hasPass = -1;
                    e.Graphics.DrawString(s, listHardwareCheck.Font, Brushes.Red, e.Bounds);
                }
            }
            else if (s.Contains("色带剩余量"))
            {
                if (pnRemain < 20)
                    e.Graphics.DrawString(s, listHardwareCheck.Font, new SolidBrush(Color.FromArgb(244, 214, 11)), e.Bounds);
                else
                    e.Graphics.DrawString(s, listHardwareCheck.Font, Brushes.Green, e.Bounds);
            }
            else
                e.Graphics.DrawString(s, listHardwareCheck.Font, Brushes.DarkSlateGray, e.Bounds);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            switch (9 - iTime)
            {
                case 0:
                    if (DealAppConfig.GetAppSettingsValue("身份证阅读器") != "无")
                    {
                        if (DeviceCheck.IDReaderCheck())
                            strIsPass = "已连接";
                        else
                            strIsPass = "未通过";
                        listHardwareCheck.Items.Add(strDevName[9 - iTime] + strIsPass);
                    }
                    break;

                case 1:
                    if (DealAppConfig.GetAppSettingsValue("二维码扫描器") != "无")
                    {
                        if (DeviceCheck.QRScanerCheck())
                        {
                            strIsPass = "已连接";
                        }
                        else
                            strIsPass = "未通过";
                        listHardwareCheck.Items.Add(strDevName[9 - iTime] + strIsPass);
                    }
                    break;
                case 2:
                    if (DealAppConfig.GetAppSettingsValue("拍照摄像头") == "双目摄像头")
                    {
                        if (showCameraStatus())
                            strIsPass = "已连接";
                        else
                            strIsPass = "未通过";
                        listHardwareCheck.Items.Add(strDevName[9 - iTime] + strIsPass);
                    }
                    else
                    {
                       // showCameraStatus();
                        strIsPass = "已连接";
                        listHardwareCheck.Items.Add(strDevName[9 - iTime] + strIsPass);
                    }
                    break;
                case 3:
                    if (DealAppConfig.GetAppSettingsValue("入卡箱") != "无")
                    {
                        iCardCase = DeviceCheck.GetDevStatus(out strCaseStatus, out strRecieveStatus);
                        if (iCardCase == 0)
                            strIsPass = "已连接";
                        else
                            strIsPass = "未通过";
                        listHardwareCheck.Items.Add(strDevName[9 - iTime] + strIsPass);
                    }
                    break;
                case 4:
                    if (DealAppConfig.GetAppSettingsValue("打印模块") == "70")
                    {
                        bPrintReady = DeviceCheck.PrintCheck(out pnRemain);
                        if (bPrintReady)
                            strIsPass = "已连接";
                        else
                            strIsPass = "未通过";
                    }
                    if (DealAppConfig.GetAppSettingsValue("打印模块") == "51")
                    {
                        bool bHasCard = true;
                       int iRet = IDP51PrintControl.checkPrinterStatus(out bHasCard,out pnRemain);
                        if (iRet ==0)
                        {
                            bPrintReady = true;
                            strIsPass = "已连接";
                        }
                        else
                            strIsPass = "未通过";
                    }
                    listHardwareCheck.Items.Add(strDevName[9 - iTime] + strIsPass);
                    break;
                case 5:
                    break;
                case 6:
                    if (DealAppConfig.GetAppSettingsValue("入卡箱") != "无" && iCardCase == 0)
                        listHardwareCheck.Items.Add(strDevName[9 - iTime] + strCaseStatus);
                    break;
                case 7:
                    if (DealAppConfig.GetAppSettingsValue("入卡箱") != "无" && iCardCase == 0)
                        listHardwareCheck.Items.Add(strDevName[9 - iTime] + strRecieveStatus);
                    break;
                case 8:
                    if (DealAppConfig.GetAppSettingsValue("打印模块") == "70")
                    {
                        if (bPrintReady)
                            listHardwareCheck.Items.Add(strDevName[9 - iTime] + pnRemain.ToString());
                    }
                    if (DealAppConfig.GetAppSettingsValue("打印模块") == "51")
                    {
                        if (bPrintReady)
                            listHardwareCheck.Items.Add(strDevName[9 - iTime] + pnRemain.ToString());
                    }
                        break;

                default:
                    break;
            }
            iTime--;
            if (iTime <= 0)
            {
                timer1.Stop();
                timer1.Enabled = false;
                if (hasPass == 0)
                {
                    btnSure.Visible = true;
                }
                if (hasPass == 1)
                {
                    listHardwareCheck.Visible = false;
                    this.Close();
                    mainForm.SetpalMidVisable(false);
                    mainForm.SetHeadText("社会保障卡发卡终端");//社会保障卡发卡终端
                }
            }
        }
        public bool showCameraStatus()
        {
            //int iret = axCieCloudWalkOCX1.cwInit();
            //iret = axCieCloudWalkOCX1.cwStartCamera();
            //if (iret != 0)
            //{
            //    axCieCloudWalkOCX1.cwUnInit();
            //    return false;
            //}

            //axCieCloudWalkOCX1.cwStopCamera();
            //axCieCloudWalkOCX1.cwUnInit();
           
            return true;
        }

        private void MechineCheck_FormClosing(object sender, FormClosingEventArgs e)
        {
           
            timer1.Stop();
            timer1.Dispose();
            //this.Dispose();
        }
    }
}

