using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using TaskScheduler;

namespace Monitor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Initob();
        }
        public const int step = 2;//10秒为记录间隔，最小2
        List<SoftItem> SList;
        int state = -1;
        delegate void Mon(string proName, DateTime s, DateTime e);
        Mon myMon;

        public class SoftItem
        {
            public int Sign { get; set; }
            public string Name { get; set; }
            public string DisName { get; set; }
            public string Kind { get; set; }
            public int count { get; set; }
            public bool changed { get; set; }
        }

        public void Initob()
        {
            if (state == 1)
            {
                if (clock != null && !clock.Enabled)
                    clock.Start();
                return;
            }

            string keyPath = Application.StartupPath + "\\key.dat";
            if (!File.Exists(keyPath))
            {
                FileStream temp = File.Create(keyPath);
                temp.Close();
            }
            string tx = File.ReadAllText(keyPath, Encoding.Default);
            SList = Utility.StringToObject<List<SoftItem>>(tx);
            if (SList == null)
            {
                SList = new List<SoftItem>();
            }

            string recodPath = Application.StartupPath + "\\" + DateTime.Now.ToString("yyyyMMdd");
            recodPath+="m";
            if (!File.Exists(recodPath))
            {
                FileStream fs = File.Create(recodPath);
                fs.Close();
            }

            using (FileStream rd = File.OpenRead(recodPath))
            {
                byte[] bd = new byte[2];
                for (int i = 0; i < rd.Length; i += 2)
                {
                    rd.Seek(i, SeekOrigin.Begin);
                    rd.Read(bd, 0, 2);
                    int ct = byte2int(bd);
                    SoftItem item = SList.Find(o => o.Sign == i);
                    if (item == null)
                        continue;
                    if (item.count != ct)
                        item.count = ct;
                }
            }

            writer = File.OpenWrite(recodPath);

            clock = new Timer();
            clock.Interval = step * 1000;
            clock.Tick += new EventHandler(t_Tick);
            clock.Start();
            state = 1;

            myMon += new Mon(detailMon);
        }
        FileStream writer;
        Timer clock;

        void detailMon(string name, DateTime s, DateTime e)
        {
            string recodPath = Application.StartupPath + "\\" + DateTime.Now.ToString("yyyyMMdd") + "d";
            if(!File.Exists (recodPath))
            {
            	FileStream temp=File.Create (recodPath);
            	temp.Close();
            }
            StreamWriter fs = new System.IO.StreamWriter(recodPath, true);
            fs.WriteLine(string.Format("{0}-{1}-{2}", name, s.ToString("HH:mm:ss"), e.ToString("HH:mm:ss")));
            fs.Close();
        }

        void t_Tick(object sender, EventArgs e)
        {
            Monit(writer);
        }

        public void key_w()
        {
            string keyPath = Application.StartupPath + "\\key.dat";
            if (!File.Exists(keyPath))
                File.Create(keyPath);
            if (SList == null)
            {
                SList = new List<SoftItem>();
            }
            string tx = Utility.ObjectToString<List<SoftItem>>(SList);
            File.WriteAllText(keyPath, tx, Encoding.Default);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Initob();
        }

        private void Monit(FileStream fs)
        {

            Process[] pAll = Process.GetProcesses();
            foreach (var p in pAll)
            {
                if (p.MainWindowHandle.ToInt64() == 0)
                    continue;
                string name = p.ProcessName;
                if (myMon != null)
                {
                    DateTime dt = DateTime.Now;
                    DateTime e = dt;
                    DateTime cs = dt.AddSeconds(-step);
                    DateTime s = p.StartTime > cs ? p.StartTime : cs;
                    myMon(name, s, e);
                }

                SoftItem item = SList.Find(o => o.Name == name && !o.changed);

                if (item == null)
                {
                    item = new SoftItem() { Name = name, Sign = SList.Count * 2, count = 0 };
                    SList.Add(item);
                }
                else
                { }

                byte[] count = new byte[2];
                try
                {
                    count = int2byte(item.count);
                }
                catch { count[1] = 0; count[0] = 0; }

                item.count++;
                add(ref count);
                fs.Seek(item.Sign, SeekOrigin.Begin);
                fs.Write(count, 0, 2);
                fs.Flush(true);
                item.changed = true;
            }

            foreach (var item in SList)
            {
                item.changed = false;
            }
        }

        private byte[] int2byte(int num)
        {
            byte[] re = new byte[2];
            re[1] = (byte)((num >> 8) & 0xFF);
            re[0] = (byte)(num & 0xFF);
            return re;
        }

        private int byte2int(byte[] bt)
        {
            int re = -1;
            return ((bt[1] << 8) & 0xFF) + (bt[0] & 0xFF);
        }

        private void add(ref byte[] re)
        {
            int len = re.Length;
            for (int i = 0; i < len; i++)
            {
                if (re[i] == 255)
                {
                    re[i] = 0;
                }
                else
                {
                    re[i]++;
                    break;
                }
            }
        }
        
        private void stopMonitor()
        {
        	if(clock!=null)
        	clock.Stop();
            state = 2;
            key_w();
            if(writer!=null)
            writer.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
        	stopMonitor();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            tj(@"C:\Users\Administrator\Desktop\myPro\tmb\Code\Monitor\bin\Debug\20170309d");
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
            Dictionary<string, List<timeSeg>> dic = new Dictionary<string, List<Form1.timeSeg>>();
            while (!rd.EndOfStream)
            {
                info = rd.ReadLine();
                string[] para = info.Split('-');
                if (!dic.ContainsKey(para[0]))
                    dic[para[0]] = new List<Form1.timeSeg>();
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
        public static IRegisteredTaskCollection GetAllTasks()
        {
            TaskSchedulerClass ts = new TaskSchedulerClass();
            ts.Connect(null, null, null, null);
            ITaskFolder folder = ts.GetFolder("\\");
            IRegisteredTaskCollection tasks_exists = folder.GetTasks(1);
            return tasks_exists;
        }
        public static bool IsExists(string taskName)
        {
            var isExists = false;
            IRegisteredTaskCollection tasks_exists = GetAllTasks();
            for (int i = 1; i <= tasks_exists.Count; i++)
            {
                IRegisteredTask t = tasks_exists[i];
                if (t.Name.Equals(taskName))
                {
                    isExists = true;
                    break;
                }
            }
            return isExists;
        }

        private static void DeleteTask(string taskName)
        {
            TaskSchedulerClass ts = new TaskSchedulerClass();
            ts.Connect(null, null, null, null);
            ITaskFolder folder = ts.GetFolder("\\");
            folder.DeleteTask(taskName, 0);
        }

        void Button4Click(object sender, EventArgs e)
        {
            ValidTask();
        }

        private static void ValidTask()
        {
            try
            {
                string taskName = "TM_monitor";
                string creator = "Administrator";
                string path = Application.StartupPath + @"\Monitor.exe";

                if (IsExists(taskName))
                {
                    return;
                    DeleteTask(taskName);
                }

                //实例化任务对象
                TaskSchedulerClass scheduler = new TaskSchedulerClass();
                scheduler.Connect(null, null, null, null);//连接
                ITaskFolder folder = scheduler.GetFolder("\\");
                //设置常规属性
                ITaskDefinition task = scheduler.NewTask(0);
                task.RegistrationInfo.Author = creator;//创建者 
                task.RegistrationInfo.Description = "描述信息";//描述
                task.Principal.RunLevel = _TASK_RUNLEVEL.TASK_RUNLEVEL_HIGHEST; //使用最高权限运行
                //设置触发器
                ILogonTrigger tt = (ILogonTrigger)task.Triggers.Create(_TASK_TRIGGER_TYPE2.TASK_TRIGGER_LOGON); //触发器里的开始任务,其他开始任务方式用的是其他接口
                tt.UserId = Environment.MachineName + "\\" + creator; //特定用户
                //设置操作
                IExecAction action = (IExecAction)task.Actions.Create(_TASK_ACTION_TYPE.TASK_ACTION_EXEC);
                action.Path = path;
                //其他设置
                task.Settings.ExecutionTimeLimit = "PT0S";
                task.Settings.DisallowStartIfOnBatteries = false;
                task.Settings.RunOnlyIfIdle = false;

                //注册任务
                IRegisteredTask regTask = folder.RegisterTaskDefinition(
                taskName, //计划任务名称
                task,
                (int)_TASK_CREATION.TASK_CREATE, //创建
                null, //user 
                null, // password 
                _TASK_LOGON_TYPE.TASK_LOGON_INTERACTIVE_TOKEN, //Principal.LogonType
                "");

                IRunningTask runTask = regTask.Run(null);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            button2_Click(null, null);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //ValidTask();
            //Initob();
            this.ShowInTaskbar =false;
            this.Opacity=0;
        }
        
        void Form1Shown(object sender, EventArgs e)
        {
        }
    }
}
