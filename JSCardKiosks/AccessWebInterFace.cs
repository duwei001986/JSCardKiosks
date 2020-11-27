using JSCardKiosks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WorkLog;

namespace JSCardKiosks
{
    class AccessWebInterFace
    {
        public const int CARD_BUSINESSTYPE_RESHOOT = 1;
        public const int CARD_BUSINESSTYPE_NEWADD = 2;
        public const int CARD_STATUS_3_52_1_NORMAL = 1;
        public const int CARD_STATUS_3_52_2_REPORTLOSS = 2;
        public const int CARD_STATUS_3_52_21_ORALLOSS = 21;
        public const int CARD_STATUS_3_52_3_LOGOUT = 3;
        public const int CARD_STATUS_3_52_9_NOCARD = 4;
        public const int CARD_STATUS_3_52_4_NOPERSON = 5;
        public static string strOperator = DealAppConfig.GetAppSettingsValue("操作员");
        public static string strWebAddr = DealAppConfig.GetAppSettingsValue("下载数据地址");
        public static string strUserName = DealAppConfig.GetAppSettingsValue("下载数据用户名");
        public static string strPassword = DealAppConfig.GetAppSettingsValue("下载数据密码");
        public static string strIP = DealAppConfig.GetAppSettingsValue("下载数据IP地址");
        public static string strMachineID = DealAppConfig.GetAppSettingsValue("下载数据设备编号");
        public static string strValidCode = DealAppConfig.GetAppSettingsValue("下载数据验证码号");
        public static string strBankCode = DealAppConfig.GetAppSettingsValue("银行代码");
        public static string strUnitCode = DealAppConfig.GetAppSettingsValue("制卡单位编码");
        public static string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Application.StartupPath + "\\jiangsu_database.mdb";

