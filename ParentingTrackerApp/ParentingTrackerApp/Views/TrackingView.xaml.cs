using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using ParentingTrackerApp.ViewModels;
using System;
using System.ComponentModel;
using System.Collections.Specialized;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ParentingTrackerApp.Views
{
    public sealed partial class TrackingView : UserControl
    {
        public TrackingView()
        {
            InitializeComponent();

            DataContextChanged += DataContextOnChanged;

            RunningEventsList.SelectionChanged += RunningEventsOnSelectionChanged;
            LoggedEventsList.SelectionChanged += LoggedEventsOnSelectionChanged;
        }

        private void DataContextOnChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            UpdateAsPerRunningItems();
            UpdateAsPerIsEditingState();
            var dc = (CentralViewModel)DataContext;
            dc.PropertyChanged += ViewModelPropertyChanged;
            dc.RunningEvents.CollectionChanged += RunningEventsOnCollectionChanged;
        }

        private void RunningEventsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateAsPerRunningItems();
        }

        private void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "IsEditing")
            {
                UpdateAsPerIsEditingState();
            }
        }

        private void UpdateAsPerRunningItems()
        {
            var dc = (CentralViewModel)DataContext;
            if (dc.RunningEvents.Count > 0)
            {
                RunningRow.MinHeight = 60;
                RunningRow.Height = new GridLength(0.2, GridUnitType.Star);
            }
            else
            {
                RunningRow.MinHeight = 0;
                RunningRow.Height = new GridLength(0);
            }
        }

        private void UpdateAsPerIsEditingState()
        {
            var dc = (CentralViewModel)DataContext;
            if (dc.IsEditing)
            {
                EditorRow.MinHeight = 150;
                EditorRow.Height = new GridLength(0.3, GridUnitType.Star);
            }
            else
            {
                EditorRow.MinHeight = 0;
                EditorRow.Height = new GridLength(0);
            }
        }

        private void RunningEventsOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RunningEventsList.SelectedItem != null)
            {
                RunningEventsList.ScrollIntoView(RunningEventsList.SelectedItem);
            }
        }

        private void LoggedEventsOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LoggedEventsList.SelectedItem != null)
            {
                LoggedEventsList.ScrollIntoView(LoggedEventsList.SelectedItem);
            }
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

        private void CloseEditorOnClick(object sender, RoutedEventArgs e)
        {
            var c = (CentralViewModel)DataContext;
            c.CloseEditor();
        }
    }
}
