using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using WorkLog;

namespace OledbConn
{
    public class OledbSqlOpration
    {
        public static string connstring = "Provider = Microsoft.Jet.OLEDB.4.0; Data Source = jiangsu_database.mdb";
        public static DataTable ReadData(string sql)
        {
            DataTable dt = new DataTable();
            OleDbConnection conn = new OleDbConnection(connstring);
            try
            {
                conn.Open();
                OleDbDataAdapter da = new OleDbDataAdapter(sql, conn);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                WriteWorkLog.WriteLogs("exception", "ReadData:" , ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return dt;
        }

      
        public static int GetMaxID()
        {
            #region 获取最大序号
            int intRecordNo = 0;
            OleDbConnection conn = new OleDbConnection(connstring);
            string strSql = "select MAX(序号) from E_人员信息";
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                WriteWorkLog.WriteLogs("日志", "异常", " conn.Open:" + ex.ToString());
                conn.Close();
                return -1;
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
            return intRecordNo;
        }
        public static int GetDataTable(string sql, out DataTable dt)
        {
            dt = new DataTable();
            OleDbConnection conn = new OleDbConnection(connstring);
            try
            {
                conn.Open();
            }
            catch (Exception exErr)
            {
                WriteWorkLog.WriteLogs("日志", "本地数据库异常", exErr.ToString());
                return -1;
            }
            OleDbDataAdapter dp = new OleDbDataAdapter(sql, conn);
            dp.Fill(dt);
            return 0;
        }
        public static int UpdateSetCardStatus(string status, string IDCode, string cardIdCode)
        {
            int iRet = -1;
            string strDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string dataPath = System.Windows.Forms.Application.StartupPath.ToString();
            string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + dataPath + "\\jiangsu_database.mdb";
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();
            string sql = "update E_人员信息 set 制卡状态 = '已回传',制证日期='" + strDate + "' where 身份证号 ='" + IDCode + "'";
            if (status == "已打印")
                sql = "update E_人员信息 set 制卡状态 = '已打印',卡识别码='" + cardIdCode + "' where 身份证号='" + IDCode + "'";
            OleDbCommand comm = null;
            try
            {
                comm = new OleDbCommand(sql, conn);
                iRet = comm.ExecuteNonQuery();
                comm.Dispose();
                conn.Close();
            }
            catch (Exception exErr)
            {
                WriteWorkLog.WriteLogs("日志", "本地数据库异常", exErr.ToString());
                comm.Dispose();
                conn.Close();
            }
            WriteWorkLog.WriteLogs("日志", "本地数据库sql：", sql);
            WriteWorkLog.WriteLogs("日志", "本地数据库ExecuteNonQuery：", iRet.ToString());
            return iRet;
        }

    }
}
