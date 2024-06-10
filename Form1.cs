using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace 用五点光滑法进行曲线拟合
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        List<Point> orgPoints = new List<Point>();
        List<Curve> answer = new List<Curve>();

        //打开并读取文件
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "文本文件|*.txt";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                StreamReader sr = new StreamReader(ofd.FileName);

                while (!sr.EndOfStream)
                {
                    string[] str = sr.ReadLine().Trim().Split(',', '，');

                    Point point = new Point();
                    point.name = str[0];
                    point.x = double.Parse(str[1]);
                    point.y = double.Parse(str[2]);

                    orgPoints.Add(point);

                }
            }

            dataGridView1.RowCount = orgPoints.Count;
            for (int i = 0; i < orgPoints.Count; i++)
            {
                dataGridView1.Rows[i].Cells[0].Value = orgPoints[i].name;
                dataGridView1.Rows[i].Cells[1].Value = orgPoints[i].x;
                dataGridView1.Rows[i].Cells[2].Value = orgPoints[i].y;

            }


        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        //闭合拟合计算
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            answer = Calculate.Built(orgPoints, true);
            textBox1.Text = GetReport(true);
            UpdateChart(orgPoints, answer);
            chart1.ChartAreas[0].AxisX.Title = "闭合拟合曲线";

        }

        //不闭合拟合计算
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            answer = Calculate.Built(orgPoints, false);
            textBox1.Text = GetReport(false);
            UpdateChart(orgPoints,answer);
            chart1.ChartAreas[0].AxisX.Title = "不闭合拟合曲线";
        }

        //生成报告
        public string GetReport(bool is_closed)
        {
            string row = "";

            if (is_closed)
            {
                row += "闭合拟合\r\n";
            }
            else
            {
                row += "不闭合拟合\r\n";
            }

            row += "起点ID\tx\ty\t终点ID\t终点x\t终点y\tE0\tE1\tE2\tE3\tF0\tF1\tF2\tF3\r\n";

            foreach (Curve curve in answer)
            {
                row += curve.start.name + "\t";
                row += curve.start.x.ToString("0.000") + "\t";
                row += curve.start.y.ToString("0.000") + "\t";
                row += curve.end.name + "\t";
                row += curve.end.x.ToString("0.000") + "\t";
                row += curve.end.y.ToString("0.000") + "\t";
                row += curve.E0.ToString("0.000") + "\t";
                row += curve.E1.ToString("0.000") + "\t";
                row += curve.E2.ToString("0.000") + "\t";
                row += curve.E3.ToString("0.000") + "\t";
                row += curve.F0.ToString("0.000") + "\t";
                row += curve.F1.ToString("0.000") + "\t";
                row += curve.F2.ToString("0.000") + "\t";
                row += curve.F3.ToString("0.000") + "\r\n\r\n";
            }
            return row;
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            //保存报告
            SaveFileDialog sav = new SaveFileDialog();
            sav.Filter = "文本文件|*.txt";
            string line = textBox1.Text;

            if (sav.ShowDialog() == DialogResult.OK)//调用 ShowDialog 显示保存文件对话框，并检查用户是否点击了“保存”按钮
            {
                StreamWriter sw = new StreamWriter(sav.FileName);
                sw.Write(line);
                sw.Flush();
                sw.Close();
            }
        }

        //退出
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void UpdateChart(List<Point> orgPoints, List<Curve> answer = null)//如果没有曲线提供，则默认为空值
        {
            chart1.Titles.Clear();//清空表格

            
            chart1.ChartAreas[0].AxisX.ArrowStyle = System.Windows.Forms.DataVisualization.Charting.AxisArrowStyle.Triangle;//箭头样式为三角形
            chart1.ChartAreas[0].AxisY.ArrowStyle = System.Windows.Forms.DataVisualization.Charting.AxisArrowStyle.Triangle;
            this.chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.Transparent;//主网线格颜色透明
            this.chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = System.Drawing.Color.Transparent;

            //清空图表中数据
            int count = chart1.Series.Count;
            for (int j = 0; j < count; j++)
            {
                chart1.Series.RemoveAt(0);
            }
            //将原点数据添加到 chart1 中
            int i = 0;//初始化计数器
            foreach (Point pt in orgPoints)//遍历原数据集
            {
                chart1.Series.Add(pt.name);//使用pt.name作为系列名称添加一个新的系列

                chart1.Series[i].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;//设置系列的图表类型为点图
                chart1.Series[i].Points.Clear();//清空默认数据
                chart1.Series[i].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;//设置数据点的标记样式为圆形

                chart1.Series[i].Points.AddXY(pt.x, pt.y); //添加Point的坐标作为数据点
                chart1.Series[i].MarkerSize = 7;//设置标记的大小为 7
                chart1.Series[i].Label = i.ToString();//设置数据点的标签为 i

                chart1.Series[i].IsVisibleInLegend = false;//隐藏系列在图例中的显示

                chart1.Series[i].Color = Color.Red;
                i++;

            }

            if (answer != null)
            {
                chart1.Series.Add("line");
                chart1.Series[i].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                chart1.Series[i].Points.Clear();
                chart1.Series[i].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.None;
                chart1.Series[i].IsVisibleInLegend = false;
                chart1.Series[i].Color = Color.Blue;

                foreach (Curve curve in answer)
                {
                    double z = 0;
                    while (z < 1)
                    {
                        double x = curve.E0 + curve.E1 * z + curve.E2 * z * z + curve.E3 * z * z * z;
                        double y = curve.F0 + curve.F1 * z + curve.F2 * z * z + curve.F3 * z * z * z;
                        chart1.Series[i].Points.AddXY(x, y);
                        z += 0.01;
                    }
                }
            }
        }

        //保存图像
        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            
            
                SaveFileDialog sav = new SaveFileDialog();
                sav.Filter = "DXF文件 (*.dxf)|*.dxf";
                

                if (sav.ShowDialog() == DialogResult.OK)
                {
                StreamWriter sw = new StreamWriter(sav.FileName);
                sw.Write(generateDxf(orgPoints,answer));
                sw.Flush();
                sw.Close();
            }
        }        

        private string generateDxf(List<Point> orgPoints, List<Curve> answer)
        {

            string str_dxf = "";
            str_dxf += "  0\r\n";
            str_dxf += "SECTION\r\n";
            str_dxf += "  2\r\n";
            str_dxf += "ENTITIES\r\n";

            //点
            for (int i = 0; i < orgPoints.Count; i++)
            {
                str_dxf += "  0\r\n";
                str_dxf += "POINT\r\n";
                str_dxf += "  8\r\n";
                str_dxf += "0\r\n";
                str_dxf += " 10\r\n";
                str_dxf += orgPoints[i].x.ToString() + "\r\n";
                str_dxf += " 20\r\n";
                str_dxf += orgPoints[i].y.ToString() + "\r\n";
            }

            //线
            foreach (Curve mycurve in answer)
            {
                double z = 0;
                double z_next = 0.01;
                while (z < 1)
                {
                    double x = mycurve.E0 + mycurve.E1 * z + mycurve.E2 * z * z + mycurve.E3 * z * z * z;
                    double y = mycurve.F0 + mycurve.F1 * z + mycurve.F2 * z * z + mycurve.F3 * z * z * z;

                    double x_next = mycurve.E0 + mycurve.E1 * z_next + mycurve.E2 * z_next * z_next + mycurve.E3 * z_next * z_next * z_next;
                    double y_next = mycurve.F0 + mycurve.F1 * z_next + mycurve.F2 * z_next * z_next + mycurve.F3 * z_next * z_next * z_next;

                    str_dxf += "  0\r\n";
                    str_dxf += "LINE\r\n";
                    str_dxf += "  8\r\n";
                    str_dxf += "0\r\n";
                    str_dxf += " 10\r\n";
                    str_dxf += x.ToString() + "\r\n";
                    str_dxf += " 20\r\n";
                    str_dxf += y.ToString() + "\r\n";
                    str_dxf += " 11\r\n";
                    str_dxf += x_next.ToString() + "\r\n";
                    str_dxf += " 21\r\n";
                    str_dxf += y_next.ToString() + "\r\n";

                    z_next += 0.01;
                    z += 0.01;
                }
            }
            str_dxf += "  0\r\n";
            str_dxf += "ENDSEC\r\n";
            str_dxf += "  0\r\n";
            str_dxf += "EOF\r\n";
            return str_dxf;
        }


    }
}
