using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace JSCardKiosks
{
    public partial class BlackForm : Form
    {
        int paramcount = 0;
        string formName = "";
        string[] parameters = { "", };
        public BlackForm()
        {
            InitializeComponent();
        }
        public BlackForm(int paramcount,string formName,string [] parameters)
        {
            this.paramcount = paramcount;
            this.formName = formName;
            this.parameters = parameters;
            InitializeComponent();
        }
        private void Form3_Load(object sender, EventArgs e)
        {
            MainForm f = Owner as MainForm;
            Location = new Point(f.Location.X, f.Location.Y);
            Assembly assembly = Assembly.GetExecutingAssembly(); // 获取当前程序集 
            dynamic obj; // 创建类的实例，返回为 object 类型，需要强制类型转换
            if(paramcount==0)
            {
                obj = assembly.CreateInstance(formName);
            }
            else
            {
                obj = assembly.CreateInstance(formName, true, System.Reflection.BindingFlags.Default, null, parameters, null, null);// 创建类的实例 
            }
            obj.TopMost = true;
            obj.Show();
        }
    }
}
