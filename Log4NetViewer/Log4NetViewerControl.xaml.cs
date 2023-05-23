using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Log4NetViewer
{
    /// <summary>
    /// Log4NetViewer.xaml 的交互逻辑
    /// </summary>
    public partial class Log4NetViewerControl : UserControl
    {
        public Log4NetViewerControl()
        {
            InitializeComponent();
        }
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource",
                typeof(IEnumerable),
                typeof(Log4NetViewerControl),
                new PropertyMetadata(null));

        private void ListBox_ScrollChanged(object sender, RoutedEventArgs e)
        {
            if (((ScrollChangedEventArgs)e).ExtentHeightChange > 0.0)
                ((ScrollViewer)e.OriginalSource).ScrollToEnd();
        }

    }
}
