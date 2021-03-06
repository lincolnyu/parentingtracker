﻿using System;
using System.Globalization;

namespace ParentingTrackerApp.Helpers
{
    public static class DateTimeHelper
    {
        public static int CompareTimeIgnoreMs(this DateTime dt1, DateTime dt2)
        {
            var ldt1 = new DateTime(dt1.Year, dt1.Month, dt1.Day, dt1.Hour, dt1.Minute, dt1.Second);
            var ldt2 = new DateTime(dt2.Year, dt2.Month, dt2.Day, dt2.Hour, dt2.Minute, dt2.Second);
            return ldt1.CompareTo(ldt2);
        }

        public static string GetTimeRangeString(DateTime dt1, DateTime dt2)
        {
            var dayDiff = GetDayDiff(dt1, dt2);
            if (dayDiff != 0)
            {
                var rdt1 = dt1.ToRelativeDateTimeString();
                var rdt2 = dt2.ToRelativeDateTimeString();
                return string.Format("{0} to {1}", rdt1, rdt2);
            }
            else if (dt1 != dt2)
            {
                var rdstr = dt1.ToRelativeDayString();
                var hm1 = dt1.ToHourMinute();
                var hm2 = dt2.ToHourMinute();
                return string.Format("{0} {1} to {2}", rdstr, hm1, hm2);
            }
            else
            {
                var rdstr = dt1.ToRelativeDayString();
                var hm1 = dt1.ToHourMinute();
                return string.Format("{0} {1}", rdstr, hm1);
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
                return string.Format("In {0} days", days);
            }
            else
            {
                return string.Format("{0} days ago", -days);
            }
        }

        public static DateTime FromNotTooLongString(this string sdt)
        {
            // TODO currently we presume that DateTim.Parse() is able to handle this...
            return DateTime.Parse(sdt);
        }

        public static string ToNotTooLongString(this DateTime dt)
        {
            var dtfi = CultureInfo.CurrentCulture.DateTimeFormat;
            var dpat = ShortenShortDayPattern(dtfi.ShortDatePattern);
            var dstr = dt.ToString(dpat);
            var tpat = dtfi.LongTimePattern;
            var tstr = dt.ToString(tpat);
            return $"{tstr} {dstr}";
        }

        public static string ToNotTooLongStringDateFirst(this DateTime dt)
        {
            var dtfi = CultureInfo.CurrentCulture.DateTimeFormat;
            var dpat = ShortenShortDayPattern(dtfi.ShortDatePattern);
            var dstr = dt.ToString(dpat);
            var tpat = dtfi.LongTimePattern;
            var tstr = dt.ToString(tpat);
            return $"{dstr} {tstr}";
        }

        public static string ToHourMinute(this DateTime dt)
        {
            var dtfi = CultureInfo.CurrentCulture.DateTimeFormat;
            var pattern = dtfi.ShortTimePattern;
            return dt.ToString(pattern);
        }

        public static string ToTimeString(this DateTime dt)
        {
            var dtfi = CultureInfo.CurrentCulture.DateTimeFormat;
            var pattern = dtfi.LongTimePattern;
            return dt.ToString(pattern);
        }

        public static string ToShortDate(this DateTime dt)
        {
            var dtfi = CultureInfo.CurrentCulture.DateTimeFormat;
            var pattern = dtfi.ShortDatePattern;
            return dt.ToString(pattern);
        }

        public static string ToRelativeDateTimeString(this DateTime dt)
        {
            var relDayStr = dt.ToRelativeDayString();
            var hm = dt.ToHourMinute();
            return string.Format("{0} {1}", relDayStr, hm);            
        }

        private static string ShortenShortDayPattern(string dpat)
        {
            return dpat.Replace("yyyy", "yy");
        }

        public static string ToStandardHtmlTime(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
