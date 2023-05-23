using log4net.Appender;
using log4net.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Log4NetViewer
{
    public class WPFViewerAppender : AppenderSkeleton
    {
        public MTObservableCollection<LoggingEventViewModel> Events { get; set; } = new MTObservableCollection<LoggingEventViewModel>();

        // public BlockingCollection<LoggingEventViewModel> Events { get; set; } = new BlockingCollection<LoggingEventViewModel>();
        protected override void Append(LoggingEvent loggingEvent)
        {
            var loggingEventTemp = new LoggingEventLight(loggingEvent);

            Events.Add(new LoggingEventViewModel() { LoggingEvent = loggingEventTemp });
            //Application.Current.Dispatcher.Invoke(new Action(() =>
            //{
            //    Events.Add(new LoggingEventViewModel() { LoggingEvent = loggingEventTemp });

            //}));
            //ThreadPool.QueueUserWorkItem(delegate
            //{
            //    SynchronizationContext.SetSynchronizationContext(new
            //        DispatcherSynchronizationContext(System.Windows.Application.Current.Dispatcher));
            //    SynchronizationContext.Current.Post(pl =>
            //    {
            //        //里;面写真正的业务内容
            //        Events.Add(new LoggingEventViewModel() { LoggingEvent = loggingEventTemp });
            //    }, null);
            //});




        }
    }
    public class MTObservableCollection<T> : ObservableCollection<T>
    {
        public override event NotifyCollectionChangedEventHandler CollectionChanged;
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler CollectionChanged = this.CollectionChanged;
            if (CollectionChanged != null)
                foreach (NotifyCollectionChangedEventHandler nh in CollectionChanged.GetInvocationList())
                {
                    DispatcherObject dispObj = nh.Target as DispatcherObject;
                    if (dispObj != null)
                    {
                        Dispatcher dispatcher = dispObj.Dispatcher;
                        if (dispatcher != null && !dispatcher.CheckAccess())
                        {
                            dispatcher.BeginInvoke(
                                (Action)(() => nh.Invoke(this,
                                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset))),
                                DispatcherPriority.DataBind);
                            continue;
                        }
                    }
                    nh.Invoke(this, e);
                }
        }
    }
    /// <summary>
    /// A simplified class from log4net.Core.LoggingEventData
    /// </summary>
    public class LoggingEventLight
    {
        public string Logger { get; set; }
        public DateTime Timestamp { get; set; }
        public string Level { get; set; }
        public string Thread { get; set; }
        public string Domain { get; set; }
        public string Username { get; set; }

        public string Message { get; set; }
        public string Exception { get; set; }

        public string Location { get; set; }

        public LoggingEventLight()
        {

        }

        public LoggingEventLight(log4net.Core.LoggingEvent log4netLoggingEvent)
        {
            Logger = log4netLoggingEvent.LoggerName;
            Timestamp = log4netLoggingEvent.TimeStamp;
            Level = log4netLoggingEvent.Level.Name;
            Thread = log4netLoggingEvent.ThreadName;
            Domain = log4netLoggingEvent.Domain;
            Username = log4netLoggingEvent.UserName;
            Message = log4netLoggingEvent.RenderedMessage;
            Exception = log4netLoggingEvent.GetExceptionString();
            if (log4netLoggingEvent.LocationInformation != null && log4netLoggingEvent.LocationInformation.FileName != null)
            {
                Location = log4netLoggingEvent.LocationInformation.FileName.Substring(log4netLoggingEvent.LocationInformation.FileName.LastIndexOf("\\") + 1) + ":" + log4netLoggingEvent.LocationInformation.LineNumber;
            }
        }
    }

}
