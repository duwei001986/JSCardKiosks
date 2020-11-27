using OledbConn;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JSCardKiosks
{
    public partial class BackManage : Form
    {
        string sql = "";
        DataTable dt;
        MainForm mainForm;
        public BackManage()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            sql = "select 批次,姓名,身份证号,制卡状态,性别,民族,出生日期,人员识别号,社保卡号,发卡日期 ,制证日期 from E_人员信息  where 身份证号 ='" + txtIdNum.Text.Trim() + "' order by 序号 ";
            OledbSqlOpration.GetDataTable(sql, out dt);
            dataGridView1.DataSource = dt;
            labAllCont.Text = dataGridView1.Rows.Count.ToString();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            string fileName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + ".xls";
            ExportChooseExcels(fileName, dataGridView1);
        }
        private void ExportChooseExcels(string fileName, DataGridView myDGV)
        {
            string saveFileName = "";
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.DefaultExt = "xls";
            saveDialog.Filter = "Excel文件|*.xls";
            saveDialog.FileName = fileName;
            if (saveDialog.ShowDialog() == DialogResult.Cancel) return;
            saveFileName = saveDialog.FileName;
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            if (xlApp == null)
            {
                MessageBox.Show("无法创建Excel对象，可能您的机子未安装Excel");
                return;
            }
            Microsoft.Office.Interop.Excel.Workbooks workbooks = xlApp.Workbooks;
            Microsoft.Office.Interop.Excel.Workbook workbook = workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
            Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];//取得sheet1
                                                                                                                                  //写入标题
            for (int i = 0; i < myDGV.ColumnCount; i++)
            {
                worksheet.Cells[1, i + 1] = myDGV.Columns[i].HeaderText;
            }
            //写入数值
            // for (int r =1; r <= myDGV.SelectedRows.Count; r++)
            for (int r = myDGV.SelectedRows.Count; r > 0; r--)
            {
                int k = myDGV.SelectedRows[r - 1].Index;
                for (int i = 0; i < myDGV.ColumnCount; i++)
                {
                    if (myDGV.Columns[i].HeaderText == "身份证号")
                    {
                        worksheet.Cells[myDGV.SelectedRows.Count - r + 2, i + 1] = "'" + myDGV.Rows[k].Cells[i].Value;
                    }
                    else
                    {
                        worksheet.Cells[myDGV.SelectedRows.Count - r + 2, i + 1] = myDGV.Rows[k].Cells[i].Value;
                    }

                }
                System.Windows.Forms.Application.DoEvents();
            }
            worksheet.Columns.EntireColumn.AutoFit();//列宽自适应
            if (saveFileName != "")
            {
                try
                {
                    workbook.Saved = true;
                    workbook.SaveCopyAs(saveFileName);
                }
                catch (Exception ex)
                {

                    MessageBox.Show("导出文件时出错,文件可能正被打开！\n", "1");
                }
            }
            xlApp.Quit();
            GC.Collect();//强行销毁
            MessageBox.Show("文件： " + fileName + ".xls 保存成功", "1");
    }
        private void ExportExcels(string fileName, DataGridView myDGV)
        {
            string saveFileName = "";
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.DefaultExt = "xls";
            saveDialog.Filter = "Excel文件|*.xls";
            saveDialog.FileName = fileName;
            if (saveDialog.ShowDialog() == DialogResult.Cancel) return;
            saveFileName = saveDialog.FileName;
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            if (xlApp == null)
            {
                MessageBox.Show("无法创建Excel对象，可能您的机子未安装Excel");
                return;
            }
            Microsoft.Office.Interop.Excel.Workbooks workbooks = xlApp.Workbooks;
            Microsoft.Office.Interop.Excel.Workbook workbook = workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
            Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];//取得sheet1
                                                                                                                                  //写入标题
            for (int i = 0; i < myDGV.ColumnCount; i++)
            {
                worksheet.Cells[1, i + 1] = myDGV.Columns[i].HeaderText;
            }
            //写入数值
            for (int r = 0; r < myDGV.Rows.Count; r++)
            {
                for (int i = 0; i < myDGV.ColumnCount; i++)
                {
                    if (myDGV.Columns[i].HeaderText == "身份证号")
                    {
                        worksheet.Cells[r + 2, i + 1] = "'" + myDGV.Rows[r].Cells[i].Value;
                    }
                    else
                    {
                        worksheet.Cells[r + 2, i + 1] = myDGV.Rows[r].Cells[i].Value;
                    }

                }
                System.Windows.Forms.Application.DoEvents();
            }
            worksheet.Columns.EntireColumn.AutoFit();//列宽自适应
            if (saveFileName != "")
            {
                try
                {
                    workbook.Saved = true;
                    workbook.SaveCopyAs(saveFileName);
                }
                catch (Exception ex)
                {

                    MessageBox.Show("导出文件时出错,文件可能正被打开！\n", "1");
                }
            }
            xlApp.Quit();
            GC.Collect();//强行销毁
            MessageBox.Show("文件： " + fileName + ".xls 保存成功", "1");
        }
        private void BackManage_Load(object sender, EventArgs e)
        {
            mainForm = (MainForm)this.Owner;
            dt = new DataTable();
            comboBox1.SelectedIndex = 0;
            SetDataGridViewStyle();
            sql = "select 批次,姓名,身份证号,制卡状态,性别,民族,出生日期,人员识别号,社保卡号,发卡日期 ,制证日期 from E_人员信息  order by 序号 ";
            OledbSqlOpration.GetDataTable(sql, out dt);
            dataGridView1.DataSource = dt;
            labAllCont.Text = dataGridView1.Rows.Count.ToString();
        }
        private void SetDataGridViewStyle()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightCyan;
            this.dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(30, 181, 173);
            dataGridViewCellStyle2.Font = new System.Drawing.Font("黑体", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.EnableHeadersVisualStyles = false;
            this.dataGridView1.GridColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false; //建议改为true；为了以后显示序号。
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.RowTemplate.ReadOnly = true;
            dataGridView1.AllowUserToResizeRows = false;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
                sql = "select 批次,姓名,身份证号,制卡状态,性别,民族,出生日期,人员识别号,社保卡号,发卡日期 ,制证日期 from E_人员信息  order by 序号 ";
            if (comboBox1.SelectedIndex == 1)
                sql = "select 批次,姓名,身份证号,制卡状态,性别,民族,出生日期,人员识别号,社保卡号,发卡日期 ,制证日期 from E_人员信息  where 制卡状态 ='已制' order by 序号";
            if (comboBox1.SelectedIndex == 2)
                sql = "select 批次,姓名,身份证号,制卡状态,性别,民族,出生日期,人员识别号,社保卡号,发卡日期 ,制证日期 from E_人员信息  where ISNULL(制卡状态)  order by 序号 ";
            OledbSqlOpration.GetDataTable(sql, out dt);
            dataGridView1.DataSource = dt;
            labAllCont.Text = dataGridView1.Rows.Count.ToString();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            string dateStart = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            dateStart += " 00:00:00";
            string dateEnd = dateTimePicker2.Value.ToString("yyyy-MM-dd");
            dateEnd += " 23:59:59";
            if (comboBox1.SelectedIndex == 0)
                sql = "select 批次,姓名,身份证号,制卡状态,性别,民族,出生日期,人员识别号,社保卡号,发卡日期 ,制证日期 from E_人员信息  where 制证日期 >='" + dateStart + "'and 制证日期<='" + dateEnd + "'  order by 序号 ";
            if (comboBox1.SelectedIndex == 1)
                sql = "select 批次,姓名,身份证号,制卡状态,性别,民族,出生日期,人员识别号,社保卡号,发卡日期 ,制证日期 from E_人员信息  where 制证日期 >='" + dateStart + "'and 制证日期<='" + dateEnd + "' and 制卡状态 ='已制' order by 序号 ";
            if (comboBox1.SelectedIndex == 2)
                sql = "select 批次,姓名,身份证号,制卡状态,性别,民族,出生日期,人员识别号,社保卡号,发卡日期 ,制证日期 from E_人员信息  where 制证日期 >='" + dateStart + "'and 制证日期<='" + dateEnd + "' and ISNULL(制卡状态) order by 序号 ";
            OledbSqlOpration.GetDataTable(sql, out dt);
            dataGridView1.DataSource = dt;
            labAllCont.Text = dataGridView1.Rows.Count.ToString();
        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {
            labRecordNo.Text = (dataGridView1.SelectedRows[0].Index + 1).ToString();
            if (dataGridView1.SelectedRows.Count > 0)
                labChoose.Text = "一共选中" + dataGridView1.SelectedRows.Count.ToString() + "条记录";
        }

        private void BackManage_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
