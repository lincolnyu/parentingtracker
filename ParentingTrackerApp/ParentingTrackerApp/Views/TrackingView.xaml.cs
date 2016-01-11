using ParentingTrackerApp.ViewModels;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using System;
using System.Linq;
using System.Threading;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ParentingTrackerApp.Views
{
    public sealed partial class TrackingView : UserControl
    {
        private bool _firstTime = true;

        private class LoggedEntryUiAdaptor
        {
            public LoggedEntryUiAdaptor(Grid grid)
            {
                Grid = grid;
                LoggedEntry = (EventViewModel)grid.DataContext;
                if (LoggedEntry != null)
                {
                    Initialise();
                }
                else
                {
                    Grid.DataContextChanged += GridOnUpdateDataContext;
                }
            }

            public EventViewModel LoggedEntry { get; private set; }

            public ToggleButton EditButton { get; private set; }

            public Button RemoveButton { get; private set; }

            public Grid Grid { get; private set; }

            private void Initialise()
            {
                EditButton = (ToggleButton)Grid.FindName("EditButton");
                RemoveButton = (Button)Grid.FindName("RemoveButton");
               
                UpdateWidth();
                Grid.Unloaded += GridOnUnloaded;
                LoggedEntry.PropertyChanged += LoggedDataContextOnPropertyChanged;
            }

            private void GridOnUnloaded(object sender, RoutedEventArgs e)
            {
                Grid.Unloaded -= GridOnUnloaded;
                if (LoggedEntry != null)
                {
                    LoggedEntry.PropertyChanged -= LoggedDataContextOnPropertyChanged;
                }
                else
                {
                    Grid.DataContextChanged -= GridOnUpdateDataContext;
                }
            }

            private void GridOnUpdateDataContext(FrameworkElement sender, DataContextChangedEventArgs args)
            {
                if (args.NewValue != null)
                {
                    LoggedEntry = (EventViewModel)args.NewValue;
                    Initialise();
                    Grid.DataContextChanged -= GridOnUpdateDataContext;
                }
            }

            public void LoggedDataContextOnPropertyChanged(object sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName == "IsEditing")
                {
                    UpdateWidth();
                }
            }

            private void UpdateWidth()
            {
                var colWidth = LoggedEntry.IsEditing ? RemoveButton.ActualWidth + EditButton.ActualWidth
                    : EditButton.ActualWidth;
                var col1 = Grid.ColumnDefinitions[1];
                col1.Width = new GridLength(colWidth);
            }
        }

        public TrackingView()
        {
            InitializeComponent();

            DataContextChanged += DataContextOnChanged;

            EventsList.SelectionChanged += LoggedEventsOnSelectionChanged;
        }

        private void DataContextOnChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            UpdateAsPerIsEditingState();
            var dc = (CentralViewModel)DataContext;
            dc.PropertyChanged += ViewModelPropertyChanged;
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
            if (_firstTime)
            {
                // NOTE from time to time XAML based tech requires this kind of silly hacks to work.
                new Timer(Hack, c, 100, Timeout.Infinite);
                _firstTime = false;
            }
        }

        private async void Hack(object state)
        {
            var c = (CentralViewModel)state;
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                c.EventInEditing.RefreshEventTypeProperties();
            });
        }

        private void CloseEditorOnClick(object sender, RoutedEventArgs e)
        {
            var c = (CentralViewModel)DataContext;
            c.CloseEditor();
        }

        private void LoggedEntryGridOnLoaded(object sender, RoutedEventArgs e)
        {
            var grid = (Grid)sender;
            var lv = EventsList;
            new LoggedEntryUiAdaptor(grid);
        }
    }
}
