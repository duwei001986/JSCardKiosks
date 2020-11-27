using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkLog;

namespace JSCardKiosks
{
    class DealGif
    {
        public static Bitmap GetGifImage(string strPicName)
        {
            DataTable dt = new DataTable();
            string strSql = "select imgName,gImg from imgList where imgName ='" + strPicName + "'";
            using (OleDbConnection conn = new OleDbConnection(Access_SQLHelper.strConn))
            {
                using (OleDbDataAdapter da = new OleDbDataAdapter(strSql, conn))
                {
                    try
                    {
                        conn.Open();
                        da.Fill(dt);
                    }
                    catch (Exception exErr)
                    {
                        WriteWorkLog.WriteLogs("日志", "异常", " 本地数据库异常(getGifList):" + exErr.ToString());
                        return null;
                    }
                }
            }
            byte[] imagebytes = (byte[])dt.Rows[0][1];
            MemoryStream ms = new MemoryStream(imagebytes);
            Bitmap bmpt = new Bitmap(ms);
            return bmpt;
        }
    }
}
