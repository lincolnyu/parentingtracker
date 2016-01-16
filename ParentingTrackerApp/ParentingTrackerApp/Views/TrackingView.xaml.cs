using ParentingTrackerApp.ViewModels;
using System.ComponentModel;
using Windows.UI;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using System.Collections.Generic;
using ParentingTrackerApp.Helpers;
using Windows.UI.Xaml.Controls.Primitives;
using System.Linq;
using System;

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
                if (LoggedEntry != null)
                {
                    LoggedEntry.PropertyChanged -= LoggedDataContextOnPropertyChanged;
                    LoggedEntry = null;
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

        private Brush _prevColor;

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
                // TODO may actually want to put this in the view model
                // NOTE from time to time XAML based tech requires this kind of silly hacks to work.
                DelayHelper.Delay(args.NewValue, Kick, 100, Dispatcher);
                _firstTime = false;
            }
        }

        private void Kick(object state)
        {
            var c = (EventViewModel)state;
            c.RefreshEventTypeProperties();
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
                EditorRow.Height = new GridLength(145);
            }
            else
            {
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

        private void RedHighlightButtonOnPointerEntered(object sender, PointerRoutedEventArgs args)
        {
            _prevColor = ((Button)sender).Background;
            ((Button)sender).Background = new SolidColorBrush(Colors.Red);
        }

        private void RedHighlightButtonOnPointerExited(object sender, PointerRoutedEventArgs args)
        {
            ((Button)sender).Background = _prevColor;
        }

        private void DateTimeButtonOnClick(object sender, RoutedEventArgs args)
        {
            var senderElement = (FrameworkElement)sender;
            var flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
            flyoutBase.ShowAt(senderElement);
        }

        private void StartAutoOnClick(object sender, RoutedEventArgs args)
        {
            var c = (CentralViewModel)DataContext;
            EventViewModel e = null;
            if (c.IsCreating)
            {
                e = c.AllEvents.FirstOrDefault(x => x.IsLoggedEvent && x.EndTime <= DateTime.Now);
            }
            else if (c.IsEditing)
            {
                e = c.AllEvents.FirstOrDefault(x => x.IsLoggedEvent && x.EndTime <= c.SelectedEvent.StartTime);
            }
            if (e != null && c.EventInEditing != null)
            {
                c.EventInEditing.StartTime = e.EndTime;
            }
        }

        private void EndAutoOnClick(object sender, RoutedEventArgs args)
        {
            var c = (CentralViewModel)DataContext;
            if (c.IsEditing)
            {
                var e = c.AllEvents.Reverse().FirstOrDefault(x => x.IsLoggedEvent && x.StartTime >= c.SelectedEvent.EndTime);
                if (e != null)
                {
                    c.SelectedEvent.EndTime = e.StartTime;
                }
            }
        }
    }
}
