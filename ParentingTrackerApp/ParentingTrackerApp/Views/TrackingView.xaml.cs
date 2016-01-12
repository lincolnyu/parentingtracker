using ParentingTrackerApp.ViewModels;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System;
using System.Threading;
using System.Collections.Generic;
using ParentingTrackerApp.Helpers;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ParentingTrackerApp.Views
{
    public sealed partial class TrackingView : UserControl
    {
        private class LoggedEntryUiAdaptor
        {
            public LoggedEntryUiAdaptor(Grid grid)
            {
                Grid = grid;
            }

            public EventViewModel LoggedEntry { get; private set; }

            public Grid Grid { get; private set; }

            private void LoggedDataContextOnPropertyChanged(object sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName == "IsEditing")
                {
                    UpdateWidth();
                }
            }

            private void UpdateWidth()
            {
                var colWidth = LoggedEntry.IsEditing ? 80 : 40;
                var col1 = Grid.ColumnDefinitions[1];
                col1.MinWidth = colWidth;
                col1.Width = new GridLength(colWidth);
            }

            internal void Unbind()
            {
                if (LoggedEntry  != null)
                {
                    LoggedEntry.PropertyChanged -= LoggedDataContextOnPropertyChanged;
                }
            }

            internal void Rebind(EventViewModel newValue)
            {
                LoggedEntry = newValue;
                if (LoggedEntry!= null)
                {
                    UpdateWidth();
                    LoggedEntry.PropertyChanged += LoggedDataContextOnPropertyChanged;
                }
            }
        }

        private bool _firstTime = true;

        private Dictionary<Grid, LoggedEntryUiAdaptor> _gridAdaptorMap = new Dictionary<Grid, LoggedEntryUiAdaptor>();


        public TrackingView()
        {
            InitializeComponent();

            EventsList.SelectionChanged += LoggedEventsOnSelectionChanged;
        }

        private void DataContextOnChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            UpdateAsPerIsEditingState();
            var dc = (CentralViewModel)DataContext;
            dc.PropertyChanged += ViewModelPropertyChanged;
        }

        private void EditorOnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (_firstTime && args.NewValue != null)
            {
                // NOTE from time to time XAML based tech requires this kind of silly hacks to work.
                DelayHelper.Delay(args.NewValue, Kick, 100);
                _firstTime = false;
            }
        }

        private async void Kick(object state)
        {
            var c = (EventViewModel)state;
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                c.RefreshEventTypeProperties();
            });
        }

        private void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "IsEditing")
            {
                UpdateAsPerIsEditingState();
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

        private void LoggedEventsOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EventsList.SelectedItem != null)
            {
                EventsList.ScrollIntoView(EventsList.SelectedItem);
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

        private void CancelOnClick(object sender, RoutedEventArgs e)
        {
            var c = (CentralViewModel)DataContext;
            c.Cancel();
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
            central.RemoveEvent(e);
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

        private void GridDataContextOnChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            var grid = (Grid)sender;
            LoggedEntryUiAdaptor adaptor = null;
            if (_gridAdaptorMap.TryGetValue(grid, out adaptor))
            {
                adaptor.Unbind();
            }
            if (args.NewValue != null)
            {
                if (adaptor == null)
                {
                    adaptor = new LoggedEntryUiAdaptor(grid);
                    _gridAdaptorMap[grid] = adaptor;
                }
                adaptor.Rebind((EventViewModel)args.NewValue);
            }
            else
            {
                _gridAdaptorMap.Remove(grid);
            }
        }
    }
}
