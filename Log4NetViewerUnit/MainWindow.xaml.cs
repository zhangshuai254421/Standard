using log4net;
using Log4NetViewer;
using System;
using System.Linq;
using System.Threading.Tasks;
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

            Task.Run(() =>
            {
                log.Error("Log Error !!!", new Exception("Error"));
            });
            log.Fatal("Log Fatal !!! -------------------------------------------------------------------------------666666666666666666666666666666643222222222222222222222222222222222222222", new Exception("Fatal-----------------------------------------------21333333333333333444444444444444444696767532788888888888888888888888888888888888888444444444444444444444444444444444444444444444444444444444444444444444444444"));
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
