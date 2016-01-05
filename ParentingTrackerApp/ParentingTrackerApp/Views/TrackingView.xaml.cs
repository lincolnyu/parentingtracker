using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using ParentingTrackerApp.ViewModels;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ParentingTrackerApp.Views
{
    public sealed partial class TrackingView : UserControl
    {
        public TrackingView()
        {
            InitializeComponent();
        }

        private void StartOnClick(object sender, RoutedEventArgs args)
        {
            var c = (CentralViewModel)DataContext;
            c.Start();
        }

        private void StopOnClick(object sender, RoutedEventArgs e)
        {
            var c = (CentralViewModel)DataContext;
            c.Stop();
        }

        private void LogOnClick(object sender, RoutedEventArgs args)
        {
            var c = (CentralViewModel)DataContext;
            c.Log();
        }

        private void RemoveLoggedOnClick(object sender, RoutedEventArgs args)
        {
            var e = (EventViewModel)((FrameworkElement)sender).DataContext;
            var central = (CentralViewModel)DataContext;
            central.RemoveLoggedEvent(e);
        }

        private void NewOnClick(object sender, RoutedEventArgs e)
        {
            var c = (CentralViewModel)DataContext;
            c.New();
        }
    }
}