        public static string GetBankName(string bankCode)
        {
            Hashtable ht = new Hashtable(); //创建一个Hashtable实例
            ht.Add("95588", "工商银行");
            ht.Add("95599", "农业银行");
            ht.Add("95566", "中国银行");
            ht.Add("95533", "建设银行");
            ht.Add("95559", "交通银行");
            ht.Add("96098", "江苏银行");
            ht.Add("95555", "招商银行");
            ht.Add("96008", "农信社");
            return (string)ht[bankCode];
        }
        public static string GetProgress(string strNum)
        {
            Hashtable ht = new Hashtable();
            ht.Add("0", "未提交制卡申请");
            ht.Add("1", "已提交制卡申请");
            ht.Add("2", "已生成网点批次");
            ht.Add("3", "已生成市级批次");
            ht.Add("4", "已生成省级批次");
            ht.Add("5", "银行开卡中");
            ht.Add("6", "银行开卡成功");
            ht.Add("7", "卡商制卡中");
            ht.Add("8", "已完成卡商制卡，物流地市中");
            ht.Add("9", "已完成市级入库");
            ht.Add("10", "市级已出库，物流网点中");
            ht.Add("11", "服务网点已领卡");
            ht.Add("12", "已完成个人领卡");
            return (string)ht[strNum];
        }
        public static string IDReadWebInterface(string strIDcode, string strName, out string strBatch)
        {
            string keyInfo = "";
            string operType = "";
            string conInfo = "";
            string busType = "";
            string strRet = "";
            strBatch = "";
            string strLogInfo = "";
            string cardProgress = "";
            string strPersonCode = "";
            string strOldBankCode = "";
            string strPhoto = "";
            string strCityCode = strUnitCode.Substring(0, 6);
            int intRecordNo = 0;
            string[] errInfo = new String[] { "1", "2", "3" };
            string[] sArray = new String[] { "1", "2", "3" };

            #region 3-65  制卡批次查询
            keyInfo = strUserName + "|" + strPassword + "|" + "754|CHN|01|" + strIDcode + "|" + strName + "||";
            conInfo = "BAZ838||96008|&";
            operType = "3";
            busType = "65";
            try
            {
                strRet = PublicFuction.GetDataByWebService(strWebAddr + "?WSDL", "transBusinessForCard", new string[] { keyInfo, conInfo, operType, busType }).ToString();
                if (strRet.Substring(0, 1) == "0")
                {
                    strRet = (strRet.Remove(0, 4)).Remove(strRet.Length - 5);
                    strBatch = strRet.Split('|')[0];
                    WriteWorkLog.WriteLogs("日志", "信息", "接口1:" + strRet);
                    return "0$0";
                }
            }
            catch (Exception ex)
            {
                WriteWorkLog.WriteLogs("日志", "异常", " 接口1:" + ex.ToString());
                return "-1$网络连接异常，请联系工作人员";
            }
            #endregion
            #region 3-52卡状态信息查询
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            keyInfo = strUserName + "|" + strPassword + "|" + "836|CHN|01|" + strIDcode + "|" + strName + "||";
            conInfo = "";
            operType = "3";
            busType = "52";
            try
            {
                strRet = PublicFuction.GetDataByWebService(strWebAddr + "?WSDL", "transBusinessForCard", new string[] { keyInfo, conInfo, operType, busType }).ToString();// 3-18 人员信息查询
                WriteWorkLog.WriteLogs("日志", "信息", "接口2:" + strRet);
            }
            catch (Exception ex)
            {
                WriteWorkLog.WriteLogs("日志", "异常", " 接口2:" + ex.ToString());
                return "-1$网络连接异常，请联系工作人员";
            }
            int iCardStatus = 0;
            if (strRet.Substring(0, 2) == "-1")
            {
                if (strRet.Split('$')[1] == "4")
                {
                    iCardStatus = CARD_STATUS_3_52_4_NOPERSON;
                }
                else if (strRet.Split('$')[1] == "9")
                {
                    iCardStatus = CARD_STATUS_3_52_9_NOCARD;
                }
                else
                {
                    return strRet;
                }
            }
            if (strRet.Substring(0, 1) == "0")
            {
                strRet = strRet.Substring(1, strRet.Length - 1);
                strRet = strRet.Replace("$", "");
                sArray = strRet.Split('|');
                if (sArray[2] == "1")
                {
                    iCardStatus = CARD_STATUS_3_52_1_NORMAL;
                }
                if (sArray[2] == "2")
                {
                    iCardStatus = CARD_STATUS_3_52_2_REPORTLOSS;
                    if (sArray[3] == "21")
                        iCardStatus = CARD_STATUS_3_52_21_ORALLOSS;
                }
                if (sArray[2] == "3")
                {
                    iCardStatus = CARD_STATUS_3_52_3_LOGOUT;
                }
            }
            #endregion
            switch (iCardStatus)
            {
                case CARD_STATUS_3_52_1_NORMAL:
                    #region 3-44 接口 网点信息查询
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    keyInfo = strUserName + "|" + strPassword + "|" + "754|CHN|01|" + strIDcode + "|" + strName + "||";
                    conInfo = "";
                    operType = "3";
                    busType = "44";
                    try
                    {
                        strRet = PublicFuction.GetDataByWebService(strWebAddr + "?WSDL", "transBusinessForCard", new string[] { keyInfo, conInfo, operType, busType }).ToString();// 3-18 人员信息查询
                    }
                    catch (Exception ex)
                    {
                        WriteWorkLog.WriteLogs("日志", "异常", "接口3-44:" + ex.ToString());
                        return "-1$网络连接异常，请联系工作人员";
                    }
                    if (strRet.Substring(0, 1) != "0")
                    {
                        errInfo = strRet.Split('$');
                        WriteWorkLog.WriteLogs("日志", "错误", "接口调用返回非0" + errInfo[2]);
                        return "-1$" + errInfo[2];
                    }
                    errInfo = strRet.Split('$');
                    string[] strArray = errInfo[3].Split('|');
                    #endregion
                    #region 3-7 接口 制卡进度查询
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    keyInfo = strUserName + "|" + strPassword + "|" + "754|CHN|01|" + strIDcode + "|" + strName + "||";
                    conInfo = "";
                    operType = "3";
                    busType = "7";
                    try
                    {
                        strRet = PublicFuction.GetDataByWebService(strWebAddr + "?WSDL", "transBusinessForCard", new string[] { keyInfo, conInfo, operType, busType }).ToString();// 3-18 人员信息查询
                    }
                    catch (Exception ex)
                    {
                        WriteWorkLog.WriteLogs("日志", "异常", "访问社保接口：" + ex.ToString());
                        return "-1$网络连接异常，请联系工作人员";
                    }
                    if (strRet.Substring(0, 1) != "0")
                    {
                        errInfo = strRet.Split('$');
                        WriteWorkLog.WriteLogs("日志", "错误", "211接口调用返回非0" + errInfo[2]);
                        return "-1$" + errInfo[2];
                    }
                    cardProgress = strRet.Substring(4, 2).Replace("|", string.Empty);
                    #endregion
                    strLogInfo = "制卡进度：" + GetProgress(cardProgress) + ",网点：" + strArray[6] + ",网点地址:" + strArray[7];
                    WriteWorkLog.WriteLogs("日志", "信息", "不能制卡提示:" + strLogInfo);
                    return "-1$该社保卡当前处于正常状态,如需补卡请先挂失  " + strLogInfo;

                case CARD_STATUS_3_52_21_ORALLOSS:
                    return "-1$该社保卡已口头挂失,如需补卡请正式挂失";

                case CARD_STATUS_3_52_2_REPORTLOSS:
                    #region 3-18 接口 人员信息查询新
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    keyInfo = strUserName + "|" + strPassword + "|" + "754|CHN|01|" + strIDcode + "|" + strName + "||";
                    conInfo = "";
                    operType = "3";
                    busType = "18";
                    try
                    {
                        strRet = PublicFuction.GetDataByWebService(strWebAddr + "?WSDL", "transBusinessForCard", new string[] { keyInfo, conInfo, operType, busType }).ToString();// 3-18 人员信息查询
                    }
                    catch (Exception ex)
                    {
                        WriteWorkLog.WriteLogs("日志", "异常", "接口3-18:" + ex.ToString());
                        return "-1$网络连接异常，请联系工作人员";
                    }
                    if (strRet.Substring(0, 1) != "0")
                    {
                        errInfo = strRet.Split('$');
                        WriteWorkLog.WriteLogs("日志", "错误", "接口调用返回非0" + errInfo[2]);
                        return "-1$" + errInfo[2];
                    }
                    else
                    {
                        strRet = strRet.Substring(1, strRet.Length - 1);
                        strRet = strRet.Replace("$", "");
                        sArray = strRet.Split('|');
                        strPersonCode = sArray[6];
                    }
                    #endregion
                    #region 3-3 接口 银行信息查询
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    keyInfo = strUserName + "|" + strPassword + "|" + "754|CHN|01|" + strIDcode + "|" + strName + "||" + strPersonCode;
                    conInfo = "";
                    operType = "3";
                    busType = "3";
                    try
                    {
                        strRet = PublicFuction.GetDataByWebService(strWebAddr + "?WSDL", "transBusinessForCard", new string[] { keyInfo, conInfo, operType, busType }).ToString();// 3-18 人员信息查询
                    }
                    catch (Exception ex)
                    {
                        WriteWorkLog.WriteLogs("日志", "异常", "接口3-3:" + ex.ToString());
                        return "-1$网络连接异常，请联系工作人员";
                    }
                    if (strRet.Substring(0, 1) != "0")
                    {
                        errInfo = strRet.Split('$');
                        WriteWorkLog.WriteLogs("日志", "错误", "接口调用返回非0" + errInfo[2]);
                        return "-1$" + errInfo[2];
                    }
                    else
                    {
                        strRet = strRet.Substring(1, strRet.Length - 1);
                        strRet = strRet.Replace("$", "");
                        sArray = strRet.Split('|');
                        strOldBankCode = sArray[0];
                        if (sArray[3] == "0")
                        {
                            WriteWorkLog.WriteLogs("日志", "信息", "开户失败,原因：" + sArray[4]);
                            return "-1$卡状态为开户失败,失败原因:" + sArray[4];
                        }
                    }
                    #endregion
                    #region 2-50补卡数据采集上传
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    keyInfo = strUserName + "|" + strPassword + "|" + "836|CHN|01|" + strIDcode + "|" + strName + "||" + strPersonCode;
                    conInfo = "BAZ838||" + strBankCode + "|&BAZ802||" + strUnitCode + "|&BAZ844||" + "0" + "|&BAZ916|||&BAZ917|||&BAZ918|||&BANK|||&";
                    if (strBankCode != strOldBankCode)
                    {
                        conInfo = "BAZ838||" + strBankCode + "|&BAZ802||" + strUnitCode + "|&BAZ844||" + "0" + "|&BAZ916|||&BAZ917|||&BAZ918|||&BANK||1|&";
                    }
                    operType = "2";
                    busType = "50";
                    try
                    {
                        strRet = PublicFuction.GetDataByWebService(strWebAddr + "?WSDL", "transBusinessForCard", new string[] { keyInfo, conInfo, operType, busType }).ToString();// 3-18 人员信息查询
                    }
                    catch (Exception ex)
                    {
                        WriteWorkLog.WriteLogs("日志", "异常", "接口2-50:" + ex.ToString());
                        return "-1$网络连接异常，请联系工作人员";
                    }
                    if (strRet.Substring(0, 1) != "0")
                    {
                        errInfo = strRet.Split('$');
                        WriteWorkLog.WriteLogs("日志", "错误", "接口2-50调用返回非0" + errInfo[2]);
                        return "-1$" + errInfo[2];
                    }
                    else
                    {
                        WriteWorkLog.WriteLogs("日志", "信息", "补卡数据下载成功");
                        strRet = strRet.Substring(1, strRet.Length - 1);
                        strBatch = strRet.Replace("$", "");
                        return "0$0";
                    }
                #endregion

                case CARD_STATUS_3_52_3_LOGOUT:
                    #region 3-6接口  制卡状态查询
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    keyInfo = strUserName + "|" + strPassword + "|" + "754|CHN|01|" + strIDcode + "|" + strName + "||";
                    conInfo = "";
                    operType = "3";
                    busType = "6";
                    try
                    {
                        strRet = PublicFuction.GetDataByWebService(strWebAddr + "?WSDL", "transBusinessForCard", new string[] { keyInfo, conInfo, operType, busType }).ToString();// 3-6
                    }
                    catch (Exception ex)
                    {
                        WriteWorkLog.WriteLogs("日志", "异常", "接口3-6:" + ex.ToString());
                        return "-1$网络连接异常，请联系工作人员";
                    }
                    if (strRet.Substring(0, 1) != "0")
                    {
                        errInfo = strRet.Split('$');
                        WriteWorkLog.WriteLogs("日志", "错误", "接口3-6调用返回非0" + errInfo[2]);
                        return "-1$" + errInfo[2];
                    }
                    else
                    {
                        strRet = strRet.Substring(1, strRet.Length - 1);
                        strRet = strRet.Replace("$", "");
                        if (strRet != "1")
                        {
                            #region 3-7 接口 制卡进度查询
                            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            keyInfo = strUserName + "|" + strPassword + "|" + "754|CHN|01|" + strIDcode + "|" + strName + "||";
                            conInfo = "";
                            operType = "3";
                            busType = "7";
                            try
                            {
                                strRet = PublicFuction.GetDataByWebService(strWebAddr + "?WSDL", "transBusinessForCard", new string[] { keyInfo, conInfo, operType, busType }).ToString();// 3-18 人员信息查询
                            }
                            catch (Exception ex)
                            {
                                WriteWorkLog.WriteLogs("日志", "异常", "访问社保接口：" + ex.ToString());
                                return "-1$网络连接异常，请联系工作人员";
                            }
                            if (strRet.Substring(0, 1) != "0")
                            {
                                errInfo = strRet.Split('$');
                                WriteWorkLog.WriteLogs("日志", "错误", "211接口调用返回非0" + errInfo[2]);
                                return "-1$" + errInfo[2];
                            }
                            cardProgress = strRet.Substring(4, 2).Replace("|", string.Empty);
                            return "-1$已提交制卡申请：" + GetProgress(cardProgress);
                            #endregion
                        }
                    }
                    #endregion
                    #region 3-18接口 人员信息查询
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    keyInfo = strUserName + "|" + strPassword + "|" + "754|CHN|01|" + strIDcode + "|" + strName + "||";
                    conInfo = "";
                    operType = "3";
                    busType = "18";
                    try
                    {
                        strRet = PublicFuction.GetDataByWebService(strWebAddr + "?WSDL", "transBusinessForCard", new string[] { keyInfo, conInfo, operType, busType }).ToString();// 3-18 人员信息查询
                    }
                    catch (Exception ex)
                    {
                        WriteWorkLog.WriteLogs("日志", "异常", "接口3-18:" + ex.ToString());
                        return "-1$网络连接异常，请联系工作人员";
                    }
                    if (strRet.Substring(0, 1) != "0")
                    {
                        errInfo = strRet.Split('$');
                        WriteWorkLog.WriteLogs("日志", "错误", "接口3-18调用返回非0" + errInfo[2]);
                        return "-1$" + errInfo[2];
                    }
                    else
                    {
                        strRet = strRet.Substring(1, strRet.Length - 1);
                        strRet = strRet.Replace("$", "");
                        sArray = strRet.Split('|');
                        strPersonCode = sArray[6];
                    }
                    #endregion
                    #region 3-41接口 照片查询
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    keyInfo = strUserName + "|" + strPassword + "|" + "754|CHN|01|" + strIDcode + "|" + strName + "||";
                    conInfo = "";
                    operType = "3";
                    busType = "41";
                    try
                    {
                        strRet = PublicFuction.GetDataByWebService(strWebAddr + "?WSDL", "transBusinessForCard", new string[] { keyInfo, conInfo, operType, busType }).ToString();// 3-18 人员信息查询
                    }
                    catch (Exception ex)
                    {
                        WriteWorkLog.WriteLogs("日志", "异常", "接口3-41:" + ex.ToString());
                        return "-1$网络连接异常，请联系工作人员";
                    }
                    if (strRet.Substring(0, 1) != "0")
                    {
                        string birthDate = strIDcode.Substring(6, 8);
                        int BirthYear = int.Parse(birthDate.Substring(0, 4));
                        int BirthMonth = int.Parse(birthDate.Substring(4, 2));
                        int BirthDay = int.Parse(birthDate.Substring(6, 2));
                        DateTime nowDateTime = DateTime.Now;
                        int age = nowDateTime.Year - BirthYear;
                        if (nowDateTime.Month < BirthMonth || (nowDateTime.Month == BirthMonth && nowDateTime.Day < BirthDay))
                        {
                            age--;
                        }
                        if (age >= 0)//16岁照片以下也要照相
                        {
                            errInfo = strRet.Split('$');
                            WriteWorkLog.WriteLogs("日志", "错误", "接口3-41 age>16 ," + errInfo[2]);
                            return "0$1";
                        }
                    }
                    else
                    {
                        string strSubRet = strRet.Substring(1, strRet.Length - 1);
                        strSubRet = strSubRet.Replace("$", "");
                        sArray = strSubRet.Split('|');
                        strPhoto = sArray[3];
                    }
                    #endregion
                    #region 2-51接口 数据采集上传
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    keyInfo = strUserName + "|" + strPassword + "|" + "836|CHN|01|" + strIDcode + "|" + strName + "||" + strPersonCode;
                    conInfo = "AAC163||CHN|&AAC058||01|&AAE135||" + strIDcode.Trim() + "|&AAC003||" + strName.Trim() + "|&BAZ805|||&AAC059|||&AAB200||" + strCityCode + "|&BAC025|||&BAC026|||&BAZ838||" + strBankCode + "|&BAE017|||&AAE005|||&AAC011|||&AAE006|||&AAB001|||&AAB004|||&AAB003||0|&AAB005|||&BAZ802||" + strUnitCode + "|&BAC200||" + strPhoto + "|&BAZ815||1|&AAE011||" + strOperator + "|&BAZ840||3|&BAB309|||&BAB310|||&AAE020|||&BAB311|||&";
                    operType = "2";
                    busType = "51";
                    try
                    {
                        strRet = PublicFuction.GetDataByWebService(strWebAddr + "?WSDL", "transBusinessForCard", new string[] { keyInfo, conInfo, operType, busType }).ToString();// 3-18 人员信息查询
                    }
                    catch (Exception ex)
                    {
                        WriteWorkLog.WriteLogs("日志", "异常", "接口2-51:" + ex.ToString());
                        return "-1$网络连接异常，请联系工作人员";
                    }
                    if (strRet.Substring(0, 1) != "0")
                    {
                        errInfo = strRet.Split('$');
                        WriteWorkLog.WriteLogs("日志", "错误", "接口2-51调用返回非0" + errInfo[2]);
                        return "-1$" + errInfo[2];
                    }
                    else
                    {
                        strRet = strRet.Substring(1, strRet.Length - 1);
                        strBatch = strRet.Replace("$", "");
                        WriteWorkLog.WriteLogs("日志", "信息", "注销重办卡数据上传成功");
                        return "0$0";
                    }
                #endregion
                case CARD_STATUS_3_52_4_NOPERSON:
                    #region 1-11接口 人员基础信息新增
                    keyInfo = strUserName + "|" + strPassword + "|" + "754|CHN|01|" + strIDcode + "|" + strName + "||";
                    string strSex = strIDcode.Substring(16, 1);
                    int iSex = Convert.ToInt32(strIDcode.Substring(16, 1));
                    int sex = 2;
                    if ((iSex & 1) == 1)
                    {
                        sex = 1;
                    }
                    conInfo = "AAC163||CHN|&AAC058||01|&AAE135||" + strIDcode + "|&AAC003||" + strName + "|&AAC004||" + sex.ToString() + "|&AAB200||" + strCityCode + "|&AAE011||" + strOperator + "|& BAE017|||& AAE005|||& AAE006|||&";
                    operType = "1";
                    busType = "11";
                    try
                    {
                        strRet = PublicFuction.GetDataByWebService(strWebAddr + "?WSDL", "transBusinessForCard", new string[] { keyInfo, conInfo, operType, busType }).ToString();// 3-18 人员信息查询
                    }
                    catch (Exception ex)
                    {
                        WriteWorkLog.WriteLogs("日志", "异常", "接口2-51:" + ex.ToString());
                        return "-1$网络连接异常，请联系工作人员";
                    }
                    if (strRet.Substring(0, 1) != "0")
                    {
                        errInfo = strRet.Split('$');
                        WriteWorkLog.WriteLogs("日志", "错误", "接口1-11调用返回非0" + errInfo[2]);
                        return "-1$" + errInfo[2];
                    }
                    else
                    {
                        string[] sArray2 = new String[] { "1", "2", "3" };
                        strRet = strRet.Substring(1, strRet.Length - 1);
                        strRet = strRet.Replace("$", "");
                        sArray2 = strRet.Split('|');
                        strPersonCode = sArray2[6];
                    }

                    #endregion
                    return "0$2";//1-11》拍照 》手机号》1-19》2-51
                case CARD_STATUS_3_52_9_NOCARD:
                    #region 3-7 接口 制卡进度查询
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    keyInfo = strUserName + "|" + strPassword + "|" + "754|CHN|01|" + strIDcode + "|" + strName + "||";
                    conInfo = "";
                    operType = "3";
                    busType = "7";
                    try
                    {
                        strRet = PublicFuction.GetDataByWebService(strWebAddr + "?WSDL", "transBusinessForCard", new string[] { keyInfo, conInfo, operType, busType }).ToString();// 3-18 人员信息查询
                    }
                    catch (Exception ex)
                    {
                        WriteWorkLog.WriteLogs("日志", "异常", "访问社保接口：" + ex.ToString());
                        return "-1$网络连接异常，请联系工作人员";
                    }
                    if (strRet.Substring(0, 1) != "0")
                    {
                        errInfo = strRet.Split('$');
                        WriteWorkLog.WriteLogs("日志", "错误", "211接口调用返回非0" + errInfo[2]);
                        return "-1$" + errInfo[2];
                    }
                    #endregion
                    cardProgress = strRet.Substring(4, 2).Replace("|", string.Empty);
                    if (cardProgress != "0")
                    {
                        strLogInfo = "您已提交制卡申请，制卡进度：" + GetProgress(cardProgress);
                        WriteWorkLog.WriteLogs("日志", "信息", "制卡进度：" + GetProgress(cardProgress));
                        return "-1$" + strLogInfo;
                    }
                    else
                    {
                        #region 3-18接口 人员信息查询
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        keyInfo = strUserName + "|" + strPassword + "|" + "754|CHN|01|" + strIDcode + "|" + strName + "||";
                        conInfo = "";
                        operType = "3";
                        busType = "18";
                        try
                        {
                            strRet = PublicFuction.GetDataByWebService(strWebAddr + "?WSDL", "transBusinessForCard", new string[] { keyInfo, conInfo, operType, busType }).ToString();// 3-18 人员信息查询
                        }
                        catch (Exception ex)
                        {
                            WriteWorkLog.WriteLogs("日志", "异常", "接口3-18:" + ex.ToString());
                            return "-1$网络连接异常，请联系工作人员";
                        }
                        if (strRet.Substring(0, 1) != "0")
                        {
                            errInfo = strRet.Split('$');
                            WriteWorkLog.WriteLogs("日志", "错误", "接口3-18调用返回非0" + errInfo[2]);
                            return "-1$" + errInfo[2];
                        }
                        else
                        {
                            strRet = strRet.Substring(1, strRet.Length - 1);
                            strRet = strRet.Replace("$", "");
                            sArray = strRet.Split('|');
                            strPersonCode = sArray[6];
                        }
                        #endregion
                        #region 3-41接口 照片查询
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        keyInfo = strUserName + "|" + strPassword + "|" + "754|CHN|01|" + strIDcode + "|" + strName + "||";
                        conInfo = "";
                        operType = "3";
                        busType = "41";
                        try
                        {
                            strRet = PublicFuction.GetDataByWebService(strWebAddr + "?WSDL", "transBusinessForCard", new string[] { keyInfo, conInfo, operType, busType }).ToString();// 3-18 人员信息查询
                        }
                        catch (Exception ex)
                        {
                            WriteWorkLog.WriteLogs("日志", "异常", "接口3-41:" + ex.ToString());
                            return "-1$网络连接异常，请联系工作人员";
                        }
                        if (strRet.Substring(0, 1) != "0")
                        {
                            string birthDate = strIDcode.Substring(6, 8);
                            int BirthYear = int.Parse(birthDate.Substring(0, 4));
                            int BirthMonth = int.Parse(birthDate.Substring(4, 2));
                            int BirthDay = int.Parse(birthDate.Substring(6, 2));
                            DateTime nowDateTime = DateTime.Now;
                            int age = nowDateTime.Year - BirthYear;
                            if (nowDateTime.Month < BirthMonth || (nowDateTime.Month == BirthMonth && nowDateTime.Day < BirthDay))
                            {
                                age--;
                            }
                            if (age >= 0)
                            {
                                errInfo = strRet.Split('$');
                                WriteWorkLog.WriteLogs("日志", "错误", "接口3-41 age>16 ," + errInfo[2]);
                                return "0$1";
                            }
                        }
                        else
                        {
                            string strSubRet = strRet.Substring(1, strRet.Length - 1);
                            strSubRet = strSubRet.Replace("$", "");
                            sArray = strSubRet.Split('|');
                            strPhoto = sArray[3];
                        }
                        #endregion
                        #region 2-51接口 数据采集上传
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        keyInfo = strUserName + "|" + strPassword + "|" + "836|CHN|01|" + strIDcode + "|" + strName + "||" + strPersonCode;
                        conInfo = "AAC163||CHN|&AAC058||01|&AAE135||" + strIDcode.Trim() + "|&AAC003||" + strName.Trim() + "|&BAZ805|||&AAC059|||&AAB200||" + strCityCode + "|&BAC025|||&BAC026|||&BAZ838||" + strBankCode + "|&BAE017|||&AAE005|||&AAC011|||&AAE006|||&AAB001|||&AAB004|||&AAB003||0|&AAB005|||&BAZ802||" + strUnitCode + "|&BAC200||" + strPhoto + "|&BAZ815||1|&AAE011||" + strOperator + "|&BAZ840||3|&BAB309|||&BAB310|||&AAE020|||&BAB311|||&";
                        operType = "2";
                        busType = "51";
                        try
                        {
                            strRet = PublicFuction.GetDataByWebService(strWebAddr + "?WSDL", "transBusinessForCard", new string[] { keyInfo, conInfo, operType, busType }).ToString();// 3-18 人员信息查询
                        }
                        catch (Exception ex)
                        {
                            WriteWorkLog.WriteLogs("日志", "异常", "接口2-51:" + ex.ToString());
                            return "-1$网络连接异常，请联系工作人员";
                        }
                        if (strRet.Substring(0, 1) != "0")
                        {
                            errInfo = strRet.Split('$');
                            WriteWorkLog.WriteLogs("日志", "错误", "接口2-51调用返回非0" + errInfo[2]);
                            return "-1$" + errInfo[2];
                        }
                        else
                        {
                            strRet = strRet.Substring(1, strRet.Length - 1);
                            strBatch = strRet.Replace("$", "");
                            WriteWorkLog.WriteLogs("日志", "信息", "没卡人员办卡采集上传成功");
                            return "0$0";
                        }
                        #endregion
                    }
                default:
                    break;
            }
            return "0";

        }
        public static string downLoadCardData(string strIDcode, string strName)
        {
            string strBatch = "";
            string strRet = AccessWebInterFace.IDReadWebInterface(strIDcode, strName, out strBatch);
            if (strRet == "0$0")
            {
                #region 获取最大序号
                int intRecordNo = -1;
                OleDbConnection conn = new OleDbConnection(AccessWebInterFace.strConn);
                string strSql = "select MAX(序号) from E_人员信息";
                try
                {
                    conn.Open();
                }
                catch (Exception ex)
                {
                    WriteWorkLog.WriteLogs("日志", "异常", " conn.Open:" + ex.ToString());
                    conn.Close();
                    return "-1$本地数据库打开失败";
                }
                finally
                {
                    conn.Close();
                }
                OleDbDataAdapter da = new OleDbDataAdapter(strSql, conn);
                DataSet ds = new DataSet();
                da.Fill(ds);
                string strRecodeNo = ds.Tables[0].Rows[0][0].ToString();
                int.TryParse(ds.Tables[0].Rows[0][0].ToString(), out intRecordNo);
                #endregion
                return Interface3_22(strBatch, intRecordNo);
            }
            return strRet;
        }
        public static string downLoadCardDataByBatch(string strBatch, string strIDcode, string strName)
        {
            #region 获取最大序号
            int intRecordNo = 0;
            OleDbConnection conn = new OleDbConnection(Access_SQLHelper.strConn);
            string strSql = "select MAX(序号) from E_人员信息";
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                WriteWorkLog.WriteLogs("日志", "异常", " conn.Open:" + ex.ToString());
                conn.Close();
                return "-1$本地数据库打开失败";
            }
            finally
            {
                conn.Close();
            }
            OleDbDataAdapter da = new OleDbDataAdapter(strSql, conn);
            DataSet ds = new DataSet();
            da.Fill(ds);
            string strRecodeNo = ds.Tables[0].Rows[0][0].ToString();
            int.TryParse(ds.Tables[0].Rows[0][0].ToString(), out intRecordNo);
            #endregion
            return Interface3_22(strBatch, intRecordNo);
        }
        public static string Interface3_22(string batch, int intRecordNo)
        {
            string keyInfo = "";
            string operType = "";
            string conInfo = "";
            string busType = "";
            string strRet = "";
            string strBatch = "";
            string strLogInfo = "";
            string[] errInfo = new String[] { "1", "2", "3" };
            string[] sArray = new String[] { "1", "2", "3" };
            #region 3-22 接口 人员信息下载
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            keyInfo = strUserName + "|" + strPassword + "|" + strIP + "|" + strMachineID + "|" + "836";
            conInfo = "BAZ031||" + batch + "|&";
            operType = "3";
            busType = "22";
            try
            {
                strRet = PublicFuction.GetDataByWebService(strWebAddr + "?WSDL", "transBusinessForCard", new string[] { keyInfo, conInfo, operType, busType }).ToString();// 3-18 人员信息查询
            }
            catch (Exception ex)
            {
                WriteWorkLog.WriteLogs("日志", "异常", "接口3-22：" + ex.ToString());
                return "网络连接异常，请联系工作人员";
            }
            if (strRet.Substring(0, 1) != "0")
            {
                errInfo = strRet.Split('$');
                WriteWorkLog.WriteLogs("日志", "错误", "接口3-22：" + errInfo[2]);
                return "-1$" + errInfo[2];
            }
            else
            {
                strRet = strRet.Remove(strRet.Length - 1);
                strRet = strRet.Substring(1).Replace("$", "");
                strRet = strRet + batch + "|" + strBankCode;
                string[] sArray14 = strRet.Split('|');
                strLogInfo = "0$$$" + sArray14[0] + "|" + sArray14[1] + "|" + sArray14[2] + "|" + sArray14[3] + "|" + sArray14[4] + "|" + sArray14[5] + "|" + sArray14[6] + "|" + sArray14[7] + "|" + sArray14[8] + "|" + sArray14[9] + "|" + "|" + sArray14[10] + "|" + sArray14[11] + "|照片|$^";
                WriteWorkLog.WriteLogs("日志", "信息", "接口3-22返回:" + strLogInfo);
                strRet = UpdateFromWebData(sArray14, intRecordNo + 1);
                if (strRet != "0")
                {
                    return strRet;
                }
                else
                {
                    return "0$0";
                }
            }
            #endregion
        }
        public static string Interface3_48(string strGID, string strGName, string personID, string strQRID, out string batch)
        {
            string keyInfo = "";
            string operType = "";
            string conInfo = "";
            string busType = "";
            string strRet = "";
            string strLogInfo = "";
            batch = "";
            string[] errInfo = new String[] { "1", "2", "3" };
            string[] sArray = new String[] { "1", "2", "3" };
            #region 3-48 接口 二维码返回批次
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            keyInfo = strUserName + "|" + strPassword + "|" + strValidCode + "|CHN|01|" + strGID + "|" + strGName + "||" + personID;//格式：用户名|密码|验证码|国家或地区|证件类型|证件号码|姓名|卡识别码|省人员识别号
            conInfo = "BAZ001||" + strQRID + "|&";
            operType = "3";
            busType = "48";
            try
            {
                strRet = PublicFuction.GetDataByWebService(strWebAddr + "?WSDL", "transBusinessForCard", new string[] { keyInfo, conInfo, operType, busType }).ToString();
                WriteWorkLog.WriteLogs("日志", "信息", "接口3-48：" + strRet);
            }
            catch (Exception ex)
            {
                WriteWorkLog.WriteLogs("日志", "异常", "接口3-22：" + ex.ToString());
                return "网络连接异常，请联系工作人员";
            }
            if (strRet.Substring(0, 1) != "0")
            {
                string[] errInfo3 = strRet.Split('$');
                return errInfo3[2];
            }
            else
            {
                string[] strCodeData = strRet.Split('$');
                batch = strCodeData[3].Substring(0, strCodeData[3].IndexOf("|"));
            }
            #endregion
            return "0";
        }
        public static string Interface2_23(string strBatch, string personID, string cardIdCode)
        {
            string keyInfo = "";
            string operType = "";
            string conInfo = "";
            string busType = "";
            string strRet = "";
            string[] errInfo = new String[] { "1", "2", "3" };
            string[] sArray = new String[] { "1", "2", "3" };
            #region 2-23 接口 回传制卡信息
            keyInfo = strUserName + "|" + strPassword + "|" + strIP + "|" + strMachineID + "|" + strValidCode;
            conInfo = "BAZ031||" + strBatch + "|&BAZ805||" + personID + "|&BAZ801||" + cardIdCode + "|&";
            operType = "2";
            busType = "23";
            try
            {
                strRet = PublicFuction.GetDataByWebService(strWebAddr + "?WSDL", "transBusinessForCard", new string[] { keyInfo, conInfo, operType, busType }).ToString();
                WriteWorkLog.WriteLogs("日志", "信息", "2-23返回" + strRet);
            }
            catch (Exception exErr)
            {
                WriteWorkLog.WriteLogs("日志", "异常", "接口返回异常(2-23)" + exErr.ToString());
                return "回传接口异常";
            }
            if (strRet.Substring(0, 1) != "0")
            {
                return "回传失败";
            }
            return "0";
            #endregion
        }
        public static string Interface2_49(string strGName, string strGID, string personID)
        {
            string keyInfo = "";
            string operType = "";
            string conInfo = "";
            string busType = "";
            string strRet = "";
            string[] errInfo = new String[] { "1", "2", "3" };
            string[] sArray = new String[] { "1", "2", "3" };
            WriteWorkLog.WriteLogs("日志", "信息", "接口2-49 :" +strGName +","+ strGID + "," + personID);
            #region 2-49 接口 个人领卡
            keyInfo = strUserName + "|" + strPassword + "|" + "836|CHN|01|" + strGID.Trim() + "|" + strGName.Trim() + "||" + personID.Trim();
            conInfo = "BAZ001|||&";
            WriteWorkLog.WriteLogs("日志", "信息", "接口2-49 :" + keyInfo);
            operType = "2";
            busType = "49";
            try
            {
                strRet = PublicFuction.GetDataByWebService(strWebAddr + "?WSDL", "transBusinessForCard", new string[] { keyInfo, conInfo, operType, busType }).ToString();
                WriteWorkLog.WriteLogs("日志", "信息", "接口2-49返回:" + strRet);
            }
            catch (Exception ex)
            {
                WriteWorkLog.WriteLogs("日志", "异常", "接口异常" + ex.ToString());
                return "网络连接异常，请联系工作人员";
            }
            if (strRet == "")
            {
                return "接口调用异常,无法连接服务器";
            }
            if (strRet.Substring(0, 1) != "0")
            {
                string[] errInfo2 = strRet.Split('$');
                WriteWorkLog.WriteLogs("日志", "信息", "接口调用返回非0" + errInfo2[2]);
                return errInfo2[2] + ",请您到前台找工作人员";
            }
            return "0";
            #endregion
        }
        public static string Interface3_52(string strIDcode, string strName)
        {
            string keyInfo = "";
            string operType = "";
            string conInfo = "";
            string busType = "";
            string strRet = "";
            string strStatus = "";
            string[] errInfo = new String[] { "1", "2", "3" };
            string[] sArray = new String[] { "1", "2", "3" };
            #region 3-52卡状态信息查询
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            keyInfo = strUserName + "|" + strPassword + "|" + "836|CHN|01|" + strIDcode + "|" + strName + "||";
            conInfo = "";
            operType = "3";
            busType = "52";
            try
            {
                strRet = PublicFuction.GetDataByWebService(strWebAddr + "?WSDL", "transBusinessForCard", new string[] { keyInfo, conInfo, operType, busType }).ToString();// 3-18 人员信息查询
                WriteWorkLog.WriteLogs("日志", "信息", "接口2:" + strRet);
            }
            catch (Exception ex)
            {
                WriteWorkLog.WriteLogs("日志", "异常", " 接口2:" + ex.ToString());
                return "-1$网络连接异常，请联系工作人员";
            }
            int iCardStatus = 0;
            if (strRet.Split('$')[0] == "-1")
            {
                if (strRet.Split('$')[1] == "4")
                {
                    iCardStatus = CARD_STATUS_3_52_4_NOPERSON;
                    strStatus = "查无此人";
                    return strStatus;
                }
                if (strRet.Split('$')[1] == "9")
                {
                    iCardStatus = CARD_STATUS_3_52_9_NOCARD;
                    strStatus = "未发卡";
                    return strStatus;
                }
            }
            if (strRet.Substring(0, 1) == "0")
            {
                strRet = strRet.Substring(1, strRet.Length - 1);
                strRet = strRet.Replace("$", "");
                sArray = strRet.Split('|');
                if (sArray[2] == "1")
                {
                    iCardStatus = CARD_STATUS_3_52_1_NORMAL;
                    strStatus = "正常";
                    return strStatus;
                }
                if (sArray[2] == "2")
                {
                    iCardStatus = CARD_STATUS_3_52_2_REPORTLOSS;
                    strStatus = "挂失";
                    if (sArray[3] == "21")
                    {
                        iCardStatus = CARD_STATUS_3_52_21_ORALLOSS;
                        strStatus = "口头挂失";
                    }
                    return strStatus;
                }
                if (sArray[2] == "3")
                {
                    iCardStatus = CARD_STATUS_3_52_3_LOGOUT;

                    strStatus = "注销";
                    return strStatus;
                }
            }
            return strStatus;
            #endregion
        }
        public static string Interface3_44(string strIDcode, string strName, out string netName, out string netAddr)
        {
            string keyInfo = "";
            string operType = "";
            string conInfo = "";
            string busType = "";
            string strRet = "";
            netName = "";
            netAddr = "";
            string[] errInfo = new String[] { "1", "2", "3" };
            string[] sArray = new String[] { "1", "2", "3" };
            #region 3-44 接口 网点信息查询
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            keyInfo = strUserName + "|" + strPassword + "|" + "754|CHN|01|" + strIDcode + "|" + strName + "||";
            conInfo = "";
            operType = "3";
            busType = "44";
            try
            {
                strRet = PublicFuction.GetDataByWebService(strWebAddr + "?WSDL", "transBusinessForCard", new string[] { keyInfo, conInfo, operType, busType }).ToString();
            }
            catch (Exception ex)
            {
                WriteWorkLog.WriteLogs("日志", "异常", "接口3-44:" + ex.ToString());
                return "-1$网络连接异常，请联系工作人员";
            }
            if (strRet.Substring(0, 1) != "0")
            {
                errInfo = strRet.Split('$');
                WriteWorkLog.WriteLogs("日志", "错误", "接口调用返回非0" + errInfo[2]);
                return "-1$" + errInfo[2];
            }
            errInfo = strRet.Split('$');
            string[] strArray = errInfo[3].Split('|');
            netName = strArray[6];
            netAddr = strArray[7];
            return "0";
            #endregion
        }
        public static string Interface3_3(string strIDcode, string strName, out string bankCode, out string bankCardNo, out string hasOpenCount, out string hasActivate)
        {
            string keyInfo = "";
            string operType = "";
            string conInfo = "";
            string busType = "";
            string strRet = "";
            string strPersonCode = "";
            hasActivate = "";
            hasOpenCount = "";
            bankCode = "";
            bankCardNo = "";
            string[] errInfo = new String[] { "1", "2", "3" };
            string[] sArray = new String[] { "1", "2", "3" };
            #region 3-18 接口 人员信息查询新
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            keyInfo = strUserName + "|" + strPassword + "|" + "754|CHN|01|" + strIDcode + "|" + strName + "||";
            conInfo = "";
            operType = "3";
            busType = "18";
            try
            {
                strRet = PublicFuction.GetDataByWebService(strWebAddr + "?WSDL", "transBusinessForCard", new string[] { keyInfo, conInfo, operType, busType }).ToString();// 3-18 人员信息查询
            }
            catch (Exception ex)
            {
                WriteWorkLog.WriteLogs("日志", "异常", "接口3-18:" + ex.ToString());
                return "-1$网络连接异常，请联系工作人员";
            }
            if (strRet.Substring(0, 1) != "0")
            {
                errInfo = strRet.Split('$');
                WriteWorkLog.WriteLogs("日志", "错误", "接口调用返回非0" + errInfo[2]);
                return "-1$" + errInfo[2];
            }
            else
            {
                strRet = strRet.Substring(1, strRet.Length - 1);
                strRet = strRet.Replace("$", "");
                sArray = strRet.Split('|');
                strPersonCode = sArray[6];
            }
            #endregion
            #region 3-3 接口 银行信息查询
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            keyInfo = strUserName + "|" + strPassword + "|" + "754|CHN|01|" + strIDcode + "|" + strName + "||" + strPersonCode;
            conInfo = "";
            operType = "3";
            busType = "3";
            try
            {
                strRet = PublicFuction.GetDataByWebService(strWebAddr + "?WSDL", "transBusinessForCard", new string[] { keyInfo, conInfo, operType, busType }).ToString();// 3-18 人员信息查询
            }
            catch (Exception ex)
            {
                WriteWorkLog.WriteLogs("日志", "异常", "接口3-3:" + ex.ToString());
                return "-1$网络连接异常，请联系工作人员";
            }
            if (strRet.Substring(0, 1) != "0")
            {
                errInfo = strRet.Split('$');
                WriteWorkLog.WriteLogs("日志", "错误", "接口调用返回非0" + errInfo[2]);
                return "-1$" + errInfo[2];
            }
            else
            {
                strRet = strRet.Substring(1, strRet.Length - 1);
                strRet = strRet.Replace("$", "");
                sArray = strRet.Split('|');
                bankCode = sArray[0];
                bankCardNo = sArray[1];
                if (sArray[2] == "0")
                {
                    hasActivate = "否";
                }
                else
                {
                    hasActivate = "是";
                }
                if (sArray[3] == "0")
                {
                    WriteWorkLog.WriteLogs("日志", "信息", "开户失败,原因：" + sArray[4]);
                    hasOpenCount = "否";
                }
                else
                {
                    hasOpenCount = "是";
                }
            }
            #endregion
            return "0";
        }
        public static string Interface3_7(string strIDcode, string strName, out string cardProgress)
        {
            string keyInfo = "";
            string operType = "";
            string conInfo = "";
            string busType = "";
            string strRet = "";
            cardProgress = "";
            string[] errInfo = new String[] { "1", "2", "3" };
            string[] sArray = new String[] { "1", "2", "3" };
            #region 3-7 接口 制卡进度查询
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            keyInfo = strUserName + "|" + strPassword + "|" + "754|CHN|01|" + strIDcode + "|" + strName + "||";
            conInfo = "";
            operType = "3";
            busType = "7";
            try
            {
                strRet = PublicFuction.GetDataByWebService(strWebAddr + "?WSDL", "transBusinessForCard", new string[] { keyInfo, conInfo, operType, busType }).ToString();// 3-18 人员信息查询
            }
            catch (Exception ex)
            {
                WriteWorkLog.WriteLogs("日志", "异常", "访问社保接口：" + ex.ToString());
                return "-1$网络连接异常，请联系工作人员";
            }
            if (strRet.Substring(0, 1) != "0")
            {
                errInfo = strRet.Split('$');
                WriteWorkLog.WriteLogs("日志", "错误", "37接口调用返回非0" + errInfo[2]);
                return "-1$" + errInfo[2];
            }
            #endregion
            cardProgress = strRet.Substring(4, 2).Replace("|", string.Empty);
            return "0";
        }
        public static string UpdateFromWebData(string[] strInfo, int recordNo)
        {
            if (strInfo[12] == "")
            {
                strInfo[12] = "/9j/4AAQSkZJRgABAQEAYABgAAD/2wBDAAIBAQIBAQICAgICAgICAwUDAwMDAwYEBAMFBwYHBwcGBwcICQsJCAgKCAcHCg0KCgsMDAwMBwkODw0MDgsMDAz/2wBDAQICAgMDAwYDAwYMCAcIDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAz/wAARCAB+AGYDASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwD9/KKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigD/9k=";
            }
            Byte[] filedate = Convert.FromBase64String(strInfo[12]);
            string sql;
            if (strInfo[4] == "1")
                strInfo[4] = "男";
            if (strInfo[4] == "2")
                strInfo[4] = "女";
            sql = "insert into  E_人员信息 (网点批次号,行政区划,姓名,身份证号,性别,民族,出生日期,姓名扩展,出生地,人员识别号,社保卡号,发卡日期,批次,银行编码,序号,照片) values(";
            sql += "'" + strInfo[0] + "','" + strInfo[1] + "','" + strInfo[2] + "','" + strInfo[3] + "','" + strInfo[4] + "','" + strInfo[5] + "','" + strInfo[6] + "','" + strInfo[7] + "','" + strInfo[8] + "','";
            sql += strInfo[9] + "','" + strInfo[10] + "','" + strInfo[11] + "','" + strInfo[13] + "','" + strInfo[14] + "'," + recordNo + ",@image)";
            OleDbConnection conn = new OleDbConnection(strConn);
            conn = new OleDbConnection(strConn);
            OleDbCommand comm = null;
            string sqlExsit = "select 身份证号 from E_人员信息 where (身份证号=" + "'" + strInfo[3] + "'" + "and 批次='" + strInfo[13] + "')";
            conn.Open();
            comm = conn.CreateCommand(); ;
            comm.CommandText = sqlExsit;
            OleDbDataReader dr = comm.ExecuteReader();
            if (!dr.HasRows)
            {
                try
                {
                    comm = new OleDbCommand(sql, conn);
                    comm.Parameters.AddWithValue("@image", filedate);
                    comm.ExecuteNonQuery();
                    comm.Dispose();
                    conn.Close();
                }
                catch (Exception exErr)
                {
                    comm.Dispose();
                    conn.Close();
                    WriteWorkLog.WriteLogs("日志", "异常", " 本地数据库异常(UpdateFromWebData):" + exErr.ToString());
                    return "保存数据出错";
                }
            }
            return "0";
        }
        public static string UpdatePeopleInfoToWeb(string strIDcode, string strName, int businessType)
        {
            string keyInfo = "";
            string operType = "";
            string conInfo = "";
            string busType = "";
            string strRet = "";
            string strBatch = "";
            string[] errInfo = new String[] { "1", "2", "3" };
            string[] sArray = new String[] { "1", "2", "3" };
            #region 3-18 接口 人员信息查询
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            keyInfo = strUserName + "|" + strPassword + "|" + "754|CHN|01|" + strIDcode + "|" + strName + "||";
            conInfo = "";
            operType = "3";
            busType = "18";
            try
            {
                strRet = PublicFuction.GetDataByWebService(strWebAddr + "?WSDL", "transBusinessForCard", new string[] { keyInfo, conInfo, operType, busType }).ToString();// 3-18 人员信息查询
                WriteWorkLog.WriteLogs("日志", "接口3返回:", strRet);
            }
            catch (Exception ex)
            {
                WriteWorkLog.WriteLogs("日志", "接口异常", ex.ToString());
                return "-1$网络连接异常，请联系工作人员";
            }
            if (strRet == "")
            {
                WriteWorkLog.WriteLogs("日志", "接口异常", "strRet == 空");
                return "-1$网络连接异常，请联系工作人员";
            }
            if (strRet.Substring(0, 1) != "0")
            {
                errInfo = strRet.Split('$');
                WriteWorkLog.WriteLogs("日志", "接口调用返回非0", errInfo[2]);
                return "-1$" + errInfo[2];
            }
            strRet = strRet.Substring(1, strRet.Length - 1);
            strRet = strRet.Replace("$", "");
            sArray = strRet.Split('|');
            string strPersonCode = sArray[6];
            string strDealPhotoPath = DealAppConfig.GetAppSettingsValue("照片处理路径") + "\\";
            string strBase64 = ImgToBase64String(AppDomain.CurrentDomain.BaseDirectory + strIDcode + "_upLoad.jpg");
            string strCityCode = strUnitCode.Substring(0, 6);
            #endregion

            #region 1-19接口 数据采集上传
            keyInfo = "||754|CHN|01|" + strIDcode + "|" + strName + "||" + strPersonCode;
            conInfo = "AAC163||CHN|&AAC058||01|&AAE135||" + strIDcode + "|&AAC003||" + strName + "|&BAZ805|||&AAC059|||&AAB200||" + strCityCode + "|&BAC025|||&BAC026|||&BAZ838||" + strBankCode + "|&BAE017|||&AAE005|||&AAC011|||&AAE006|||&AAB001|||&AAB004|||&AAB003||0|&AAB005|||&BAZ802||" + strUnitCode + "|&BAC200||" + strBase64 + "|&BAZ815||0|&AAE011||" + strOperator + "|&BAZ840||3|&BAB309|||&BAB310|||&AAE020|||&BAB311|||&";
            operType = "1";
            busType = "19";
            try
            {
                strRet = PublicFuction.GetDataByWebService(strWebAddr + "?WSDL", "transBusinessForCard", new string[] { keyInfo, conInfo, operType, busType }).ToString();//  人员信息查询
                WriteWorkLog.WriteLogs("日志", "接口" + operType + "-" + busType + "返回:", strRet);
            }
            catch (Exception ex)
            {
                WriteWorkLog.WriteLogs("日志", "接口异常", ex.ToString());
                return "-1$网络连接异常，请联系工作人员";
            }
            if (strRet.Substring(0, 1) != "0")
            {
                WriteWorkLog.WriteLogs("日志", "接口" + operType + "-" + busType + "keyinfo:", keyInfo);
                WriteWorkLog.WriteLogs("日志", "接口" + operType + "-" + busType + "coninfo:", conInfo);         //用户名|密码|验证码|国家或地区|证件类型|证件号码|姓名|卡识别码|省人员识别号
                errInfo = strRet.Split('$');
                WriteWorkLog.WriteLogs("日志", "119接口调用返回非0", errInfo[2]);
                return "-1$网络连接异常，请联系工作人员";
            }
            #endregion

            #region 2-51接口 数据采集上传
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////

            keyInfo = strUserName + "|" + strPassword + "|" + "836|CHN|01|" + strIDcode + "|" + strName + "||" + strPersonCode;
            conInfo = "AAC163||CHN|&AAC058||01|&AAE135||" + strIDcode.Trim() + "|&AAC003||" + strName.Trim() + "|&BAZ805|||&AAC059|||&AAB200||" + strCityCode + "|&BAC025|||&BAC026|||&BAZ838||" + strBankCode + "|&BAE017|||&AAE005|||&AAC011|||&AAE006|||&AAB001|||&AAB004|||&AAB003||0|&AAB005|||&BAZ802||" + strUnitCode + "|&BAC200||" + strBase64 + "|&BAZ815||1|&AAE011||" + strOperator + "|&BAZ840||3|&BAB309|||&BAB310|||&AAE020|||&BAB311|||&";
            operType = "2";
            busType = "51";
            try
            {
                strRet = PublicFuction.GetDataByWebService(strWebAddr + "?WSDL", "transBusinessForCard", new string[] { keyInfo, conInfo, operType, busType }).ToString();// 3-18 人员信息查询
            }
            catch (Exception ex)
            {
                WriteWorkLog.WriteLogs("日志", "异常", "接口2-51:" + ex.ToString());
                return "-1$网络连接异常，请联系工作人员";
            }
            if (strRet.Substring(0, 1) != "0")
            {
                errInfo = strRet.Split('$');
                WriteWorkLog.WriteLogs("日志", "错误", "接口2-51调用返回非0" + errInfo[2]);
                return "-1$" + errInfo[2];
            }
            else
            {
                strRet = strRet.Substring(1, strRet.Length - 1);
                strRet = strRet.Substring(1, strRet.Length - 1);
                strBatch = strRet.Replace("$", "");
                WriteWorkLog.WriteLogs("日志", "信息", "人员办卡采集上传成功");
            }
            #endregion
            #region 获取最大序号
            int intRecordNo = -1;
            OleDbConnection conn = new OleDbConnection(AccessWebInterFace.strConn);
            string strSql = "select MAX(序号) from E_人员信息";
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                WriteWorkLog.WriteLogs("日志", "异常", " conn.Open:" + ex.ToString());
                conn.Close();
                return "-1$本地数据库打开失败";
            }
            finally
            {
                conn.Close();
            }
            OleDbDataAdapter da = new OleDbDataAdapter(strSql, conn);
            DataSet ds = new DataSet();
            da.Fill(ds);
            string strRecodeNo = ds.Tables[0].Rows[0][0].ToString();
            int.TryParse(ds.Tables[0].Rows[0][0].ToString(), out intRecordNo);
            #endregion
            return Interface3_22(strBatch, intRecordNo);
            return "0";
        }
        public static string ImgToBase64String(string Imagefilename)
        {
            try
            {
                Bitmap bmp = new Bitmap(Imagefilename);
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                bmp.Dispose();
                return Convert.ToBase64String(arr);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
