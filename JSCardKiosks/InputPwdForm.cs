using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using WorkLog;

namespace JSCardKiosks
{
    public partial class InputPwdForm : Form
    {
        MainForm mainForm;
        bool bExit = false;
        [DllImport("shell32.dll")]
        public static extern int ShellExecute(IntPtr hwnd, StringBuilder lpszOp, StringBuilder lpszFile, StringBuilder lpszParams, StringBuilder lpszDir, int FsShowCmd);
        DataTable dt = new DataTable();
        string temp = "";//临时保存部分mima
        public InputPwdForm()
        {
            InitializeComponent();
        }
        public InputPwdForm(string p1)
        {
            InitializeComponent();
        }
        public void SetWindowRegion()
        {
            GraphicsPath FormPath;
            Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
            FormPath = GetRoundedRectPath(rect, 20);
            this.Region = new Region(FormPath);

        }
        /// <summary>
        /// 绘制圆角路径
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            int diameter = radius;
            Rectangle arcRect = new Rectangle(rect.Location, new Size(diameter, diameter));
            GraphicsPath path = new GraphicsPath();

            // 左上角
            path.AddArc(arcRect, 180, 90);

            // 右上角
            arcRect.X = rect.Right - diameter;
            path.AddArc(arcRect, 270, 90);

            // 右下角
            arcRect.Y = rect.Bottom - diameter;
            path.AddArc(arcRect, 0, 90);

            // 左下角
            arcRect.X = rect.Left;
            path.AddArc(arcRect, 90, 90);
            path.CloseFigure();//闭合曲线
            return path;
        }

