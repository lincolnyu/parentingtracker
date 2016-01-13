using System;
using ParentingTrackerApp.Helpers;
using ParentingTrackerApp.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Linq;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ParentingTrackerApp.Views
{
    public sealed partial class CustomizingView : UserControl
    {
        private bool _firstTime = true;

        public CustomizingView()
        {
            InitializeComponent();
        }

        private void AddOnClicked(object sender, RoutedEventArgs e)
        {
            var tvm = (CentralViewModel)DataContext;
            var etvm = new EventTypeViewModel();
            tvm.EventTypes.Add(etvm);

            // TODO may actually want to put this in the view model
            // NOTE from time to time XAML based tech requires this kind of silly hacks to work.
            if (_firstTime)
            {
                DelayHelper.Delay(etvm, Kick, 100, Dispatcher);
                _firstTime = false;
            }
            else
            {
                etvm.SelectedColor = etvm.AvailableColors.FirstOrDefault();
            }
        }

        private void Kick(object state)
        {
            var etvm = (EventTypeViewModel)state;
            etvm.SelectedColor = etvm.AvailableColors.FirstOrDefault();
        }

        private void DelOnClicked(object sender, RoutedEventArgs e)
        {
            var tvm = (CentralViewModel)DataContext;
            var del = (EventTypeViewModel)((FrameworkElement)sender).DataContext;
            tvm.EventTypes.Remove(del);
        }

        private void ResetOnClicked(object sender, RoutedEventArgs e)
        {
            var tvm = (CentralViewModel)DataContext;
            tvm.ResetEventTypesToDefault();
        }

        private void GridOnSizeChanged(object sender, SizeChangedEventArgs args)
        {
            ButtonRow.Height = new GridLength(AddButton.ActualHeight);
        }
    }
}
