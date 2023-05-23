using log4net.Appender;
using log4net.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log4NetViewer
{
    public class WPFViewerAppender : AppenderSkeleton
    {
        public ObservableCollection<LoggingEventViewModel> Events { get; set; } = new ObservableCollection<LoggingEventViewModel>();
        protected override void Append(LoggingEvent loggingEvent)
        {
            var loggingEventTemp = new LoggingEventLight(loggingEvent);
            Events.Add(new LoggingEventViewModel() { LoggingEvent = loggingEventTemp });
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
