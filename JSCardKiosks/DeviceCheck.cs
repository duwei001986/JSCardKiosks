using JSCardKiosks;
using CZIDCardReader;
using QrCodeScan;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Management;
using WorkLog;

namespace JSCardKiosks
{
   
    class DeviceCheck
    {
        public static bool IDReaderCheck()
        {
            string strPath2 = AppDomain.CurrentDomain.BaseDirectory + "temp";
            if (!Directory.Exists(strPath2))
            {
                Directory.CreateDirectory(strPath2);
            }
            int iRet = ReadIDBadge.IDReaderOpen(strPath2);
            if (iRet == 0)
            {
                ReadIDBadge.Uninit();
                return true;
            }
            else
                return false;

        }
        public static IntPtr CardCaseOpen()
        {
            IntPtr iHandle = IntPtr.Zero;
            List<string> coms = new List<string>();
            string port = DealAppConfig.GetAppSettingsValue("入卡箱端口");
            string bauth = DealAppConfig.GetAppSettingsValue("入卡箱波特率"); 
            if (port != "")
            {
                port = DealAppConfig.GetAppSettingsValue("入卡箱端口");
                iHandle = CRT_591_H001Help.CRT591H001ROpenWithBaut(port, Convert.ToUInt32(bauth));
                if (iHandle != IntPtr.Zero)
                {
                    return iHandle;
                }
                else
                    return IntPtr.Zero;
            }
            else
            {
                coms = AutoGetCOM();
                if (coms.Count <= 0)
                {
                    WriteWorkLog.WriteLogs("日志", "错误", "没有串口设备连接");
                    return IntPtr.Zero;
                }
                port = coms[0];
                for (int i = 0; i < coms.Count; i++)
                {
                    iHandle = CRT_591_H001Help.CRT591H001ROpenWithBaut(coms[i], Convert.ToUInt32(bauth));
                    byte[] InData = { 0x43, 0x31, 0x30 };
                    byte[] RxData = new byte[255];
                    int RxDataLen = 0;
                    int iRet = CRT_591_H001Help.RS232_ExeCommand(iHandle, 3, InData, ref RxDataLen, RxData);
                    if (RxData[0] == 0x50)
                    {
                        WriteWorkLog.WriteLogs("日志", "信息", "打开设备成功,COM口：" + coms[i]);
                        return iHandle;
                    }
                    if (i == coms.Count - 1)
                    {
                        WriteWorkLog.WriteLogs("日志", "信息", "打开设备失败");
                        return IntPtr.Zero;
                    }

                }
                return iHandle;
            }
        }
        public static List<string> AutoGetCOM()
        {
            List<string> coms = new List<string>();

            try
            {
                //搜索设备管理器中的所有条目
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_PnPEntity"))
                {
                    var hardInfos = searcher.Get();
                    foreach (var hardInfo in hardInfos)
                    {
                        if (hardInfo.Properties["Name"].Value != null)
                        {
                            if (hardInfo.Properties["Name"].Value.ToString().Contains("COM"))
                            {
                                if (!hardInfo.Properties["Name"].Value.ToString().Contains("RFCOMM"))
                                {
                                    string portName = hardInfo.Properties["Name"].Value.ToString();
                                    if (portName.Contains("Prolific USB-to-Serial Comm Port"))
                                    {
                                        coms.Add(portName.Substring(portName.IndexOf('(') + 1, portName.IndexOf(')') - portName.IndexOf('(') - 1));
                                    }
                                    else
                                    {
                                        coms.Add(hardInfo.Properties["Name"].Value.ToString());
                                    }
                                }
                            }
                        }
                    }
                    searcher.Dispose();
                }

            }
            catch
            {
                return null;
            }
            return coms;
        }
        public static int GetDevStatus(out string strCaseStatus, out string strRecieveStatus)
        {
            Hashtable ht = new Hashtable();
            ht.Add(0x30, "无卡");
            ht.Add(0x31, "卡少");
            ht.Add(0x32, "卡足");
            Hashtable ht2 = new Hashtable();
            ht2.Add(0x30, "无卡或未满");
            ht2.Add(0x31, "有卡");
            ht2.Add(0x32, "回收满");
            strCaseStatus = "";
            strRecieveStatus = "";
            byte[] InData = { 0x43, 0x31, 0x30 };
            byte[] RxData = new byte[255];
            int RxDataLen = 0;
            IntPtr iHanlde = CardCaseOpen();
            if (iHanlde == IntPtr.Zero)
            {
                return -1;
            }
            int iRet = CRT_591_H001Help.RS232_ExeCommand(iHanlde, 3, InData, ref RxDataLen, RxData);
            CRT_591_H001Help.CRT591H001RClose(iHanlde);
            if (iRet != 0)
                return -2;
            if (RxData[0] == 0x50)
            {
                string str = ht[0x00000030].ToString();
                int iByte = RxData[6];
                strCaseStatus = "卡箱1:" + (string)ht[iByte = RxData[6]] + "，卡箱2:" + (string)ht[iByte = RxData[8]] + "，卡箱3:" + (string)ht[iByte = RxData[10]];
                strRecieveStatus = "回收箱1:" + (string)ht2[iByte = RxData[7]] + "，回收箱2:" + (string)ht2[iByte = RxData[9]] + "，回收箱3:" + (string)ht2[iByte = RxData[11]];
            }
            else
                return -3;
            return 0;
        }
        public static bool PrintCheck(out int pnRemain)
        {
            int pnType = 0;
            pnRemain = 0;
            int pnMax = 0;
            UInt64 status = 0;
            int iRet = PrintControl.IDPGetRibbonRemain(0x40, out pnType, out pnRemain, out pnMax, out status);
            WriteWorkLog.WriteLogs("日志", "信息", "打印机status:" + Convert.ToString((long)status, 16).PadLeft(16, '0'));
            if (iRet == 0)
            {
                if (pnRemain>0)
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }
        public static bool QRScanerCheck()
        {
           DeviceControlHelper m_device = new DeviceControlHelper();
            try
            {
                string strPort = DealAppConfig.GetAppSettingsValue("二维码扫描器端口");
                if (!m_device.OpenDevice(strPort))
                {
                    WriteWorkLog.WriteLogs("日志", "错误", "二维码端口打开失败");
                    return false;
                }
                else
                {
                    WriteWorkLog.WriteLogs("日志", "二维码端口打开", strPort);
                    return true;
                }
            }
            catch (Exception exErr)
            {
                WriteWorkLog.WriteLogs("日志", "异常", "二维码端口异常" + exErr.ToString());
                return false;
            }
            finally
            {
                m_device.CloseDevice();
            }

        }
    }
}
