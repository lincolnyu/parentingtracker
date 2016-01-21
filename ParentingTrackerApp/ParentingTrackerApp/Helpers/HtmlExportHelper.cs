using ParentingTrackerApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ParentingTrackerApp.Helpers
{
    public static class HtmlExportHelper
    {
        public class DocInfo
        {
            public string Title { get; set; }
        }

        public static IEnumerable<EventViewModel> ReadFromTable(this IEnumerable<string> lines,
            CentralViewModel cvm, DocInfo docInfo)
        {
            var state = 0;
            EventViewModel evm = null;
            var thCount = 0;
            foreach (var l in lines)
            {
                var line = l.Trim();
                if (line.StartsWith("<title"))
                {
                    var content = line.IndexOf('>') + 1;
                    var t = line.Substring(7, line.Length - content - 8);
                    docInfo.Title = t;
                }
                else if (line.StartsWith("<th"))
                {
                    if (state == 1)
                    {
                        thCount = 1;
                        state = 2;
                    }
                    else
                    {
                        thCount++;
                    }
                }
                else if (line.StartsWith("<tr"))
                {
                    state = 1;
                    evm = new EventViewModel(cvm) { Status = EventViewModel.Statuses.Logged }; // should always be of logged type
                }
                else if (state >= 1 && line.StartsWith("<td") && !line.StartsWith("<td colspan=\"3\""))
                {
                    if (thCount==3)
                    {
                        if (ParseVersion1p2(line, cvm, evm, ref state))
                        {
                            yield return evm;
                        }
                    }
                    else if (thCount==4)
                    {
                        if (ParseInitialVersion(line, cvm, evm, ref state))
                        {
                            yield return evm;
                        }
                    }
                }
            }
        }

        private static bool ParseInitialVersion(string line, CentralViewModel cvm, EventViewModel evm, ref int state)
        {
            var content = line.IndexOf('>') + 1;
            var val = line.Substring(content, line.Length - content - 5);
            switch (state)
            {
                case 1:
                    evm.StartTime = DateTime.Parse(val);
                    state = 2;
                    return false;
                case 2:
                    evm.EndTime = DateTime.Parse(val);
                    state = 3;
                    return false;
                case 3:
                    evm.EventType = GetOrCreateEventType(cvm.EventTypes, val);
                    state = 4;
                    return false;
                case 4:
                    evm.Notes = val;
                    state = 0;
                    return true;
            }
            return true;// unexpected
        }

        private static bool ParseVersion1p2(string line, CentralViewModel cvm, EventViewModel evm, ref int state)
        {
            var content = line.IndexOf('>') + 1;
            var val = line.Substring(content, line.Length - content - 5);
            switch (state)
            {
                case 1:
                    {
                        const string hdt = "<time datetime=\"";
                        // should have at least one
                        var i1 = val.IndexOf(hdt);
                        var is1 = i1 + hdt.Length;
                        var ie1 = val.IndexOf('"', is1);
                        var s1 = val.Substring(is1, ie1 - is1);
                        evm.StartTime = DateTime.Parse(s1);
                        var i2 = val.IndexOf(hdt, ie1);
                        if (i2 >= 0)
                        {
                            var is2 = i2 + hdt.Length;
                            var ie2 = val.IndexOf('"', is2);
                            var s2 = val.Substring(is2, ie2 - is2);
                            evm.EndTime = DateTime.Parse(s2);
                        }
                        else
                        {
                            evm.EndTime = evm.StartTime;
                        }
                    }
                    state = 2;
                    return false;
                case 2:
                    evm.EventType = GetOrCreateEventType(cvm.EventTypes, val);
                    state = 3;
                    return false;
                case 3:
                    evm.Notes = val;
                    state = 0;
                    return true;
            }
            return true;// unexpected
        }
        
        private static EventTypeViewModel GetOrCreateEventType(ICollection<EventTypeViewModel> eventTypes, string name)
        {
            var et = eventTypes.FirstOrDefault(x => x.Name == name);
            if (et == null)
            {
                // create a temporary one
                et = new EventTypeViewModel
                {
                    Name = name
                };
            }
            return et;
        }

        public static IEnumerable<EventViewModel> Merge(IEnumerable<EventViewModel> existing, IEnumerable<EventViewModel> toadd)
        {
            var merged = existing.Concat(toadd).OrderBy(x => x);
            // Since Distinct() seems not to work... (maybe due to the accuracy issue 
            // which has now been fixed by the date-time compare helper
            // But still this could be more effient than Distinct() as it's aware of
            // the advantage of OrderBy()
            EventViewModel last = null;
            foreach (var x in merged)
            {
                if (last != null && x.CompareTo(last) == 0)
                {
                    continue;
                }
                last = x;
                yield return x;
            }
        }

        public static IEnumerable<string> WriteToTable(this IEnumerable<EventViewModel> events, 
            DocInfo docInfo)
        {
            yield return "<!DOCTYPE html>";
            yield return "<!-- Parenting Tracker Exported File Version 1.2 -->";
            yield return "<html>";
            yield return "  <head>";
            yield return string.Format("    <title>{0}</title>", docInfo != null? docInfo.Title 
                : "Untitiled");
            yield return "    <meta charset=\"utf-8\">";
            yield return "    <style>";
            yield return "      table { width: 100%; font-family:'Arial'; }";
            yield return "      td { text-align: center; }";
            yield return "    </style>";
            yield return "  </head>";
            yield return "<body>";
            var indent = GetIndent(1);
            foreach (var l in WriteTableHeader(indent))
            {
                yield return l;
            }
            string lastGroupName = null;
            foreach (var ev in events)
            {
                if (ev.GroupName != lastGroupName)
                {
                    foreach (var l in WriteGroupHeader(ev, indent))
                    {
                        yield return l;
                    }
                    lastGroupName = ev.GroupName;
                }
                foreach (var l in WriteEntry(ev, indent))
                {
                    yield return l;
                }
            }
            foreach (var l in WriteTableFooter(indent))
            {
                yield return l;
            }
            
            yield return "</body>";
            yield return "</html>";
        }

        public static IEnumerable<string> GetEmptyHtmlLines(string title)
        {
            var docInfo = new DocInfo { Title = title };
            return WriteToTable(Enumerable.Empty<EventViewModel>(), docInfo);
        }

        private static string GetIndent(int len)
        {
            return new string(' ', len * 2);
        }

        private static IEnumerable<string> WriteTableHeader(string indent)
        {
            yield return string.Format("{0}<table>", indent);
            yield return string.Format("{0}  <tr>", indent);
            yield return string.Format("{0}    <th>Time</th>", indent);
            yield return string.Format("{0}    <th>Type</th>", indent);
            yield return string.Format("{0}    <th>Notes</th>", indent);
            yield return string.Format("{0}  </tr>", indent);
        }

        private static IEnumerable<string> WriteGroupHeader(EventViewModel ev, string indent)
        {
            var date = ev.StartTime.ToShortDate();
            yield return string.Format("{0}  <tr>", indent);
            yield return string.Format("{0}    <td colspan=\"3\" style=\"text-align:left;\">{1}<hr></td>", indent, date);
            yield return string.Format("{0}  </tr>", indent);
        }

        private static IEnumerable<string> WriteEntry(EventViewModel ev, string indent)
        {
            var bgcolor = ev.Color;
            var fgcolor = bgcolor.GetConstrastingBlackOrWhite();
            yield return string.Format("{0}  <tr bgcolor=\"{1}\" style=\"color:{2}\">", indent, bgcolor.ToHtmlColor(), fgcolor.ToHtmlColor());
            var startTime = ev.StartTime.ToTimeString();
            var endTime = ev.EndDate == ev.StartDate ?
                ev.EndTime.ToTimeString() : ev.EndTime.ToNotTooLongStringDateFirst();
            var duration = ev.Duration;
            var standardStart = ev.StartTime.ToStandardHtmlTime();
            var standardEnd = ev.EndTime.ToStandardHtmlTime();
            if (ev.StartTime.CompareTimeIgnoreMs(ev.EndTime) == 0)
            {
                yield return $"{indent}    <td style=\"width:40%\"><time datetime=\"{standardStart}\">{startTime}</time></td>";
            }
            else if (string.IsNullOrWhiteSpace(duration))
            {
                yield return $"{indent}    <td style=\"width:40%\"><time datetime=\"{standardStart}\">{startTime}</time> ~ <time datetime=\"{standardEnd}\">{endTime}</time></td>";
            }
            else
            {
                yield return $"{indent}    <td style=\"width:40%\"><time datetime=\"{standardStart}\">{startTime}</time> ~ <time datetime=\"{standardEnd}\">{endTime}</time> {duration}</td>";
            }
            yield return $"{indent}    <td style=\"width:30%\">{ev.EventTypeName}</td>";
            yield return $"{indent}    <td style=\"width:30%\">{ev.Notes}</td>";
            yield return $"{indent}  </tr>";
        }

        private static IEnumerable<string> WriteTableFooter(string indent)
        {
            yield return string.Format("{0}</table>", indent);
        }
    }
}
