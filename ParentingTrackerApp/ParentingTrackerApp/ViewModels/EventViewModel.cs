using ParentingTrackerApp.Helpers;
using System;
using System.Collections.ObjectModel;
using Windows.UI;

namespace ParentingTrackerApp.ViewModels
{
    public class EventViewModel : BaseViewModel, IComparable<EventViewModel>
    {
        #region Enumerations

        public enum Statuses
        {
            Logged,
            Running,
            Editing,
        }

        #endregion

        #region Fields

        private DateTime _startTime;
        private DateTime _endTime;
        private EventTypeViewModel _eventType;
        private string _notes;
        private Statuses _status;

        #endregion

        #region Constructors

        public EventViewModel(CentralViewModel cvm)
        {
            CentralViewModel = cvm;
        }

        #endregion

        #region Properties

        public CentralViewModel CentralViewModel
        {
            get; private set;
        }

        /// <summary>
        ///  All available event types
        /// </summary>
        public ObservableCollection<EventTypeViewModel> EventTypes
        {
            get
            {
                return CentralViewModel.EventTypes;
            }
        }

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

        public DateTimeOffset StartDate
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
                if (_eventType != value && !SuppressUpdate)
                {
                    _eventType = value;
                    RaisePropertyChangedEvent("EventType");
                    RaisePropertyChangedEvent("RunningTag");
                    RaisePropertyChangedEvent("EventTypeName");
                    RaisePropertyChangedEvent("Color");
                }
            }
        }

        public string EventTypeName
        {
            get { return EventType != null ?
                    EventType.Name : "(null)"; }
        }

        public Color Color
        {
            get { return EventType.Color; }
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
        public string RunningTag
        {
            get
            {
                var isNormal = StartTime <= DateTime.Now;
                var diff = isNormal ? (DateTime.Now - StartTime) : StartTime - DateTime.Now;
                var timeOfDayStr = diff.ToString(@"hh\:mm\:ss");
                var diffStr = diff.Days > 1 ? string.Format("{0} days {1}", diff.Days, timeOfDayStr)
                    : diff.Days == 1? string.Format("1 day {0}", timeOfDayStr) :
                    timeOfDayStr;

                return string.Format("{0} since {1} {2} {3}", EventTypeName, 
                    StartTime.ToString(), isNormal? "for" : "in", diffStr);
            }
        }

        public Statuses Status
        {
            get
            {
                return _status;
            }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    RaiseStatusChangedEvents();
                }
            }
        }

        public bool IsLoggedEvent
        {
            get
            {
                return Status == Statuses.Logged || Status == Statuses.Editing;
            }
        }

        public bool IsRunningEvent
        {
            get
            {
                return Status == Statuses.Running;
            }
        }

        public bool IsRunning
        {
            get
            {
                return Status == Statuses.Running;
            }
        }

        public bool IsEditing
        {
            get
            {
                return Status == Statuses.Editing;
            }
            set
            {
                if (value)
                {
                    Status = Statuses.Editing;
                }
                else if (Status == Statuses.Editing)
                {
                    Status = Statuses.Logged;
                }
            }
        }

        public bool IsEditingOrRunning
        {
            get
            {
                return IsEditing || IsRunning;
            }
        }
        
        /// <summary>
        ///  Suppress update of certain properties during the switching of data context
        ///  to avoid undesired data transfer
        /// </summary>
        public bool SuppressUpdate
        {
            get; set;
        }

        #endregion

        #region Methods

        #region  IComparable<EventViewModel> members

        public int CompareTo(EventViewModel other)
        {
            return -CompareToAscending(other);
        }

        #endregion
        
        private int CompareToAscending(EventViewModel other)
        {
            var c = Status.CompareTo(other.Status);
            if (c != 0) return c;
            c = StartTime.CompareTimeIgnoreMs(other.StartTime);
            if (c != 0) return c;
            c = EndTime.CompareTimeIgnoreMs(other.EndTime);
            if (c != 0) return c;
            c = EventType.Name.CompareTo(other.EventType.Name);
            if (c != 0) return c;
            if (Notes == null)
            {
                return other.Notes != null ? -1 : 0;
            }
            else if (other.Notes == null)
            {
                return 1;
            }
            return Notes.CompareTo(other.Notes);
        }

        private void RaiseStartTimeChangedEvent()
        {
            RaisePropertyChangedEvent("StartDate");
            RaisePropertyChangedEvent("StartTime");
            RaisePropertyChangedEvent("StartTimeOfDay");
            RaisePropertyChangedEvent("RunningTag");
            RaisePropertyChangedEvent("LocalisedTimeRange");
        }

        private void RaiseEndTimeChangedEvent()
        {
            RaisePropertyChangedEvent("EndDate");
            RaisePropertyChangedEvent("EndTime");
            RaisePropertyChangedEvent("EndTimeOfDay");
            RaisePropertyChangedEvent("LocalisedTimeRange");
        }

        private void RaiseStatusChangedEvents()
        {
            RaisePropertyChangedEvent("Status");
            RaisePropertyChangedEvent("IsEditing");
            RaisePropertyChangedEvent("IsRunning");
            RaisePropertyChangedEvent("IsEditingOrRunning");
            RaisePropertyChangedEvent("IsLoggedEvent");
            RaisePropertyChangedEvent("IsRunningEvent");
        }

        public void Refresh()
        {
            RaisePropertyChangedEvent("RunningTag");
        }

        public bool IsDataProperty(string name)
        {
            return name == "StartTime" || name == "EndTime" || name == "EventType" || name == "Notes";
        }

        #endregion
    }
}
