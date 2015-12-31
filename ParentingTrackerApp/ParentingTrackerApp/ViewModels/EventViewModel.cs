using ParentingTrackerApp.Helpers;
using System;

namespace ParentingTrackerApp.ViewModels
{
    public class EventViewModel : BaseViewModel, IComparable<EventViewModel>
    {
        #region Fields

        private DateTime _startTime;
        private DateTime _endTime;
        private EventTypeViewModel _eventType;
        private string _notes;
        private bool _isEditing;

        #endregion

        #region Properties

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
                    RaiseStartTimeChangedEvent();
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
                    RaiseEndTimeChangedEvent();
                }
            }
        }

        public DateTime StartDate
        {
            get
            {
                return StartTime.Date;
            }
            set
            {
                if (StartTime.Date != value)
                {
                    StartTime = new DateTime(value.Year, value.Month, value.Day,
                        StartTime.Hour, StartTime.Minute, StartTime.Second);
                    RaiseStartTimeChangedEvent();
                }
            }
        }

        public TimeSpan StartTimeOfDay
        {
            get
            {
                return StartTime.TimeOfDay;
            }
            set
            {
                if (StartTime.TimeOfDay != value)
                {
                    StartTime = new DateTime(StartTime.Year, StartTime.Month, 
                        StartTime.Day, value.Hours, value.Minutes, value.Seconds);
                    RaiseStartTimeChangedEvent();
                }
            }
        }

        public DateTime EndDate
        {
            get
            {
                return EndTime.Date;
            }
            set
            {
                if (EndTime.Date != value)
                {
                    EndTime = new DateTime(value.Year, value.Month, value.Day,
                        EndTime.Hour, EndTime.Minute, EndTime.Second);
                    RaiseEndTimeChangedEvent();
                }
            }
        }

        public TimeSpan EndTimeOfDay
        {
            get
            {
                return EndTime.TimeOfDay;
            }
            set
            {
                if (EndTime.TimeOfDay != value)
                {
                    EndTime = new DateTime(EndTime.Year, EndTime.Month,
                        EndTime.Day, value.Hours, value.Minutes, value.Seconds);
                    RaiseStartTimeChangedEvent();
                }
            }
        }

        public string LocalisedTimeRange
        {
            get
            {
                return DateTimeHelper.GetTimeRangeString(StartTime, EndTime);
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

        #endregion

        #region Methods

        #region  IComparable<EventViewModel> members

        public int CompareTo(EventViewModel other)
        {
            var c = StartTime.CompareTo(other.StartTime);
            if (c != 0) return c;
            c = EndTime.CompareTo(other.EndTime);
            if (c != 0) return c;
            c = EventType.Name.CompareTo(other.EventType.Name);
            if (c != 0) return c;
            return Notes.CompareTo(other.Notes);
        }

        #endregion

        private void RaiseStartTimeChangedEvent()
        {
            RaisePropertyChangedEvent("StartDate");
            RaisePropertyChangedEvent("StartTime");
            RaisePropertyChangedEvent("StartTimeOfDay");
            RaisePropertyChangedEvent("Title");
            RaisePropertyChangedEvent("LocalisedTimeRange");
        }

        private void RaiseEndTimeChangedEvent()
        {
            RaisePropertyChangedEvent("EndDate");
            RaisePropertyChangedEvent("EndTime");
            RaisePropertyChangedEvent("EndTimeOfDay");
            RaisePropertyChangedEvent("LocalisedTimeRange");
        }

        #endregion
    }
}
