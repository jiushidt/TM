/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 2017/4/6
 * Time: 15:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;
using System.IO;

namespace Reg
{
    class Program
    {
        public static void Main(string[] args)
        {

            string cpath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string ct = "create.bat";
            string apath = (cpath + ct);
            StreamWriter wt = new StreamWriter(apath);
            wt.WriteLine("@echo off");
            //wt.WriteLine ("set path="+apath);
            wt.WriteLine("schtasks /delete /f /tn \"TM_monitor\"");
            wt.WriteLine("schtasks /create /tn \"TM_monitor\" /tr " + cpath + "Monitor.exe" + " /sc ONLOGON"+" /ru "+System.Environment.UserName);
            wt.WriteLine("del /f create.bat");
            wt.Flush();
            wt.Close();

            Process p = new Process();
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = apath;
            info.WindowStyle = ProcessWindowStyle.Hidden;
            p.StartInfo = info;
            p.Start();
        }
    }
}