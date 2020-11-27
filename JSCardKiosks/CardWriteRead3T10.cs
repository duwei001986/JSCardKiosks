
using CardWriteReadT10;
using sun.swing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using WorkLog;

namespace MyThirdCard 
{
    public struct CARegisterInfo
    {
        public string qmzs;
        public string jmzs;
        public string jmmy;
        public string zkmy;
        public string glypin;
    }
    public class CardWriteReadThirdT10
    {
        [DllImport("SmAlgo.dll", EntryPoint = "Sm4Crypt", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Sm4Crypt(
                                                            int iCryptMode,
                                                            int iPadMode,
                                                            int iChainMode,
                                                             byte[] ucpKey, int iKeyLen,
                                                             byte[] ucpInData, int iInDataLen,
                                                             byte[] ucpIcv, int iIcvLen,
                                                            [MarshalAs(UnmanagedType.LPArray)]byte[] ucpOutData, ref int ipOutDataLen);

        [DllImport("SmAlgo.dll", EntryPoint = "Sm3Hash", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Sm3Hash(
                                                        byte[] ucpKey, int iKeyLen,
                                                       [MarshalAs(UnmanagedType.LPArray)]byte[] ucpOutData, ref int ipOutDataLen);

        public static String byteToChar(int length, byte[] data)
        {
            StringBuilder stringbuiler = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                String temp = data[i].ToString("x");
                if (temp.Length == 1)
                {
                    stringbuiler.Append("0" + temp);
                }
                else
                {
                    stringbuiler.Append(temp);
                }
            }
            return (stringbuiler.ToString());
        }
        public static int ReadBankNo(out string bankNo, out string strErr)
        {
            bankNo = "";
            strErr = "";
            string strRData = "";
            string strInstruct = "";
            int iHandle;
            int st;
            byte[] snr = new byte[128];
            byte rlen = 0;
            byte[] rbuff = new byte[128];
            iHandle = T10ReaderHelp.dc_init(100, 115200);
            if (iHandle < 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "设备未打开");
                strErr = "设备未打开";
                return -1;
            }
            st = T10ReaderHelp.dc_setcpu(iHandle, 0x0c);
            if (st != 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "dc set cpu error");
                T10ReaderHelp.dc_exit(iHandle);
                strErr = "设置卡类型失败";
                return -101;
            }
            st = T10ReaderHelp.dc_cpureset(iHandle, ref rlen, snr);
            if (st != 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "IC复位失败");
                T10ReaderHelp.dc_exit(iHandle);
                strErr = "IC复位失败";
                return -2;
            }
            else
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "复位成功:" + byteToChar(rlen, snr));
            }
            strInstruct = "00A4040008A000000333010101";//  
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "00A4041：" + strRData);
                    T10ReaderHelp.dc_exit(iHandle);
                    return -30;
                }
                string strReadData = strRData.Substring(4, strRData.Length - 8);
            }
            else
            {
                return -31;
            }
            strInstruct = "00B2011400";//  
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if(strRData=="")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "00B20114：" + strRData);
                    T10ReaderHelp.dc_exit(iHandle);
                    return -28;
                }
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "00B20114：" + strRData);
                    T10ReaderHelp.dc_exit(iHandle);
                    return -28;
                }
                int iTag = strRData.IndexOf("5A");
                string strReadData = strRData.Substring(iTag+4, 19);
                bankNo = strReadData;
            }
            else
            {
                strInstruct = "00B2011C00";//  
                if (SendCommand(iHandle, strInstruct, out strRData))
                {
                    if (strRData == "")
                    {
                        WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "00B20114：" + strRData);
                        T10ReaderHelp.dc_exit(iHandle);
                        return -28;
                    }
                    if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                    {
                        WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "00B20114：" + strRData);
                        T10ReaderHelp.dc_exit(iHandle);
                        return -28;
                    }
                    int iTag = strRData.IndexOf("5A");
                    string strReadData = strRData.Substring(iTag + 4, 19);
                    bankNo = strReadData;
                }
                else
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    return -34;
                }
               
            }
            T10ReaderHelp.dc_beep(iHandle, 10);
            T10ReaderHelp.dc_exit(iHandle);
            return 0;
        }
        public static int ReadCard(out List<string> strCardInfo)
        {
            strCardInfo = new List<string>();
            string strInstruct = "";
            int iHandle;
            int st;
            byte[] snr = new byte[128];
            byte rlen = 0;
            byte[] rbuff = new byte[128];
            iHandle = T10ReaderHelp.dc_init(100, 115200);
            if (iHandle < 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "设备未打开");
                return -1;
            }
            st = T10ReaderHelp.dc_setcpu(iHandle, 0x0c);
            if (st != 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "dc set cpu error");
                T10ReaderHelp.dc_exit(iHandle);
                return -101;
            }
            st = T10ReaderHelp.dc_cpureset(iHandle, ref rlen, snr);
            if (st != 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "IC复位失败");
                T10ReaderHelp.dc_exit(iHandle);
                return -2;
            }
            else
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "复位成功:" + byteToChar(rlen, snr));
            }
            strInstruct = "00A404000F7378312E73682EC9E7BBE1B1A3D5CF";
            string strRData = "";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                //if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                //{
                //    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "选择社保文件：" + strRData);
                //    return -3;
                //}
            }
            else
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", strInstruct + "：" + strRData);
                return -4;
            }
            strInstruct = "00A4000002EF05";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(0, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "选择社保文件EF05：" + strRData);
                    return -5;
                }
            }
            else
            {
                return -6;
            }
            strInstruct = "00B2010412";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "读取卡识别码：" + strRData);
                    return -6;
                }
                strCardInfo.Add(strRData.Substring(4, strRData.Length - 8));
            }
            else
            {
                return -7;
            }
            strInstruct = "00B2052806";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "读发卡日期：" + strRData);
                    return -8;
                }
                strCardInfo.Add(strRData.Substring(4, strRData.Length - 8));
            }
            else
            {
                return -9;
            }
            strInstruct = "00B207280B";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "读卡号：" + strRData);
                    return -10;
                }
                string strReadData = strRData.Substring(4, strRData.Length - 8);
                strCardInfo.Add(GetChsFromHex(strReadData));
            }
            else
            {
                return -11;
            }
            strInstruct = "00B2083014";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "读社会保障号码：" + strRData);
                    return -12;
                }
                string strReadData = strRData.Substring(4, strRData.Length - 8);
                strCardInfo.Add(GetChsFromHex(strReadData));
            }
            else
            {
                return -13;
            }
            strInstruct = "00B2093020";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "读姓名：" + strRData);
                    return -14;
                }
                string strReadData = strRData.Substring(4, strRData.Length - 8);
                strCardInfo.Add((GetChsFromHex(strReadData)));
            }
            else
            {
                return -15;
            }
            strInstruct = "00B24E3016";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "读姓名扩展：" + strRData);
                    return -16;
                }
                string strReadData = strRData.Substring(4, strRData.Length - 8);
                strCardInfo.Add(GetChsFromHex(strReadData));
            }
            else
            {
                return -17;
            }
            strInstruct = "00B20A3003";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "读性别：" + strRData);
                    return -18;
                }
                string strReadData = strRData.Substring(4, strRData.Length - 8);
                strCardInfo.Add(GetChsFromHex(strReadData));
            }
            else
            {
                return -19;
            }
            strInstruct = "00B20B3003";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "读民族：" + strRData);
                    return -20;
                }
                strCardInfo.Add(strRData.Substring(4, strRData.Length - 8));
            }
            else
            {
                return -21;
            }
            strInstruct = "00B20D3006";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "读出生日期：" + strRData);
                    return -22;
                }
                strCardInfo.Add(strRData.Substring(4, strRData.Length - 8));
            }
            else
            {
                return -23;
            }
            strInstruct = "00B20C3005";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "读出生地代码：" + strRData);
                    return -22;
                }
                strCardInfo.Add(strRData.Substring(4, strRData.Length - 8));
            }
            else
            {
                return -23;
            }
            strInstruct = "00A4000002EF0D";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(0, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "选择社保文件EF0D：" + strRData);
                    return -24;
                }
            }
            else
            {
                return -25;
            }
            strInstruct = "00B201040E";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "读人员识别号：" + strRData);
                    return -26;
                }
                string strReadData = strRData.Substring(4, strRData.Length - 8);
                strCardInfo.Add(GetChsFromHex(strReadData));
            }
            else
            {
                return -27;
            }

            strInstruct = "00B2020405";//  
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "读初次申请地代码：" + strRData);
                    return -28;
                }
                string strReadData = strRData.Substring(4, strRData.Length - 8);
                strCardInfo.Add(strReadData);
            }
            else
            {
                return -29;
            }
            st = T10ReaderHelp.dc_cpureset(iHandle, ref rlen, snr);
            if (st != 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "IC复位失败");
                T10ReaderHelp.dc_exit(iHandle);
                return -2;
            }
            else
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "复位成功:" + byteToChar(rlen, snr));
            }
            strInstruct = "00A4040008A000000333010101";//  
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "00A4041：" + strRData);
                    return -30;
                }
                string strReadData = strRData.Substring(4, strRData.Length - 8);
            }
            else
            {
                return -31;
            }
            strInstruct = "00B2011400";//  
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "00B20114：" + strRData);
                    return -28;
                }
                string strReadData = strRData.Substring(4, strRData.Length - 8);
                strCardInfo.Add(strReadData.Substring(16, 19));
            }
            else
            {
                return -34;
            }
            T10ReaderHelp.dc_beep(iHandle, 10);
            T10ReaderHelp.dc_exit(iHandle);
            return 0;
        }

        public static int WriteCard2(List<string> strCardInfo, out string cardIdCode)
        {
            string strInstruct = "";
            cardIdCode = "";
            int iHandle;
            int st;
            byte[] snr = new byte[128];
            byte rlen = 0;
            byte[] rbuff = new byte[128];
            string strRData = "";
            iHandle = T10ReaderHelp.dc_init(100, 115200);
            if (iHandle < 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "设备未打开");
                return -1;
            }
            st = T10ReaderHelp.dc_setcpu(iHandle, 0x0c);
            if (st != 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "dc set cpu error");
                T10ReaderHelp.dc_exit(iHandle);
                return -101;
            }
            st = T10ReaderHelp.dc_cpureset(iHandle, ref rlen, snr);
            if (st != 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "IC复位失败");
                T10ReaderHelp.dc_exit(iHandle);
                return -2;
            }
            else
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "复位成功:" + byteToChar(rlen, snr));
            }
            string strReset = byteToChar(rlen, snr);
            string cardDiv = strReset.Substring(strReset.Length - 16);
            WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "cardDiv:" + cardDiv);
            strInstruct = "00A404000F7378312E73682EC9E7BBE1B1A3D5CF";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                //if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                //{
                //    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "选择社保文件：" + strRData);
                //    return -3;
                //}
            }
            else
            {
                return -4;
            }
            strInstruct = "00A4000002EF05";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(0, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "选择社保文件EF05：" + strRData);
                    return -5;
                }
            }
            else
            {
                return -6;
            }
            strInstruct = "00B2010412";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "读取卡识别码：" + strRData);
                    return -6;
                }
            }
            else
            {
                return -7;
            }
            cardIdCode = strRData.Substring(4, strRData.Length - 8).ToUpper();

            string cityDiv = GetHexFromChs(strRData.Substring(4, 6)) + "7378";
            WriteWorkLog.WriteLogs("IC卡操作日志", "信息", "cityDiv：" + cityDiv);
            strInstruct = "0084000008";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "取随机数：" + strRData);
                    return -8;
                }
            }
            else
            {
                return -9;
            }
            string random = strRData.Substring(0, 16);
            WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "random:" + random);
            st = T10ReaderHelp.dc_setcpu(iHandle, 0x0D);//PSAM
            if (st != 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "dc set PSAM error");
                T10ReaderHelp.dc_exit(iHandle);
                return -101;
            }
            st = T10ReaderHelp.dc_cpureset(iHandle, ref rlen, snr);
            if (st != 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "PSAM卡复位失败1");
                T10ReaderHelp.dc_exit(iHandle);
                return -2;
            }
            else
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "PSAM卡复位成功:" + byteToChar(rlen, snr));
            }
            strInstruct = "00A4040006D15600000590";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                //if (strRData.Substring(0, 2) != "61")
                //{
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "选择PSAM文件：" + strRData);
                //    return -11;
                //}
            }
            else
            {
                return -12;
            }
            strInstruct = "BFDE481218" + cardDiv + cityDiv + random;
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "分散密钥：" + strRData);
                    return -13;
                }
            }
            else
            {
                return -14;
            }
            strInstruct = "80FA000008" + random;
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(0, 2) != "61")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "info", "80FA000008：" + strRData);
                }
                else
                {
                    strInstruct = "00C0000008";
                    if (SendCommand(iHandle, strInstruct, out strRData))
                    {
                        if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                        {
                            WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "取CPU卡随机数：" + strRData);
                            return -17;
                        }
                    }
                    else
                    {
                        return -18;
                    }
                }
            }
            else
            {
                return -16;
            }

            String macData = strRData.Substring(0, 16);
            st = T10ReaderHelp.dc_setcpu(iHandle, 0x0C);//CPU
            if (st != 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "dc set CPU error");
                T10ReaderHelp.dc_exit(iHandle);
                return -101;
            }
            strInstruct = "0082000410" + macData + random;
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "外部认证：" + strRData);
                    return -19;
                }
            }
            else
            {
                return -20;
            }
            WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "外部认证：" + strRData);
            #region 写发卡日期
            strInstruct = "00DC0528060504" + strCardInfo[0];
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "写发卡日期：" + strRData);
                    return -21;
                }
            }
            else
            {
                return -22;
            }
            #endregion
            #region 写卡号
            strInstruct = "00DC07280B0709" + GetHexFromChs(strCardInfo[1].Trim());
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "写卡号：" + strRData);
                    return -23;
                }
            }
            else
            {
                return -24;
            }
            #endregion
            #region 写社会保障号码
            strInstruct = "00DC0830140812" + GetHexFromChs(strCardInfo[2].Trim());
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "写社会保障号码 ：" + strRData);
                    return -24;
                }
            }
            else
            {
                return -25;
            }
            #endregion
            #region 写姓名
            strInstruct = "00DC093020091E" + GetHexFromChs(strCardInfo[3].Trim()).PadRight(60, '0');
            WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "strInstruct ：" + strInstruct);
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "写姓名 ：" + strRData);
                    return -26;
                }
            }
            else
            {
                return -27;
            }
            #endregion
            #region 写姓名扩展
            if (strCardInfo[4] != "")
            {
                strInstruct = "00DC4E30164E14" + GetHexFromChs(strCardInfo[4].Trim()).PadRight(40, 'F');
                if (SendCommand(iHandle, strInstruct, out strRData))
                {
                    if (strRData != "9000")
                    {
                        WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "写姓名扩展 ：" + strRData);
                        return -28;
                    }
                }
                else
                {
                    return -29;
                }
            }
            #endregion
            # region 写性别
            strInstruct = "00DC0A30030A01" + (GetHexFromChs(strCardInfo[5].Trim()));
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "写性别  ：" + strRData);
                    return -30;
                }
            }
            else
            {
                return -31;
            }
            #endregion
            #region 写民族
            strInstruct = "00DC0B30030B01" + strCardInfo[6].Trim();
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "写民族  ：" + strRData);
                    return -32;
                }
            }
            else
            {
                return -33;
            }
            #endregion
            #region 写出生日期
            strInstruct = "00DC0D30060D04" + (strCardInfo[7].Trim());
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "写出生日期  ：" + strRData);
                    return -34;
                }
            }
            else
            {
                return -35;
            }
            #endregion
            #region 写出生地代码
            if (strCardInfo[8].Trim().Length > 2)
            {
                strInstruct = "00DC0C30050C03" + (strCardInfo[8].Trim());
                if (SendCommand(iHandle, strInstruct, out strRData))
                {
                    if (strRData != "9000")
                    {
                        WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "写出生地代码  ：" + strRData);
                        return -36;
                    }
                }
                else
                {
                    return -37;
                }

            }
            #endregion
            #region 外部认证2
            strInstruct = "0084000008";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "取随机数：" + strRData);
                    return -38;
                }
            }
            else
            {
                return -39;
            }
            random = strRData.Substring(0, 16);
            st = T10ReaderHelp.dc_setcpu(iHandle, 0x0D);//PSAM
            if (st != 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "dc set PSAM error");
                T10ReaderHelp.dc_exit(iHandle);
                return -101;
            }
            strInstruct = "00A4040006D15600000590";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                //if (strRData.Substring(0, 2) != "61")
                //{
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "选择PSAM文件：" + strRData);
                //    return -41;
                //}
            }
            else
            {
                return -42;
            }
            strInstruct = "BFDE481318" + cardDiv + cityDiv + random;
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "分散密钥：" + strRData);
                    return -43;
                }
            }
            else
            {
                return -44;
            }
            strInstruct = "80FA000008" + random;
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(0, 2) != "61")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "加密CPU卡随机数：" + strRData);
                    // return -45;
                }
                else
                {
                    strInstruct = "00C0000008";
                    if (SendCommand(iHandle, strInstruct, out strRData))
                    {
                        if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                        {
                            WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "取CPU卡随机数：" + strRData);
                            return -47;
                        }
                    }
                    else
                    {
                        return -48;
                    }
                }
            }
            else
            {
                return -46;
            }


            macData = strRData.Substring(0, 16);
            st = T10ReaderHelp.dc_setcpu(iHandle, 0x0C);//CPU
            if (st != 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "dc set CPU error");
                T10ReaderHelp.dc_exit(iHandle);
                return -101;
            }
            strInstruct = "0082000D10" + macData + random;
            #endregion
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "外部认证2：" + strRData);
                    return -49;
                }
            }
            else
            {
                return -50;
            }
            strInstruct = "00A4000002EF0D";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(0, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "选择社保文件EF05：" + strRData);
                    return -51;
                }
            }
            else
            {
                return -52;
            }
            #region 写人员识别码
            strInstruct = "00DC01040EEA0C" + (GetHexFromChs(strCardInfo[9].Trim()));
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "写人员识别码  ：" + strRData);
                    return -53;
                }
            }
            else
            {
                return -54;
            }
            #endregion
            #region 写初次申领地
            if (strCardInfo[10].Trim() != "")
            {
                strInstruct = "00DC020405EB03" + strCardInfo[10].Trim();
                if (SendCommand(iHandle, strInstruct, out strRData))
                {
                    if (strRData != "9000")
                    {
                        WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "写初次申领地  ：" + strRData);
                        return -55;
                    }
                }
                else
                {
                    return -56;
                }
            }
            #endregion
            T10ReaderHelp.dc_beep(iHandle, 10);
            T10ReaderHelp.dc_exit(iHandle);
            return 0;
        }
        public static int WriteCard(List<string> strCardInfo, byte PsamNo, out string cardIdCode)
        {
            string strInstruct = "";
            cardIdCode = "";
            int iHandle;
            int st;
            byte[] snr = new byte[128];
            byte rlen = 0;
            byte[] rbuff = new byte[128];
            string strRData = "";
            iHandle = T10ReaderHelp.dc_init(100, 115200);
            if (iHandle < 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "设备未打开");
                return -1;
            }
            st = T10ReaderHelp.dc_setcpu(iHandle, 0x0c);
            if (st != 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "dc set cpu error");
                T10ReaderHelp.dc_exit(iHandle);
                return -101;
            }
            st = T10ReaderHelp.dc_cpureset(iHandle, ref rlen, snr);
            if (st != 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "IC复位失败");
                T10ReaderHelp.dc_exit(iHandle);
                return -2;
            }
            else
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "复位成功:" + byteToChar(rlen, snr));
            }
            string strReset = byteToChar(rlen, snr);
            string cardDiv = strReset.Substring(strReset.Length - 16);
            WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "cardDiv:" + cardDiv);
            strInstruct = "00A404000F7378312E73682EC9E7BBE1B1A3D5CF";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                //if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                //{
                //    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "选择社保文件：" + strRData);
                //    return -3;
                //}
            }
            else
            {
                return -4;
            }
            strInstruct = "00A4000002EF05";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(0, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "选择社保文件EF05：" + strRData);
                    return -5;
                }
            }
            else
            {
                return -6;
            }
            strInstruct = "00B2010412";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "读取卡识别码：" + strRData);
                    return -6;
                }
            }
            else
            {
                return -7;
            }
            cardIdCode = strRData.Substring(4, strRData.Length - 8).ToUpper();

            string cityDiv = GetHexFromChs(strRData.Substring(4, 6)) + "7378";
            WriteWorkLog.WriteLogs("IC卡操作日志", "信息", "cityDiv：" + cityDiv);
            strInstruct = "0084000008";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "取随机数：" + strRData);
                    return -8;
                }
            }
            else
            {
                return -9;
            }
            string random = strRData.Substring(0, 16);
            WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "random:" + random);
            st = T10ReaderHelp.dc_setcpu(iHandle, PsamNo);//PSAM
            if (st != 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "dc set PSAM error");
                T10ReaderHelp.dc_exit(iHandle);
                return -101;
            }
            st = T10ReaderHelp.dc_cpureset(iHandle, ref rlen, snr);
            if (st != 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "PSAM卡复位失败");
                T10ReaderHelp.dc_exit(iHandle);
                return -2;
            }
            else
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "PSAM卡复位成功:" + byteToChar(rlen, snr));
            }
            strInstruct = "00A4040006D15600000590";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                //if (strRData.Substring(0, 2) != "61")
                //{
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "选择PSAM文件：" + strRData);
                //    return -11;
                //}
            }
            else
            {
                return -12;
            }
            strInstruct = "BFDE481218" + cardDiv + cityDiv + random;
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "分散密钥：" + strRData);
                    return -13;
                }
            }
            else
            {
                return -14;
            }
            strInstruct = "80FA000008" + random;
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(0, 2) != "61")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "info", "80FA000008：" + strRData);
                }
                else
                {
                    strInstruct = "00C0000008";
                    if (SendCommand(iHandle, strInstruct, out strRData))
                    {
                        if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                        {
                            WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "取CPU卡随机数：" + strRData);
                            return -17;
                        }
                    }
                    else
                    {
                        return -18;
                    }
                }
            }
            else
            {
                return -16;
            }

            String macData = strRData.Substring(0, 16);
            st = T10ReaderHelp.dc_setcpu(iHandle, 0x0C);//CPU
            if (st != 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "dc set CPU error");
                T10ReaderHelp.dc_exit(iHandle);
                return -101;
            }
            strInstruct = "0082000410" + macData + random;
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "外部认证：" + strRData);
                    return -19;
                }
            }
            else
            {
                return -20;
            }
            WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "外部认证：" + strRData);
            #region 写发卡日期
            strInstruct = "00DC0528060504" + strCardInfo[0];
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "写发卡日期：" + strRData);
                    return -21;
                }
            }
            else
            {
                return -22;
            }
            #endregion
            #region 写卡号
            strInstruct = "00DC07280B0709" + GetHexFromChs(strCardInfo[1].Trim());
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "写卡号：" + strRData);
                    return -23;
                }
            }
            else
            {
                return -24;
            }
            #endregion
            #region 写社会保障号码
            strInstruct = "00DC0830140812" + GetHexFromChs(strCardInfo[2].Trim());
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "写社会保障号码 ：" + strRData);
                    return -24;
                }
            }
            else
            {
                return -25;
            }
            #endregion
            #region 写姓名
            strInstruct = "00DC093020091E" + GetHexFromChs(strCardInfo[3].Trim()).PadRight(60, '0');
            WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "strInstruct ：" + strInstruct + strCardInfo[3].Trim());
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "写姓名 ：" + strRData);
                    return -26;
                }
            }
            else
            {
                return -27;
            }
            #endregion
            #region 写姓名扩展
            if (strCardInfo[4] != "")
            {
                strInstruct = "00DC4E30164E14" + GetHexFromChs(strCardInfo[4].Trim()).PadRight(40, 'F');
                if (SendCommand(iHandle, strInstruct, out strRData))
                {
                    if (strRData != "9000")
                    {
                        WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "写姓名扩展 ：" + strRData);
                        return -28;
                    }
                }
                else
                {
                    return -29;
                }
            }
            #endregion
            # region 写性别
            strInstruct = "00DC0A30030A01" + (GetHexFromChs(strCardInfo[5].Trim()));
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "写性别  ：" + strRData);
                    return -30;
                }
            }
            else
            {
                return -31;
            }
            #endregion
            #region 写民族
            strInstruct = "00DC0B30030B01" + strCardInfo[6].Trim();
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "写民族  ：" + strRData);
                    return -32;
                }
            }
            else
            {
                return -33;
            }
            #endregion
            #region 写出生日期
            strInstruct = "00DC0D30060D04" + (strCardInfo[7].Trim());
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "写出生日期  ：" + strRData);
                    return -34;
                }
            }
            else
            {
                return -35;
            }
            #endregion
            #region 写出生地代码
            if (strCardInfo[8].Trim().Length > 2)
            {
                strInstruct = "00DC0C30050C03" + (strCardInfo[8].Trim());
                if (SendCommand(iHandle, strInstruct, out strRData))
                {
                    if (strRData != "9000")
                    {
                        WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "写出生地代码  ：" + strRData);
                        return -36;
                    }
                }
                else
                {
                    return -37;
                }

            }
            #endregion
            #region 外部认证2
            strInstruct = "0084000008";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "取随机数：" + strRData);
                    return -38;
                }
            }
            else
            {
                return -39;
            }
            random = strRData.Substring(0, 16);
            st = T10ReaderHelp.dc_setcpu(iHandle, PsamNo);//PSAM
            if (st != 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "dc set PSAM error");
                T10ReaderHelp.dc_exit(iHandle);
                return -101;
            }
            //st = T10ReaderHelp.dc_cpureset(iHandle, ref rlen, snr);
            //if (st != 0)
            //{
            //    WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "PSAM卡复位失败");
            //    T10ReaderHelp.dc_exit(iHandle);
            //    return -2;
            //}
            //else
            //{
            //    WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "PSAM卡复位成功:" + byteToChar(rlen, snr));
            //}
            strInstruct = "00A4040006D15600000590";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                //if (strRData.Substring(0, 2) != "61")
                //{
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "选择PSAM文件：" + strRData);
                //    return -41;
                //}
            }
            else
            {
                return -42;
            }
            strInstruct = "BFDE481318" + cardDiv + cityDiv + random;
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "分散密钥：" + strRData);
                    return -43;
                }
            }
            else
            {
                return -44;
            }
            strInstruct = "80FA000008" + random;
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(0, 2) != "61")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "加密CPU卡随机数：" + strRData);
                    // return -45;
                }
                else
                {
                    strInstruct = "00C0000008";
                    if (SendCommand(iHandle, strInstruct, out strRData))
                    {
                        if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                        {
                            WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "取CPU卡随机数：" + strRData);
                            return -47;
                        }
                    }
                    else
                    {
                        return -48;
                    }
                }
            }
            else
            {
                return -46;
            }


            macData = strRData.Substring(0, 16);
            st = T10ReaderHelp.dc_setcpu(iHandle, 0x0C);//CPU
            if (st != 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "dc set CPU error");
                T10ReaderHelp.dc_exit(iHandle);
                return -101;
            }
            strInstruct = "0082000D10" + macData + random;
            #endregion
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "外部认证2：" + strRData);
                    return -49;
                }
            }
            else
            {
                return -50;
            }
            strInstruct = "00A4000002EF0D";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(0, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "选择社保文件EF05：" + strRData);
                    return -51;
                }
            }
            else
            {
                return -52;
            }
            #region 写人员识别码
            strInstruct = "00DC01040EEA0C" + (GetHexFromChs(strCardInfo[9].Trim()));
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "写人员识别码  ：" + strRData);
                    return -53;
                }
            }
            else
            {
                return -54;
            }
            #endregion
            #region 写初次申领地
            if (strCardInfo[10].Trim() != "")
            {
                strInstruct = "00DC020405EB03" + strCardInfo[10].Trim();
                if (SendCommand(iHandle, strInstruct, out strRData))
                {
                    if (strRData != "9000")
                    {
                        WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "写初次申领地  ：" + strRData);
                        return -55;
                    }
                }
                else
                {
                    return -56;
                }
            }
            #endregion
            T10ReaderHelp.dc_beep(iHandle, 10);
            T10ReaderHelp.dc_exit(iHandle);
            return 0;
        }
        public static string ByteToString(byte[] InBytes)
        {
            string StringOut = "";
            foreach (byte InByte in InBytes)
            {
                StringOut = StringOut + String.Format("{0:X2}", InByte);
            }
            return StringOut;
        }
        private static byte[] strToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }
        /// <summary>
        /// 从汉字转换到16进制
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string GetHexFromChs(string s)
        {
            //if ((s.Length % 2) != 0)
            //{
            //    s += " ";//空格
            //             //throw new ArgumentException("s is not valid chinese string!");
            //}

            System.Text.Encoding chs = System.Text.Encoding.GetEncoding("gb2312");

            byte[] bytes = chs.GetBytes(s);

            string str = "";

            for (int i = 0; i < bytes.Length; i++)
            {
                str += string.Format("{0:X}", bytes[i]);
            }

            return str;
        }
        /// <summary>
        /// 从16进制转换成汉字
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static string GetChsFromHex(string hex)
        {
            if (hex == null)
                throw new ArgumentNullException("hex");
            //if (hex.Length % 2 != 0)
            //{
            //    hex += "20";//空格
            //                //throw new ArgumentException("hex is not a valid number!", "hex");
            //}
            // 需要将 hex 转换成 byte 数组。
            byte[] bytes = new byte[hex.Length / 2];

            for (int i = 0; i < bytes.Length; i++)
            {
                try
                {
                    // 每两个字符是一个 byte。
                    bytes[i] = byte.Parse(hex.Substring(i * 2, 2),
                        System.Globalization.NumberStyles.HexNumber);
                }
                catch
                {
                    // Rethrow an exception with custom message.
                    throw new ArgumentException("hex is not a valid hex number!", "hex");
                }
            }

            // 获得 GB2312，Chinese Simplified。
            System.Text.Encoding chs = System.Text.Encoding.GetEncoding("gb2312");


            return chs.GetString(bytes);
        }
        private static bool SendCommand(int icdev, string strInstruct, out string strRData)
        {
            strRData = "";
            byte rlen = 0;
            byte[] rbuff = new byte[512];
            int iRet = T10ReaderHelp.dc_cpuapdu(icdev, Convert.ToByte(strInstruct.Length / 2), strToToHexByte(strInstruct), ref rlen, rbuff);
            if (iRet == 0)
            {
                strRData = byteToChar(rlen, rbuff).ToUpper(); // ByteToString(rbuff);
                WriteWorkLog.WriteLogs("IC卡操作日志", strInstruct, strRData);
                return true;
            }
            else
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", strInstruct + "命令发送失败");
                return false;
            }
        }

        public static int GetPublicKey(out string strPubKey)
        {
            strPubKey = "";
            string strInstruct = "";
            int iHandle;
            int st;
            byte[] snr = new byte[128];
            byte rlen = 0;
            byte[] rbuff = new byte[128];
            iHandle = T10ReaderHelp.dc_init(100, 115200);
            if (iHandle < 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "设备未打开");
                return -1;
            }
            st = T10ReaderHelp.dc_setcpu(iHandle, 0x0c);
            if (st != 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "dc set cpu error");
                T10ReaderHelp.dc_exit(iHandle);
                return -101;
            }


            st = T10ReaderHelp.dc_cpureset(iHandle, ref rlen, snr);
            if (st != 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "IC复位失败");
                T10ReaderHelp.dc_exit(iHandle);
                return -2;
            }
            else
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "复位成功:" + byteToChar(rlen, snr));
            }
            string strRData = "";


            strInstruct = "00A404000C504B492EC9E7BBE1B1A3D5CF";

            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                //if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                //{
                //    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "选择社保文件：" + strRData);
                //    return -3;
                //}
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", strInstruct + "：" + strRData);
                return -4;
            }

            //00B209042A

            strInstruct = "00A4000002DF01";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "选择社保文件EF05：" + strRData);
                    return -5;
                }
            }
            else
            {
                return -6;
            }
            strInstruct = "0084000008";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "取随机数：" + strRData);
                    return -8;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -9;
            }

            string strInput = strRData.Substring(0, strRData.Length - 4) + "0000000000000000";
            string strKey = "3DE741F445DD357BEA4E6C3FE6437036";
            string strVector = "00000000000000000000000000000000";

            byte[] ucpKey;
            ucpKey = strToToHexByte(strKey);
            int iKeyLen = strKey.Trim().Length / 2;

            byte[] ucpInData;
            ucpInData = strToToHexByte(strInput.Trim());
            int iInDataLen = strInput.Trim().Length / 2;

            byte[] ucpIcv;
            ucpIcv =  strToToHexByte(strVector.Trim());
            int iIcvLen = strVector.Trim().Length / 2;
            byte[] ucpOutData = Enumerable.Repeat((byte)0x00, 128).ToArray(); ;
            int ipOutDataLen = 0;
            int iRet = Sm4Crypt(0, 0, 0, ucpKey, iKeyLen, ucpInData, iInDataLen, ucpIcv, iIcvLen, ucpOutData, ref ipOutDataLen);
            string strOutPut =  ByteToString(ucpOutData).Substring(0, ipOutDataLen * 2);


            #region 校验pin
            strInstruct = "0020018110" + strOutPut;
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "校验pin：" + strRData);
                    return -10;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -11;
            }
            #endregion
            strInstruct = "8040000008C0020012C2020013";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "校验pin：" + strRData);
                    return -12;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -13;
            }
            strInstruct = "80C9820008C0020012C2020013";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "校验pin：" + strRData);
                    return -14;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -15;
            }
            strInstruct = "80C9828042";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "校验pin：" + strRData);
                    return -16;
                }
                else
                {
                    strPubKey = strRData.Substring(4, 128);
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -17;
            }
            T10ReaderHelp.dc_beep(iHandle, 10);
            T10ReaderHelp.dc_exit(iHandle);
            return 0;
        }

        public static int WriteCACert(CARegisterInfo registerInfo)
        {

            string strInstruct = "";
            int iHandle;
            int st;
            byte[] snr = new byte[128];
            byte rlen = 0;
            byte[] rbuff = new byte[128];
            iHandle = T10ReaderHelp.dc_init(100, 115200);
            if (iHandle < 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "设备未打开");
                return -1;
            }
            st = T10ReaderHelp.dc_setcpu(iHandle, 0x0c);
            if (st != 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "dc set cpu error");
                T10ReaderHelp.dc_exit(iHandle);
                return -101;
            }


            st = T10ReaderHelp.dc_cpureset(iHandle, ref rlen, snr);
            if (st != 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "IC复位失败");
                T10ReaderHelp.dc_exit(iHandle);
                return -2;
            }
            else
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "复位成功:" + byteToChar(rlen, snr));
            }
            string strRData = "";

            #region 校验pin
            strInstruct = "00A404000C504B492EC9E7BBE1B1A3D5CF";

            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                //if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                //{
                //    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "选择社保文件：" + strRData);
                //    return -3;
                //}
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", strInstruct + "：" + strRData);
                return -4;
            }

            strInstruct = "00A4000002DF01";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "选择社保文件EF05：" + strRData);
                    return -5;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -6;
            }
            strInstruct = "0084000008";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "取随机数：" + strRData);
                    return -7;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -8;
            }

            string strInput = strRData.Substring(0, strRData.Length - 4) + "0000000000000000";
            string strKey = "3DE741F445DD357BEA4E6C3FE6437036";// 用户PIN ：123456 sm3基本算法（hash值）前16
            string strVector = "00000000000000000000000000000000";

            byte[] ucpKey;
            ucpKey =  strToToHexByte(strKey);
            int iKeyLen = strKey.Trim().Length / 2;

            byte[] ucpInData;
            ucpInData =  strToToHexByte(strInput.Trim());
            int iInDataLen = strInput.Trim().Length / 2;

            byte[] ucpIcv;
            ucpIcv = strToToHexByte(strVector.Trim());
            int iIcvLen = strVector.Trim().Length / 2;
            byte[] ucpOutData = Enumerable.Repeat((byte)0x00, 128).ToArray(); ;
            int ipOutDataLen = 0;
            int iRet = Sm4Crypt(0, 0, 0, ucpKey, iKeyLen, ucpInData, iInDataLen, ucpIcv, iIcvLen, ucpOutData, ref ipOutDataLen);
            string strOutPut = ByteToString(ucpOutData).Substring(0, ipOutDataLen * 2);



            strInstruct = "0020018110" + strOutPut;
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "校验pin：" + strRData);
                    return -9;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -10;
            }
            #endregion
            string jmmy = registerInfo.jmmy;
            string writejmmyX = jmmy.Substring(480, 64);
            string writejmmyY = jmmy.Substring(608, 64);
            string writejmmyHash = jmmy.Substring(672, 64);
            string writejmmyCipher = jmmy.Substring(744, 32);
            strInstruct = "804E000178C2020013C1820070" + writejmmyX+ writejmmyY+ writejmmyHash+ writejmmyCipher;
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "804E：" + strRData);
                    return -11;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -12;
            }
            string cardKey = strRData.Substring(0, strRData.Length - 4);

            strInstruct = "804A021012C110" + cardKey;
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "804a：" + strRData);
                    return -13;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -14;
            }

            string publicKey1 = jmmy.Substring(224, 64);// "F731699930C417D4DD17C315291BE2FB2326180A4313BB5451AF5772866B8FBF";
            string publickey2 = jmmy.Substring(352, 64); //"C112D74949D501852CEA761C36BDCF02C0E41096FEEDF6CB795AC906E1F34971";
            strInstruct = "80C2400046C0020014CA40" + publicKey1 + publickey2;
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "80C2，p1p2：" + strRData);
                    return -15;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -16;
            }
            string privateKey = jmmy.Substring(88,64);//"0C710B32A261821C73783E49D6893A4751366E387DEBB69ED9C349ED8ACF7038";
            strInstruct = "80C2C00126C2020015CB20" + privateKey;
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "80C2：" + strRData);
                    return -17;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -18;
            }

            strInstruct = "00A40000020018";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "00A4：" + strRData);
                    return -19;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -20;
            }

            string strqmzs = registerInfo.qmzs;
            string qmzs = strqmzs.PadRight(2040, '0');
            strInstruct = "00D60000FA" + qmzs.Substring(0, 500);
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "00D6：" + strRData);
                    return -21;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -22;
            }
            strInstruct = "00D600FAFA" + qmzs.Substring(500, 500);
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "00D6：" + strRData);
                    return -23;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -24;
            }
            strInstruct = "00D601F4FA" + qmzs.Substring(1000, 500);
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "00D6：" + strRData);
                    return -25;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -26;
            }
            strInstruct = "00D602EEFA" + qmzs.Substring(1500, 500);
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "00D6：" + strRData);
                    return -27;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -28;
            }
            strInstruct = "00D603E814" + qmzs.Substring(2000);
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "804E：" + strRData);
                    return -29;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -30;
            }
            strInstruct = "00A40000020019";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "A4：" + strRData);
                    return -31;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -32;
            }
            string strJMZS = registerInfo.jmzs;
            string jmzs = strJMZS.PadRight(2040, '0');
            strInstruct = "00D60000FA" + qmzs.Substring(0, 500);
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "00D6：" + strRData);
                    return -33;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -34;
            }
            strInstruct = "00D600FAFA" + qmzs.Substring(500, 500);
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "00D6：" + strRData);
                    return -35;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -11;
            }
            strInstruct = "00D601F4FA" + qmzs.Substring(1000, 500);
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "804E：" + strRData);
                    return -36;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -37;
            }
            strInstruct = "00D602EEFA" + qmzs.Substring(1500, 500);
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "00D6：" + strRData);
                    return -38;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -39;
            }
            strInstruct = "00D603E814" + qmzs.Substring(2000);
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "00D6：" + strRData);
                    return -40;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -41;
            }

            T10ReaderHelp.dc_beep(iHandle, 10);
            T10ReaderHelp.dc_exit(iHandle);
            return 0;
        }

        public static int Write3thCard(List<string> strCardInfo, out string cardIdCode)
        {
            string strInstruct = "";
            cardIdCode = "";
            int iHandle;
            int st;
            byte[] snr = new byte[128];
            byte rlen = 0;
            byte[] rbuff = new byte[128];
            string strRData = "";
            iHandle = T10ReaderHelp.dc_init(100, 115200);
            if (iHandle < 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "设备未打开");
                return -1;
            }
            st = T10ReaderHelp.dc_setcpu(iHandle, 0x0c);
            if (st != 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "dc set cpu error");
                T10ReaderHelp.dc_exit(iHandle);
                return -101;
            }
            st = T10ReaderHelp.dc_cpureset(iHandle, ref rlen, snr);
            if (st != 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "IC复位失败");
                T10ReaderHelp.dc_exit(iHandle);
                return -2;
            }
            else
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "复位成功:" + byteToChar(rlen, snr));
            }
            string strReset = byteToChar(rlen, snr);
            string cardDiv = strReset.Substring(strReset.Length - 16);
            WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "cardDiv:" + cardDiv);
            strInstruct = "00A404000F7378312E73682EC9E7BBE1B1A3D5CF";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                //if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                //{
                //    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "选择社保文件：" + strRData);
                //    return -3;
                //}
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -4;
            }
            strInstruct = "00A4000002EF05";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(0, 4) != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "选择社保文件EF05：" + strRData);
                    return -5;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -6;
            }
            strInstruct = "00B2010412";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "读取卡识别码：" + strRData);
                    return -6;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -7;
            }
            cardIdCode = strRData.Substring(4, strRData.Length - 8).ToUpper();

            string cityDiv = GetHexFromChs(strRData.Substring(4, 6)) + "7378";
            WriteWorkLog.WriteLogs("IC卡操作日志", "信息", "cityDiv：" + cityDiv);
            #region 外部认证
            strInstruct = "0084000008";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "取随机数：" + strRData);
                    return -8;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -9;
            }
            string random = strRData.Substring(0, 16);
            WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "random:" + random);
            st = T10ReaderHelp.dc_setcpu(iHandle, 0x0D);//PSAM
            if (st != 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "dc set PSAM error");
                T10ReaderHelp.dc_exit(iHandle);
                return -101;
            }
            st = T10ReaderHelp.dc_cpureset(iHandle, ref rlen, snr);
            if (st != 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "PSAM卡复位失败");
                T10ReaderHelp.dc_exit(iHandle);
                return -2;
            }
            else
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "PSAM卡复位成功:" + byteToChar(rlen, snr));
            }

            strInstruct = "00A404000F7378332E73682EC9E7BBE1B1A3D5CF";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                //if (strRData.Substring(0, 2) != "61")
                //{
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "选择PSAM文件：" + strRData);
                //    return -11;
                //}
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -12;
            }
            strInstruct = "00A4000002DF01";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                //if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                //{
                //    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "分散密钥：" + strRData);
                //    return -13;
                //}
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -14;
            }
            if (strRData.Substring(strRData.Length - 4, 4) != "610A")
            {
                //00C000000A
            }
            //BFDE670B28   BFDE481218
            strInstruct = "BFDE481220" + cardDiv + cityDiv + random + "0000000000000000";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "分散密钥：" + strRData);
                    return -13;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -14;
            }
            strInstruct = "80FA000010" + random + "0000000000000000";
            string encodeKey = "";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "info", "80FA000008：" + strRData);
                    //strInstruct = "00C0000008";
                    //if (SendCommand(iHandle, strInstruct, out strRData))
                    //{
                    //    if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                    //    {
                    //        WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "取CPU卡随机数：" + strRData);
                    //        return -17;
                    //    }
                    //}
                    //else
                    //{
                    //    return -18;
                    //}
                }
                else
                {
                    encodeKey = strRData.Substring(0, strRData.Length - 4);
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -16;
            }

            string str1 = encodeKey.Substring(0, 16);
            string str2 = encodeKey.Substring(16);
            string macData = (Convert.ToInt64(str1, 16) ^ Convert.ToInt64(str2, 16)).ToString("X2");
            macData = macData.PadLeft(16, '0');
            st = T10ReaderHelp.dc_setcpu(iHandle, 0x0C);//CPU
            if (st != 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "dc set CPU error");
                T10ReaderHelp.dc_exit(iHandle);
                return -101;
            }
            strInstruct = "0082000411" + macData + random + "01";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "外部认证：" + strRData);
                    return -19;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -20;
            }
            WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "外部认证：" + strRData);
            #endregion
            #region 校验pin
            //strInstruct = "0020000003123456" ;
            //if (SendCommand(iHandle, strInstruct, out strRData))
            //{
            //    if (strRData != "9000")
            //    {
            //        WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "写发卡日期：" + strRData);
            //        return -121;
            //    }
            //}
            //else
            //{
            //    return -122;
            //}
            #endregion
            #region 写发卡日期
            strInstruct = "00DC0528060504" + strCardInfo[0];
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "写发卡日期：" + strRData);
                    return -21;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -22;
            }
            #endregion
            #region 写卡号
            strInstruct = "00DC07280B0709" + GetHexFromChs(strCardInfo[1].Trim());
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "写卡号：" + strRData);
                    return -23;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -24;
            }
            #endregion
            #region 写社会保障号码
            strInstruct = "00DC0830140812" + GetHexFromChs(strCardInfo[2].Trim());
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "写社会保障号码 ：" + strRData);
                    return -24;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -25;
            }
            #endregion
            #region 写姓名
            strInstruct = "00DC093020091E" + GetHexFromChs(strCardInfo[3].Trim()).PadRight(60, '0');
            WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "strInstruct ：" + strInstruct);
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "写姓名 ：" + strRData);
                    return -26;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -27;
            }
            #endregion
            #region 写姓名扩展
            if (strCardInfo[4] != "")
            {
                strInstruct = "00DC4E30164E14" + GetHexFromChs(strCardInfo[4].Trim()).PadRight(40, 'F');
                if (SendCommand(iHandle, strInstruct, out strRData))
                {
                    if (strRData != "9000")
                    {
                        T10ReaderHelp.dc_exit(iHandle);
                        WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "写姓名扩展 ：" + strRData);
                        return -28;
                    }
                }
                else
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    return -29;
                }
            }
            #endregion
            # region 写性别
            strInstruct = "00DC0A30030A01" + (GetHexFromChs(strCardInfo[5].Trim()));
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "写性别  ：" + strRData);
                    return -30;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -31;
            }
            #endregion
            #region 写民族
            strInstruct = "00DC0B30030B01" + strCardInfo[6].Trim();
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "写民族  ：" + strRData);
                    return -32;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -33;
            }
            #endregion
            #region 写出生日期
            strInstruct = "00DC0D30060D04" + (strCardInfo[7].Trim());
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "写出生日期  ：" + strRData);
                    return -34;
                }
            }
            else
            {
                return -35;
            }
            #endregion
            #region 写出生地代码
            if (strCardInfo[8].Trim().Length > 2)
            {
                strInstruct = "00DC0C30050C03" + (strCardInfo[8].Trim());
                if (SendCommand(iHandle, strInstruct, out strRData))
                {
                    if (strRData != "9000")
                    {
                        T10ReaderHelp.dc_exit(iHandle);
                        WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "写出生地代码  ：" + strRData);
                        return -36;
                    }
                }
                else
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    return -37;
                }

            }
            #endregion
            #region 外部认证2
            strInstruct = "0084000008";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "取随机数：" + strRData);
                    return -8;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -9;
            }
            random = strRData.Substring(0, 16);
            WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "random:" + random);
            st = T10ReaderHelp.dc_setcpu(iHandle, 0x0D);//PSAM
            if (st != 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "dc set PSAM error");
                T10ReaderHelp.dc_exit(iHandle);
                return -101;
            }
            st = T10ReaderHelp.dc_cpureset(iHandle, ref rlen, snr);
            if (st != 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "PSAM卡复位失败");
                T10ReaderHelp.dc_exit(iHandle);
                return -2;
            }
            else
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "PSAM卡复位成功:" + byteToChar(rlen, snr));
            }

            strInstruct = "00A404000F7378332E73682EC9E7BBE1B1A3D5CF";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                //if (strRData.Substring(0, 2) != "61")
                //{
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "选择PSAM文件：" + strRData);
                //    return -11;
                //}
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -12;
            }
            strInstruct = "00A4000002DF01";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                //if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                //{
                //    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "分散密钥：" + strRData);
                //    return -13;
                //}
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -14;
            }
            if (strRData.Substring(strRData.Length - 4, 4) != "610A")
            {
                //00C000000A
            }
            //BFDE670B28   BFDE481218
            strInstruct = "BFDE481320" + cardDiv + cityDiv + random + "0000000000000000";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "分散密钥：" + strRData);
                    return -13;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -14;
            }
            strInstruct = "80FA000010" + random + "0000000000000000";
            encodeKey = "";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "info", "80FA000008：" + strRData);
                    //strInstruct = "00C0000008";
                    //if (SendCommand(iHandle, strInstruct, out strRData))
                    //{
                    //    if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                    //    {
                    //        WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "取CPU卡随机数：" + strRData);
                    //        return -17;
                    //    }
                    //}
                    //else
                    //{
                    //    return -18;
                    //}
                }
                else
                {
                    encodeKey = strRData.Substring(0, strRData.Length - 4);
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -16;
            }

            str1 = encodeKey.Substring(0, 16);
            str2 = encodeKey.Substring(16);
            macData = (Convert.ToInt64(str1, 16) ^ Convert.ToInt64(str2, 16)).ToString("X2");

            st = T10ReaderHelp.dc_setcpu(iHandle, 0x0C);//CPU
            if (st != 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "dc set CPU error");
                T10ReaderHelp.dc_exit(iHandle);
                return -101;
            }
            strInstruct = "0082000D11" + macData + random + "01";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "外部认证：" + strRData);
                    return -19;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -20;
            }
            WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "外部认证：" + strRData);
            #endregion


            strInstruct = "00A4000002EF0D";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(0, 4) != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "选择社保文件EF05：" + strRData);
                    return -51;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -52;
            }
            #region 写人员识别码
            strInstruct = "00DC01040EEA0C" + (GetHexFromChs(strCardInfo[9].Trim()));
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "写人员识别码  ：" + strRData);
                    return -53;
                }
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -54;
            }
            #endregion
            #region 写初次申领地
            if (strCardInfo[10].Trim() != "")
            {
                strInstruct = "00DC020405EB03" + strCardInfo[10].Trim();
                if (SendCommand(iHandle, strInstruct, out strRData))
                {
                    if (strRData != "9000")
                    {
                        T10ReaderHelp.dc_exit(iHandle);
                        WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "写初次申领地  ：" + strRData);
                        return -55;
                    }
                }
                else
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    return -56;
                }
            }
            #endregion
            T10ReaderHelp.dc_beep(iHandle, 10);
            T10ReaderHelp.dc_exit(iHandle);
            return 0;
        }
        private static int UpdateMainkeySub(int iHandle, string oldkey, string newkey)
        {
            string strRData = "";
            string strInstruct = "0084000008";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "取随机数：" + strRData);
                    return -6;
                }
            }
            else
            {
                return -7;
            }

            string strVector = "00000000000000000000000000000000";
            string strRandom = strRData.Substring(0, 16).ToUpper() + "0000000000000000";

            string strOutPut = RunSm4Crypt(0, strRandom, oldkey, strVector);
            strInstruct = "0082000010" + strOutPut;
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "校验pin：" + strRData);
                    return -8;
                }
            }
            else
            {
                return -9;
            }
            strInstruct = "0084000008";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "取随机数：" + strRData);
                    return -10;
                }
            }
            else
            {
                return -11;
            }
            strRandom = strRData.Substring(0, 16) + "0000000000000000";
            string dek = newkey;
            string dekold = oldkey;
            string data = "13000000" + dek + "800000000000000000000000";
            string strInput = data;
            strVector = "00000000000000000000000000000000";
            string datainfo = RunSm4Crypt(0, strInput, dekold, strVector);
            string rcmd = "84D4010024" + datainfo + "8000000000000000000000";
            string seslk1 = rcmd.Substring(0, 32);
            string seslk = RunSm4Crypt(1, seslk1, dekold, strRandom);
            seslk1 = rcmd.Substring(32, 32);
            seslk = RunSm4Crypt(1, seslk1, dekold, seslk);
            seslk1 = rcmd.Substring(64, 32);
            seslk = RunSm4Crypt(1, seslk1, dekold, seslk);
            seslk1 = seslk.Substring(0, 8);
            string seslk2 = seslk.Substring(8, 8);
            seslk2 = (Convert.ToInt64(seslk1, 16) ^ Convert.ToInt64(seslk2, 16)).ToString("X2");
            seslk1 = seslk.Substring(16, 8);
            seslk2 = (Convert.ToInt64(seslk1, 16) ^ Convert.ToInt64(seslk2, 16)).ToString("X2");
            seslk1 = seslk.Substring(24, 8);
            seslk2 = (Convert.ToInt64(seslk1, 16) ^ Convert.ToInt64(seslk2, 16)).ToString("X2");
            seslk = seslk2.Substring(0, 8);
            strInstruct = "84D4010024" + datainfo + seslk;
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", strInstruct, strRData);
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "84D4010024：" + strRData);
                    return -12;
                }
            }
            else
            {
                return -13;
            }

            return 0;
        }
        public static int UpdateManagePinSub(int iHandle, string newPin)
        {
            string oldPin = "12345678";
            string strInstruct = "";
            string strRData = "";
            #region 校验pin
            strInstruct = "00A404000C504B492EC9E7BBE1B1A3D5CF";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                //if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                //{
                //    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "选择社保文件：" + strRData);
                //    return -3;
                //}
            }
            else
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", strInstruct + "：" + strRData);
                return -304;
            }
            strInstruct = "00A4000002DF01";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                //if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                //{
                //    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "选择社保文件：" + strRData);
                //    return -3;
                //}
            }
            else
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", strInstruct + "：" + strRData);
                return -305;
            }

            strInstruct = "0084000008";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "取随机数：" + strRData);
                    return -306;
                }
            }
            else
            {
                return -307;
            }
            string strVector = "00000000000000000000000000000000";
            string strKey = RunSM3(oldPin);
            string strRandom = strRData.Substring(0, 16).ToUpper() + "0000000000000000";

            string strOutPut = RunSm4Crypt(0, strRandom, strKey, strVector);


            strInstruct = "0020010010" + strOutPut;
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "校验pin：" + strRData);
                    return -308;
                }
            }
            else
            {
                return -309;
            }
            #endregion
            #region 修改PIN
            strInstruct = "0084000010";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "取随机数：" + strRData);
                    return -310;
                }
            }
            else
            {
                return -311;
            }
           // strKey = RunSM3(oldPin);
            string rand = strRData.Substring(0, 32);
            string data = "04" + newPin + "8000000000000000000000";
            string datainfo = "";
            string rcmd = "";
            string seslk1 = "";
            string seslk2 = "";
            string seslk = "";
            datainfo = RunSm4Crypt(0, data, strKey, strVector);// @sm4fpjsecb(% data, strKey, 00)
            rcmd = "845E010014" + datainfo + "800000000000000000000000";
            seslk1 = rcmd.Substring(0, 32);// @strmidh(% rcmd, 0, 16)
            seslk = RunSm4Crypt(1, seslk1, strKey, rand); //@sm4fpjscbc(% seslk1, F92E0AE6D3DF69D6435E8F9F3C42468E,% rand, 00)
            seslk1 = rcmd.Substring(32, 32);// @strmidh(% rcmd, 16, 16)
            seslk = RunSm4Crypt(1, seslk1, strKey, seslk);// @sm4fpjscbc(% seslk1, F92E0AE6D3DF69D6435E8F9F3C42468E,% seslk, 00)
            seslk1 = seslk.Substring(0, 8);// @strmidh(% seslk, 0, 4)
            seslk2 = seslk.Substring(8, 8); //@strmidh(% seslk, 4, 4)
            seslk2 = (Convert.ToInt64(seslk1, 16) ^ Convert.ToInt64(seslk2, 16)).ToString("X2");// @xor(% seslk1,% seslk2)
            seslk1 = seslk.Substring(16, 8);//@strmidh(% seslk, 8, 4)
            seslk2 = (Convert.ToInt64(seslk1, 16) ^ Convert.ToInt64(seslk2, 16)).ToString("X2");// @xor(% seslk1,% seslk2)
            seslk1 = seslk.Substring(24, 8);//@strmidh(% seslk, 12, 4)
            seslk2 = (Convert.ToInt64(seslk1, 16) ^ Convert.ToInt64(seslk2, 16)).ToString("X2"); //@xor(% seslk1,% seslk2)
            seslk = seslk2.Substring(0, 8);// @strmidh(% seslk2, 0, 4)
            rcmd = "845E010014" + datainfo;//<--! %rcmd=845E010014 + %datainfo
            strInstruct = rcmd + seslk;//sendtxt %rcmd %seslk
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "取随机数：" + strRData);
                    return -312;
                }
            }
            else
            {
                return -313;
            }
            #endregion
            return 0;
        }
        public static int UpdateKey(string oldMainkey, string newMainkey, string newManagePin, out string strErr)
        {
            string strInstruct = "";
            strErr = "";
            int iHandle;
            int st;
            byte[] snr = new byte[128];
            byte rlen = 0;
            byte[] rbuff = new byte[128];
            iHandle = T10ReaderHelp.dc_init(100, 115200);
            if (iHandle < 0)
            {
                strErr = "设备未打开";
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "设备未打开");
                return -1;
            }
            st = T10ReaderHelp.dc_setcpu(iHandle, 0x0c);
            if (st != 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "dc set cpu error");
                T10ReaderHelp.dc_exit(iHandle);
                return -101;
            }
            st = T10ReaderHelp.dc_cpureset(iHandle, ref rlen, snr);
            if (st != 0)
            {
                strErr = "CPU卡复位失败";
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "IC复位失败");
                T10ReaderHelp.dc_exit(iHandle);
                return -2;
            }
            else
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "复位成功:" + byteToChar(rlen, snr));
            }
            string strRData = "";
            strInstruct = "00A404000C504B492EC9E7BBE1B1A3D5CF";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                //if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                //{
                //    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "选择社保文件：" + strRData);
                //    return -3;
                //}
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", strInstruct + "：" + strRData);
                return -4;
            }
            int iRet = UpdateMainkeySub(iHandle, oldMainkey, newMainkey);
            if (iRet != 0)
            {
                strErr = "修改DDF主控失败";
                T10ReaderHelp.dc_exit(iHandle);
                return iRet;
            }
            strInstruct = "00A404000C53532E434552542E41444631";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                //if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                //{
                //    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "选择社保文件：" + strRData);
                //    return -3;
                //}
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", strInstruct + "：" + strRData);
                return -5;
            }
            iRet = UpdateMainkeySub(iHandle, oldMainkey, newMainkey);
            if (iRet != 0)
            {
                T10ReaderHelp.dc_exit(iHandle);
                strErr = "修改DF01主控失败";
                return iRet;
            }
            iRet = UpdateManagePinSub(iHandle, newManagePin);
            if (iRet != 0)
            {
                T10ReaderHelp.dc_exit(iHandle);
                strErr = "修改管理员pin失败";
                return iRet;
            }
            T10ReaderHelp.dc_beep(iHandle, 10);
            T10ReaderHelp.dc_exit(iHandle);
            return 0;
        }
        private static string RunSm4Crypt(int mode, string strInput, string strKey, string strVector)
        {
            byte[] ucpKey;
            ucpKey =  strToToHexByte(strKey);
            int iKeyLen = strKey.Trim().Length / 2;

            byte[] ucpInData;
            ucpInData =  strToToHexByte(strInput.Trim());
            int iInDataLen = strInput.Trim().Length / 2;

            byte[] ucpIcv;
            ucpIcv =  strToToHexByte(strVector.Trim());
            int iIcvLen = strVector.Trim().Length / 2;
            byte[] ucpOutData = Enumerable.Repeat((byte)0x00, 128).ToArray(); ;
            int ipOutDataLen = 0;
            int iRet = Sm4Crypt(0, 0, mode, ucpKey, iKeyLen, ucpInData, iInDataLen, ucpIcv, iIcvLen, ucpOutData, ref ipOutDataLen);
            string datainfo = ByteToString(ucpOutData).Substring(0, ipOutDataLen * 2);

            WriteWorkLog.WriteLogs("IC卡操作日志", "strInput", datainfo);
            return datainfo;
        }
        public static int ReadBankNo(out string bankNo, out string cardChipInfo, out string strErr)
        {
            bankNo = "";
            strErr = "";
            cardChipInfo = "";
            string strRData = "";
            string version = "";
            string strInstruct = "";
            string strReset = "";
            int iHandle;
            int st;
            byte[] snr = new byte[128];
            byte rlen = 0;
            byte[] rbuff = new byte[128];
            iHandle = T10ReaderHelp.dc_init(100, 115200);
            if (iHandle < 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "设备未打开");
                strErr = "设备未打开";
                return -1;
            }
            st = T10ReaderHelp.dc_setcpu(iHandle, 0x0c);
            if (st != 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "dc set cpu error");
                T10ReaderHelp.dc_exit(iHandle);
                strErr = "设置卡类型失败";
                return -101;
            }
            st = T10ReaderHelp.dc_cpureset(iHandle, ref rlen, snr);
            if (st != 0)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "IC复位失败");
                T10ReaderHelp.dc_exit(iHandle);
                strErr = "IC复位失败";
                return -2;
            }
            else
            {
                strReset = byteToChar(rlen, snr);
                strReset = strReset.Substring(strReset.Length - 26, 26).ToUpper();
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "复位成功:" + byteToChar(rlen, snr));
            }
            strInstruct = "00A404000F7378312E73682EC9E7BBE1B1A3D5CF";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                //if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                //{
                //    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "选择社保文件：" + strRData);
                //    return -3;
                //}
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", strInstruct + "：" + strRData);
                return -4;
            }
            strInstruct = "00A4000002EF05";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(0, 4) != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "选择社保文件EF05：" + strRData);
                    return -5;
                }
            }
            else
            {
                return -6;
            }
            strInstruct = "00B2030406";// "00B2010412";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "版本：" + strRData);
                    return -7;
                }
                string strReadData = strRData.Substring(4, strRData.Length - 8);
                version = GetChsFromHex(strReadData);
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -8;
            }
            strInstruct = "00B2010412";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "读取卡识别码：" + strRData);
                    return -6;
                }
            }
            else
            {
                return -7;
            }
            string cardIdCode = strRData.Substring(4, strRData.Length - 8).ToUpper();

            strInstruct = "00A4040008A000000333010101";//  
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "00A4041：" + strRData);
                    return -30;
                }
                string strReadData = strRData.Substring(4, strRData.Length - 8);
            }
            else
            {
                T10ReaderHelp.dc_exit(iHandle);
                return -31;
            }
            strInstruct = "00B2011400";//  
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData == "")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "00B20114：" + strRData);
                    return -28;
                }
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "00B20114：" + strRData);
                    return -28;
                }
                strRData = strRData.ToUpper();
                int iTag = strRData.IndexOf("5A");
                string strReadData = strRData.Substring(iTag + 4, 19);
                bankNo = strReadData;
            }
            else
            {
                strInstruct = "00B2011C00";//  
                if (SendCommand(iHandle, strInstruct, out strRData))
                {
                    if (strRData == "")
                    {
                        T10ReaderHelp.dc_exit(iHandle);
                        WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "00B20114：" + strRData);
                        return -28;
                    }
                    if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                    {
                        T10ReaderHelp.dc_exit(iHandle);
                        WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "00B20114：" + strRData);
                        return -28;
                    }
                    int iTag = strRData.IndexOf("5A");
                    string strReadData = strRData.Substring(iTag + 4, 19);
                    bankNo = strReadData;
                }
                else
                {
                    T10ReaderHelp.dc_exit(iHandle);
                    return -34;
                }

            }

            strInstruct = "00A4040008A000000632010105";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {

            }
            else
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", strInstruct + "：" + strRData);
                return -4;
            }
            strInstruct = "00b0950A0A";//00b0950B0A
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(strRData.Length - 4, 4) != "9000")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "选择社保文件EF05：" + strRData);
                    return -5;
                }
            }
            else
            {
                return -6;
            }
            string cardFaceNo = strRData.Substring(1, 19);
            cardChipInfo = strReset + "|" + cardIdCode + "|" + cardFaceNo;
            T10ReaderHelp.dc_beep(iHandle, 10);
            T10ReaderHelp.dc_exit(iHandle);
            return 0;
        }

        private static string RunSM3(string oldPin)
        {
            if (oldPin.Length < 8)
                return "";
            byte[] ucpInData;
            ucpInData = strToToHexByte(oldPin);
            int iInDataLen = oldPin.Length / 2;

            byte[] ucpOutData = Enumerable.Repeat((byte)0x00, 128).ToArray(); ;
            int ipOutDataLen = 0;
            int iRet = Sm3Hash(ucpInData, iInDataLen, ucpOutData, ref ipOutDataLen);
            string strKey = ByteToString(ucpOutData).Substring(0, ipOutDataLen * 2).Substring(0, 32);
            return strKey;
        }
    }
}
