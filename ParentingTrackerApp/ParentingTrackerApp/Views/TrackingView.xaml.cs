﻿using ParentingTrackerApp.ViewModels;
using System.ComponentModel;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
                if (LoggedEntry != null)
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

            EventsList.SelectionChanged += EventsListOnSelectionChanged;
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

        private void EventsListOnSelectionChanged(object sender, SelectionChangedEventArgs e)
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
            MainPage.RedHighlightButtonOnPointerEntered(sender);
        }

        private void RedHighlightButtonOnPointerExited(object sender, PointerRoutedEventArgs args)
        {
            MainPage.RedHighlightButtonOnPointerExited(sender);
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
                e = LastEndTimeBefore(c.NewEvent.StartTime, null);
            }
            else if (c.IsEditing)
            {
                e = LastEndTimeBefore(c.SelectedEvent.StartTime, c.SelectedEvent);
            }
            if (c.EventInEditing != null)
            {
                c.EventInEditing.StartTime = e?.EndTime ?? DateTime.Now;
            }
        }

        private void EndAutoOnClick(object sender, RoutedEventArgs args)
        {
            var c = (CentralViewModel)DataContext;
            EventViewModel e = null;
            if (c.IsCreating)
            {
                e = FirstStartTimeAfter(c.NewEvent.EndTime, null);
            }
            else if (c.IsEditing)
            {
                e = FirstStartTimeAfter(c.SelectedEvent.EndTime, c.SelectedEvent);
            }
            if (c.EventInEditing != null)
            {
                c.EventInEditing.EndTime = e?.StartTime ?? DateTime.Now;
            }
        }

        private EventViewModel LastEndTimeBefore(DateTime dt, EventViewModel but)
        {
            var c = (CentralViewModel)DataContext;
            var logged = c.LoggedEvents.FirstOrDefault(x => x != but && x.EndTime <= dt);
            var running = c.RunningEvents.FirstOrDefault(x => x != but && x.EndTime <= dt);
            if (running == null)
            {
                return logged;
            }
            if (logged == null)
            {
                return running;
            }
            return logged.EndTime > running.EndTime ? logged : running;
        }

        private EventViewModel FirstStartTimeAfter(DateTime dt, EventViewModel but)
        {
            var c = (CentralViewModel)DataContext;
            var logged = c.LoggedEvents.FirstOrDefault(x => x != but && x.StartTime >= dt);
            var running = c.RunningEvents.FirstOrDefault(x => x != but && x.StartTime >= dt);
            if (running == null)
            {
                return logged;
            }
            if (logged == null)
            {
                return running;
            }
            return logged.StartTime < running.StartTime ? logged : running;
        }

        /// <summary>
        ///  Restores the tracking view from group selection
        /// </summary>
        /// <returns>True if it's been restored or false if there's no need to restore</returns>
        public bool Restore()
        {
            if (GroupList.Visibility == Visibility.Visible)
            {
                GroupList.Visibility = Visibility.Collapsed;
                return true;
            }
            return false;
        }

        private void GroupHeaderClicked(object sender, PointerRoutedEventArgs args)
        {
            GroupList.Visibility = Visibility.Visible;
        }

        private void GroupListOnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            var sel = args.AddedItems.FirstOrDefault() as CentralViewModel.GroupName;
            if (sel != null)
            {
                ScrollToGroup(sel.Name);
            }
        }

        private void ScrollToGroup(string name)
        {
            GroupList.Visibility = Visibility.Collapsed;
            if (name != null)
            {
                var c = (CentralViewModel)DataContext;
                var first = c.AllEvents.FirstOrDefault(x => x.GroupName == name);
                if (first != null)
                {
                    EventsList.ScrollIntoView(first, ScrollIntoViewAlignment.Leading);
                }
            }
            GroupList.SelectedItem = null;
        }

        private async void DeleteGroupsButtonOnClick(object sender, RoutedEventArgs args)
        {
            var gn = (CentralViewModel.GroupName)((Button)sender).DataContext;
            var c = (CentralViewModel)DataContext;
            var f = c.AllEvents.FirstOrDefault(x => x.GroupName == gn.Name);
            if (f != null)
            {
                var res = await MainPage.PromptUserToConfirm($"All events that occured {gn.Name} or ealier will be deleted. Are you sure to continue?");
                if (!res)
                {
                    return;
                }
                var index = c.AllEvents.IndexOf(f);
                c.StartFastCollectionChange();
                for (var i = c.AllEvents.Count - 1; i >= index; i--)
                {
                    c.AllEvents.RemoveAt(i);
                }
                c.StopFastCollectionChange();

                if (c.AllEvents.Count == 0 || index == 0)
                {
                    // cleared
                    GroupList.Visibility = Visibility.Collapsed;
                    GroupList.SelectedItem = null;
                }
            }
        }

        #region Testing
#if false

        private void TestOnClick(object sender, RoutedEventArgs e)
        {
            var r = new Random();
            var day = DateTime.Today;
            const int m = 100;
            day = day.AddDays(-m + 1);
            var c = (CentralViewModel)DataContext;
            c.StartFastCollectionChange();
            for (var i = 0; i < m; i++)
            {
                var n = r.Next(6, 12);
                var delta = 24.0 / (n+1);
                var duration = delta / 2;
                var start = day;
                for (var j = 0; j < n; j++)
                {
                    start = start.AddHours(delta);
                    var end = start.AddHours(duration);
                    AddRecord(start, end);
                }
                day = day.AddDays(1);
            }
            c.StopFastCollectionChange();
        }

        private void AddRecord(DateTime startTime, DateTime endTime)
        {
            var c = (CentralViewModel)DataContext;
            var evm = new EventViewModel(c)
            {
                StartTime = startTime,
                EndTime = endTime,
                EventType = c.EventTypes.FirstOrDefault(),
                Notes = ""
            };
            evm.ValidateEndTimeAgainstStartTime();
            evm.Status = EventViewModel.Statuses.Logged;
            c.AllEvents.Insert(evm);
        }

        private void FinaliseAdd()
        {
            var c = (CentralViewModel)DataContext;
            c.NewEvent = null;
            c.UpdateIsEditingAndRelated();
        }

#endif
        #endregion
    }
}
