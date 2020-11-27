
using CardWriteReadT10;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkLog;

namespace MyCard 
{
    public class CardWriteReadT10
    {

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
        public static int ReadBankNo(out string bankNo,out string strErr)
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
                bankNo = strReadData.Substring(16, 19);
            }
            else
            {
                return -34;
            }
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
            byte[] rbuff = new byte[128];
            int iRet = T10ReaderHelp.dc_cpuapdu(icdev, Convert.ToByte(strInstruct.Length / 2), strToToHexByte(strInstruct), ref rlen, rbuff);
            if (iRet == 0)
            {
                strRData = byteToChar(rlen, rbuff); // ByteToString(rbuff);
                return true;
            }
            else
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", strInstruct + "命令发送失败");
                return false;
            }
        }
        public static int ReadBankNo(out string bankNo, out string version, out string strErr)
        {
            bankNo = "";
            strErr = "";
            version = "";
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
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "读取卡识别码：" + strRData);
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
            T10ReaderHelp.dc_beep(iHandle, 10);
            T10ReaderHelp.dc_exit(iHandle);
            return 0;
        }
    }
}
