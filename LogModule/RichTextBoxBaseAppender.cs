using log4net.Appender;
using log4net.Layout;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;


namespace LogModule
{
    //public class RichTextBoxBaseAppender : AppenderSkeleton
    //{
    //    public System.Windows.Controls.RichTextBox RichTextBox { get; set; }
        
    //    /// <summary>
    //    /// 日志显示等级字段和属性
    //    /// </summary>
    //    private volatile LogLevel _dispMinRade = LogLevel.Debug;
    //    public LogLevel DispMinRade 
    //    {
    //        get { return _dispMinRade; }
    //        set { _dispMinRade = value; }
    //    }

    //    static RichTextBoxBaseAppender()
    //    {
    //    }

    //    protected override void Append(log4net.Core.LoggingEvent loggingEvent)
    //    {
    //        PatternLayout patternLayout = (PatternLayout)this.Layout;
    //        string str = string.Empty;

    //        if (patternLayout != null)
    //        {
    //            str = patternLayout.Format(loggingEvent);

    //            if (loggingEvent.ExceptionObject != null)
    //                str += loggingEvent.ExceptionObject.ToString();
    //        }
    //        else
    //            str = loggingEvent.LoggerName + "-" + loggingEvent.RenderedMessage;

    //        // 打印
    //        PrintfLog(str);
    //    }

    //    /// <summary>
    //    /// 线程开启
    //    /// </summary>
    //    private bool _flag = false;

    //    private List<string> _logStrList = new List<string>();

    //    private object _lockObj = new object();

    //    private void PrintfLog(string str)
    //    {
    //        lock (_lockObj)
    //        {
    //            _logStrList.Add(str);
    //        }

    //        if (_flag == false)
    //        {
    //            _flag = true;
    //            Task.Run(() =>
    //            {
    //                while (true)
    //                {
    //                    Task.Delay(300).Wait();//日志刷新周期是300ms 避免 native刷新太块 抢占了主线程
    //                    //Thread.Sleep(300);
    //                    if (_logStrList.Count == 0) continue;

    //                    List<string> tempList = new List<string>();
    //                    lock (_lockObj)
    //                    {
    //                        for (int i = 0; i < _logStrList.Count; i++)
    //                        {
    //                            tempList.Add(_logStrList[i]);
    //                        }
    //                        _logStrList.Clear();
    //                    }

    //                    if (tempList.Count > 0)
    //                    {
    //                        try
    //                        {
    //                            this.RichTextBox.Dispatcher.Invoke(new Action(() =>
    //                            {
    //                                try
    //                                {
    //                                    System.Windows.Documents.FlowDocument doc = RichTextBox.Document;
    //                                    if (doc.Blocks.Count > 1000)
    //                                    {
    //                                        doc.Blocks.Clear();
    //                                    }

    //                                    foreach (var strTemp in tempList)
    //                                    {
    //                                        Paragraph paragraph = new Paragraph();
    //                                        string o = strTemp.Substring(22, 1);
    //                                        LogLevel level = LogLevel.Debug;
    //                                        switch (o)
    //                                        {
    //                                            case "D":
    //                                                level = LogLevel.Debug;
    //                                                paragraph.Foreground = System.Windows.Media.Brushes.WhiteSmoke;//Color.Black;
    //                                                break;

    //                                            case "I":
    //                                                level = LogLevel.Info;
    //                                                paragraph.Foreground = System.Windows.Media.Brushes.WhiteSmoke; //Color.Black;
    //                                                break;

    //                                            case "W":
    //                                                level = LogLevel.Warn;
    //                                                paragraph.Foreground = System.Windows.Media.Brushes.DarkOrange; //Color.DarkOrange;
    //                                                break;

    //                                            case "E":
    //                                                level = LogLevel.Error;
    //                                                paragraph.Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#E24E26")); //Color.Red;
    //                                                break;

    //                                            case "F":
    //                                                level = LogLevel.Fatal;
    //                                                paragraph.Foreground = System.Windows.Media.Brushes.Red; //Color.Red;
    //                                                break;

    //                                            default:
    //                                                paragraph.Foreground = System.Windows.Media.Brushes.WhiteSmoke; //Color.Black;
    //                                                break;
    //                                        }
    //                                        //高于最低等级的日志才会显示
    //                                        if (level >= DispMinRade)
    //                                        {
    //                                            string[] strArray = strTemp.Split("\r\n");
    //                                            paragraph.LineHeight = 1;
    //                                            paragraph.Inlines.Add(strArray[0]);
    //                                            this.RichTextBox.Document.FontSize = System.Windows.SystemFonts.MessageFontSize;
    //                                            this.RichTextBox.Document.Blocks.Add(paragraph);

    //                                            //RichTextBox.AppendText(strTemp);
    //                                            //RichTextBox.SelectionStart = RichTextBox.TextLength;
    //                                            //RichTextBox.ScrollToCaret(); //Caret意思：脱字符号；插入符号; (^)
    //                                        }
    //                                    }
    //                                    this.RichTextBox.ScrollToEnd();
    //                                }
    //                                catch (Exception e)
    //                                {
    //                                    throw;
    //                                }
    //                            }));
    //                        }
    //                        catch (TaskCanceledException)
    //                        {
    //                            //TaskCanceledException 异常是关闭时候可能出现 直接忽略 yoga  2019-9-8 06:46:55
    //                        }
    //                        catch (Exception e)
    //                        {
    //                            //throw;
    //                        }
    //                    }
    //                }
    //            });
    //        }
    //    }
    //}
}