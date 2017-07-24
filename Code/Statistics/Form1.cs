using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Statistics
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        void Button1Click(object sender, EventArgs e)
        {
            //System.Windows.Forms.DataVisualization.Charting.Chart chart1 = this.chart1;
            //定义一个chart
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = chart1.ChartAreas[0];
            //定义一个绘图区域
            System.Windows.Forms.DataVisualization.Charting.Series series1 = chart1.Series[0];
            //定义一个数据列
            chartArea1.Name = "ChartArea1";
            //其实没有必要，可以使用chart1。ChartAreas[0]就可以了
            //chart1.ChartAreas.Add(chartArea1);
            //完成Chart和chartArea的关联
            //System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            //legend1.Name = "图标";
            //chart1.Legends.Add(legend1);
            chart1.Name = "chart1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            //设置线性
            Random rd = new Random();
            double[] num = new double[20];
            for (int i = 0; i < 20; i++)
            {
                int valuey = rd.Next(20, 100);
                DataPoint point = new DataPoint((i + 1), valuey);
                series1.Points.Add(point);
            }
            //产生点的坐标
            //chart1.Titles[0].Text = "";

            series1.ChartArea = "ChartArea1";
            chartArea1.AxisX.Title = "日期";
            chartArea1.AxisY.Title = "值";
            chartArea1.AxisX.Interval = 1;
            chartArea1.AxisY.Interval = 5;
            chartArea1.AxisY.Minimum = 20;
            //series1.Legend = "图标";
            series1.Name = "Series1";
            chart1.Text = "测试";
            chart1.Size = new System.Drawing.Size(700, 500);
            //chart1.Location = new System.Drawing.Point(50,120);
            series1.Color = Color.Blue;
            chart1.Text = "ceshi";
            //chart1.Titles[0].Text = "fff";
            //chart1.Series.Add(series1);
            //这一句很重要，缺少的话绘图区域将显示空白
            //chart1.SizeChanged += new System.EventHandler(DoSizeChanged);
            //chart1.AllowDrop = true;
            chart1.BackColor = Color.FromArgb(243, 223, 193);
            //设置chart背景颜色
            chartArea1.BackColor = Color.FromArgb(243, 223, 193);
            //设置c绘图区域背景颜色
            series1.BorderWidth = 2;
            series1.IsValueShownAsLabel = true;
            //是否显示Y的值
            //this.groupBox9.Controls.Add(chart1);
            chart1.Visible = true;
            //this.label10.Visible = true;
            //this.label10.Text = "【" + tn.Name + "】图";
            //chart1.Titles.Add("【" + tn.Name + "】图");
            //为Chart1添加标题
            chartArea1.AxisX.IsMarginVisible = true;
            //是否显示X轴两端的空白

        }

        void Chart1Click(object sender, EventArgs e)
        {

        }
    }
}
