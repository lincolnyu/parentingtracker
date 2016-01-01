using ParentingTrackerApp.ViewModels;
using System.Collections.Generic;
using Windows.Storage;
using Windows.UI;
using System;

namespace ParentingTrackerApp.Helpers
{
    public static class RoamingSettingsHelper
    {
        public static void LoadExportSettings(out string path)
        {
            var roamingSettings = ApplicationData.Current.RoamingSettings;
            if (roamingSettings.Values.ContainsKey("exportPath"))
            {
                path = (string)roamingSettings.Values["exportPath"];
            }
            else
            {
                path = "";
            }
        }

        public static void SaveExportSettings(string path)
        {
            var roamingSettings = ApplicationData.Current.RoamingSettings;
            roamingSettings.Values["exportPath"] = path;
        }

        public static void LoadRoamingColorMapping(this ICollection<EventTypeViewModel> colors)
        {
            var roamingSettings = ApplicationData.Current.RoamingSettings;
            if (roamingSettings.Values.ContainsKey("colorMapping"))
            {
                var cm =roamingSettings.Values["colorMapping"];
                var cc = (ApplicationDataCompositeValue)cm;
                colors.Clear();
                foreach (var p in cc)
                {
                    var color = ((uint)p.Value).ArgbToColor();
                    var et = new EventTypeViewModel(p.Key, color);                    
                    colors.Add(et);
                }
            }
            else
            {
                colors.LoadDefaultParentingColorMapping();
                colors.SaveRoamingColorMapping();
            }
        }

        public static void SaveRoamingColorMapping(this ICollection<EventTypeViewModel> colors)
        {
            var roamingSettings = ApplicationData.Current.RoamingSettings;
            var cc = new ApplicationDataCompositeValue();
            // NOTE this must be called before assigning cc to the registry below
            // NOTE and the assignment should be made regardless
            foreach (var color in colors)
            {
                cc[color.Name] = ColorToArgb(color.Color);
            }
            roamingSettings.Values["colorMapping"] = cc;
        }

        private static uint ColorToArgb(this Color color)
        {
            uint res = color.A;
            res <<= 8;
            res |= color.R;
            res <<= 8;
            res |= color.G;
            res <<= 8;
            res |= color.B;
            return res;
        }

        private static Color ArgbToColor(this uint ucolor)
        {
            var a = (byte)(ucolor >> 24);
            var r = (byte)((ucolor >> 16) & 0xff);
            var g = (byte)((ucolor >> 8) & 0xff);
            var b = (byte)(ucolor & 0xff);
            return Color.FromArgb(a, r, g, b);
        }

        internal static void LoadExportSettings(out object expPath)
        {
            throw new NotImplementedException();
        }
    }
}
