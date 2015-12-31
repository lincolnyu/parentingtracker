using ParentingTrackerApp.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ParentingTrackerApp.ViewModels
{
    public class CentralViewModel : BaseViewModel
    {
        private enum States
        {
            Init,
            Synced,
            Dirty
        }

        private States _state = States.Init;
        private string _notes;
        private DateTime _startDate;
        private DateTime _endDate;
        private TimeSpan _startTimeOfDay;
        private TimeSpan _endTimeOfDay;
        private EventTypeViewModel _selectedEventType;
        private EventViewModel _selectedRunningEvent;
        private EventViewModel _selectedLoggedEvent;

        public CentralViewModel()
        {
            EventTypes.CollectionChanged += EventTypesOnCollectionChanged;
            LoggedEvents.CollectionChanged += LoggedEventsOnCollectionChanged;
        }

        public ObservableCollection<EventTypeViewModel> EventTypes { get; } =
            new ObservableCollection<EventTypeViewModel>();

        public ObservableCollection<EventViewModel> RunningEvents { get; }
            = new ObservableCollection<EventViewModel>();

        public ObservableCollection<EventViewModel> LoggedEvents { get; }
            = new ObservableCollection<EventViewModel>();

        public EventViewModel SelectedRunningEvent
        {
            get { return _selectedRunningEvent; }
            set
            {
                if (_selectedRunningEvent != value)
                {
                    _selectedRunningEvent = value;
                    RaisePropertyChangedEvent("SelectedRunningEvent");
                    RaisePropertyChangedEvent("CanStop");
                    AffectPickerEnabled();
                    AffectMutliRoleFields();
                }
            }
        }

        public EventViewModel SelectedLoggedEvent
        {
            get { return _selectedLoggedEvent; }
            set
            {
                if (_selectedLoggedEvent != value)
                {
                    _selectedLoggedEvent = value;
                    FinishEditing(value);
                    RaisePropertyChangedEvent("SelectedLoggedEvent");
                    AffectPickerEnabled();
                    AffectMutliRoleFields();
                }
            }
        }

        public bool CanStop
        {
            get
            {
                return SelectedRunningEvent != null;
            }
        }

        /// <summary>
        ///  enabled when is editing or is going
        /// </summary>
        public bool StartPickerEnabled
        {
            get
            {
                return SelectedRunningEvent != null ||
                    SelectedLoggedEvent != null && SelectedLoggedEvent.IsEditing;
            }
        }

        /// <summary>
        ///  enabled only when is editing
        /// </summary>
        public bool EndPickerEnabled
        {
            get
            {
                return SelectedLoggedEvent != null && SelectedLoggedEvent.IsEditing;
            }
        }

        public EventTypeViewModel SelectedEventType
        {
            get
            {
                return SelectedRunningEvent?.EventType ??
                  (SelectedLoggedEvent != null && SelectedLoggedEvent.IsEditing ?
                  SelectedLoggedEvent.EventType : _selectedEventType);
            }
            set
            {
                if (SelectedRunningEvent != null)
                {
                    SelectedRunningEvent.EventType = value;
                    RaisePropertyChangedEvent("SelectedEventType");
                }
                else if (SelectedLoggedEvent != null && SelectedLoggedEvent.IsEditing)
                {
                    SelectedLoggedEvent.EventType = value;
                    RaisePropertyChangedEvent("SelectedEventType");
                }
                else if (_selectedEventType != value)
                {
                    _selectedEventType = value;
                    RaisePropertyChangedEvent("SelectedEventType");
                }
            }
        }
        public string Notes
        {
            get
            {
                return SelectedRunningEvent?.Notes ??
                  (SelectedLoggedEvent != null && SelectedLoggedEvent.IsEditing ?
                  SelectedLoggedEvent.Notes :
                  _notes);
            }
            set
            {
                if (SelectedRunningEvent != null)
                {
                    SelectedRunningEvent.Notes = value;
                    RaisePropertyChangedEvent("Notes");
                }
                else if (SelectedLoggedEvent != null && SelectedLoggedEvent.IsEditing)
                {
                    SelectedLoggedEvent.Notes = value;
                    RaisePropertyChangedEvent("Notes");
                }
                else if (_notes != value)
                {
                    _notes = value;
                    RaisePropertyChangedEvent("Notes");
                }
            }
        }

        public DateTime StartDate
        {
            get
            {
                return SelectedRunningEvent?.StartDate ??
                  (SelectedLoggedEvent != null && SelectedLoggedEvent.IsEditing ?
                  SelectedLoggedEvent.StartDate :
                  _startDate);
            }
            set
            {
                if (SelectedRunningEvent != null)
                {
                    SelectedRunningEvent.StartDate = value;
                    RaisePropertyChangedEvent("StartDate");
                }
                else if (SelectedLoggedEvent != null && SelectedLoggedEvent.IsEditing)
                {
                    SelectedLoggedEvent.StartDate = value;
                    RaisePropertyChangedEvent("StartDate");
                }
                else if (_startDate != value)
                {
                    _startDate = value;
                    RaisePropertyChangedEvent("StartDate");
                }
            }
        }

        public TimeSpan StartTimeOfDay
        {
            get
            {
                return SelectedRunningEvent?.StartTimeOfDay ??
                  (SelectedLoggedEvent != null && SelectedLoggedEvent.IsEditing ?
                  SelectedLoggedEvent.StartTimeOfDay :
                  _startTimeOfDay);
            }
            set
            {
                if (SelectedRunningEvent != null)
                {
                    SelectedRunningEvent.StartTimeOfDay = value;
                    RaisePropertyChangedEvent("StartTimeOfDay");
                }
                else if (SelectedLoggedEvent != null && SelectedLoggedEvent.IsEditing)
                {
                    SelectedLoggedEvent.StartTimeOfDay = value;
                    RaisePropertyChangedEvent("StartTimeOfDay");
                }
                else if (_startTimeOfDay != value)
                {
                    _startTimeOfDay = value;
                    RaisePropertyChangedEvent("StartTimeOfDay");
                }
            }
        }
        public DateTime EndDate
        {
            get
            {
                return SelectedRunningEvent?.EndDate ??
                  (SelectedLoggedEvent != null && SelectedLoggedEvent.IsEditing ?
                  SelectedLoggedEvent.EndDate :
                  _endDate);
            }
            set
            {
                if (SelectedRunningEvent != null)
                {
                    SelectedRunningEvent.EndDate = value;
                    RaisePropertyChangedEvent("EndDate");
                }
                else if (SelectedLoggedEvent != null && SelectedLoggedEvent.IsEditing)
                {
                    SelectedLoggedEvent.EndDate = value;
                    RaisePropertyChangedEvent("EndDate");
                }
                else if (_endDate != value)
                {
                    _endDate = value;
                    RaisePropertyChangedEvent("EndDate");
                }
            }
        }

        public TimeSpan EndTimeOfDay
        {
            get
            {
                return SelectedRunningEvent?.EndTimeOfDay ??
                  (SelectedLoggedEvent != null && SelectedLoggedEvent.IsEditing ?
                  SelectedLoggedEvent.EndTimeOfDay :
                  _endTimeOfDay);
            }
            set
            {
                if (SelectedRunningEvent != null)
                {
                    SelectedRunningEvent.EndTimeOfDay = value;
                    RaisePropertyChangedEvent("EndTimeOfDay");
                }
                else if (SelectedLoggedEvent != null && SelectedLoggedEvent.IsEditing)
                {
                    SelectedLoggedEvent.EndTimeOfDay = value;
                    RaisePropertyChangedEvent("EndTimeOfDay");
                }
                else if (_endTimeOfDay != value)
                {
                    _endTimeOfDay = value;
                    RaisePropertyChangedEvent("EndTimeOfDay");
                }
            }
        }
        public async Task<bool> Load()
        {
            if (_state == States.Init)
            {
                EventTypes.LoadRoamingColorMapping();
                ResetWithEventTypes();
                var res = await LoggedEvents.LoadEvents(EventTypes);
                LoggedEvents.QuickSort();
                _state = States.Synced;
                return res;
            }
            else
            {
                return true;
            }
        }


        public async Task Save()
        {
            if (_state == States.Dirty)
            {
                EventTypes.SaveRoamingColorMapping();
                await LoggedEvents.SaveEvents();
                _state = States.Synced;
            }
        }

        private void LoggedEventsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Reset)
            {
                SubscribeForLoadedLoggedEvents();
            }
            else
            {
                if (args.OldItems != null)
                {
                    foreach (var oldItem in args.OldItems.Cast<EventViewModel>())
                    {
                        oldItem.PropertyChanged -= LoggedEventPropertyChanged;
                    }
                }
                if (args.NewItems != null)
                {
                    foreach (var newItem in args.NewItems.Cast<EventViewModel>())
                    {
                        newItem.PropertyChanged += LoggedEventPropertyChanged;
                    }
                }
            }

            if (_state == States.Synced)
            {
                _state = States.Dirty;
            }
        }

        private void SubscribeForLoadedLoggedEvents()
        {
            foreach (var loggedEvent in LoggedEvents)
            {
                loggedEvent.PropertyChanged += LoggedEventPropertyChanged;
            }
        }

        public void Start()
        {
            var time = DateTime.Now;
            var evm = new EventViewModel
            {
                StartTime = time,
                EventType = SelectedEventType,
                Notes = Notes
            };
            RunningEvents.Add(evm);
            SelectedRunningEvent = evm;
        }

        public void Stop()
        {
            var sre = SelectedRunningEvent;
            if (sre == null)
            {
                return;
            }
            var t = DateTime.Now;
            sre.EndTime = t;
            RunningEvents.Remove(SelectedRunningEvent);
            AddLogggedEvent(sre);
            SelectedRunningEvent = null; // not needed
        }

        public void Log()
        {
            var time = DateTime.Now;
            var evm = new EventViewModel
            {
                StartTime = time,
                EndTime = time,
                EventType = SelectedEventType,
                Notes = Notes
            };
            AddLogggedEvent(evm);
        }

        /// <summary>
        ///  Use this to add an event as it also makes sure events are in order
        /// </summary>
        /// <param name="evm"></param>
        public void AddLogggedEvent(EventViewModel evm)
        {
            var index = LoggedEvents.BinarySearch(evm);
            if (index < 0)
            {
                index = -index - 1;
            }
            LoggedEvents.Insert(index, evm);
        }

        public void RemoveLoggedEvent(EventViewModel evm)
        {
            LoggedEvents.Remove(evm);
        }

        private void LoggedEventPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "IsEditing")
            {
                var evm = (EventViewModel)sender;
                if (evm.IsEditing)
                {
                    FinishEditing(evm);
                    SelectedLoggedEvent = evm;
                }
                AffectPickerEnabled();
                AffectMutliRoleFields();
            }
        }
        
        private void FinishEditing(EventViewModel evm)
        {
            foreach (var le in LoggedEvents.Where(x => x != evm))
            {
                le.IsEditing = false;
            }
        }

        private void AffectMutliRoleFields()
        {
            RaisePropertyChangedEvent("Notes");
            RaisePropertyChangedEvent("SelectedEventType");
            RaisePropertyChangedEvent("StartDate");
            RaisePropertyChangedEvent("StartTimeOfDay");
            RaisePropertyChangedEvent("EndDate");
            RaisePropertyChangedEvent("EndTimeOfDay");
        }

        private void AffectPickerEnabled()
        {
            RaisePropertyChangedEvent("StartPickerEnabled");
            RaisePropertyChangedEvent("EndPickerEnabled");
        }

        private void EventTypesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            ResetWithEventTypes();

            // TODO validate event references...
            if (args.OldItems != null)
            {
                var set = new HashSet<EventTypeViewModel>();
                foreach (var oi in args.OldItems.Cast<EventTypeViewModel>())
                {
                    set.Add(oi);
                }
                foreach (var ev in RunningEvents)
                {
                    if (set.Contains(ev.EventType))
                    {
                        ev.EventType = null;
                    }
                }
                foreach(var ev in LoggedEvents)
                {
                    if (set.Contains(ev.EventType))
                    {
                        ev.EventType = null;
                    }
                }
            }
        }

        private void ResetWithEventTypes()
        {
            if (EventTypes.Count > 0)
            {
                SelectedEventType = EventTypes[0];
            }
            else
            {
                SelectedEventType = null;
            }
        }
    }
}
