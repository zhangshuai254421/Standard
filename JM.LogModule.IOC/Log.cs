
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading;


[assembly: log4net.Config.DOMConfigurator(ConfigFileExtension = "log4net.config", Watch = true)]
namespace JM.LogModule.IOC
{
    public interface ILog
    {
    }

    public class Log : ILog
    {
        /// <summary>
        /// 程序正在加载中
        /// </summary>
        public  bool IsStarting = false;

        public string Name { get;set; }
        /// <summary>
        /// 加载过程中错误信息列表
        /// </summary>
        public  List<string> ErrMsg = new List<string>();

        /// <summary>
        /// 全局日志 "logLogger"与log4net.config配置里的名称一致
        /// </summary>
        private log4net.ILog _log4Net;

        /// <summary>
        /// 模块参数修改记录
        /// </summary>
      //  private static readonly ILog _logModify = LogManager.GetLogger("JMModifyLogger");

        /// <summary>
        /// 模块参数修改记录
        /// </summary>
       // private static readonly ILog _logException = LogManager.GetLogger("JMModifyLogger");

        /// <summary>
        /// 日志清理线程变量
        /// </summary>
        private  Thread _cleanThread = null;

        /// <summary>
        /// 日志保留天数字段和属性
        /// </summary>
        private  volatile int _logRemainDays = 10;

        public  int LogRemainDays
        {
            get { return _logRemainDays; }
            set { _logRemainDays = value; }
        }
        public Log()
        {

        }
        public Log(string LoggerName)
        {
            IsStarting = true;

            string str = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".log4net.config";
            Stream stream = System.Reflection.Assembly.GetExecutingAssembly()?.GetManifestResourceStream(str);
            log4net.Config.XmlConfigurator.Configure(stream);


            if (LoggerName.Length > 0)
            {
                _log4Net = LogManager.GetLogger(LoggerName);
            }
            else
            {
                _log4Net = LogManager.GetLogger("JMModifyLogger");
            }
        }
        /// <summary>
        /// 富文本显示
        /// </summary>
      //  private static RichTextBoxBaseAppender list_logAppender = new RichTextBoxBaseAppender();

        public  void RegisterLog(string LogName="")
        {
           
           
            //按理说可以通过配置log4net来达到天数控制,但是这个有时候会调整天数,故自己写逻辑
            DeleteLog(10);//删除10天的日志
        }

        /// <summary>
        /// /只需要在窗口初始化的时候注册RichTextBox就可以了,意思是让RichTextBox这个控件也显示日志信息
        /// </summary>
        /// <param name="richTextBox_log"></param>
        //public static void InitializeRichTextBox(System.Windows.Controls.RichTextBox richTextBox)
        //{
        //    if (richTextBox == null) return;

        //    // 设置listview打印日志
        //    //var logPattern = "%d{yyyy-MM-dd HH:mm:ss.fff} --%-5p-- %m%n"; //lzpE 想显示到ms加上fff则日志栏无法显示，但同样的格式写文件正常
        //    var logPattern = "%d{yyyy-MM-dd HH:mm:ss} --%-5p-- %m%n";
        //    list_logAppender.RichTextBox = richTextBox;
        //    list_logAppender.Layout = new PatternLayout(logPattern);

        //    Logger log4NetLogger = _log4Net.Logger as Logger;//单独配置appender
        //    log4NetLogger.AddAppender(list_logAppender);
        //}

        //public static void Alert(string msg, Form_Alert.enmType type)
        //{
        //    Form_Alert frm = new Form_Alert();
        //    frm.showAlert(msg, type);
        //}

        public  void Debug(string str)
        {
            _log4Net.Debug(str.Trim());
            //Alert(str, Form_Alert.enmType.Info);
        }

        public  void Info(string str)
        {
            _log4Net.Info(str.Trim());
            //Alert(str, Form_Alert.enmType.Info);
        }

        public  void Warn(string str)
        {
            if (IsStarting)
            {
                if (!ErrMsg.Contains(str)) ErrMsg.Add(str);//窗体加载过程中 需要弹窗提示
            }
           
            _log4Net.Warn(str.Trim());
         //   throw new Exception(str);
            //Alert(str, Form_Alert.enmType.Warning);
        }

        public  void Error(string str)
        {
            if (IsStarting)
            {
                if (!ErrMsg.Contains(str)) ErrMsg.Add(str);//窗体加载过程中 需要弹窗提示
            }
           
            _log4Net.Error(str.Trim());
         //   throw new Exception(str);
            //Alert(str, Form_Alert.enmType.Error);
        }

        public  void Fatal(string str)
        {
            if (IsStarting)
            {
                if (!ErrMsg.Contains(str)) ErrMsg.Add(str);//窗体加载过程中 需要弹窗提示
            }
           
            _log4Net.Fatal(str.Trim());
           // throw new Exception(str);
            //Alert(str, Form_Alert.enmType.Error);
        }