        private void InputPwdForm_Resize(object sender, EventArgs e)
        {
            SetWindowRegion();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {

        }

        private void textPassword_Click(object sender, EventArgs e)
        {
            labPwdErrInfo.Text = "";
            
        }

        private void buttonN1_Click(object sender, EventArgs e)
        {
            labPwdErrInfo.Text = "";
            Button button = (Button)sender;
            temp = button.Name.Substring(button.Name.Length - 1);
            txtPassword.Text += temp;
            txtPassword.SelectionStart = txtPassword.Text.Length;
        }

        private void InputPwdForm_Load(object sender, EventArgs e)
        {
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            labPwdErrInfo.Text = "";
            txtPassword.Clear();
        }

        private void buttonNX_Click(object sender, EventArgs e)
        {
            labPwdErrInfo.Text = "";
            if (txtPassword.Text != null)
            {
                try
                {
                    txtPassword.Text = txtPassword.Text.Substring(0, txtPassword.Text.Length - 1);
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        private void labMamgeForm_MouseEnter(object sender, EventArgs e)
        {
            labMamgeForm.ForeColor = Color.Black;
        }

        private void labMamgeForm_Click(object sender, EventArgs e)
        {
            labPwdErrInfo.Visible = true;
            labPwdErrInfo.Text = "管理员密码错误，请重新输入!";
            if (Access_SQLHelper.Logon(txtPassword.Text.Trim()))
            {
                labPwdErrInfo.Text = "";
                this.TopMost = false;
                this.Dispose();
                BlackForm.ActiveForm.Dispose();
                BackManage backManage = new BackManage();
                backManage.ShowDialog();
            }
        }

        private void labMamgeForm_MouseDown(object sender, MouseEventArgs e)
        {
            labMamgeForm.ForeColor = Color.Black;
        }

        private void labMamgeForm_MouseLeave(object sender, EventArgs e)
        {
            labMamgeForm.ForeColor = Color.Gray;
        }

        private void InputPwdForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.OpenForms["BlackForm"].Close();
            this.Dispose(true);
            if(bExit)
            {
                base.OnClosing(e);
                MainForm.ActiveForm.Close();
                Application.Exit(e);
            }
        }

        private void InputPwdForm_FormClosed(object sender, FormClosedEventArgs e)
        {
          
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            labPwdErrInfo.Visible = true;
            labPwdErrInfo.Text = "管理员密码错误，请重新输入!";
            if (Access_SQLHelper.Logon(txtPassword.Text.Trim()))
            {
                labPwdErrInfo.Text = "";
                bExit = true;
                this.Close();
            }
            

        }

        private void labClose_Enter(object sender, EventArgs e)
        {

        }

        private void labClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void labClose_MouseEnter(object sender, EventArgs e)
        {
            labClose.BackColor = Color.FromArgb(228, 228, 228);
        }

        private void labClose_MouseLeave(object sender, EventArgs e)
        {
            labClose.BackColor = Color.Transparent;
        }
        private void GetImage(string filePath)
        {
            Stream s = File.Open(filePath, FileMode.Open);
            int leng = 0;
            if (s.Length < Int32.MaxValue)
                leng = (int)s.Length;
            byte[] by = new byte[leng];
            s.Read(by, 0, leng);//把图片读到字节数组中 
            s.Close();
            string str = Convert.ToBase64String(by);//把字节数组转换成字符串     
            StreamWriter sw = File.CreateText(AppDomain.CurrentDomain.BaseDirectory + "sysword.dat");//存入11.txt文件    
            sw.Write(str);
            sw.Close();
            sw.Dispose();
        }
        private void InputPwdForm_KeyDown(object sender, KeyEventArgs e)
        {
            if ((int)e.Modifiers == ((int)Keys.Control + (int)Keys.Alt) && e.KeyCode == Keys.Y && Access_SQLHelper.Logon(txtPassword.Text.Trim()))
            {

                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Multiselect = true;//该值确定是否可以选择多个文件
                dialog.Title = "请选择LOGO图片";
                dialog.Filter = "Image Files(*.BMP;*.JPG;*.PNG)|*.BMP;*.JPG;*.PNG";
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string file = dialog.FileName;
                    GetImage(file);
                }
            }
            if ((int)e.Modifiers == ((int)Keys.Control + (int)Keys.Alt) && e.KeyCode == Keys.X && Access_SQLHelper.Logon(txtPassword.Text.Trim()))
            {
                panel1.Visible = true;
                if (getGifList(out dt) == 0)
                {
                    byte[] imagebytes = (byte[])dt.Rows[0][1];
                    MemoryStream ms = new MemoryStream(imagebytes);
                    Bitmap bmpt = new Bitmap(ms);
                    pictureBox1.Image = bmpt;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        comboBox1.Items.Add(dt.Rows[i][0]);
                    }
                    comboBox1.SelectedIndex = 0;
                }
                else
                {
                    MessageBox.Show("访问本地数据库失败");
                }
            }
        }
        private int getGifList(out DataTable dt)
        {
            dt = new DataTable();
            string strSql = "select imgName,gImg from imgList";
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
                        return -1;
                    }
                    return 0;
                }
            }
        }
        private int UpDateImg(string imgName, byte[] imageBytes)
        {
            //string strSql = "insert into  imgList (imgName,gImg)values ('"+imgName+"',@image)";
            string strSql = "update imgList set gImg = @image where imgName='" + imgName + "'";
            string connStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + AppDomain.CurrentDomain.BaseDirectory + "jiangsu_database.mdb";
            using (OleDbConnection conn = new OleDbConnection(connStr))
            {
                using (OleDbCommand cmd = new OleDbCommand(strSql, conn))
                {
                    try
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@image", imageBytes);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        conn.Close();
                        return rows;
                    }
                    catch (Exception exErr)
                    {
                        WriteWorkLog.WriteLogs("日志", "异常", " 本地数据库异常(UpDateImg):" + exErr.ToString());
                        return -1;
                    }
                }
            }

        }
      
        private void btnChangeGif_Click(object sender, EventArgs e)
        {
            string filePath = "";
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;//该值确定是否可以选择多个文件
            dialog.Title = "请选择LOGO图片";
            dialog.Filter = "Image Files(*.GIF;*.JPG;*.PNG)|*.GIF;*.JPG;*.PNG";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filePath = dialog.FileName;
                FileStream fs = new FileStream(filePath, FileMode.Open);
                byte[] imageBytes = new byte[fs.Length];
                BinaryReader br = new BinaryReader(fs);
                imageBytes = br.ReadBytes(Convert.ToInt32(fs.Length));//图片转换成二进制流
                UpDateImg(comboBox1.Text, imageBytes);
                fs.Close();
            }
            if (getGifList(out dt) == 0)
            {
                byte[] imagebytes = (byte[])dt.Rows[comboBox1.SelectedIndex][1];
                MemoryStream ms = new MemoryStream(imagebytes);
                Bitmap bmpt = new Bitmap(ms);
                pictureBox1.Image = bmpt;
            }
            else
            {
                MessageBox.Show("访问本地数据库失败");
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            byte[] imagebytes = (byte[])dt.Rows[comboBox1.SelectedIndex][1];
            MemoryStream ms = new MemoryStream(imagebytes);
            Bitmap bmpt = new Bitmap(ms);
            pictureBox1.Image = bmpt;
        }

        private void btnPalDisVisable_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
        }
    }
}
