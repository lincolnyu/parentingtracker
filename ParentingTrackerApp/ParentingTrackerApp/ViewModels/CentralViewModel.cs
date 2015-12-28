using ParentingTrackerApp.Default;
using System;
using System.Collections.ObjectModel;

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

        public EventTypeViewModel SelectedEventType
        {
            get { return SelectedRunningEvent?.EventType?? 
                    SelectedLoggedEvent?.EventType??
                    _selectedEventType; }
            set
            {
                if (SelectedRunningEvent != null)
                {
                    SelectedRunningEvent.EventType = value;
                    RaisePropertyChanged("SelectedEventType");
                }
                else if (SelectedLoggedEvent != null)
                {
                    SelectedLoggedEvent.EventType = value;
                    RaisePropertyChanged("SelectedEventType");
                }
                else if (_selectedEventType != value)
                {
                    _selectedEventType = value;
                    RaisePropertyChanged("SelectedEventType");
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
                    RaisePropertyChanged("SelectedRunningEvent");
                    RaisePropertyChanged("CanStop");
                    RaisePropertyChanged("Notes");
                    RaisePropertyChanged("SelectedEventType");
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
                    RaisePropertyChanged("SelectedLoggedEvent");
                    RaisePropertyChanged("Notes");
                    RaisePropertyChanged("SelectedEventType");
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

        public string Notes
        {
            get
            {
                return SelectedRunningEvent?.Notes ??
                  SelectedLoggedEvent?.Notes ??
                  _notes;
            }
            set
            {
                if (SelectedRunningEvent != null)
                {
                    SelectedRunningEvent.Notes = value;
                    RaisePropertyChanged("Notes");
                }
                else if (SelectedLoggedEvent != null)
                {
                    SelectedLoggedEvent.Notes = value;
                    RaisePropertyChanged("Notes");
                }
                else if (_notes != value)
                {
                    _notes = value;
                    RaisePropertyChanged("Notes");
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
            LoggedEvents.Add(sre);
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
            LoggedEvents.Add(evm);
        }
    }
}
