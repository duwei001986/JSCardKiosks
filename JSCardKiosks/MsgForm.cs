using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JSCardKiosks
{
    public partial class MsgForm : Form
    {
        int iTime = 0;
        string strErr = "";
        public static int iRet = 0;
        private int thisRet = 0;
        string strMsgType = "0";
        public MsgForm()
        {
            InitializeComponent();
        }
        public MsgForm(string strTime, string strErr, string strMsgType = "0")
        {
            this.iTime = Convert.ToInt32(strTime);
            this.strErr = strErr;
            this.strMsgType = strMsgType;
            InitializeComponent();
        }
        public MsgForm(string strTime, string strErr)
        {
            this.iTime = Convert.ToInt32(strTime);
            this.strErr = strErr;
            InitializeComponent();
        }
        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.OpenForms["BlackForm"].Close();
        }
        /// <summary>
        /// 设置窗体的Region
        /// </summary>
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
        private void Form2_Load(object sender, EventArgs e)
        {
            labErrText.Text = strErr;// "系统无法识别您的身份证,请您确认是否插入正确的证件或者联系工作人员。";//，
            if (strMsgType == "0")
            {
                labCancel.Visible = false;
            }
        }

        private void Form2_Resize(object sender, EventArgs e)
        {
            SetWindowRegion();
        }

        private void labRet_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            thisRet = 1;
            timer1.Stop();
            this.Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            MsgForm.iRet = thisRet;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            iTime--;
            if (iTime <= 1)
            {
                this.DialogResult = DialogResult.OK;
                thisRet = 0;
                this.Close();
                timer1.Stop();
            }
        }

        private void labCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            thisRet = 0;
            this.Close();
        }
    }
}
