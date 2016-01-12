﻿using System;
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

            // NOTE from time to time XAML based tech requires this kind of silly hacks to work.
            if (_firstTime)
            {
                DelayHelper.Delay(etvm, Kick, 100);
                _firstTime = false;
            }
            else
            {
                etvm.SelectedColor = etvm.AvailableColors.FirstOrDefault();
            }
        }

        private async void Kick(object state)
        {
            var etvm = (EventTypeViewModel)state;
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                etvm.SelectedColor = etvm.AvailableColors.FirstOrDefault();
            });
        }

        private void DelOnClicked(object sender, RoutedEventArgs e)
        {
            var tvm = (CentralViewModel)DataContext;
            var del = (EventTypeViewModel)((FrameworkElement)sender).DataContext;
            tvm.EventTypes.Remove(del);
        }
    }
}
