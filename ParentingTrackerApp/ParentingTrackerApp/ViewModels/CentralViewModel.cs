using ParentingTrackerApp.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Core;

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

        #region Constants

        private const string EventFileName = "events.csv";

        #endregion

        #region Backing fields

        private States _state = States.Init;
        private string _exportPath;
        private string _exportFileToken;

        private EventViewModel _selectedEvent;

        #endregion

        #region Flags

        private bool _wasSelected;
        private bool _isInLoggedEventPropertyChanged;
        private bool _suppressAllEventsCollectionChangedHandler;
        private static bool _suppressEventTypeCollectionChangeHandling;
        private EventViewModel _newEvent;

        #endregion

        #endregion

        #region Constructors

        public CentralViewModel()
        {
            UpdateIsEditingAndRelated();
            EventTypes.CollectionChanged += EventTypesOnCollectionChanged;
            AllEvents.CollectionChanged += AllEventsOnCollectionChanged;
        }

        #endregion

        #region Properties

        public CoreDispatcher Dispatcher { get; set; }

        public ObservableCollection<EventTypeViewModel> EventTypes { get; } =
            new ObservableCollection<EventTypeViewModel>();

        public ObservableCollection<EventViewModel> RunningEvents { get; }
            = new ObservableCollection<EventViewModel>();

        public ObservableCollection<EventViewModel> LoggedEvents { get; }
            = new ObservableCollection<EventViewModel>();

        public ObservableCollection<EventViewModel> AllEvents { get; }
            = new ObservableCollection<EventViewModel>();

        public EventViewModel NewEvent
        {
            get { return _newEvent; }
            set
            {
                if (_newEvent != value)
                {
                    if (_newEvent != null)
                    {
                        _newEvent.PropertyChanged -= NewEventOnPropertyChanged;
                    }
                    _newEvent = value;
                    if (_newEvent != null)
                    {
                        _newEvent.PropertyChanged += NewEventOnPropertyChanged;
                    }
                }
            }
        }

        /// <summary>
        ///  Data context for the editor
        /// </summary>
        public EventViewModel EventInEditing
        {
            get; private set;
        }

        /// <summary>
        ///  If the editor is showing for either selected object or object being created
        /// </summary>
        public bool IsEditing
        {
            get; private set;
        }

        /// <summary>
        ///  If is creating a new event
        /// </summary>
        public bool IsCreating
        {
            get;
            private set;
        }


        public bool CanStop
        {
            get; private set;
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
        public EventViewModel SelectedLoggedEvent
        {
            get
            {
                return SelectedEvent != null && SelectedEvent.IsLoggedEvent? SelectedEvent : null;
            }
            set
            {
                if (SelectedEvent != null && SelectedEvent.IsLoggedEvent
                        || value != null && value.IsLoggedEvent)
                {
                    SelectedEvent = value;
                }
            }
        }

        public EventViewModel SelectedRunningEvent
        {
            get
            {
                return SelectedEvent != null && SelectedEvent.IsRunningEvent ? SelectedEvent : null;
            }
            set
            {
                if (SelectedEvent != null && SelectedEvent.IsRunningEvent
                        || value != null && value.IsRunningEvent)
                {
                    SelectedEvent = value;
                }
            }
        }
        
        public EventViewModel SelectedEvent
        {
            get { return _selectedEvent;  }
            set
            {
                if (_selectedEvent != value)
                {
                    _selectedEvent = value;
                    FinishEditingBut(value);
                    RaisePropertyChangedEvent("SelectedEvent");
                    RaisePropertyChangedEvent("SelectedLoggedEvent");
                    RaisePropertyChangedEvent("SelectedRunningEvent");
                    UpdateIsEditingAndRelated();
                }
            }
        }

        public bool NewStartTimeChanged { get; set; }

        public bool NewEndTimeChanged { get; set; }

        #endregion

        #region Methods

        private void UpdateIsEditingAndRelated()
        {
            if (SelectedEvent != null)
            {
                NewEvent = null;
            }
            IsEditing = SelectedEvent != null || NewEvent != null;
            IsCreating = NewEvent != null;
            CanStop = SelectedEvent != null && SelectedEvent.IsRunning;
            var prevDc = EventInEditing;
            if (SelectedEvent != null)
            {
                EventInEditing = SelectedEvent;
            }
            else
            {
                EventInEditing = NewEvent;
            }
            RaisePropertyChangedEvent("IsEditing");
            RaisePropertyChangedEvent("IsCreating");
            RaisePropertyChangedEvent("CanStop");
            if (EventInEditing != prevDc)
            {
                var wasSuppressing = EventViewModel.SuppressUpdate;
                EventViewModel.SuppressUpdate = true;
                RaisePropertyChangedEvent("EventInEditing");
                EventViewModel.SuppressUpdate = wasSuppressing;
            }
            else
            {
                RaisePropertyChangedEvent("EventInEditing");
            }

            // TODO consider adding delayed update here?
        }

        public void New()
        {
            SelectedEvent = null;
            FinishEditingBut(null);
            NewEvent = new EventViewModel(this)
            {
                Status = EventViewModel.Statuses.Running,
                EventType = EventTypes.FirstOrDefault(),
                StartTime = DateTime.Now,
                EndTime = DateTime.Now
            };
            NewStartTimeChanged = false;
            NewEndTimeChanged = false;
            UpdateIsEditingAndRelated();
        }


        public void CloseEditor()
        {
            _wasSelected = false;
            CloseEditor(SelectedEvent);
        }

        private void CloseEditor(EventViewModel e)
        {
            if (e != null && e == SelectedEvent 
                && (e.IsRunningEvent || !_wasSelected))
            {
                SelectedEvent = null;
            }

            FinishEditingBut(null);

            NewEvent = null;
            UpdateIsEditingAndRelated();
        }

        public void Start()
        {
            var evm = NewEvent?? new EventViewModel(this)
            {
                EventType = EventTypes.FirstOrDefault(),
                Notes = ""
            };
            evm.Status = EventViewModel.Statuses.Running;
            if (!NewStartTimeChanged)
            {
                evm.StartTime = DateTime.Now;
            }
            AllEvents.Insert(evm);
            SelectedEvent = evm;
            NewEvent = null;
            UpdateIsEditingAndRelated();
        }

        public void Stop()
        {
            var sre = SelectedEvent;
            if (sre == null)
            {
                return;
            }
            var t = DateTime.Now;
            AllEvents.Remove(sre);
            sre.EndTime = t;
            sre.Status = EventViewModel.Statuses.Logged;
            AllEvents.Insert(sre);
            SelectedEvent = AllEvents.FirstOrDefault(x => x.IsRunning);
            UpdateIsEditingAndRelated();
        }

        public void Cancel()
        {
            if (SelectedEvent != null && SelectedEvent.IsRunning)
            {
                AllEvents.Remove(SelectedEvent);
                SelectedEvent = AllEvents.FirstOrDefault(x=>x.IsRunning);
                UpdateIsEditingAndRelated();
            }
        }

        public void Log()
        {
            var time = DateTime.Now;
            var evm = NewEvent?? new EventViewModel(this)
            {
                StartTime = time,
                EndTime = time,
                EventType = EventTypes.FirstOrDefault(),
                Notes = ""
            };
            evm.Status = EventViewModel.Statuses.Logged;
            AllEvents.Insert(evm);
            NewEvent = null;
            UpdateIsEditingAndRelated();
        }


        public async Task<bool> Load()
        {
            if (_state == States.Init)
            {
                string expPath, expToken;
                RoamingSettingsHelper.LoadExportSettings(out expPath, out expToken);
                ExportPath = expPath;
                ExportFileToken = expToken;
                EventTypes.LoadRoamingColorMapping();

                var wasSuppressing = _suppressAllEventsCollectionChangedHandler;
                _suppressAllEventsCollectionChangedHandler = true;
                // TODO this method should be called at the start and they should be empty 
                //      or it may be problematic
                foreach (var e in AllEvents)
                {
                    e.PropertyChanged -= EventOnPropertyChanged;
                }
                AllEvents.Clear();
                LoggedEvents.Clear();
                RunningEvents.Clear();
                var evlines = await EventFileName.LoadEventsLines();
                if (evlines != null)
                {
                    var events = evlines.LoadEvents(this);
                    foreach (var e in events)
                    {
                        AllEvents.Add(e);
                        e.PropertyChanged += EventOnPropertyChanged;
                        if (e.Status == EventViewModel.Statuses.Running)
                        {
                            RunningEvents.Add(e);
                        }
                        else
                        {
                            LoggedEvents.Add(e);
                            e.Status = EventViewModel.Statuses.Logged;// make sure it's logged event
                        }
                    }
                }

                SortLists(null);

                _suppressAllEventsCollectionChangedHandler = wasSuppressing;

                _state = States.Synced;
                return evlines != null;
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
                await AllEvents.SaveEvents(EventFileName);
                _state = States.Synced;
            }
        }
        
        public void RemoveEvent(EventViewModel evm)
        {
            AllEvents.Remove(evm);
        }

        private void EventOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (_isInLoggedEventPropertyChanged)
            {
                return;
            }
            _isInLoggedEventPropertyChanged = true;
            var evm = (EventViewModel)sender;
            if (args.PropertyName == "IsEditing")
            {
                if (evm.IsEditing)
                {
                    _wasSelected = SelectedEvent == evm;
                    if (!_wasSelected)
                    {
                        SelectedEvent = evm;
                    }
                    FinishEditingBut(evm);
                }
                else
                {
                    // for logged event reordering happens after editing is finished
                    CloseEditor(evm);
                    DelayMinimumSort(evm);
                }
            }
            else if (evm.IsRunning && evm.IsDataProperty(args.PropertyName))
            {
                _wasSelected = evm == SelectedEvent;
                DelayMinimumSort(evm);
            }
            MarkAsDirty();
            _isInLoggedEventPropertyChanged = false;
        }

        private void DelayMinimumSort(EventViewModel evm)
        {
            if (Dispatcher != null)
            {
                DelayHelper.Delay(evm, x => MinimumSort((EventViewModel)x), 100, Dispatcher);
            }
        }

        private void MinimumSort(EventViewModel evm)
        {
            var wasSuppressing = _suppressAllEventsCollectionChangedHandler;
            _suppressAllEventsCollectionChangedHandler = true;
            MinimumSort(AllEvents, evm);
            MinimumSort(RunningEvents, evm);
            MinimumSort(LoggedEvents, evm);
            if (_wasSelected)
            {
                SelectedEvent = evm;
            }
            _suppressAllEventsCollectionChangedHandler = wasSuppressing;
        }

        private static void MinimumSort(IList<EventViewModel> list, EventViewModel evm)
        {
            var index = list.IndexOf(evm);
            if (index < 0)
            {
                return;
            }
            var t = index-1;
            for (; t >= 0 && list[t].CompareTo(evm) > 0; t--)
            {
            }
            if (t+1 < index)
            {
                list.RemoveAt(index);
                list.Insert(t+1, evm);
                return;
            }
            t = index + 1;
            for (; t < list.Count && evm.CompareTo(list[t])>0; t++)
            {
            }
            if (t-1 > index)
            {
                list.RemoveAt(index);
                list.Insert(t-1, evm);
            }
        }

        private void SortLists(EventViewModel evm)
        {
            var wasSuppressing = _suppressAllEventsCollectionChangedHandler;
            _suppressAllEventsCollectionChangedHandler = true;
            AllEvents.QuickSort();
            RunningEvents.QuickSort();
            LoggedEvents.QuickSort();
            if (_wasSelected)
            {
                SelectedEvent = evm;
            }
            _suppressAllEventsCollectionChangedHandler = wasSuppressing;
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

        private void AllEventsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (_suppressAllEventsCollectionChangedHandler)
            {
                return;
            }
            _suppressAllEventsCollectionChangedHandler = true;
            if (args.Action == NotifyCollectionChangedAction.Reset)
            {
                RunningEvents.Clear();
                LoggedEvents.Clear();
                SubscribeForEvents();
            }
            else
            {
                if (args.OldItems != null)
                {
                    foreach (var oldItem in args.OldItems.Cast<EventViewModel>())
                    {
                        if (oldItem.IsRunningEvent)
                        {
                            RunningEvents.Remove(oldItem);
                        }
                        else if (oldItem.IsLoggedEvent)
                        {
                            LoggedEvents.Remove(oldItem);
                        }
                        oldItem.PropertyChanged -= EventOnPropertyChanged;
                    }
                }
                if (args.NewItems != null)
                {
                    foreach (var newItem in args.NewItems.Cast<EventViewModel>())
                    {
                        if (newItem.IsRunningEvent)
                        {
                            RunningEvents.Insert(newItem);
                        }
                        else if (newItem.IsLoggedEvent)
                        {
                            LoggedEvents.Insert(newItem);
                        }
                        newItem.PropertyChanged += EventOnPropertyChanged;
                    }
                }
            }
       
            MarkAsDirty();
            _suppressAllEventsCollectionChangedHandler = false;
        }

        private void SubscribeForEvents()
        {
            foreach (var e in AllEvents)
            {
                e.PropertyChanged += EventOnPropertyChanged;
            }
        }

        private void EventTypesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (_suppressEventTypeCollectionChangeHandling)
            {
                return;
            }
            if (args.Action == NotifyCollectionChangedAction.Reset)
            {
                ResetEventTypes();
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
                    foreach (var ev in AllEvents)
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

        private void ResetEventTypes()
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

        private void MarkAsDirty()
        {
            if (_state == States.Synced)
            {
                _state = States.Dirty;
            }
        }
        
        public void Refresh()
        {
            foreach (var e in RunningEvents)
            {
                e.RefreshRunningTag();
            }
        }

        public void ResetEventTypesToDefault()
        {
            var wasSuppressing = _suppressEventTypeCollectionChangeHandling;
            _suppressEventTypeCollectionChangeHandling = true;
            var wasSuppressing2 = EventViewModel.SuppressUpdate;
            EventViewModel.SuppressUpdate = true;
            EventTypes.LoadDefaultColorMapping();
            EventViewModel.SuppressUpdate = false;
            EventViewModel.SuppressUpdate = wasSuppressing2;
            _suppressEventTypeCollectionChangeHandling = wasSuppressing;

            wasSuppressing = _suppressAllEventsCollectionChangedHandler;
            _suppressAllEventsCollectionChangedHandler = true;
            var allEvents = AllEvents.ToList();
            foreach (var ev in allEvents)
            {
                ev.EventType = EventTypes.FirstOrDefault(x => EventTypeEqualityComparer.Instance.Equals(x, ev.EventType));
            }
            _suppressAllEventsCollectionChangedHandler = wasSuppressing;
            MarkAsDirty();
        }

        private void NewEventOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "StartTime")
            {
                NewStartTimeChanged = true;
            }
            else if (args.PropertyName == "EndTime")
            {
                NewEndTimeChanged = true;
            }
        }

        #endregion
    }
}
