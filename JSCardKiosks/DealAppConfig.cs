using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using WorkLog;

namespace JSCardKiosks
{
    class DealAppConfig
    {
        #region 更新配置文件
        /// <summary>
                /// 更新配置文件
                /// </summary>
                /// <param name="Xvalue">节点值</param>
                /// <param name="NodeName">节点名</param>
        public static void SetValue(string AppKey, string AppValue, string path)
        {
            System.Xml.XmlDocument xDoc = new System.Xml.XmlDocument();
            xDoc.Load(path);
            System.Xml.XmlNode xNode;
            System.Xml.XmlElement xElem1;
            System.Xml.XmlElement xElem2;
            xNode = xDoc.SelectSingleNode("//appSettings");

            xElem1 = (System.Xml.XmlElement)xNode.SelectSingleNode("//add[@key='" + AppKey + "']");
            if (xElem1 != null) xElem1.SetAttribute("value", AppValue);
            else
            {
                xElem2 = xDoc.CreateElement("add");
                xElem2.SetAttribute("key", AppKey);
                xElem2.SetAttribute("value", AppValue);
                xNode.AppendChild(xElem2);
            }
            xDoc.Save(path);
        }
        #endregion
        #region 获取配置文件
        /// <summary>
                /// 更新配置文件
                /// </summary>
                /// <param name="Xvalue">节点值</param>
                /// <param name="NodeName">节点名</param>
        public static string GetValue(string AppKey, string path)
        {
            System.Xml.XmlDocument xDoc = new System.Xml.XmlDocument();
            xDoc.Load(path);
            System.Xml.XmlNode xNode;
            System.Xml.XmlElement xElem1;
            xNode = xDoc.SelectSingleNode("//appSettings");
            xElem1 = (System.Xml.XmlElement)xNode.SelectSingleNode("//add[@key='" + AppKey + "']");
            if (xElem1 != null)
            {
                return xElem1.GetAttribute("value");
            }
            else
            {
                return "";
            }
        }
        #endregion
        public static string GetAppSettingsValue(string key)
        {
            ConfigurationManager.RefreshSection("appSettings");
            return ConfigurationManager.AppSettings[key] ?? string.Empty;
        }
        public static bool UpdateAppSettings(string key, string value)
        {
            var _config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (!_config.HasFile)
            {
                throw new ArgumentException("程序配置文件缺失！");
            }
            KeyValueConfigurationElement _key = _config.AppSettings.Settings[key];
            if (_key == null)
                _config.AppSettings.Settings.Add(key, value);
            else
                _config.AppSettings.Settings[key].Value = value;
            _config.Save(ConfigurationSaveMode.Modified);
            return true;
        }
        public static List<string> ReadTxts(string path)
        {

            List<string> readTxtList = new List<string>();
            if (!string.IsNullOrEmpty(path))
            {
                if (!File.Exists(path))
                {
                    WriteWorkLog.WriteLogs("日志", "err", "文件不存在");
                }
                else
                {
                    try
                    {
                        // 创建一个 StreamReader 的实例来读取文件 
                        // using 语句也能关闭 StreamReader
                        using (StreamReader sr = new StreamReader(path, Encoding.GetEncoding("gb2312")))
                        {
                            string line;
                            while ((line = sr.ReadLine()) != null)
                            {
                                readTxtList.Add(line);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        // 向用户显示出错消息
                        WriteWorkLog.WriteLogs("日志", "excption", "读取配置文件失败:" + e.Message);
                    }
                }
            }
            return readTxtList;
        }
    }

}
