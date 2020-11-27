using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkLog;

namespace JSCardKiosks
{
    class Access_SQLHelper
    {
        public static string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + AppDomain.CurrentDomain.BaseDirectory + "jiangsu_database.mdb";
        public static bool Logon(string password)
        {
            DataTable dt = new DataTable();
            OleDbConnection conn = new OleDbConnection(strConn);
            string sql = "select * from lg_登陆";//编写SQL查询语句
            try
            {
                conn.Open();
                OleDbCommand cmd = new OleDbCommand(sql, conn);
                OleDbDataReader reader = cmd.ExecuteReader();
                bool falg = false;//标记用户名密码是否正确
                while (reader.Read())
                {
                   // string username = reader["UserName"].ToString().Trim();//取出当前记录姓名
                    string pwd = reader["UserPwd"].ToString().Trim();//取出当前记录密码
                    if (pwd == password)//判断用户名密码是否正确//name == username && 
                    {
                        falg = true;//输入用户名密码正确
                        break;
                    }
                }
                if (falg)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                WriteWorkLog.WriteLogs("日志", "异常", "ReadData:" + ex.Message);
                return false;
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