        ///// <summary>
        ///// 记录模块参数变更记录<br />
        ///// 用于记录额外的一些重要记录,比如关键参数修改记录-->记录在log/modify文件下
        ///// </summary>
        ///// <param name="str"></param>
        //public static void ModifyPara(string str)
        //{
        //    _logModify.Info(str);
        //}

        ///// <summary>
        ///// 保存异常日志
        ///// </summary>
        ///// <param name="str"></param>
        //public static void SaveException(string str)
        //{
        //    _logException.Error(str + "\r\n");
        //}

        public  void Print(LogLevel LogLevel, string str)
        {
            switch (LogLevel)
            {
                case LogLevel.Debug:
                    {
                        this.Debug(str);
                        break;
                    }
                case LogLevel.Info:
                    {
                        this.Info(str);
                        break;
                    }
                case LogLevel.Warn:
                    {
                        this.Warn(str);
                        break;
                    }
                case LogLevel.Error:
                    {
                        this.Error(str);
                        break;
                    }
                case LogLevel.Fatal:
                    {
                        this.Fatal(str);
                        break;
                    }
                default:
                    {
                        this.Debug(str);
                        break;
                    }
            }
        }

        /// <summary>
        /// 获取日志存储文件夹
        /// </summary>
        /// <returns></returns>
        private  string GetFolder()
        {
            Logger log4NetLogger = _log4Net.Logger as Logger;
            var appender = log4NetLogger.GetAppender("LogFile") as RollingFileAppender;
            return Path.GetDirectoryName(appender.File);
        }

        public  void DeleteLog(int dayNum)
        {
            try
            {
                DateTime tempDate;
                DirectoryInfo dir;
                FileInfo[] fileInfo;
                //删除普通日志
                dir = new DirectoryInfo(GetFolder());
                fileInfo = dir.GetFiles();
                foreach (FileInfo NextFile in fileInfo)
                {
                    tempDate = NextFile.LastWriteTime;
                    int days = (DateTime.Now - tempDate).Days;
                    // 删除指定天前
                    if (days >= dayNum)
                        File.Delete(NextFile.FullName);
                }
                //删除修改参数记录日志
                dir = new DirectoryInfo(GetFolder() + "\\modify\\");
                fileInfo = dir.GetFiles();
                foreach (FileInfo NextFile in fileInfo)
                {
                    tempDate = NextFile.LastWriteTime;
                    int days = (DateTime.Now - tempDate).Days;
                    // 删除指定天前
                    if (days >= dayNum)
                        File.Delete(NextFile.FullName);
                }
                //删除异常日志
                dir = new DirectoryInfo(GetFolder() + "\\exception\\");
                fileInfo = dir.GetFiles();
                foreach (FileInfo NextFile in fileInfo)
                {
                    tempDate = NextFile.LastWriteTime;
                    int days = (DateTime.Now - tempDate).Days;
                    // 删除指定天前
                    if (days >= 30)
                        File.Delete(NextFile.FullName);
                }
            }
            catch (Exception ex)
            {
                Error(ex.ToString());
            }
        }

        /// <summary>
        /// 更新显示日志等级
        /// </summary>
        /// <param name="level">等级</param>
        public  void UpdateDispMinRade(string strRade)
        {
            LogLevel level = LogLevel.Debug;
            switch (strRade)
            {
                case "Debug":
                    {
                        level = LogLevel.Debug;
                        break;
                    }
                case "Info":
                    {
                        level = LogLevel.Info;
                        break;
                    }
                case "Warn":
                    {
                        level = LogLevel.Warn;
                        break;
                    }
                case "Error":
                    {
                        level = LogLevel.Error;
                        break;
                    }
                case "Fatal":
                    {
                        level = LogLevel.Fatal;
                        break;
                    }
                default:
                    {
                        level = LogLevel.Debug;
                        break;
                    }
            }
           // list_logAppender.DispMinRade = level;
        }

        /// <summary>
        /// 开启日志自动清理线程
        /// </summary>
        public  void StartCleanThread()
        {
            if (_cleanThread == null)
            {
                _cleanThread = new Thread(new ParameterizedThreadStart(delegate
                {
                    while (true)
                    {
                        DeleteLog(_logRemainDays);
                        //Thread.Sleep(12 * 3600 * 60);   //12h//日志刷新周期是300ms 避免 native刷新太块 抢占了主线程
                        Task.Delay(12 * 3600 * 60).Wait();
                    }
                }));
                _cleanThread.IsBackground = true;
                _cleanThread.Start();
            }
        }
    }
    public enum LogLevel
    {
        Debug = 1,
        Info,
        Warn,
        Error,
        Fatal
    }
}
