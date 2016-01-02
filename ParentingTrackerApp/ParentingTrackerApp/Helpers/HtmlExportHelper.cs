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
            ICollection<EventTypeViewModel> eventTypes, DocInfo docInfo)
        {
            var state = 0;
            EventViewModel evm = null;
            foreach (var l in lines)
            {
                var line = l.Trim();
                if (line.StartsWith("<title>"))
                {
                    var t = line.Substring(7, line.Length - 7 - 8);
                    docInfo.Title = t;
                }
                else if (line.StartsWith("<tr>"))
                {
                    state = 1;
                    evm = new EventViewModel();
                }
                else if (line.StartsWith("<td>"))
                {
                    var val = line.Substring(4, line.Length - 4 - 5);
                    switch (state)
                    {
                        case 1:
                            evm.StartTime = DateTime.Parse(val);
                            state = 2;
                            break;
                        case 2:
                            evm.EndTime = DateTime.Parse(val);
                            state = 3;
                            break;
                        case 3:
                            evm.EventType = GetOrCreateEventType(eventTypes, val);
                            state = 4;
                            break;
                        case 4:
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
                if (last != null && x.CompareTo(last)==0)
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
            yield return "<html>";
            yield return "  <head>";
            yield return string.Format("    <title>{0}</title>", docInfo.Title);
            yield return "    <meta charset=\"utf-8\">";
            yield return "    <style>";
            yield return "      table { width: 100%; }";
            yield return "      td { text-align: center; }";
            yield return "    </style>";
            yield return "  </head>";
            yield return "<body>";
            var indent = GetIndent(1);
            foreach (var l in WriteTableHeader(indent))
            {
                yield return l;
            }
            foreach (var ev in events)
            {
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

        private static string GetIndent(int len)
        {
            return new string(' ', len * 2);
        }

        private static IEnumerable<string> WriteTableHeader(string indent)
        {
            yield return string.Format("{0}<table>", indent);
            yield return string.Format("{0}  <tr>", indent);
            yield return string.Format("{0}    <th>Start</th>", indent);
            yield return string.Format("{0}    <th>End</th>", indent);
            yield return string.Format("{0}    <th>Type</th>", indent);
            yield return string.Format("{0}    <th>Notes</th>", indent);
            yield return string.Format("{0}  </tr>", indent);
        }

        private static IEnumerable<string> WriteEntry(EventViewModel ev, string indent)
        {
            yield return string.Format("{0}  <tr>", indent);
            yield return string.Format("{0}    <td>{1}</td>", indent, ev.StartTime);
            yield return string.Format("{0}    <td>{1}</td>", indent, ev.EndTime);
            yield return string.Format("{0}    <td>{1}</td>", indent, ev.Type);
            yield return string.Format("{0}    <td>{1}</td>", indent, ev.Notes);
            yield return string.Format("{0}  </tr>", indent);
        }

        private static IEnumerable<string> WriteTableFooter(string indent)
        {
            yield return string.Format("{0}</table>", indent);
        }
    }
}
