using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using WorkLog;

namespace JSCardKiosks
{
    class CardCaseHelp
    {
        public const int CARDCASECHOOSE_NOCARD = 0;
        public const int CARDCASECHOOSE_CASE1 = 1;
        public const int CARDCASECHOOSE_CASE2 = 2;
        public const int CARDCASECHOOSE_CASE3 = 3;
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
        public static int GetDevStatus(IntPtr iHandle,out bool bHasCard, out int icardCaseStatus)
        {
            byte[] InData = { 0x43, 0x31, 0x30 };
            byte[] RxData = new byte[255];
            int RxDataLen = 0;
            bHasCard = true;
            icardCaseStatus = 0;
            int iRet = CRT_591_H001Help.RS232_ExeCommand(iHandle, 3, InData, ref RxDataLen, RxData);
            if (iRet != 0)
                return -1;
            if (RxData[0] == 0x50)
            {
                if (RxData[4] == 0x30)
                {
                    bHasCard = false;
                }
                if (RxData[6] != 0x30)
                {
                    icardCaseStatus = 1;
                }
                else
                {
                    if (RxData[8] != 0x30)
                    {
                        icardCaseStatus = 2;
                    }
                    else
                    {
                        if (RxData[10] != 0x30)
                        {
                            icardCaseStatus = 3;
                        }
                    }
                }
                if (RxData[11] == 0x32)
                {
                    return -3;
                }
            }
            else
                return -2;
            return 0;
        }
        public  static string CaseCardIn(IntPtr iHandle)
        {
            byte[] InData = { 0x43, 0x32, 0x32, 0x30 };
            byte[] RxData = new byte[255];
            int RxDataLen = 0;
            int iCardCaseStatus;
            string strReportInfo ="";
            bool bHasCard;
            int iRet = GetDevStatus(iHandle,out bHasCard, out iCardCaseStatus);
            if (iRet != 0)
            {
                strReportInfo = "获取卡箱状态失败";
                WriteWorkLog.WriteLogs("日志", "错误", strReportInfo);
                return strReportInfo;
            }
            if (iCardCaseStatus == CARDCASECHOOSE_NOCARD)
            {
                strReportInfo = "卡箱没卡";
                WriteWorkLog.WriteLogs("日志", "错误", strReportInfo);
                return strReportInfo;
            }
            #region 进卡到读写器
            if (iCardCaseStatus == CARDCASECHOOSE_CASE1)
                InData[3] = 0x31;
            if (iCardCaseStatus == CARDCASECHOOSE_CASE2)
                InData[3] = 0x32;
            if (iCardCaseStatus == CARDCASECHOOSE_CASE3)
                InData[3] = 0x33;
            iRet = CRT_591_H001Help.RS232_ExeCommand(iHandle, 4, InData, ref RxDataLen, RxData);
            if (RxData[0] != 0x50)
            {
                strReportInfo = "进卡失败";
                WriteWorkLog.WriteLogs("日志", "错误", strReportInfo);
                return strReportInfo;
            }
            #endregion
            #region  IC头下压
                InData[0] = 0x43;
                InData[1] = 0x40;
                InData[2] = 0x30;
                iRet = CRT_591_H001Help.RS232_ExeCommand(iHandle, 3, InData, ref RxDataLen, RxData);
                if (RxData[0] != 0x50)
                {
                    strReportInfo = "IC头下压失败";
                    WriteWorkLog.WriteLogs("日志", "错误", strReportInfo);
                    return strReportInfo;
                }
            return "0";
            #endregion
        }
        public static string CaseCardMoveToPrinter(IntPtr iHandle)
        {
            byte[] RxData = new byte[255];
            int RxDataLen = 0;
            string strReportInfo = "";
            byte[] InData = { 0x00, 0x00, 0x00, 0x00 };
            InData[0] = 0x43;
            InData[1] = 0x33;
            InData[2] = 0x31;
            InData[3] = 0x34;
            int iRet = 0;
            iRet = CRT_591_H001Help.RS232_ExeCommand(iHandle, 4, InData, ref RxDataLen, RxData);
            if (RxData[0] != 0x50)
            {
                strReportInfo = "送卡到打印机失败";
                WriteWorkLog.WriteLogs("日志", "错误", strReportInfo);
                return strReportInfo;
            }
            return "0";
        }
        public static string CaseCardMoveFromPrinter(IntPtr iHandle)
        {
            byte[] RxData = new byte[255];
            int RxDataLen = 0;
            string strReportInfo = "";
            byte[] InData = { 0x00, 0x00, 0x00, 0x00 };
            InData[0] = 0x43;
            InData[1] = 0x32;
            InData[2] = 0x40;
            InData[3] = 0x34;
            int iRet = 0;
            iRet = CRT_591_H001Help.RS232_ExeCommand(iHandle, 4, InData, ref RxDataLen, RxData);
            if (RxData[0] != 0x50)
            {
                strReportInfo = "卡箱从打印机收卡失败";
                WriteWorkLog.WriteLogs("日志", "错误", strReportInfo);
                return strReportInfo;
            }
            return "0";
        }
        public static string CaseCardRecieve(IntPtr iHandle)
        {
            byte[] RxData = new byte[255];
            int RxDataLen = 0;
            string strReportInfo = "";
            byte[] InData = { 0x00, 0x00, 0x00, 0x00 };
            InData[0] = 0x43;
            InData[1] = 0x33;
            InData[2] = 0x31;
            InData[3] = 0x31;
            int iRet = 0;
            iRet = CRT_591_H001Help.RS232_ExeCommand(iHandle, 4, InData, ref RxDataLen, RxData);
            if (RxData[0] != 0x50)
            {
                strReportInfo = "卡箱回收卡失败";
                WriteWorkLog.WriteLogs("日志", "错误", strReportInfo);
                return strReportInfo;
            }
            return "0";
        }
        public static string CaseCardCarToReader(IntPtr iHandle)
        {
            byte[] RxData = new byte[255];
            int RxDataLen = 0;
            string strReportInfo = "";
            byte[] InData = { 0x00, 0x00, 0x00, 0x00 };
            InData[0] = 0x43;
            InData[1] = 0x32;
            InData[2] = 0x32;
            InData[3] = 0x40;
            int iRet = 0;
            iRet = CRT_591_H001Help.RS232_ExeCommand(iHandle, 4, InData, ref RxDataLen, RxData);
            if (RxData[0] != 0x50)
            {
                strReportInfo = "小车到读写器失败";
                WriteWorkLog.WriteLogs("日志", "错误", strReportInfo);
                return strReportInfo;
            }
            return "0";
        }
    }
}
