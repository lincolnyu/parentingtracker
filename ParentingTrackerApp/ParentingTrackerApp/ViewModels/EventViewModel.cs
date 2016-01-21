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
            Creating,
        }

        #endregion

        #region Fields

        private DateTime _startTime;
        private DateTime _endTime;
        private EventTypeViewModel _eventType;
        private string _notes;
        private Statuses _status;
        private string _groupName;

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
                    // TODO validation...
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
                    // TODO validation...
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

        public DateTimeOffset EndDate
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
                    RaiseEndTimeChangedEvent();
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

        public string Duration
        {
            get
            {
                if (StartTime >= EndTime) return "";
                var diff = EndTime - StartTime;
                var hourStr = diff.Hours > 1 ? $"{diff.Hours} hrs " : diff.Hours > 0 ? $"{diff.Hours} hr "
                    : "";
                var minStr = diff.Minutes > 1 ? $"{diff.Minutes} mins" : diff.Minutes > 0 ? $"{diff.Minutes} min"
                    : diff.TotalMinutes > 1 ? "" : "0 mins";
                var timeStr = hourStr + minStr;
                    string.Format("{0}{1}", diff.Hours, diff.Minutes);// diff.ToString(@"hh\:mm\:ss");
                var diffStr = diff.Days > 1 ? string.Format("{0} days {1}", diff.Days, timeStr)
                    : diff.Days == 1 ? string.Format("1 day {0}", timeStr) :
                    timeStr;
                return "(" + diffStr + ")";
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
                    if (!SuppressUpdate)
                    {
                        _eventType = value;
                    }
                    RefreshEventTypeProperties();
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
            get { return EventType != null ? EventType.Color : default(Color); }
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
        public string RunningTime
        {
            get
            {
                var isNormal = StartTime <= DateTime.Now;
                var diff = isNormal ? (DateTime.Now - StartTime) : StartTime - DateTime.Now;
                var timeOfDayStr = diff.ToString(@"hh\:mm\:ss");
                var diffStr = diff.Days > 1 ? string.Format("{0} days {1}", diff.Days, timeOfDayStr)
                    : diff.Days == 1? string.Format("1 day {0}", timeOfDayStr) :
                    timeOfDayStr;
                var rdt = StartTime.ToRelativeDateTimeString();
                return string.Format("since {0} {1} {2}",
                    rdt, isNormal? "for" : "in", diffStr);
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

        public bool IsCreatingOrEditing
        {
            get
            {
                return Status == Statuses.Editing || Status == Statuses.Creating;
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

        public bool IsCreatingOrRunningOrEditing
        {
            get
            {
                return IsCreatingOrEditing || IsRunning;
            }
        }
        
        /// <summary>
        ///  Suppress update of certain properties during the switching of data context
        ///  to avoid undesired data transfer
        /// </summary>
        public static bool SuppressUpdate
        {
            get; set;
        }

        public string GroupName
        {
            get
            {
                return _groupName;
            }
            set
            {
                if (_groupName != value)
                {
                    _groupName = value;
                    RaisePropertyChangedEvent("GroupName");
                }
            }
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
            RaisePropertyChangedEvent("RunningTime");
            RaisePropertyChangedEvent("LocalisedTimeRange");
            RaisePropertyChangedEvent("Duration");
            UpdateGroupName();
        }

        private void RaiseEndTimeChangedEvent()
        {
            RaisePropertyChangedEvent("EndDate");
            RaisePropertyChangedEvent("EndTime");
            RaisePropertyChangedEvent("EndTimeOfDay");
            RaisePropertyChangedEvent("LocalisedTimeRange");
            RaisePropertyChangedEvent("Duration");
        }

        private void RaiseStatusChangedEvents()
        {
            RaisePropertyChangedEvent("Status");
            RaisePropertyChangedEvent("IsEditing");
            RaisePropertyChangedEvent("IsRunning");
            RaisePropertyChangedEvent("IsLoggedEvent");
            RaisePropertyChangedEvent("IsRunningEvent");
            RaisePropertyChangedEvent("IsCreatingOrEditing");
            RaisePropertyChangedEvent("IsCreatingOrRunningOrEditing");
            UpdateGroupName();
        }

        public void RefreshEventTypeProperties()
        {
            RaisePropertyChangedEvent("EventType");
            RaisePropertyChangedEvent("RunningTime");
            RaisePropertyChangedEvent("EventTypeName");
            RaisePropertyChangedEvent("Color");
        }

        public void RefreshAll()
        {
            RaisePropertyChangedEvent("Notes");
            RaiseStartTimeChangedEvent();
            RaiseEndTimeChangedEvent();
            RaiseStatusChangedEvents();
            RefreshEventTypeProperties();
            if (IsRunning)
            {
                RefreshRunningTag();
            }
        }

        public void RefreshRunningTag()
        {
            RaisePropertyChangedEvent("RunningTime");
        }

        public bool IsDataProperty(string name)
        {
            return name == "StartTime" || name == "EndTime" || name == "EventType" || name == "Notes";
        }

        public void RefreshTimeDependent()
        {
            UpdateGroupName();
        }

        private void UpdateGroupName()
        {
            if (IsRunningEvent)
            {
                GroupName = "Currently Running";
            }
            else
            {
                var d = DateTimeHelper.GetDayDiff(StartTime, DateTime.Now);
                if (d == 0)
                {
                    GroupName = "Today";
                }
                else if (d == 1)
                {
                    GroupName = "Yesterday";
                }
                else if (d == -1)
                {
                    GroupName = "Tomorrow";
                }
                else if (d > 0)
                {
                    GroupName = $"{d} days ago";
                }
                else
                {
                    GroupName = $"in {-d} days";
                }
            }
        }

        public void ValidateEndTimeAgainstStartTime()
        {
            if (EndTime < StartTime)
            {
                EndTime = StartTime;
            }
        }

        public void ValidateStartTimeAgainstEndTime()
        {
            if (EndTime < StartTime)
            {
                StartTime = EndTime;
            }
        }

        #endregion
    }
}
