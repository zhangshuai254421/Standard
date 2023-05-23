using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace Log4NetViewer
{
    public class LoggingEventViewModel : INotifyPropertyChanged
    {
        public LoggingEventLight LoggingEvent { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Display the Details Button if the LoggingEvent contains an exception.
        /// </summary>
        public Visibility DetailsButtonVisibility
        {
            get
            {
                return string.IsNullOrEmpty(LoggingEvent.Exception) ? Visibility.Hidden : Visibility.Visible;
            }
        }

        private bool _displayDetails = false;
        /// <summary>
        /// Binded to the Detail button to toogle the 
        /// </summary>
        public bool DisplayDetails
        {
            get
            {
                return _displayDetails;
            }
            set
            {
                if (_displayDetails == value)
                {
                    return;
                }

                _displayDetails = value;
                RaisePropertyChanged("DetailsVisibility");
            }
        }

        /// <summary>
        /// Display the Details Textblock according to the DisplayDetails property.
        /// </summary>
        public Visibility DetailsVisibility
        {
            get
            {
                return DisplayDetails ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }

}
