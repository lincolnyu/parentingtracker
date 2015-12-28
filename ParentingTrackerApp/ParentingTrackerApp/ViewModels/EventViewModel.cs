using System;

namespace ParentingTrackerApp.ViewModels
{
    public class EventViewModel : BaseViewModel
    {
        private DateTime _startTime;
        private DateTime _endTime;
        private EventTypeViewModel _eventType;
        private string _notes;

        public DateTime StartTime
        {
            get
            {
                return _startTime;
            }
            set
            {
                if (_startTime != value)
                {
                    _startTime = value;
                    RaisePropertyChanged("StartTime");
                    RaisePropertyChanged("Title");
                }
            }
        }

        public DateTime EndTime
        {
            get
            {
                return _endTime;
            }
            set
            {
                if (_endTime != value)
                {
                    _endTime = value;
                    RaisePropertyChanged("EndTime");
                }
            }
        }

        public EventTypeViewModel EventType
        {
            get
            {
                return _eventType;
            }
            set
            {
                if (_eventType != value)
                {
                    _eventType = value;
                    RaisePropertyChanged("EventType");
                    RaisePropertyChanged("Title");
                    RaisePropertyChanged("Type");
                }
            }
        }

        public string Type
        {
            get { return EventType.Name; }
        }

        public string Notes
        {
            get
            {
                return _notes;
            }
            set
            {
                if (_notes != value)
                {
                    _notes = value;
                    RaisePropertyChanged("Notes");
                }
            }
        }

        public string Title
        {
            get
            {
                return string.Format("{0} {1}", EventType.Name, StartTime.ToString());
            }
        }
    }
}
