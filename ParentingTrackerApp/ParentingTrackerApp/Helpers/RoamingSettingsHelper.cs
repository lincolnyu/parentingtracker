using ParentingTrackerApp.ViewModels;
using System.Collections.Generic;
using Windows.Storage;
using Windows.UI;
using System;

namespace ParentingTrackerApp.Helpers
{
    public static class RoamingSettingsHelper
    {
        public static void LoadExportSettings(out string path, out string token, 
            out string oneDriveFile, out bool oneDriveSdk)
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
            if (roamingSettings.Values.ContainsKey("exportToken"))
            {
                token = (string)roamingSettings.Values["exportToken"];
            }
            else
            {
                token = "";
            }
            if (roamingSettings.Values.ContainsKey("exportOneDriveFile"))
            {
                oneDriveFile = (string)roamingSettings.Values["exportOneDriveFile"];
            }
            else
            {
                oneDriveFile = "";
            }
            if (roamingSettings.Values.ContainsKey("oneDriveSdk"))
            {
                oneDriveSdk = (bool)roamingSettings.Values["oneDriveSdk"];
            }
            else
            {
                oneDriveSdk = false;
            }
        }

        public static void SaveExportSettings(string path, string token, string oneDriveFile, bool oneDriveSdk)
        {
            var roamingSettings = ApplicationData.Current.RoamingSettings;
            roamingSettings.Values["exportPath"] = path;
            roamingSettings.Values["exportToken"] = token;
            roamingSettings.Values["exportOneDriveFile"] = oneDriveFile;
            roamingSettings.Values["oneDriveSdk"] = oneDriveSdk;
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
                LoadDefaultColorMapping(colors);
            }
        }

        public static void LoadDefaultColorMapping(this ICollection<EventTypeViewModel> colors)
        {
            colors.LoadDefaultParentingColorMapping();
            colors.SaveRoamingColorMapping();
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
    }
}
