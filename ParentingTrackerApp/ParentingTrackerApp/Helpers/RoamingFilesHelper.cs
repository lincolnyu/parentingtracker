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
        public static async Task<bool> LoadEvents(this ICollection<EventViewModel> events, ICollection<EventTypeViewModel> eventTypes)
        {
            events.Clear();
            var roamingFolder = ApplicationData.Current.RoamingFolder;
            try
            {
                var eventsFile = await roamingFolder.GetFileAsync("events.csv");
                var eventsLines = await FileIO.ReadLinesAsync(eventsFile);
                
                foreach (var line in eventsLines)
                {
                    var segs = line.Split(',');
                    var sstart = segs[0];
                    var sstop = segs[1];
                    var bsType = segs[2];
                    var bsNotes = segs[3];
                    var dstart = long.Parse(sstart);
                    var dstop = long.Parse(sstop);

                    var typeName = bsType.Base64UtfToString();
                    var notes = bsNotes.Base64UtfToString();
                    var et = eventTypes.FirstOrDefault(x => x.Name == typeName);
                    var ev = new EventViewModel
                    {
                        StartTime = DateTime.FromBinary(dstart),
                        EndTime = DateTime.FromBinary(dstop),
                        EventType = et,
                        Notes = notes
                    };
                    events.Add(ev);
                }
                return true;
            }
            catch (Exception e)
            {
                // files not found
                return false;
            }
        }

        public static async Task SaveEvents(this ICollection<EventViewModel> events)
        {
            var roamingFolder = ApplicationData.Current.RoamingFolder;
            var eventsFile = await roamingFolder.CreateFileAsync("events.csv",
                CreationCollisionOption.ReplaceExisting);
            var lines = new List<string>(events.Count);
            foreach (var ev in events)
            {
                var line = string.Format("{0},{1},{2},{3}",
                    ev.StartTime.ToBinary().ToString(),
                    ev.EndTime.ToBinary().ToString(),
                    ev.EventType.Name.StringToBase64Utf(),
                    ev.Notes.StringToBase64Utf());
                lines.Add(line);
            }
            await FileIO.WriteLinesAsync(eventsFile, lines);
        }
    }
}
