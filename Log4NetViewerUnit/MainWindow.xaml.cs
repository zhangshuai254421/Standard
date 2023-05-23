using log4net;
using Log4NetViewer;
using System;
using System.Linq;
using System.Windows;

namespace Log4NetViewerUnit
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MainWindow));
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }
        private void LogFatal_Click(object sender, RoutedEventArgs e)
        {
            log.Fatal("Log Fatal !!!", new Exception("Fatal-----------------------"));
        }

        private void LogError_Click(object sender, RoutedEventArgs e)
        {
            log.Error("Log Error !!!", new Exception("Error"));
        }

        private void LogWarn_Click(object sender, RoutedEventArgs e)
        {
            log.Warn("Log Warn !!!");
        }

        private void LogInfo_Click(object sender, RoutedEventArgs e)
        {
            log.Info("Log Info !!! 这是一共Info类型的日志");
        }

        private void LogDebug_Click(object sender, RoutedEventArgs e)
        {
            log.Debug("Log Debug !!!");
        }
        public WPFViewerAppender Appender
        {
            get
            {

                return LogManager.GetRepository().GetAppenders().FirstOrDefault(a => a is WPFViewerAppender) as WPFViewerAppender;
            }
        }
    }
}
