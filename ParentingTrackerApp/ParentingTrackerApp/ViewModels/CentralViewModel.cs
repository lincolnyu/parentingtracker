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
        private string _exportPath;
        private string _exportFileToken;

        private bool _wasLoggedSelectedBeforeEditing;

        private EventViewModel _newEvent;

        private const string EventFileName = "events.csv";
        private EventViewModel _selectedEvent;
        private bool _isInLoggedEventPropertyChanged;
        private bool _inAllEventsCollectionChangedHandler;

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

        public ObservableCollection<EventTypeViewModel> EventTypes { get; } =
            new ObservableCollection<EventTypeViewModel>();

        public ObservableCollection<EventViewModel> RunningEvents { get; }
            = new ObservableCollection<EventViewModel>();

        public ObservableCollection<EventViewModel> LoggedEvents { get; }
            = new ObservableCollection<EventViewModel>();

        public ObservableCollection<EventViewModel> AllEvents { get; }
            = new ObservableCollection<EventViewModel>();

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

        #endregion

        #region Methods

        private void UpdateIsEditingAndRelated()
        {
            IsEditing = SelectedEvent != null || _newEvent != null;
            IsCreating = _newEvent != null;
            CanStop = SelectedEvent != null && SelectedEvent.IsRunning;
            var prevDc = EventInEditing;
            if (SelectedEvent != null)
            {
                EventInEditing = SelectedEvent;
            }
            else
            {
                EventInEditing = _newEvent;
            }
            RaisePropertyChangedEvent("IsEditing");
            RaisePropertyChangedEvent("IsCreating");
            RaisePropertyChangedEvent("CanStop");
            if (EventInEditing == null && prevDc != null)
            {
                var wasSuppressing = prevDc.SuppressUpdate;
                prevDc.SuppressUpdate = true;
                RaisePropertyChangedEvent("EventInEditing");
                prevDc.SuppressUpdate = wasSuppressing;
            }
            else
            {
                RaisePropertyChangedEvent("EventInEditing");
            }
        }

        public void New()
        {
            SelectedEvent = null;
            FinishEditingBut(null);
            _newEvent = new EventViewModel(this)
            {
                EventType = EventTypes.FirstOrDefault(),
                StartTime = DateTime.Now,
                EndTime = DateTime.Now
            };
            UpdateIsEditingAndRelated();
        }


        public void CloseEditor()
        {
            CloseEditor(SelectedEvent);
        }

        private void CloseEditor(EventViewModel e)
        {
            if (e != null && e == SelectedEvent 
                && (e.IsRunningEvent || !_wasLoggedSelectedBeforeEditing))
            {
                SelectedEvent = null;
            }

            FinishEditingBut(null);

            _newEvent = null;
            UpdateIsEditingAndRelated();
        }

        public void Start()
        {
            var time = DateTime.Now;
            var evm = _newEvent?? new EventViewModel(this)
            {
                EventType = EventTypes.FirstOrDefault(),
                Notes = ""
            };
            evm.Status = EventViewModel.Statuses.Running;
            evm.StartTime = time;
            AllEvents.Insert(evm);
            SelectedEvent = evm;
            _newEvent = null;
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
            var evm = _newEvent?? new EventViewModel(this)
            {
                StartTime = time,
                EventType = EventTypes.FirstOrDefault(),
                Notes = ""
            };
            evm.EndTime = evm.StartTime;
            evm.Status = EventViewModel.Statuses.Logged;
            AllEvents.Insert(evm);
            _newEvent = null;
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

                AllEvents.Clear();
                var evlines = await EventFileName.LoadEventsLines();
                if (evlines != null)
                {
                    var events = evlines.LoadEvents(this);
                    foreach (var e in events)
                    {
                        if (e.Status != EventViewModel.Statuses.Running)
                        {
                            e.Status = EventViewModel.Statuses.Logged;// make sure it's logged event
                        }
                        AllEvents.Insert(e);
                    }
                }

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
            if (args.PropertyName == "IsEditing")
            {
                var evm = (EventViewModel)sender;
                if (evm.IsEditing)
                {
                    _wasLoggedSelectedBeforeEditing = SelectedEvent == evm;
                    if (!_wasLoggedSelectedBeforeEditing)
                    {
                        SelectedEvent = evm;
                    }
                    FinishEditingBut(evm);
                }
                else
                {
                    CloseEditor(evm);
                }
            }
            MarkAsDirty();
            _isInLoggedEventPropertyChanged = false;
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
            if (_inAllEventsCollectionChangedHandler)
            {
                return;
            }
            _inAllEventsCollectionChangedHandler = true;
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
                            var index = RunningEvents.BinarySearch(newItem);
                            if (index < 0) index = -index - 1;
                            RunningEvents.Insert(index, newItem);
                        }
                        else if (newItem.IsLoggedEvent)
                        {
                            var index = LoggedEvents.BinarySearch(newItem);
                            if (index < 0) index = -index - 1;
                            LoggedEvents.Insert(index, newItem);
                        }
                        newItem.PropertyChanged += EventOnPropertyChanged;
                    }
                }
            }
       
            MarkAsDirty();
            _inAllEventsCollectionChangedHandler = false;
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

        private void MarkAsDirty()
        {
            if (_state == States.Synced)
            {
                _state = States.Dirty;
            }
        }

        #endregion
    }
}
