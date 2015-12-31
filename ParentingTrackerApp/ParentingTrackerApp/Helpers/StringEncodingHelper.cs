using System;
using System.Text;

namespace ParentingTrackerApp.Helpers
{
    public static class StringEncodingHelper
    {
        public static string StringToBase64Utf(this string orig)
        {
            if (orig == null)
            {
                orig = string.Empty;
            }
            var b = Encoding.UTF8.GetBytes(orig);
            var s = Convert.ToBase64String(b);
            return s;
        }

        public static string Base64UtfToString(this string bs)
        {
            var b = Convert.FromBase64String(bs);
            var s = Encoding.UTF8.GetString(b);
            return s;
        }
    }
}
