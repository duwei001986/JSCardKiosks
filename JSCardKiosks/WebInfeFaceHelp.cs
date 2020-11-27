using com.neusoft.golf.piles.bcp.sdk.util;

using JSCardKiosks;
using MyThirdCard;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PostMessageHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WorkLog;
using Type = System.Type;

namespace HttpPostTest
{
    class WebInfeFaceHelp
    {
        public const int STATUS_CARD_NORMAL = 0;
        public const int STATUS_CARD_REPORTPASS = 1;
        public const int STATUS_CARD_NEWDATA = 2;
        public const int STATUS_CARD_NODATA = 3;
        public static string postIP = DealAppConfig.GetAppSettingsValue("数据下载IP");
        public static string deviceID = DealAppConfig.GetAppSettingsValue("设备ID");
        public static string clientID = DealAppConfig.GetAppSettingsValue("消费端ID");
        public static string clientKey = DealAppConfig.GetAppSettingsValue("消费端密钥");
        public static string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Application.StartupPath + "\\jiangsu_database.mdb";

        private static string CompositonText(object dataJson, string apiCode)
        {
            string strTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            string strRet = "";
            var headObject = new
            {
                serviceCode = "301-0001",
                version = "1.0",
                sourceCode = "301-10001",
                apiCode = apiCode,
                timestamp = strTime,
                signature = ""
            };
            var bodyObject = new
            {
                equipmentID = deviceID,// "6000002068395846",//" 
                equipmentPwd = clientKey,//"8000001601909156",
                data = dataJson
            };
            var allObject = new
            {
                header = headObject,
                body = bodyObject
            };
            strRet = JsonConvert.SerializeObject(allObject);
            return strRet;
        }
        private static string jwtSignFun(string apicode)
        {
            ClientUtil clientUtil = ClientUtil.getSingleton();
            return clientUtil.jwtSign(Client.builder().apiId(apicode)
                    .clientId(clientID).timestamp()
                    .build(), clientKey);
        }
        public static int StatusCheck(CardBaseData cardBaseData)
        {
            string strReceiveJson = "";
            string apicode = GetAPIid("450-0005");
            var dataObject = new
            {
                aac003 = cardBaseData.Aac003,
                aac147 = cardBaseData.Aac147,
                baz805 = ""
            };
            string strPostJson = CompositonText(dataObject, "450-0005");
            WorkLog.WriteWorkLog.WriteDebugLogs("日志", "信息", "StatusCheckInput:" + strPostJson);
            string jwtSign = jwtSignFun(apicode);
            string posturl = postIP + "?bcpJwtAuth=" + jwtSign;
            strReceiveJson = HttpPostMessage.PostWebRequest(posturl, strPostJson, null);
            if (strReceiveJson == "")
                return -400;
            WorkLog.WriteWorkLog.WriteDebugLogs("日志", "信息", "StatusCheckOutput:" + strReceiveJson);
            JObject jo = (JObject)JsonConvert.DeserializeObject(strReceiveJson);
            JObject jk = JObject.Parse(jo["body"].ToString());
            string[] values = jk.Properties().Select(item => item.Value.ToString()).ToArray();
            string respMsgCode = values[0];
            string respMsg = values[1];
            if (respMsgCode == "999")
            {
                return STATUS_CARD_NODATA;
            }
            if (respMsgCode == "EH2601101" && respMsg == "未查询到数据")
            {
                return STATUS_CARD_NEWDATA;
            }
            if (jk["data"].ToString() != "")
            {
                JObject jkdata = JObject.Parse(jk["data"].ToString());
                cardBaseData.Aaz502 = jkdata["aaz502"].ToString();
                WorkLog.WriteWorkLog.WriteLogs("日志", "信息", "卡状态：" + cardBaseData.Aaz502);
                switch (cardBaseData.Aaz502)
                {
                    case "1":
                        return STATUS_CARD_NORMAL;
                    case "2":
                        return STATUS_CARD_REPORTPASS;
                    case "3":
                        return -1;
                    case "4":
                        return STATUS_CARD_NORMAL;
                    case "9":
                        return STATUS_CARD_NEWDATA;
                    default:
                        return -1;
                }
            }
            WorkLog.WriteWorkLog.WriteLogs("日志", "信息", "卡状态报文：" + strReceiveJson);
            return -1;
        }
        public static int BaseInfoSearch(CardBaseData cardBaseData, string cardType)
        {
            string apicode = GetAPIid("450-0001");
            var dataObject = new
            {
                aac003 = cardBaseData.Aac003,
                aac147 = cardBaseData.Aac147,
                baz805 = "",
                aaz411 = cardType
            };
            string strPostJson = CompositonText(dataObject, "450-0001");
            WorkLog.WriteWorkLog.WriteDebugLogs("日志", "信息", "StatusCheckInput:" + strPostJson);
            string jwtSign = jwtSignFun(apicode);
            string posturl = postIP + "?bcpJwtAuth=" + jwtSign;
            string strReceiveJson = HttpPostMessage.PostWebRequest(posturl, strPostJson, null);
            if (strReceiveJson == "")
                return -400;
            WorkLog.WriteWorkLog.WriteDebugLogs("日志", "信息", "StatusCheckOutput:" + strReceiveJson);
            JObject jo = (JObject)JsonConvert.DeserializeObject(strReceiveJson);
            JObject jk = JObject.Parse(jo["body"].ToString());
            string[] values = jk.Properties().Select(item => item.Value.ToString()).ToArray();
            string respMsgCode = values[0];
            string respMsg = values[1];
            if (respMsgCode == "EH2601103")//已制卡或制卡中
            {
                return 2;
            }
            if (respMsgCode == "999")//已制卡或制卡中
            {
                return 2;
            }
            if (jk["data"].ToString() != "")
            {
                JObject jkdata = JObject.Parse(jk["data"].ToString());

                string checkresult = jkdata["checkresult"].ToString();
                if (checkresult == "1")
                {
                    cardBaseData.Baz805 = jkdata["baz805"].ToString();
                    cardBaseData.Aac058 = jkdata["aac058"].ToString();
                    cardBaseData.Aac004 = jkdata["aac004"].ToString();
                    cardBaseData.Aac161 = jkdata["aac161"].ToString();
                    cardBaseData.Aae005 = jkdata["aae005"].ToString();
                    cardBaseData.Aae006 = jkdata["aae006"].ToString();
                    string checkedFlag = jkdata["checkedFlag"].ToString();
                    cardBaseData.Image = jkdata["image"].ToString();
                    cardBaseData.Aaz502 = jkdata["aaz502"].ToString();
                    return 0;
                }
                else
                    return -1;
            }
            else
            {
                if (respMsgCode == "00")
                    return 2;
            }
            WorkLog.WriteWorkLog.WriteLogs("日志", "信息", "制卡前验证报文：" + strReceiveJson);
            return -1;
        }
        public static int KioskBatchSearch(CardBaseData cardBaseData, out string strErr)
        {
            strErr = "";
            string apicode = GetAPIid("450-0003");
            var dataObject = new
            {
                aac003 = cardBaseData.Aac003,
                aac147 = cardBaseData.Aac147,
                baz805 = ""
            };
            string strPostJson = CompositonText(dataObject, "450-0003");
            WorkLog.WriteWorkLog.WriteDebugLogs("日志", "信息", "450-0003:" + strPostJson);
            string jwtSign = jwtSignFun(apicode);
            string posturl = postIP + "?bcpJwtAuth=" + jwtSign;
            string strReceiveJson = HttpPostMessage.PostWebRequest(posturl, strPostJson, null);
            if (strReceiveJson == "")
                return -400;
            WorkLog.WriteWorkLog.WriteDebugLogs("日志", "信息", "450-0003:" + strReceiveJson);
            JObject jo = (JObject)JsonConvert.DeserializeObject(strReceiveJson);
            JObject jk = JObject.Parse(jo["body"].ToString());
            if (jk.ContainsKey("errorCode"))
            {
                if (jk["errorCode"].ToString() == "00")
                {
                    JObject jkdata = JObject.Parse(jk["data"].ToString());
                    cardBaseData.Baz033 = jkdata["baz033"].ToString();
                    return 0;
                }
                else
                {
                    strErr = jk["errorMsg"].ToString();
                    WorkLog.WriteWorkLog.WriteLogs("日志", "错误", "450-0002-errorMsg:" + strErr);
                    return -1;
                }
            }
            else
            {
                strErr = "申请新制卡异常";
                WorkLog.WriteWorkLog.WriteLogs("日志", "信息", "450-0002:" + strReceiveJson);
                return -1;
            }
            // return HttpPostMessage.PostWebRequest(posturl, strPostJson, null);
        }
        public static int CardFirstRequest(CardBaseData cardBaseData, out string strErr)
        {
            strErr = "";
            string apicode = GetAPIid("450-0002");
            var dataObject = new
            {
                aac003 = cardBaseData.Aac003.ToString(),
                aac147 = cardBaseData.Aac147.ToString(),
                baz805 = cardBaseData.Baz805.ToString(), //省人员识别号
                aac058 = cardBaseData.Aac058.ToString(), //证件类型
                aac004 = cardBaseData.Aac004.ToString(), //性别
                aac161 = cardBaseData.Aac161.ToString(), //国家或地区
                aae005 = cardBaseData.Aae005.ToString(), //移动电话
                aae006 = cardBaseData.Aae006.ToString(), //联系地址
                aac059 = cardBaseData.Aac059.ToString(), //证件有效期截止日期
                bac025 = cardBaseData.Bac025, //职业
                aae008 = cardBaseData.Aae008.ToString(), //银行类别
                checkedFlag = "1",// 核准标识 是   备用 1 已核准
                image = cardBaseData.Image//照片
            };
            string strPostJson = CompositonText(dataObject, "450-0002");
            WorkLog.WriteWorkLog.WriteDebugLogs("日志", "信息", "450-0002:" + strPostJson);
            string jwtSign = jwtSignFun(apicode);
            string posturl = postIP + "?bcpJwtAuth=" + jwtSign;
            string strReceiveJson = HttpPostMessage.PostWebRequest(posturl, strPostJson, null);
            if (strReceiveJson == "")
                return -400;
            WorkLog.WriteWorkLog.WriteDebugLogs("日志", "信息", "450-0002:" + strReceiveJson);
            JObject jo = (JObject)JsonConvert.DeserializeObject(strReceiveJson);
            JObject jk = JObject.Parse(jo["body"].ToString());
            if (jk.ContainsKey("errorCode"))//jk.ContainsKey("baz033")
            {
                if (jk["errorCode"].ToString() == "00")
                {
                    JObject jkdata = JObject.Parse(jk["data"].ToString());
                    cardBaseData.Baz033 = jkdata["baz033"].ToString();
                    return 0;
                }
                else
                {
                    strErr = jk["errorMsg"].ToString();
                    WorkLog.WriteWorkLog.WriteLogs("日志", "错误", "450-0002-errorMsg:" + strErr);
                    return -1;
                }
            }
            else
            {
                strErr = "申请新制卡异常";
                WorkLog.WriteWorkLog.WriteLogs("日志", "信息", "450-0002:" + strReceiveJson);
                return -1;
            }
        }
        public static int CardMakeUp(CardBaseData cardBaseData, out string strErr)
        {
            strErr = "";
            string apicode = GetAPIid("450-0006");
            var dataObject = new
            {
                aac003 = cardBaseData.Aac003,
                aac147 = cardBaseData.Aac147,
                baz805 = "",
                aac059 = cardBaseData.Aac059,
                bac025 = cardBaseData.Bac025,
                aae008 = cardBaseData.Aae008
            };
            string strPostJson = CompositonText(dataObject, "450-0006");
            WorkLog.WriteWorkLog.WriteDebugLogs("日志", "信息", "450-0006:" + strPostJson);
            string jwtSign = jwtSignFun(apicode);
            string posturl = postIP + "?bcpJwtAuth=" + jwtSign;
            string strReceiveJson = HttpPostMessage.PostWebRequest(posturl, strPostJson, null);
            if (strReceiveJson == "")
                return -400;
            WorkLog.WriteWorkLog.WriteDebugLogs("日志", "信息", "450-0006:" + strReceiveJson);
            JObject jo = (JObject)JsonConvert.DeserializeObject(strReceiveJson);
            JObject jk = JObject.Parse(jo["body"].ToString());
            if (jk.ContainsKey("errorCode"))//jk.ContainsKey("baz033")
            {
                if (jk["errorCode"].ToString() == "00")
                {
                    JObject jkdata = JObject.Parse(jk["data"].ToString());
                    cardBaseData.Baz033 = jkdata["baz033"].ToString();
                    return 0;
                }
                else
                {
                    strErr = jk["errorMsg"].ToString();
                    WorkLog.WriteWorkLog.WriteLogs("日志", "错误", "450-0002-errorMsg:" + strErr);
                    return -1;
                }
            }
            else
            {
                strErr = "申请新制卡异常";
                WorkLog.WriteWorkLog.WriteLogs("日志", "信息", "450-0002:" + strReceiveJson);
                return -1;
            }
        }
        public static string GetBankName(string bankCode)
        {
            Hashtable ht = new Hashtable(); //创建一个Hashtable实例
            ht.Add("95588", "工商银行");
            ht.Add("95599", "农业银行");
            ht.Add("95566", "中国银行");
            ht.Add("95533", "建设银行");
            ht.Add("95559", "交通银行");
            ht.Add("95580", "邮储银行");
            ht.Add("96008", "省农信社");
            ht.Add("95555", "招商银行");
            ht.Add("95319", "江苏银行");
            ht.Add("95302", "南京银行");
            ht.Add("96067", "苏州银行");
            return (string)ht[bankCode];
        }
        public static string GetAPIid(string APICode)
        {
            Hashtable ht = new Hashtable(); //创建一个Hashtable实例
            ht.Add("450-0001", "ff8080817452d901017472b664e0002a");
            ht.Add("450-0002", "ff8080817452d901017495def9fb0035");
            ht.Add("450-0003", "ff8080817452d901017495e80acf004c");
            ht.Add("450-0004", "ff8080817452d901017495eb79210054");
            ht.Add("450-0005", "ff8080817452d90101749ec7b4b400b4");
            ht.Add("450-0006", "ff8080817452d9010174962e1d3f006d");
            ht.Add("450-0008", "ff8080817452d9010174962f33310071");
            ht.Add("450-0101", "ff8080817452d9010174963001a70075");
            ht.Add("450-0102", "ff8080817452d901017496313c4e0079");
            ht.Add("450-0103", "ff8080817452d901017496323556007d");
            ht.Add("450-0201", "ff8080817452d9010174963336fa0081");
            ht.Add("450-0011", "8acfb0b2752263bc0175598bf071008b");
            return (string)ht[APICode];
        }
        public static string GetJobCode(string Job)
        {
            Hashtable ht = new Hashtable(); //创建一个Hashtable实例
            ht.Add("公务员", "11");
            ht.Add("事业单位管理人员", "12");
            ht.Add("事业单位技术人员", "13");
            ht.Add("公务员(警察)", "15");
            ht.Add("企业管理人员", "21");
            ht.Add("企业技术人员", "22");
            ht.Add("企业员工", "23");
            ht.Add("个体工商户及雇员", "31");
            ht.Add("自由职业者", "41");
            ht.Add("农林牧渔业生产及辅助人员", "51");
            ht.Add("军人", "61");
            ht.Add("离退休人员", "71");
            ht.Add("城乡居民", "81");
            ht.Add("学生", "91");
            ht.Add("儿童", "92");
            return (string)ht[Job];
        }
        public static int MulCardDataDownload(CardBaseData cardBaseData, out string strErr)
        {

            strErr = "";
            string apicode = GetAPIid("450-0102");
            var dataObject = new
            {
                baz033 = cardBaseData.Baz033,
            };
            string strPostJson = CompositonText(dataObject, "450-0102");
            WorkLog.WriteWorkLog.WriteDebugLogs("日志", "信息", "450-0102:" + strPostJson);
            string jwtSign = jwtSignFun(apicode);
            string posturl = postIP + "?bcpJwtAuth=" + jwtSign;
            string strReceiveJson = HttpPostMessage.PostWebRequest(posturl, strPostJson, null);
            if (strReceiveJson == "")
                return -400;
            WorkLog.WriteWorkLog.WriteDebugLogs("日志", "信息", "450-0102:" + strReceiveJson);
            JObject jo = (JObject)JsonConvert.DeserializeObject(strReceiveJson);
            JObject jk = JObject.Parse(jo["body"].ToString());
            if (jk.ContainsKey("errorCode"))
            {
                JObject jkdata = JObject.Parse(jk["data"].ToString());
                string total = jkdata["total"].ToString();
                string userList = jkdata["list"].ToString();
                JArray jar = (JArray)JsonConvert.DeserializeObject(userList);
                if (jk["errorCode"].ToString() == "00")
                {
                    for (int i = 0; i < Convert.ToInt32(total); i++)
                    {
                        JObject jCell = JObject.Parse(jar[i].ToString());
                        List<string> strCardInfo = new List<string>();
                        string[] values = jCell.Properties().Select(item => item.Value.ToString()).ToArray();
                        for (int j = 0; j < 13; j++)
                        {
                            strCardInfo.Add(values[j]);
                        }
                    }
                    return 0;
                }
                else
                {
                    strErr = jk["errorMsg"].ToString();
                    WorkLog.WriteWorkLog.WriteLogs("日志", "错误", "450-0102-errorMsg:" + strErr);
                    return -1;
                }
            }
            else
            {
                strErr = "下载数据异常";
                WorkLog.WriteWorkLog.WriteLogs("日志", "信息", "450-0102:" + strReceiveJson);
                return -1;
            }
            //string jwtSign = jwtSignFun(apicode);
            //string posturl = postIP + "?bcpJwtAuth=" + jwtSign;
            //return HttpPostMessage.PostWebRequest(posturl, strPostJson, null);
        }
        public static int CardDataDownload(CardBaseData cardBaseData, out string strErr)
        {
            strErr = "";
            string apicode = GetAPIid("450-0102");
            var dataObject = new
            {
                baz033 = cardBaseData.Baz033,
            };
            string strPostJson = CompositonText(dataObject, "450-0102");
            WorkLog.WriteWorkLog.WriteDebugLogs("日志", "信息", "450-0102:" + strPostJson);
            string jwtSign = jwtSignFun(apicode);
            string posturl = postIP + "?bcpJwtAuth=" + jwtSign;
            string strReceiveJson = HttpPostMessage.PostWebRequest(posturl, strPostJson, null);
            if (strReceiveJson == "")
                return -400;
            WorkLog.WriteWorkLog.WriteDebugLogs("日志", "信息", "450-0102:" + strReceiveJson);
            JObject jo = (JObject)JsonConvert.DeserializeObject(strReceiveJson);
            JObject jk = JObject.Parse(jo["body"].ToString());
            if (jk.ContainsKey("errorCode"))
            {

                if (jk["errorCode"].ToString() == "00")
                {
                    JObject jkdata = JObject.Parse(jk["data"].ToString());
                    string total = jkdata["total"].ToString();
                    string userList = jkdata["list"].ToString();
                    JArray jar = (JArray)JsonConvert.DeserializeObject(userList);
                    JObject jCell = JObject.Parse(jar[0].ToString());
                    cardBaseData.Aac003 = jCell["aac003"].ToString();
                    cardBaseData.Aac147 = jCell["aac002"].ToString();
                    cardBaseData.Aab301 = jCell["aab301"].ToString();
                    cardBaseData.Baz030 = jCell["baz030"].ToString();
                    cardBaseData.Aab301 = jCell["aab301"].ToString();
                    cardBaseData.Aac002 = jCell["aac002"].ToString();
                    cardBaseData.Aac004 = jCell["aac004"].ToString();
                    cardBaseData.Baz103 = jCell["baz103"].ToString();
                    cardBaseData.Aac025 = jCell["aac025"].ToString();
                    cardBaseData.Baz805 = jCell["baz805"].ToString();
                    cardBaseData.Aac500 = jCell["aac500"].ToString();
                    cardBaseData.Aaz503 = jCell["aaz503"].ToString();
                    //cardBaseData.Image = jCell["image"].ToString();
                    cardBaseData.ImageCode = jCell["imageID"].ToString();
                    int iRet = WebInfeFaceHelp.GetPhotoBase64(cardBaseData.ImageCode, cardBaseData, out strErr);
                    if (iRet != 0)
                    {
                        return -1;
                    }
                    return 0;
                }
                else
                {
                    strErr = jk["errorMsg"].ToString();
                    WorkLog.WriteWorkLog.WriteLogs("日志", "错误", "450-0102-errorMsg:" + strErr);
                    return -1;
                }
            }
            else
            {
                WorkLog.WriteWorkLog.WriteLogs("日志", "信息", "450-0102:" + strReceiveJson);
                return -1;
            }
            //string jwtSign = jwtSignFun(apicode);
            //string posturl = postIP + "?bcpJwtAuth=" + jwtSign;
            //return HttpPostMessage.PostWebRequest(posturl, strPostJson, null);
        }
        public static int QRCodeCardRequest(string qrCode, CardBaseData cardBaseData, out string DeCode)
        {
            DeCode = "";
            string apicode = GetAPIid("450-0008");
            var dataObject = new
            {
                qrCodeID = qrCode
            };
            string strPostJson = CompositonText(dataObject, "450-0008");
            WorkLog.WriteWorkLog.WriteDebugLogs("日志", "信息", "450-0008:" + strPostJson);
            string jwtSign = jwtSignFun(apicode);
            string posturl = postIP + "?bcpJwtAuth=" + jwtSign;
            string strReceiveJson = HttpPostMessage.PostWebRequest(posturl, strPostJson, null);
            if (strReceiveJson == "")
                return -400;
            WorkLog.WriteWorkLog.WriteDebugLogs("日志", "信息", "450-0008:" + strReceiveJson);
            JObject jo = (JObject)JsonConvert.DeserializeObject(strReceiveJson);
            JObject jk = JObject.Parse(jo["body"].ToString());
            if (jk.ContainsKey("errorCode"))//jk.ContainsKey("baz033")
            {
                if (jk["errorCode"].ToString() == "00")
                {
                    JObject jkdata = JObject.Parse(jk["data"].ToString());
                    cardBaseData.Baz033 = jkdata["baz033"].ToString();
                    DeCode = cardBaseData.Baz033;
                    return 0;
                }
                else
                {
                    DeCode = jk["errorMsg"].ToString();
                    WorkLog.WriteWorkLog.WriteLogs("日志", "错误", "450-0008-errorMsg:" + DeCode);
                    return -1;
                }
            }
            else
            {
                DeCode = "二维码解码异常";
                WorkLog.WriteWorkLog.WriteLogs("日志", "信息", "450-0008:" + strReceiveJson);
                return -1;
            }
        }
        public static int CardDataReBack(CardBaseData cardBaseData, out string strErr)
        {
            strErr = "";
            string apicode = GetAPIid("450-0103");
            JObject Jlist = new JObject();
            JObject obj = new JObject();
            obj["baz030"] = cardBaseData.Baz030;
            obj["aaz501"] = cardBaseData.Aaz501;
            obj["baz805"] = cardBaseData.Baz805;

            JArray list = new JArray();
            list.Add(obj);
            Jlist["total"] = "1";
            Jlist["list"] = list;

            string strPostJson = CompositonText(Jlist, "450-0103");
            WorkLog.WriteWorkLog.WriteDebugLogs("日志", "信息", "450-0103:" + strPostJson);
            string jwtSign = jwtSignFun(apicode);
            string posturl = postIP + "?bcpJwtAuth=" + jwtSign;
            string strReceiveJson = HttpPostMessage.PostWebRequest(posturl, strPostJson, null);
            if (strReceiveJson == "")
                return -400;
            WorkLog.WriteWorkLog.WriteDebugLogs("日志", "信息", "450-0103:" + strReceiveJson);
            JObject jo = (JObject)JsonConvert.DeserializeObject(strReceiveJson);
            JObject jk = JObject.Parse(jo["body"].ToString());
            if (jk.ContainsKey("errorCode"))//jk.ContainsKey("baz033")
            {
                if (jk["errorCode"].ToString() == "00")
                {
                    return 0;
                }
                else
                {
                    strErr = jk["errorMsg"].ToString();
                    WorkLog.WriteWorkLog.WriteLogs("日志", "错误", "450-0103-errorMsg:" + strErr);
                    return -1;
                }
            }
            else
            {
                strErr = "回盘异常";
                WorkLog.WriteWorkLog.WriteLogs("日志", "信息", "450-0103:" + strReceiveJson);
                return -1;
            }
        }
        public static int GetCard(CardBaseData cardBaseData, out string strErr)
        {
            strErr = "";
            string apiCode = "450-0004";
            string apiID = GetAPIid(apiCode);
            var dataObject = new
            {
                aac003 = cardBaseData.Aac003,
                aac147 = cardBaseData.Aac147,
                baz805 = cardBaseData.Baz805,
                aaz501 = cardBaseData.Aaz501,
                qrCodeID = ""
            };
            string strPostJson = CompositonText(dataObject, apiCode);
            WorkLog.WriteWorkLog.WriteDebugLogs("日志", "信息", "apiCode:" + strPostJson);
            string jwtSign = jwtSignFun(apiID);
            string posturl = postIP + "?bcpJwtAuth=" + jwtSign;
            string strReceiveJson = HttpPostMessage.PostWebRequest(posturl, strPostJson, null);
            if (strReceiveJson == "")
                return -400;
            WorkLog.WriteWorkLog.WriteDebugLogs("日志", "信息", "450-0004:" + strReceiveJson);
            JObject jo = (JObject)JsonConvert.DeserializeObject(strReceiveJson);
            JObject jk = JObject.Parse(jo["body"].ToString());
            if (jk.ContainsKey("errorCode"))//jk.ContainsKey("baz033")
            {
                if (jk["errorCode"].ToString() == "00")
                {
                    return 0;
                }
                else
                {
                    strErr = jk["errorMsg"].ToString();
                    WorkLog.WriteWorkLog.WriteLogs("日志", "错误", "450-0004-errorMsg:" + strErr);
                    return -1;
                }
            }
            else
            {
                strErr = "领卡异常";
                WorkLog.WriteWorkLog.WriteLogs("日志", "信息", "450-0004:" + strReceiveJson);
                return -1;
            }
        }
        public static string CARegist(CardBaseData cardBaseData)
        {
            var dataObject = new
            {
                aac003 = cardBaseData.Aac003,
                aac500 = cardBaseData.Aac500,
                aac002 = cardBaseData.Aac002,
                qmgy = "",
                sf = "",
                aab301 = cardBaseData.Aab301
            };
            string strRecieveJson = CompositonText(dataObject, "450-0201");
            return strRecieveJson;
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
                    WorkLog.WriteWorkLog.WriteLogs("日志", "异常", " 本地数据库异常(UpdateFromWebData):" + exErr.ToString());
                    return "保存数据出错";
                }
            }
            return "0";
        }
        public static DataTable GetDownloadData()
        {
            DataTable dt = new DataTable("cardInfo");
            DataColumn dc1 = new DataColumn("网点制卡批次", Type.GetType("System.String"));
            DataColumn dc2 = new DataColumn("行政区划", Type.GetType("System.String"));
            DataColumn dc3 = new DataColumn("姓名", Type.GetType("System.String"));
            DataColumn dc4 = new DataColumn("社会保障号码", Type.GetType("System.String"));
            DataColumn dc5 = new DataColumn("性别", Type.GetType("System.String"));
            DataColumn dc6 = new DataColumn("民族", Type.GetType("System.String"));
            DataColumn dc7 = new DataColumn("出生日期", Type.GetType("System.String"));
            DataColumn dc8 = new DataColumn("姓名扩展", Type.GetType("System.String"));
            DataColumn dc9 = new DataColumn("出生地", Type.GetType("System.String"));
            DataColumn dc10 = new DataColumn("人员识别号", Type.GetType("System.String"));
            DataColumn dc11 = new DataColumn("9位卡号", Type.GetType("System.String"));
            DataColumn dc12 = new DataColumn("发卡日期", Type.GetType("System.String"));


            dt.Columns.Add(dc1);
            dt.Columns.Add(dc2);
            dt.Columns.Add(dc3);
            dt.Columns.Add(dc4);
            dt.Columns.Add(dc5);
            dt.Columns.Add(dc6);
            dt.Columns.Add(dc7);
            dt.Columns.Add(dc8);
            dt.Columns.Add(dc9);
            dt.Columns.Add(dc10);
            dt.Columns.Add(dc11);
            dt.Columns.Add(dc12);


            //以上代码完成了DataTable的构架，但是里面是没有任何数据的  
            for (int i = 0; i < 1; i++)
            {
                DataRow dr = dt.NewRow();
                dr["网点制卡批次"] = "娃娃";
                dr["行政区划"] = "娃娃";
                dr["姓名"] = "娃娃";
                dr["社会保障号码"] = "娃娃";
                dr["性别"] = "娃娃";
                dr["民族"] = "娃娃";
                dr["出生日期"] = "娃娃";
                dr["姓名扩展"] = "娃娃";
                dr["出生地"] = "娃娃";
                dr["人员识别号"] = "娃娃";
                dr["9位卡号"] = "娃娃";
                dr["发卡日期"] = "娃娃";
                dt.Rows.Add(dr);
            }
            return dt;
        }
        public static int GetMulBathc(CardBaseData cardBaseData, out List<string> strCardInfo, out string strErr)
        {
            strCardInfo = new List<string>();
            strErr = "";
            string apiCode = "450-0101";
            string apiID = GetAPIid(apiCode);
            var dataObject = new
            {
            };
            string strPostJson = CompositonText(dataObject, apiCode);
            WorkLog.WriteWorkLog.WriteDebugLogs("日志", "信息", "450-0101:" + strPostJson);
            string jwtSign = jwtSignFun(apiID);
            string posturl = postIP + "?bcpJwtAuth=" + jwtSign;
            string strReceiveJson = HttpPostMessage.PostWebRequest(posturl, strPostJson, null);
            if (strReceiveJson == "")
                return -400;
            JObject jo = (JObject)JsonConvert.DeserializeObject(strReceiveJson);
            JObject jk = JObject.Parse(jo["body"].ToString());
            if (jk.ContainsKey("errorCode"))
            {
                JObject jkdata = JObject.Parse(jk["data"].ToString());
                string total = jkdata["total"].ToString();
                string userList = jkdata["list"].ToString();
                JArray jar = (JArray)JsonConvert.DeserializeObject(userList);
                if (jk["errorCode"].ToString() == "00")
                {
                    for (int i = 0; i < Convert.ToInt32(total); i++)
                    {
                        JObject jCell = JObject.Parse(jar[i].ToString());
                        string cellInfo = "";
                        cellInfo = jCell["baz033"] + "|" + jCell["baz838"];
                        strCardInfo.Add(cellInfo);
                    }
                    return 0;
                }
                else
                {
                    strErr = jk["errorMsg"].ToString();
                    WorkLog.WriteWorkLog.WriteLogs("日志", "错误", "450-0102-errorMsg:" + strErr);
                    return -1;
                }
            }
            else
            {
                strErr = "下载数据异常";
                WorkLog.WriteWorkLog.WriteLogs("日志", "信息", "450-0102:" + strReceiveJson);
                return -1;
            }
        }
        public static int GetDataFromWeb(CardBaseData cardBaseData, out string strErr)
        {
            strErr = "";
            int iRet = StatusCheck(cardBaseData);
            switch (iRet)
            {
                case WebInfeFaceHelp.STATUS_CARD_NORMAL:
                    strErr = "您已有社保卡，如需补卡请先正式挂失";
                    return 1;
                case WebInfeFaceHelp.STATUS_CARD_REPORTPASS:
                    #region 补卡
                    iRet = BaseInfoSearch(cardBaseData, "2");
                    if (iRet == 0)
                    {
                        iRet = CardMakeUp(cardBaseData, out strErr);   //"可申请制卡";
                        if (iRet == 0)
                        {
                            iRet = WebInfeFaceHelp.CardDataDownload(cardBaseData, out strErr);//下载数据
                            return iRet;
                        }
                        if (iRet == -1)
                        {
                            return -1;
                        }
                    }
                    else
                    {
                        strErr = "不可申请制卡";
                        return -1;
                    }
                    #endregion
                    return 0;
                case WebInfeFaceHelp.STATUS_CARD_NEWDATA:
                    #region 新制卡
                    iRet = BaseInfoSearch(cardBaseData, "1");
                    if (iRet == 0)
                    {
                        if (cardBaseData.Bac025 == "" || cardBaseData.Bac025 == null)
                        {
                            return 1;
                        }
                        iRet = CardFirstRequest(cardBaseData, out strErr);   //"可申请制卡";
                        if (iRet == 0)
                        {
                            if (cardBaseData.Bac025 == "" || cardBaseData.Bac025 == null)
                            {
                                return 1;
                            }
                            iRet = WebInfeFaceHelp.CardDataDownload(cardBaseData, out strErr);//下载数据
                            return iRet;
                        }
                        if (iRet == -1)
                        {
                            return -1;
                        }
                    }
                    else if (iRet == 2)//查找批次做卡
                    {
                        iRet = KioskBatchSearch(cardBaseData, out strErr);
                        if (iRet == 0)
                        {
                            iRet = WebInfeFaceHelp.CardDataDownload(cardBaseData, out strErr);//下载数据
                            return iRet;
                        }
                        if (iRet == -1)
                        {
                            return -1;
                        }
                    }
                    else
                    {
                        strErr = "不可申请制卡";
                        return -1;
                    }
                    #endregion
                    return 0;
                case WebInfeFaceHelp.STATUS_CARD_NODATA:
                    strErr = "系统中查不到您的信息";
                    return -1;
                case -1:
                    strErr = "您的信息不符合制卡要求";
                    return -1;
                default:
                    return -1;
            }
        }
        public static int GetPhotoBase64(string imgeCode, CardBaseData cardBaseData, out string strErr)
        {
            strErr = "";
            if (imgeCode == "-1")
            {
                cardBaseData.Image = "/9j/4AAQSkZJRgABAQEAYABgAAD/2wBDAAIBAQIBAQICAgICAgICAwUDAwMDAwYEBAMFBwYHBwcGBwcICQsJCAgKCAcHCg0KCgsMDAwMBwkODw0MDgsMDAz/2wBDAQICAgMDAwYDAwYMCAcIDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAz/wAARCAB+AGYDASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwD9/KKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigD/9k=";
                strErr = "儿童没有照片";
                return 0;
            }
            string apicode = GetAPIid("450-0011");
            var dataObject = new
            {
                baz805 = cardBaseData.Baz805,
                imageID = imgeCode
            };
            string strPostJson = CompositonText(dataObject, "450-0011");
            WorkLog.WriteWorkLog.WriteDebugLogs("日志", "信息", "450-0011:" + strPostJson);
            string jwtSign = jwtSignFun(apicode);
            string posturl = postIP + "?bcpJwtAuth=" + jwtSign;
            string strReceiveJson = HttpPostMessage.PostWebRequest(posturl, strPostJson, null);
            if (strReceiveJson == "")
                return -400;
            WorkLog.WriteWorkLog.WriteDebugLogs("日志", "信息", "450-0011:" + strReceiveJson);
            JObject jo = (JObject)JsonConvert.DeserializeObject(strReceiveJson);
            JObject jk = JObject.Parse(jo["body"].ToString());
            if (jk.ContainsKey("errorCode"))//jk.ContainsKey("baz033")
            {
                if (jk["errorCode"].ToString() == "00")
                {
                    JObject jkdata = JObject.Parse(jk["data"].ToString());
                    cardBaseData.Image = jkdata["image"].ToString();
                    return 0;
                }
                else
                {
                    strErr = "照片获取失败：" + jk["errorMsg"].ToString();
                    WorkLog.WriteWorkLog.WriteLogs("日志", "错误", "450-0011-errorMsg:" + strErr);
                    // cardBaseData.Image = "/9j/4AAQSkZJRgABAQEAYABgAAD/2wBDAAIBAQIBAQICAgICAgICAwUDAwMDAwYEBAMFBwYHBwcGBwcICQsJCAgKCAcHCg0KCgsMDAwMBwkODw0MDgsMDAz/2wBDAQICAgMDAwYDAwYMCAcIDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAz/wAARCAB+AGYDASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwD9/KKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigD/9k=";
                    return -1;
                }
            }
            else
            {
                strErr = "照片获取异常";
                WorkLog.WriteWorkLog.WriteLogs("日志", "信息", "450-0011:" + strReceiveJson);
                //cardBaseData.Image = "/9j/4AAQSkZJRgABAQEAYABgAAD/2wBDAAIBAQIBAQICAgICAgICAwUDAwMDAwYEBAMFBwYHBwcGBwcICQsJCAgKCAcHCg0KCgsMDAwMBwkODw0MDgsMDAz/2wBDAQICAgMDAwYDAwYMCAcIDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAz/wAARCAB+AGYDASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwD9/KKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigAooooAKKKKACiiigD/9k=";
                return -1;
            }
        }
        public static int CARegist(CardBaseData cardBaseData, string publickey, out CARegisterInfo registerInfo, out string strErr)
        {
            strErr = "";
            registerInfo.glypin = "";
            registerInfo.jmmy = "";
            registerInfo.zkmy = "";
            registerInfo.jmzs = "";
            registerInfo.qmzs = "";
            // registerInfo = new CARegisterInfo();
            string apiCode = "450-0201";
            string apiID = GetAPIid(apiCode);
            var dataObject = new
            {
                aac003 = cardBaseData.Aac003,
                aac500 = cardBaseData.Aac500,
                aac002 = cardBaseData.Aac002,
                qmgy = publickey,
                sf = "SM2",
                aab301 = "320100"
            };
            string strPostJson = CompositonText(dataObject, apiCode);
            WriteWorkLog.WriteDebugLogs("日志", "信息", apiCode + ":" + strPostJson);
            string jwtSign = jwtSignFun(apiID);
            string posturl = postIP + "?bcpJwtAuth=" + jwtSign;
            string strReceiveJson = HttpPostMessage.PostWebRequest(posturl, strPostJson, null);
            if (strReceiveJson == "")
            {
                strErr = "网络连接失败";
                return -400;
            }
            WriteWorkLog.WriteDebugLogs("日志", "信息", apiCode + ":" + strReceiveJson);
            JObject jo = (JObject)JsonConvert.DeserializeObject(strReceiveJson);
            JObject jk = JObject.Parse(jo["body"].ToString());
            if (jk.ContainsKey("errorCode"))
            {
                if (jk["errorCode"].ToString() == "00")
                {
                    JObject jkdata = JObject.Parse(jk["data"].ToString());
                    string strBase = jkdata["qmzs"].ToString();
                    if (strBase == "")
                    {
                        strErr = "注册CA失败";
                        return -1;
                    }
                    byte[] decodedByteArray = Convert.FromBase64String(strBase);
                    registerInfo.qmzs = CardWriteReadThirdT10.ByteToString(decodedByteArray);
                    strBase = jkdata["jmzs"].ToString();
                    decodedByteArray = Convert.FromBase64String(strBase);
                    registerInfo.jmzs = CardWriteReadThirdT10.ByteToString(decodedByteArray);

                    strBase = jkdata["jmmy"].ToString();
                    decodedByteArray = Convert.FromBase64String(strBase);
                    registerInfo.jmmy = CardWriteReadThirdT10.ByteToString(decodedByteArray);
                    registerInfo.zkmy = jkdata["zkmy"].ToString();
                    registerInfo.glypin = jkdata["glypin"].ToString();
                    return 0;
                }
                else
                {
                    strErr = jk["errorMsg"].ToString();
                    WriteWorkLog.WriteLogs("日志", "错误", apiCode + ":" + strErr);
                    return -1;
                }
            }
            else
            {
                strErr = "CA注册异常";
                WriteWorkLog.WriteLogs("日志", "信息", apiCode + ":" + strReceiveJson);
                return -1;
            }
        }

        public static int ReportCardLoss(CardBaseData cardBaseData, out string strErr)
        {
            strErr = "";
            string apiCode = "450-0009";
            string apiID = GetAPIid(apiCode);
            var dataObject = new
            {
                aac003 = cardBaseData.Aac003,
                aac147 = cardBaseData.Aac147,
                baz805 = cardBaseData.Baz805,
            };
            string strPostJson = CompositonText(dataObject, apiCode);
            WorkLog.WriteWorkLog.WriteDebugLogs("日志", "信息", "apiCode:" + strPostJson);
            string jwtSign = jwtSignFun(apiID);
            string posturl = postIP + "?bcpJwtAuth=" + jwtSign;
            string strReceiveJson = HttpPostMessage.PostWebRequest(posturl, strPostJson, null);
            if (strReceiveJson == "")
            {
                strErr = "网络不通";
                return -400;
            }
            WorkLog.WriteWorkLog.WriteDebugLogs("日志", "信息", "450-0009:" + strReceiveJson);
            JObject jo = (JObject)JsonConvert.DeserializeObject(strReceiveJson);
            JObject jk = JObject.Parse(jo["body"].ToString());
            if (jk.ContainsKey("errorCode"))//jk.ContainsKey("baz033")
            {
                if (jk["errorCode"].ToString() == "00")
                {
                    return 0;
                }
                else
                {
                    strErr = jk["errorMsg"].ToString();
                    WorkLog.WriteWorkLog.WriteLogs("日志", "错误", "450-0009-errorMsg:" + strErr);
                    return -1;
                }
            }
            else
            {
                strErr = "挂失失败";
                WorkLog.WriteWorkLog.WriteLogs("日志", "信息", "450-0009:" + strReceiveJson);
                return -1;
            }
        }
    }

}
