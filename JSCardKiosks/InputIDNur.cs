using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WorkLog;

namespace JSCardKiosks
{
    public partial class InputIDNur : Form
    {
        public const int CARD_BUSINESSTYPE_RESHOOT = 1;
        public const int CARD_BUSINESSTYPE_NEWADD = 2;
        public static bool isShowNumBoard = false;
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool Wow64DisableWow64FsRedirection(ref IntPtr ptr);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool Wow64RevertWow64FsRedirection(IntPtr ptr);
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        public extern static IntPtr FindWindow(string lpClassName, string lpWindowName);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "MoveWindow")]
        public static extern bool MoveWindow(System.IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        IdInfo idInfo = new IdInfo();
        int iTime = 60;
        MainForm mainForm;
        Process osk;
        public InputIDNur()
        {
            InitializeComponent();
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }
        private void killOsk()
        {
            Process[] pro = Process.GetProcesses();//获取已开启的所有进程
            //遍历所有查找到的进程
            for (int i = 0; i < pro.Length; i++)
            {
                //判断此进程是否是要查找的进程
                if (pro[i].ProcessName.ToString().ToLower() == "osk")
                {
                    pro[i].Kill();//结束进程
                }
            }
        }
        private static bool CheckIdCard18(string idNumber)
        {
            if (idNumber.Length < 18)
                return false;
            if (long.TryParse(idNumber.Remove(17), out var n) == false
                || n < Math.Pow(10, 16)
                || long.TryParse(idNumber.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证 
            }
            //省份编号
            const string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(idNumber.Remove(2), StringComparison.Ordinal) == -1)
            {
                return false;//省份验证 
            }
            var birth = idNumber.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            if (DateTime.TryParse(birth, out _) == false)
            {
                return false;//生日验证 
            }
            string[] arrArrifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = idNumber.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                // 加权求和 
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            //得到验证码所在位置
            Math.DivRem(sum, 11, out var y);
            var x = idNumber.Substring(17, 1).ToLower();
            var yy = arrArrifyCode[y];
            if (arrArrifyCode[y] != idNumber.Substring(17, 1).ToLower())
            {
                return false;//校验码验证 
            }
            return true;//符合GB11643-1999标准 
        }
        private void buttonOk_Click(object sender, EventArgs e)
        {
            killOsk();
            if (txtName.Text == "")
            {
                labErrInfo.Text = "姓名不能为空";
                txtName.Focus();
                return;
            }
            if(!CheckIdCard18(txtIdNum.Text))
            {
                labErrInfo.Text = "身份证号码不正确";
                txtIdNum.Focus();
                return;
            }
            idInfo.num = txtIdNum.Text;
            idInfo.name = txtName.Text;
            string strErrInfo = "";
            object[] objPara = new object[1];
            objPara[0] = idInfo;
            // mainForm.ShowChildForm("JSSYCardKiosks.IdVerification", 1, objPara);//有人脸比对
            string strRet = "";
            try
            {
                strRet = AccessWebInterFace.downLoadCardData(idInfo.num, idInfo.name);
            }
            catch (Exception ex)
            {

                WriteWorkLog.WriteLogs("日志", "错误", "downLoadCardData:" + ex.ToString());
                if (iTime < 10)
                {
                    iTime = 10;
                }
                string[] paras1 = { iTime.ToString(), "下载社保数据失败，" + ex.ToString() };
                BlackForm blackForm1 = new BlackForm(2, "JSCardKiosks.MsgForm", paras1);
                blackForm1.Show(this);
            }
            switch (strRet)
            {
                case "0$0":
                    timer1.Stop();
                    mainForm.SetHeadText("社保卡信息显示");
                    mainForm.ShowChildForm("JSCardKiosks.ShowCardInfo", 1, objPara);
                    timer1.Stop();
                    timer1.Dispose();
                    this.Dispose();
                    return;
                case "0$1":
                    timer1.Stop();
                    strErrInfo = "您没有采集社保照片，如有疑问请咨询工作人员";
                    break;

                case "0$2":
                    timer1.Stop();
                    strErrInfo = "您没有采集个人社保信息，如有疑问请咨询工作人员";
                    break;
                default:
                    strErrInfo = strRet.Split('$')[1];
                    WriteWorkLog.WriteLogs("日志", "错误", "downLoadCardData:" + strErrInfo);
                    break;
            }
            if (iTime < 10)
            {
                iTime = 10;
            }
            string[] paras = { iTime.ToString(), "下载社保数据失败，" + strErrInfo };
            BlackForm blackForm = new BlackForm(2, "JSCardKiosks.MsgForm", paras);
            blackForm.Show(this);
            timer1.Stop();
            timer1.Dispose();
            this.Dispose();
        }
        private void openKeyBoard()
        {
            string processName = System.IO.Path.GetFileNameWithoutExtension("osk.exe");

            // Check whether the application is not running 
            var query = from process in Process.GetProcesses()
                        where process.ProcessName == processName
                        select process;
            var keyboardProcess = query.FirstOrDefault();
            if (keyboardProcess == null)
            {
                IntPtr ptr = new IntPtr(); ;
                bool sucessfullyDisabledWow64Redirect = false;
                // Disable x64 directory virtualization if we're on x64,
                // otherwise keyboard launch will fail.
                if (System.Environment.Is64BitOperatingSystem)
                {
                    sucessfullyDisabledWow64Redirect = Wow64DisableWow64FsRedirection(ref ptr);
                }
                // osk.exe is in windows/system folder. So we can directky call it without path
                using (osk = new Process())
                {
                    osk.StartInfo.FileName = "osk.exe";
                    osk.Start();
                    //osk.WaitForInputIdle(2000);
                }
                try
                {
                    // 上面的语句在打开软键盘后，系统还没用立刻把软键盘的窗口创建出来了。所以下面的代码用循环来查询窗口是否创建，只有创建了窗口
                    // FindWindow才能找到窗口句柄，才可以移动窗口的位置和设置窗口的大小。这里是关键。
                    IntPtr intptr = IntPtr.Zero;
                    while (IntPtr.Zero == intptr)
                    {
                        System.Threading.Thread.Sleep(100);
                        intptr = FindWindow(null, "屏幕键盘");
                    }
                    // 获取屏幕尺寸
                    int iActulaWidth = Screen.PrimaryScreen.Bounds.Width;
                    int iActulaHeight = Screen.PrimaryScreen.Bounds.Height;
                    // 设置软键盘的显示位置，底部居中
                    int posX = (iActulaWidth - this.Width) / 2;
                    int posY = (iActulaHeight / 2);
                    //设定键盘显示位置
                    MoveWindow(intptr, posX, posY, this.Width, Convert.ToInt32(this.Width * 0.3), true);
                    //设置软键盘到前端显示
                    SetForegroundWindow(intptr);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            iTime--;
            labRetTime.Text = iTime.ToString().PadLeft(2, '0');
            if (iTime <= 0)
            {
                this.Close();
            }
        }

        private void InputIDNur_FormClosing(object sender, FormClosingEventArgs e)
        {

            killOsk();
            mainForm.SetbtnRetmainVisable(false);
            mainForm.SetpalMidVisable(false);
            mainForm.SetHeadText("社会保障卡发卡终端");
            timer1.Stop();
        }

        private void txtName_Click(object sender, EventArgs e)
        {
            openKeyBoard();
            labErrInfo.Text = "";
        }

      

      

        private void InputIDNur_Load(object sender, EventArgs e)
        {
            labRetTime.Text = iTime.ToString().PadLeft(2, '0');
            mainForm = (MainForm)this.Owner;
            openKeyBoard();
        }

       
    }
}
