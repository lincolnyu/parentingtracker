﻿using System;

namespace ParentingTrackerApp.Helpers
{
    public static class DateTimeHelper
    {
        public static string GetTimeRangeString(DateTime dt1, DateTime dt2)
        {
            var dayDiff = GetDayDiff(dt1, dt2);
            if (dayDiff != 0)
            {
                var rdt1 = dt1.ToRelativeDateTimeString();
                var rdt2 = dt2.ToRelativeDateTimeString();
                return string.Format("{0} to {1}", rdt1, rdt2);
            }
            else
            {
                var rdstr = dt1.ToRelativeDayString();
                var hm1 = dt1.ToHourMinute();
                var hm2 = dt2.ToHourMinute();
                return string.Format("{0} {1} to {2}", rdstr, hm1, hm2);
            }
        }

        public static int GetDayDiff(DateTime dt1, DateTime dt2)
        {
            var days = (int)Math.Round((dt2.Date - dt1.Date).TotalDays);
            return days;
        }

        public static string ToRelativeDayString(this DateTime dt)
        {
            var days = GetDayDiff(DateTime.Today, dt);
            if (days == 0)
            {
                return "Today";
            }
            else if (days == -1)
            {
                return "Yesterday";
            }
            else if (days == 1)
            {
                return "Tomorrow";
            }
            else if (days > 0)
            {
                return string.Format("In {0} days");
            }
            else
            {
                return string.Format("{0} days ago");
            }
        }

        public static string ToHourMinute(this DateTime dt)
        {
            return string.Format("{0:00}:{1:00}", dt.Hour, dt.Minute);
        }

        public static string ToRelativeDateTimeString(this DateTime dt)
        {
            var relDayStr = dt.ToRelativeDayString();
            var hm = dt.ToHourMinute();
            return string.Format("{0} {1}", relDayStr, hm);            
        }
    }
}