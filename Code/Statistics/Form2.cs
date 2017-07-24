/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 2017/3/26
 * Time: 18:23
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Statistics
{
    /// <summary>
    /// Description of Form2.
    /// </summary>
    public partial class Form2 : Form
    {
        public Form2()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();

            //
            // TODO: Add constructor code after the InitializeComponent() call.
            //
        }

        public class timeSeg
        {
            public string curDT { get; set; }
            public string sDT { get; set; }
            public string eDT { get; set; }

            public bool Append(string sd, string ed)
            {
                if (sd == curDT)
                {
                    curDT = eDT = ed;
                    return true;
                }
                else
                {
                    if (Convert.ToDateTime(sd) > Convert.ToDateTime(eDT).AddSeconds(5))
                        return false;
                    else
                    {
                        curDT = eDT = ed;
                        return true;
                    }
                }
                return false;
            }
        }

        private void tj(string path)
        {
            if (!File.Exists(path))
            {
                return;
            }

            StreamReader rd = new StreamReader(path);
            string info;
            Dictionary<string, List<timeSeg>> dic = new Dictionary<string, List<timeSeg>>();
            while (!rd.EndOfStream)
            {
                info = rd.ReadLine();
                string[] para = info.Split('-');
                if (!dic.ContainsKey(para[0]))
                    dic[para[0]] = new List<timeSeg>();
                int i = dic[para[0]].Count;
                if (i == 0)
                {
                    dic[para[0]].Add(new timeSeg() { curDT = para[2], sDT = para[1], eDT = para[2] });
                    i++;
                }
                bool conti = dic[para[0]][i - 1].Append(para[1], para[2]);
                if (!conti)
                    dic[para[0]].Add(new timeSeg() { curDT = para[2], sDT = para[1], eDT = para[2] });
            }
            rd.Close();

            StreamWriter w = new StreamWriter(path + "r");
            foreach (var pair in dic)
            {
                w.WriteLine(pair.Key);
                foreach (var fo in pair.Value)
                {
                    w.WriteLine(fo.sDT + ";" + fo.eDT);
                }
            }
            w.Flush();
            w.Close();
        }

        void Button1Click(object sender, EventArgs e)
        {
            DrawImg(Application.StartupPath+"\\"+ comboBox1.Text+"R");
        }

        private void Initob()
        {
            string curpath = Application.StartupPath;
            string[] fls = Directory.GetFiles(curpath, "*d");
            List<string> fs = new List<string>();
            foreach (var f in fls)
            {
                if (File.Exists(f + "r"))
                { }
                else
                {
                    tj(f);
                }
                fs.Add(Path.GetFileName(f));
            }
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(fs.ToArray());
        }

        private void DrawImg(string path)
        {
            if (!File.Exists(path))
                return;

            Dictionary<string, List<DateTime>> so = new Dictionary<string, List<DateTime>>();

            StreamReader reader = new StreamReader(path);
            string text;
            string key = string.Empty;
            int maxLen = 1;
            while (true)
            {
                text = reader.ReadLine();
                if (text.Contains(";"))
                {
                    string[] ds = text.Split(';');
                    so[key].Add(Convert.ToDateTime(ds[0]));
                    so[key].Add(Convert.ToDateTime(ds[1]));
                }
                else
                {
                    if (text.Length > maxLen)
                        maxLen = text.Length;
                    if (so.ContainsKey(text))
                    { }
                    else
                    {
                        so[text] = new List<DateTime>();
                        key = text;
                    }
                }
                if (reader.EndOfStream)
                    break;
            }
            reader.Close();

            Graphics g = pnlView.CreateGraphics();
            g.Clear(Color.White);
            int n = so.Keys.Count + 1;

            float width = g.VisibleClipBounds.Width;
            float height = g.VisibleClipBounds.Height;

            float xo = 10, yo = 10, Roff = 10;
            xo += g.MeasureString("A", this.Font).Width * maxLen;

            Pen p;
            Pen sp = new Pen(Color.Black, 1);
            float step = (width - xo - Roff) / 24;
            for (int i = 0; i < 24; i++)
            {
                p = new Pen(Color.FromArgb(50, 0, i * 10), i);
                float x = xo + i * step;
                g.DrawLine(sp, x, yo, x, height);
                g.DrawLine(p, x, yo, x + step, yo);                
            }

            int nn = 0;

            foreach (string tt in so.Keys)
            {
                nn++;
                g.DrawString(tt, this.Font, new SolidBrush(Color.Red), xo-g.MeasureString(tt,this.Font).Width, yo + 15 * nn);
                p = new Pen(Color.FromArgb(50, nn * 10, 0), 10);

                List<DateTime> dl = so[tt];

                for (int i = 0; i < dl.Count; i += 2)
                {
                    DateTime d1 = dl[i];
                    int h = d1.Hour;
                    int m = d1.Minute;
                    int s = d1.Second;
                    float xf = xo + (float)((h / 24.0 + m / 24.0 / 60 + s / 24.0 / 60 / 60) * (width - xo-Roff));

                    DateTime d2 = dl[i + 1];
                    int h2 = d2.Hour;
                    int m2 = d2.Minute;
                    int s2 = d2.Second;
                    float xt = xo + (float)((h2 / 24.0 + m2 / 24.0 / 60 + s2 / 24.0 / 60 / 60) * (width -xo-Roff));

                    g.DrawLine(p, xf, yo + 15 * nn, xt, yo + 15 * nn);
                }

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            Initob();
        }
    }
}
