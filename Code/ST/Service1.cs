using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
//using System.Windows.Forms;
using System.Timers;

namespace ST
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            System.Timers.Timer t = new System.Timers.Timer(1000); //这里的1000指的是Timer的时间间隔为1000毫秒

            t.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Click); //Timer_Click是到达时间的时候执行事件的函数

            t.AutoReset = true; //设置是执行一次（false）还是一直执行(true)

            t.Enabled = true; //是否执行System.Timers.Timer.Elapsed事件
            t.Start();
        }

        private void Timer_Click(Object sender, ElapsedEventArgs e)
        {
            Process[] localByName = Process.GetProcessesByName("Monitor.exe");

            //这里的360tray.exe就是你想要执行的程序的进程的名称。基本上就是.exe文件的文件名。

            //localByName得到的是进程中所有名称为"360tray.exe"的进程。
            if (localByName.Length == 0) //如果得到的进程数是0, 那么说明程序未启动，需要启动程序
            {
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = @"C:\Users\Administrator.PC-20130430XNLE\Desktop\Pro\intime\Code\Monitor\bin\Debug\Monitor.exe";
                info.WindowStyle = ProcessWindowStyle.Maximized;
                Process pp = Process.Start(info); //启动程序 "c://360tray.exe" 是程序的路径
            }

            else
            {
                int n = 100;
                while (n > 0)
                {
                    string path = @"C:\Users\Administrator.PC-20130430XNLE\Desktop\Pro\intime\Code\ST\bin\Debug\" + n.ToString() + ".txt";
                    if (!System.IO.File.Exists(path))
                    {
                        System.IO.File.CreateText(path);
                        break;
                    }
                    n--;
                }
                //如果程序已经启动，则执行这一部分代码

            }
        }

        protected override void OnStop()
        {
        }
    }
}
