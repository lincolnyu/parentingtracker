using System;

namespace ParentingTrackerApp.ViewModels
{
    public class EventViewModel : BaseViewModel
    {
        private DateTime _startTime;
        private DateTime _endTime;
        private EventTypeViewModel _eventType;
        private string _notes;
        private bool _isEditing;

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
                    RaisePropertyChangedEvent("StartTime");
                    RaisePropertyChangedEvent("Title");
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
                    RaisePropertyChangedEvent("EndTime");
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
                    RaisePropertyChangedEvent("EventType");
                    RaisePropertyChangedEvent("Title");
                    RaisePropertyChangedEvent("Type");
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
                    RaisePropertyChangedEvent("Notes");
                }
            }
        }

        /// <summary>
        ///  The event displayed as a tag when being halfway created
        /// </summary>
        public string Title
        {
            get
            {
                return string.Format("{0} {1}", EventType.Name, StartTime.ToString());
            }
        }

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
    }
}
