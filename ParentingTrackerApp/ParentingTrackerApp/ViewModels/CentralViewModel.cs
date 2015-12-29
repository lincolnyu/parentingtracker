using ParentingTrackerApp.Default;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace ParentingTrackerApp.ViewModels
{
    public class CentralViewModel : BaseViewModel
    {
        private string _notes;
        private EventTypeViewModel _selectedEventType;
        private EventViewModel _selectedRunningEvent;
        private EventViewModel _selectedLoggedEvent;

        public CentralViewModel()
        {
            EventTypes.LoadDefaultBreastFeedingEventTypes();

            SelectedEventType = EventTypes[0];
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
                    AffectMutliRoleFields();
                    RaisePropertyChangedEvent("SelectedLoggedEvent");
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

        private void AddLogggedEvent(EventViewModel evm)
        {
            LoggedEvents.Add(evm);
            evm.PropertyChanged += LoggedEventPropertyChanged;
        }

        public void RemoveLoggedEvent(EventViewModel evm)
        {
            evm.PropertyChanged -= LoggedEventPropertyChanged;
            LoggedEvents.Remove(evm);
        }


        private void LoggedEventPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "IsEditing")
            {
                var evm = (EventViewModel)sender;
                if (evm.IsEditing)
                {
                    SelectedLoggedEvent = evm;
                    FinishEditing(evm);
                }
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
        }
    }
}
