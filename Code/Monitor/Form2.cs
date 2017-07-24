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

namespace Monitor
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
		
		void Button1Click(object sender, EventArgs e)
		{
			Dictionary<string,List<DateTime>> so=new Dictionary<string, List<DateTime>> ();
			
			string path=@"C:\Users\Administrator\Desktop\myPro\tmb\Code\Monitor\bin\Debug\20170309dr";
			StreamReader reader=new StreamReader (path);
			string text;
			string key=string.Empty;
			while(true)
			{
				text =reader .ReadLine ();
				if(text .Contains(";"))
				{
					string[] ds=text.Split(';');
					so[key].Add(Convert .ToDateTime(ds[0]));
					so[key].Add(Convert .ToDateTime(ds[1]));
				}
				else
				{
					if(so.ContainsKey(text))
					{}
					else
					{
						so[text]=new List<DateTime>();
						key=text;
					}
				}
				if(reader .EndOfStream )
					break ;
			}
			reader .Close();
			
			Graphics g=pnlView .CreateGraphics();
			int n=so.Keys .Count +1;
			
			float width=g.VisibleClipBounds.Width ;
			float height=g.VisibleClipBounds .Height ;
			
			float xo=80,yo=10;
			int yoff=25;
			//绘制时间格线
			Pen p ;
			Pen sp=new Pen (Color .Black,1);
			sp.DashStyle=System .Drawing .Drawing2D.DashStyle.Custom ;
			sp.DashPattern = new float[] { 1f, 1f }; 
			float step=(width-2*xo)/24;
			for(int i=0;i<24;i++){
				p=new Pen(Color.FromArgb (50,0,i*10),i);
				float x=xo+i*step;
				
				g.DrawLine(sp,x,yo,x,height);
				g.DrawLine (p,x,yo,x+step,yo);
				
				
				g.DrawString(i.ToString(),this.Font,new SolidBrush(Color .Red),x,yo+yoff);
			}
			
			//绘制软件使用线段
			int nn=1;
			
			
			foreach(string tt in so.Keys)
			{
				nn++;
				SizeF stringSize=g.MeasureString (tt,this.Font);
				g.DrawString(tt,this.Font,new SolidBrush(Color .Red),xo-stringSize.Width,yo+yoff*nn);
				
				
				List<DateTime> dl=so[tt];
				
				for(int i=0;i<dl.Count;i+=2 )
				{
					DateTime d1=dl[i];
					int h=d1.Hour;
					int m=d1.Minute;
					int s=d1.Second;
					float xf=xo+(float)((h/24.0+m/24.0/60+s/24.0/60/60)*(width-2*xo));
					
					DateTime d2=dl[i+1];
					int h2=d2.Hour;
					int m2=d2.Minute;
					int s2=d2.Second;
					float xt=xo+(float)((h2/24.0+m2/24.0/60+s2/24.0/60/60)*(width-2*xo));
					
					p=new Pen(getColorByStep(d1,d2),10);
					g.DrawLine(p,xf,yo+yoff*nn,xt,yo+yoff*nn);
				}
				
			}
			
			
				
		}
		
		Color getColorByStep(DateTime d1,DateTime d2)
		{
			if(d2<d1)
				return Color.Black;
			TimeSpan step=d2.Subtract (d1);
			if (step.Ticks>3600L*1000L*1000L)
				return Color.Red;
			else
				return Color.Green;
		}
	}
}
