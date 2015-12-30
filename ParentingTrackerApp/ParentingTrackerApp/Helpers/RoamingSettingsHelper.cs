using ParentingTrackerApp.ViewModels;
using System.Collections.Generic;
using Windows.Storage;
using Windows.UI;

namespace ParentingTrackerApp.Helpers
{
    public static class RoamingSettingsHelper
    {
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
            ApplicationDataCompositeValue cc;
            if (roamingSettings.Values.ContainsKey("colorMapping"))
            {
                var cm = roamingSettings.Values["colorMapping"];
                cc = (ApplicationDataCompositeValue)cm;
                cc.Clear();
                foreach (var color in colors)
                {
                    cc[color.Name] = ColorToArgb(color.Color);
                }
            }
            else
            {
                cc = new ApplicationDataCompositeValue();
                foreach (var color in colors)
                {
                    cc[color.Name] = ColorToArgb(color.Color);
                }
                roamingSettings.Values["colorMapping"] = cc;
            }
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
    }
}
