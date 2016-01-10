using System;
using ParentingTrackerApp.ViewModels;
using System.Collections.Generic;
using Windows.Storage;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace ParentingTrackerApp.Helpers
{
    public static class RoamingFilesHelper
    {

        public static async Task<IList<string>> LoadEventsLines(this string fileName)
        {
            var roamingFolder = ApplicationData.Current.RoamingFolder;
            try
            {
                var eventsFile = await roamingFolder.GetFileAsync(fileName);
                var eventsLines = await FileIO.ReadLinesAsync(eventsFile);
                return eventsLines;
            }
            catch (Exception)
            {
                // files not found
                return null;
            }
        }

        public static IEnumerable<EventViewModel> LoadEvents(this IList<string> eventsLines, CentralViewModel cvm)
        {
            foreach (var line in eventsLines)
            {
                var segs = line.Split(',');
                var sstart = segs[0];
                var sstop = segs[1];
                var bsType = segs[2];
                var bsNotes = segs[3];
                var dstart = long.Parse(sstart);
                var dstop = long.Parse(sstop);
                EventViewModel.Statuses status;
                if (segs.Length > 4)
                {
                    var sstatus = segs[4];
                    status = (EventViewModel.Statuses)int.Parse(sstatus);
                }
                else
                {
                    status = EventViewModel.Statuses.Logged;
                }

                var typeName = bsType.Base64UtfToString();
                var notes = bsNotes.Base64UtfToString();
                var et = cvm.EventTypes.FirstOrDefault(x => x.Name == typeName);
                var ev = new EventViewModel(cvm)
                {
                    StartTime = DateTime.FromBinary(dstart),
                    EndTime = DateTime.FromBinary(dstop),
                    EventType = et,
                    Notes = notes,
                    Status = status
                };
                yield return ev;
            }
        }

        public static async Task SaveEvents(this IEnumerable<EventViewModel> events, string fileName)
        {
            var roamingFolder = ApplicationData.Current.RoamingFolder;
            var eventsFile = await roamingFolder.CreateFileAsync(fileName,
                CreationCollisionOption.ReplaceExisting);
            var lines = new List<string>();
            foreach (var ev in events)
            {
                var line = string.Format("{0},{1},{2},{3},{4}",
                    ev.StartTime.ToBinary().ToString(),
                    ev.EndTime.ToBinary().ToString(),
                    ev.EventType.Name.StringToBase64Utf(),
                    ev.Notes.StringToBase64Utf(),
                    (int)ev.Status);
                lines.Add(line);
            }
            await FileIO.WriteLinesAsync(eventsFile, lines);
        }
    }
}
