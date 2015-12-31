using ParentingTrackerApp.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ParentingTrackerApp.Views
{
    public sealed partial class CustomizingView : UserControl
    {
        public CustomizingView()
        {
            InitializeComponent();
        }

        private void AddOnClicked(object sender, RoutedEventArgs e)
        {
            var tvm = (CentralViewModel)DataContext;
            var etvm = new EventTypeViewModel();
            tvm.EventTypes.Add(etvm);
        }

        private void DelOnClicked(object sender, RoutedEventArgs e)
        {
            var tvm = (CentralViewModel)DataContext;
            var del = (EventTypeViewModel)((FrameworkElement)sender).DataContext;
            tvm.EventTypes.Remove(del);
        }
    }
}
