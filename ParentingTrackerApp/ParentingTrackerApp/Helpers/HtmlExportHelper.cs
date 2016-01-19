using ParentingTrackerApp.ViewModels;
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
            foreach (var l in lines)
            {
                var line = l.Trim();
                if (line.StartsWith("<title"))
                {
                    var content = line.IndexOf('>') + 1;
                    var t = line.Substring(7, line.Length - content - 8);
                    docInfo.Title = t;
                }
                else if (line.StartsWith("<tr"))
                {
                    state = 1;
                    evm = new EventViewModel(cvm);
                }
                else if (line.StartsWith("<td") && !line.StartsWith("<td colspan=\"3\">"))
                {
                    var content = line.IndexOf('>')+1;
                    var val = line.Substring(content, line.Length - content - 5);
                    switch (state)
                    {
                        case 1:
                            {
                                var split = val.Split('~');
                                evm.StartTime = split[0].FromNotTooLongString();
                                evm.EndTime = split[1].FromNotTooLongString();
                            }
                            state = 2;
                            break;
                        case 2:
                            evm.EventType = GetOrCreateEventType(cvm.EventTypes, val);
                            state = 3;
                            break;
                        case 3:
                            evm.Notes = val;
                            state = 0;
                            yield return evm;
                            break;
                    }
                }
            }
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
                    foreach (var l in WriteGroupHeader(ev.GroupName, indent))
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

        private static IEnumerable<string> WriteGroupHeader(string groupName, string indent)
        {
            yield return string.Format("{0}  <tr>", indent);
            yield return string.Format("{0}    <td colspan=\"3\" style=\"text-align:left;\">{1}<hr></td>", indent, groupName);
            yield return string.Format("{0}  </tr>", indent);
        }

        private static IEnumerable<string> WriteEntry(EventViewModel ev, string indent)
        {
            var bgcolor = ev.Color;
            var fgcolor = bgcolor.GetConstrastingBlackOrWhite();
            yield return string.Format("{0}  <tr bgcolor=\"{1}\" style=\"color:{2}\">", indent, bgcolor.ToHtmlColor(), fgcolor.ToHtmlColor());
            var startTime = DateTimeHelper.ToNotTooLongString(ev.StartTime);
            var endTime = DateTimeHelper.ToNotTooLongString(ev.EndTime);
            yield return string.Format("{0}    <td style=\"width:40%\">{1}</td>", indent, $"{startTime} ~ {endTime}");
            yield return string.Format("{0}    <td style=\"width:30%\">{1}</td>", indent, ev.EventTypeName);
            yield return string.Format("{0}    <td style=\"width:30%\">{1}</td>", indent, ev.Notes);
            yield return string.Format("{0}  </tr>", indent);
        }

        private static IEnumerable<string> WriteTableFooter(string indent)
        {
            yield return string.Format("{0}</table>", indent);
        }
    }
}
