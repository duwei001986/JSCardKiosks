using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkLog;

namespace JSCardKiosks
{
    class CardWriteRead
    {
        public static int ReadCard(IntPtr iHandle, out List<string> strCardInfo)
        {
            strCardInfo = new List<string>();
            string strInstruct = "";
            if (iHandle == IntPtr.Zero)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "设备未打开");
                return -1;
            }

            byte[] InData = { 0x43, 0x49, 0x30, 0x30 };
            byte[] RxData = new byte[128];
            int RxDataLen = 0;
            int iRet = CRT_591_H001Help.RS232_ExeCommand(iHandle, 4, InData, ref RxDataLen, RxData);
            if (RxData[0] == 0x50)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "复位成功:" + ByteToString(RxData).Substring(10, RxDataLen * 2 - 10));
            }
            else
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "IC复位失败");
                return -2;
            }
            strInstruct = "00A404000F7378312E73682EC9E7BBE1B1A3D5CF";
            string strRData = "";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(0, 2) != "61")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "选择社保文件：" + strRData);
                    return -3;
                }
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
                strCardInfo.Add(GetChsFromHex(strReadData));
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
            return 0;
        }
        public static int WriteCard(IntPtr iHandle, List<string> strCardInfo,out string cardIdCode)
        {
            string strInstruct = "";
            cardIdCode = "";
            if (iHandle == IntPtr.Zero)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "设备未打开");
                return -1;
            }

            byte[] InData = { 0x43, 0x49, 0x30, 0x30 };
            byte[] RxData = new byte[128];
            int RxDataLen = 0;
            int iRet = CRT_591_H001Help.RS232_ExeCommand(iHandle, 4, InData, ref RxDataLen, RxData);
            if (RxData[0] == 0x50)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "复位成功:" + ByteToString(RxData).Substring(10, RxDataLen * 2 - 10));
            }
            else
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "IC复位失败");
                return -2;
            }
            string cardDiv = ByteToString(RxData).Substring(RxDataLen * 2 - 16, 16);
            strInstruct = "00A404000F7378312E73682EC9E7BBE1B1A3D5CF";
            string strRData = "";
            if (SendCommand(iHandle, strInstruct, out strRData))
            {
                //if (strRData.Substring(0, 2) != "61")
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
            cardIdCode = strRData.Substring(4, strRData.Length - 8);
            string cityDiv = GetHexFromChs(strRData.Substring(4, 6)) + "7378";
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
            byte[] InData2 = { 0x43, 0x49, 0x40, 0x30 };
            iRet = CRT_591_H001Help.RS232_ExeCommand(iHandle, 4, InData2, ref RxDataLen, RxData);
            if (RxData[0] == 0x50)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "PSAM卡复位成功:" + ByteToString(RxData).Substring(10, RxDataLen * 2 - 10));
            }
            else
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "PSAM卡复位失败");
                return -10;
            }
            strInstruct = "00A4040006D15600000590";
            if (SendPSAMCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(0, 2) != "61")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "选择PSAM文件：" + strRData);
                    return -11;
                }
            }
            else
            {
                return -12;
            }
            strInstruct = "BFDE481218" + cardDiv + cityDiv + random;
            if (SendPSAMCommand(iHandle, strInstruct, out strRData))
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
            if (SendPSAMCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(0, 2) != "61")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "加密CPU卡随机数：" + strRData);
                    return -15;
                }
            }
            else
            {
                return -16;
            }
            strInstruct = "00C0000008";
            if (SendPSAMCommand(iHandle, strInstruct, out strRData))
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
            String macData = strRData.Substring(0, 16);
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
            string redtata = strCardInfo[1];
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
                           
           strInstruct = "00DC093020091E" + GetHexFromChs(strCardInfo[3].Trim()).PadRight(60,'0');
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
            strInstruct = "00DC4E30164E14" + GetHexFromChs(strCardInfo[4].Trim()).PadRight(40, '0');
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
            iRet = CRT_591_H001Help.RS232_ExeCommand(iHandle, 4, InData2, ref RxDataLen, RxData);
            if (RxData[0] == 0x50)
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "PSAM卡复位成功:" + ByteToString(RxData).Substring(10, RxDataLen * 2 - 10));
            }
            else
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", "PSAM卡复位失败");
                return -40;
            }
            strInstruct = "00A4040006D15600000590";
            if (SendPSAMCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(0, 2) != "61")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "选择PSAM文件：" + strRData);
                    return -41;
                }
            }
            else
            {
                return -42;
            }
            strInstruct = "BFDE481318" + cardDiv + cityDiv + random;
            if (SendPSAMCommand(iHandle, strInstruct, out strRData))
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
            if (SendPSAMCommand(iHandle, strInstruct, out strRData))
            {
                if (strRData.Substring(0, 2) != "61")
                {
                    WriteWorkLog.WriteLogs("IC卡操作日志", "Err", "加密CPU卡随机数：" + strRData);
                    return -45;
                }
            }
            else
            {
                return -46;
            }
            strInstruct = "00C0000008";
            if (SendPSAMCommand(iHandle, strInstruct, out strRData))
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
            macData = strRData.Substring(0, 16);
            strInstruct = "0082000D10" + macData + random;
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
        private static bool SendCommand(IntPtr iHandle, string strInstruct, out string strRData)
        {
            byte[] InData = { 0, };
            byte[] RxData = new byte[128];
            strRData = "";
            int RxDataLen = 0;
            InData = strToToHexByte("434933" + strInstruct);
            int iRet = CRT_591_H001Help.RS232_ExeCommand(iHandle, InData.Length, InData, ref RxDataLen, RxData);
            if (iRet == 0 && RxData[0] == 0x50)
            {
                strRData = ByteToString(RxData).Substring(10, RxDataLen * 2 - 10);
                return true;
            }
            else
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", strInstruct + "命令发送失败");
                return false;
            }
        }
        private static bool SendPSAMCommand(IntPtr iHandle, string strInstruct, out string strRData)
        {
            byte[] InData = { 0, };
            byte[] RxData = new byte[128];
            strRData = "";
            int RxDataLen = 0;
            InData = strToToHexByte("434949" + strInstruct);
            int iRet = CRT_591_H001Help.RS232_ExeCommand(iHandle, InData.Length, InData, ref RxDataLen, RxData);
            if (iRet == 0 && RxData[0] == 0x50)
            {
                strRData = ByteToString(RxData).Substring(10, RxDataLen * 2 - 10);
                return true;
            }
            else
            {
                WriteWorkLog.WriteLogs("IC卡操作日志", "Info", strInstruct + "PSAM卡命令发送失败");
                return false;
            }
        }
    }
}
