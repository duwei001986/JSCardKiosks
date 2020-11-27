using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using WorkLog;

namespace PostMessageHelper
{
    class HttpPostMessage
    {
        static string strKey ="737A697C7F1F408BAFA2D0E0426698F2";
        public HttpPostMessage()
        {
        }
        #region 验证证书
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        #endregion
        #region Post方法
        public static string Post(string url, string dataStr)
        {
            System.GC.Collect();//垃圾回收，回收没有正常关闭的http连接
            string retStr = "";//数据返回
            HttpWebRequest request = null;
            HttpWebResponse response = null;

            byte[] data = Encoding.GetEncoding("utf-8").GetBytes(dataStr);

            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateServerCertificate);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "POST";
            request.ContentType = "application/json;charset=utf-8";
            request.ContentLength = data.Length;
            request.Proxy = null;
            //接收的数据格式
            request.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
            //request.Timeout = 10000;
            try
            {
                Stream sm = request.GetRequestStream();
                sm.Write(data, 0, data.Length);
                sm.Close();

                response = (HttpWebResponse)request.GetResponse();

                using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")))
                {
                    retStr = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return retStr;
        }
        #endregion
        #region MD5加密
        public static string GetMD5Hash(string value)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] hashByte = md5.ComputeHash(Encoding.Default.GetBytes(value));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashByte)
            {
                sb.Append(b.ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }
        #endregion
        #region 签名计算
        public static string GeneralSearchSign(Hashtable ht, string strKey)
        {
            string stringA = "";
            ArrayList akeys = new ArrayList(ht.Keys);
            akeys.Sort();
            foreach (string skey in akeys)
            {
                stringA += $"{skey}={ht[skey]}&";
            }
            stringA = stringA.Substring(0, stringA.Length - 1);
            string strSignTemp = stringA + "&key=" + strKey;
            return GetMD5Hash(strSignTemp).ToUpper();
        }
        #endregion
        #region 用Post发送报文
        public static void PostMessage(string URL, Hashtable ht,  out string ReceiveInfo)
        {
            ReceiveInfo = "";
            string sign = HttpPostMessage.GeneralSearchSign(ht, strKey);
             WriteWorkLog.WriteLogs("日志", "sign：",sign);
            string strPostText = "";
            foreach (DictionaryEntry de in ht) //ht为一个Hashtable实例
            {
                string key = de.Key.ToString();
                string value = de.Value.ToString();
                strPostText += "\"" + key + "\":\"" + value + "\",";
            }
            strPostText += "\"sign\":" + "\"" + sign + "\"";
            WriteWorkLog.WriteLogs("日志", "strPostText：",strPostText);
            ReceiveInfo = Post(URL, strPostText);
        }
        #endregion
        #region Post方法
        public static string Post2(string url, string dataStr)
        {
            System.GC.Collect();//垃圾回收，回收没有正常关闭的http连接
            string retStr = "";//数据返回
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            byte[] data = Encoding.GetEncoding("utf-8").GetBytes(dataStr);

            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateServerCertificate);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "POST";
            request.ContentType = "application/json;charset=utf-8";
            request.ContentLength = data.Length;
            request.Proxy = null;

            //接收的数据格式
            request.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
             request.Timeout = 5000;
            try
            {
                Stream sm = request.GetRequestStream();
                sm.Write(data, 0, data.Length);
                sm.Close();

                response = (HttpWebResponse)request.GetResponse();

                using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")))
                {
                    retStr = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                retStr = "";
                return retStr;
            }
            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();

                }

            }
            return retStr;
        }
        #endregion
        public static string PostWebRequest(string postUrl, string paramData, Encoding dataEncode, string header = "")
        {
            string ret = string.Empty;
            try
            {
                if (dataEncode == null)
                    dataEncode = Encoding.UTF8;

                byte[] byteArray = dataEncode.GetBytes(paramData); //转化
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
                webReq.Method = "POST";
                webReq.ContentType = "application/json";

                webReq.Headers.Add("Session-Key", header);

                webReq.ContentLength = byteArray.Length;
                Stream newStream = webReq.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);//写入参数
                newStream.Close();
                HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
                var c = response.GetResponseStream();
                StreamReader sr = new StreamReader(c, Encoding.UTF8);

                Char[] read = new Char[4000];
                int count = sr.Read(read, 0, read.Length);

                while (count > 0)
                {
                    String str = new String(read, 0, count);
                    ret += str;

                    count = sr.Read(read, 0, read.Length);
                }

                sr.Close();
                response.Close();
                newStream.Close();

            }
            catch (Exception ex)
            {
                return "";
            }
            return ret;
        }
    }
}
