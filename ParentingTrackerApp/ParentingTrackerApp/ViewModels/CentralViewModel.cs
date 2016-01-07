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
        #region Enumerations

        private enum States
        {
            Init,
            Synced,
            Dirty
        }

        #endregion

        #region Fields

        private States _state = States.Init;
        private string _notes;
        private DateTime _startDate;
        private DateTime _endDate;
        private TimeSpan _startTimeOfDay;
        private TimeSpan _endTimeOfDay;
        private EventTypeViewModel _selectedEventType;
        private EventViewModel _selectedRunningEvent;
        private EventViewModel _selectedLoggedEvent;
        private string _exportPath;
        private string _exportFileToken;

        private bool _suppressSorting;
        private bool _inLoggedCollectionChangedHandler;
        private bool _isEditing;
        private bool _wasLoggedSelectedBeforeEditing;

        #endregion

        #region Constructors

        public CentralViewModel()
        {
            EventTypes.CollectionChanged += EventTypesOnCollectionChanged;
            LoggedEvents.CollectionChanged += LoggedEventsOnCollectionChanged;
        }

        #endregion

        #region Properties

        public ObservableCollection<EventTypeViewModel> EventTypes { get; } =
            new ObservableCollection<EventTypeViewModel>();

        public ObservableCollection<EventViewModel> RunningEvents { get; }
            = new ObservableCollection<EventViewModel>();

        public ObservableCollection<EventViewModel> LoggedEvents { get; }
            = new ObservableCollection<EventViewModel>();

        public bool IsEditing
        {
            get
            {
                return _isEditing;
            }
            set
            {
                if (_isEditing != value)
                {
                    _isEditing = value;
                    RaisePropertyChangedEvent("IsEditing");
                }
            }
        }

        /// <summary>
        ///  File to export to
        /// </summary>
        public string ExportPath
        {
            get { return _exportPath; }
            set
            {
                if (_exportPath != value)
                {
                    _exportPath = value;
                    RaisePropertyChangedEvent("ExportPath");
                    MarkAsDirty();
                }
            }
        }

        public string ExportFileToken
        {
            get { return _exportFileToken; }
            set
            {
                if (_exportFileToken != value)
                {
                    _exportFileToken = value;
                    RaisePropertyChangedEvent("ExportFileToken");
                    MarkAsDirty();
                }
            }
        }

        public EventViewModel SelectedRunningEvent
        {
            get { return _selectedRunningEvent; }
            set
            {
                if (_selectedRunningEvent != value)
                {
                    _selectedRunningEvent = value;
                    IsEditing = value != null;
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
                    IsEditing = value != null;
                    FinishEditingBut(value);
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
                  (SelectedLoggedEvent != null?
                  SelectedLoggedEvent.EventType : _selectedEventType);
            }
            set
            {
                if (SelectedRunningEvent != null)
                {
                    SelectedRunningEvent.EventType = value;
                    RaisePropertyChangedEvent("SelectedEventType");
                }
                else if (SelectedLoggedEvent != null)
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
                  (SelectedLoggedEvent != null?
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
                else if (SelectedLoggedEvent != null)
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
                  (SelectedLoggedEvent != null?
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
                else if (SelectedLoggedEvent != null)
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
                  (SelectedLoggedEvent != null?
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
                else if (SelectedLoggedEvent != null)
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
                  (SelectedLoggedEvent != null?
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
                else if (SelectedLoggedEvent != null)
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
                  (SelectedLoggedEvent != null?
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
                else if (SelectedLoggedEvent != null)
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

        #endregion

        #region Methods

        public async Task<bool> Load()
        {
            if (_state == States.Init)
            {
                string expPath, expToken;
                RoamingSettingsHelper.LoadExportSettings(out expPath, out expToken);
                ExportPath = expPath;
                ExportFileToken = expToken;
                EventTypes.LoadRoamingColorMapping();
                ResetWithEventTypes();

                var wasSuppressing = _suppressSorting;
                _suppressSorting = true;
                var res = await LoggedEvents.LoadEvents(EventTypes);
                _suppressSorting = false;
                SortLoggedEvents();
                _suppressSorting = wasSuppressing;

                _state = States.Synced;
                return res;
            }
            else
            {
                return true;
            }
        }

        public async Task Save(bool forceSave = false)
        {
            if (_state == States.Dirty || forceSave)
            {
                RoamingSettingsHelper.SaveExportSettings(ExportPath, ExportFileToken);
                EventTypes.SaveRoamingColorMapping();
                await LoggedEvents.SaveEvents();
                _state = States.Synced;
            }
        }
        public void New()
        {
            SelectedRunningEvent = null;
            FinishEditingBut(null);
            SelectedLoggedEvent = null;
            IsEditing = true;
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
            RunningEvents.Insert(0, evm);
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
            SelectedRunningEvent = RunningEvents.FirstOrDefault();
        }

        public void Cancel()
        {
            if (SelectedRunningEvent != null)
            {
                RunningEvents.Remove(SelectedRunningEvent);
                SelectedRunningEvent = RunningEvents.FirstOrDefault();
            }
        }

        public void CloseEditor()
        {
            SelectedRunningEvent = null;
            FinishEditingBut(null);
            SelectedLoggedEvent = null;
            IsEditing = false;
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
            var wasSuppressed = _suppressSorting;
            _suppressSorting = true;
            LoggedEvents.Insert(index, evm);
            _suppressSorting = wasSuppressed;
        }

        public void RemoveLoggedEvent(EventViewModel evm)
        {
            var wasSuppressed = _suppressSorting;
            _suppressSorting = true;
            LoggedEvents.Remove(evm);
            _suppressSorting = wasSuppressed;
        }

        private void LoggedEventPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "IsEditing")
            {
                var evm = (EventViewModel)sender;
                if (evm.IsEditing)
                {
                    SelectedRunningEvent = null;
                    _wasLoggedSelectedBeforeEditing = SelectedLoggedEvent == evm;
                    if (!_wasLoggedSelectedBeforeEditing)
                    {
                        SelectedLoggedEvent = evm;
                    }
                    FinishEditingBut(evm);
                }
                else
                {
                    // finished editing, sort it
                    SortLoggedEvents();
                    // make the event remain selected and therefore details displayed
                    if (_wasLoggedSelectedBeforeEditing)
                    {
                        SelectedLoggedEvent = evm;
                    }
                }
                AffectPickerEnabled();
                AffectMutliRoleFields();
            }
            MarkAsDirty();
        }
        
        /// <summary>
        ///  Mark as not-editing except the specified
        /// </summary>
        /// <param name="evm">The event that's not marked not-editing</param>
        private void FinishEditingBut(EventViewModel evm)
        {
            var buf = LoggedEvents.Where(x => x != evm).ToList();
            foreach (var le in buf)
            {
                le.IsEditing = false;
            }
        }

        private void LoggedEventsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (_inLoggedCollectionChangedHandler)
            {
                return;
            }
            _inLoggedCollectionChangedHandler = true;

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
            SortLoggedEvents();
            MarkAsDirty();

            _inLoggedCollectionChangedHandler = false;
        }

        private void SortLoggedEvents()
        {
            if (!_suppressSorting)
            {
                _suppressSorting = true;
                LoggedEvents.QuickSort();
                _suppressSorting = false;
            }
        }

        private void SubscribeForLoadedLoggedEvents()
        {
            foreach (var loggedEvent in LoggedEvents)
            {
                loggedEvent.PropertyChanged += LoggedEventPropertyChanged;
            }
        }

        private void EventTypesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            ResetWithEventTypes();

            if (args.Action == NotifyCollectionChangedAction.Reset)
            {
                SubscribeForLoadedEventTypes();
            }
            else
            {
                if (args.OldItems != null)
                {
                    var set = new HashSet<EventTypeViewModel>();
                    foreach (var oi in args.OldItems.Cast<EventTypeViewModel>())
                    {
                        set.Add(oi);
                        oi.PropertyChanged -= EventTypeOnPropertyChanged;
                    }
                    foreach (var ev in RunningEvents)
                    {
                        if (set.Contains(ev.EventType))
                        {
                            ev.EventType = null;
                        }
                    }
                    foreach (var ev in LoggedEvents)
                    {
                        if (set.Contains(ev.EventType))
                        {
                            ev.EventType = null;
                        }
                    }
                }
                if (args.NewItems != null)
                {
                    foreach (var ni in args.NewItems.Cast<EventTypeViewModel>())
                    {
                        ni.PropertyChanged += EventTypeOnPropertyChanged;
                    }
                }
            }
           
            MarkAsDirty();
        }

        private void SubscribeForLoadedEventTypes()
        {
            foreach (var et in EventTypes)
            {
                et.PropertyChanged += EventTypeOnPropertyChanged;
            }
        }

        private void EventTypeOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            MarkAsDirty();
        }

        /// <summary>
        ///  note this may also set the broken event type on running event
        ///  to the first type available
        /// </summary>
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

        private void MarkAsDirty()
        {
            if (_state == States.Synced)
            {
                _state = States.Dirty;
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

        #endregion
    }
}
