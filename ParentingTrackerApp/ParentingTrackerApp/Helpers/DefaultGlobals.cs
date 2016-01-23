using ParentingTrackerApp.ViewModels;
using System;
using System.Collections.Generic;
using Windows.UI;

namespace ParentingTrackerApp.Helpers
{
    public static class DefaultGlobals
    {
        public static EventTypeViewModel[] DefaultEventTypeList =
        {
            new EventTypeViewModel("Breast feed (Left)", Colors.LightPink),
            new EventTypeViewModel("Breast feed (Right)", Colors.Wheat),
            new EventTypeViewModel("Breast feed (Both)", Colors.Pink),
            new EventTypeViewModel("Express (Left)", Colors.LightPink),
            new EventTypeViewModel("Express (Right)", Colors.Wheat),
            new EventTypeViewModel("Express (Both)", Colors.Pink),
            new EventTypeViewModel("Bottle feed", Colors.Cyan),
            new EventTypeViewModel("Nappy change", Colors.Brown),
            new EventTypeViewModel("Bath", Colors.Blue),
            new EventTypeViewModel("Sleep", Colors.DarkBlue),
            new EventTypeViewModel("Play", Colors.Orange),
            new EventTypeViewModel("Measurement", Colors.Gray),
            new EventTypeViewModel("Others", Colors.Green),
        };

        public static void LoadColorOptions(this ICollection<ColorOptionViewModel> colors)
        {
            colors.Clear();
            colors.Add(new ColorOptionViewModel("Antique White", Colors.AntiqueWhite));
            colors.Add(new ColorOptionViewModel("Aqua", Colors.Aqua));
            colors.Add(new ColorOptionViewModel("Aquamarine", Colors.Aquamarine));
            colors.Add(new ColorOptionViewModel("Azure", Colors.Azure));
            colors.Add(new ColorOptionViewModel("Beige", Colors.Beige));
            colors.Add(new ColorOptionViewModel("Bisque", Colors.Bisque));
            colors.Add(new ColorOptionViewModel("Black", Colors.Black));
            colors.Add(new ColorOptionViewModel("Blanched Almond", Colors.BlanchedAlmond));
            colors.Add(new ColorOptionViewModel("Blue", Colors.Blue));
            colors.Add(new ColorOptionViewModel("Blue Violet", Colors.BlueViolet));
            colors.Add(new ColorOptionViewModel("Brown", Colors.Brown));
            colors.Add(new ColorOptionViewModel("Burly Wood", Colors.BurlyWood));
            colors.Add(new ColorOptionViewModel("Cadet Blue", Colors.CadetBlue));
            colors.Add(new ColorOptionViewModel("Chartreuse", Colors.Chartreuse));
            colors.Add(new ColorOptionViewModel("Chocolate", Colors.Chocolate));
            colors.Add(new ColorOptionViewModel("Coral", Colors.Coral));
            colors.Add(new ColorOptionViewModel("Cornflower Blue", Colors.CornflowerBlue));
            colors.Add(new ColorOptionViewModel("Cornsilk", Colors.Cornsilk));
            colors.Add(new ColorOptionViewModel("Crimson", Colors.Crimson));
            colors.Add(new ColorOptionViewModel("Cyan", Colors.Cyan));
            colors.Add(new ColorOptionViewModel("Dark Blue", Colors.DarkBlue));
            colors.Add(new ColorOptionViewModel("Dark Cyan", Colors.DarkCyan));
            colors.Add(new ColorOptionViewModel("Dark Goldenrod", Colors.DarkGoldenrod));
            colors.Add(new ColorOptionViewModel("Dark Gray", Colors.DarkGray));
            colors.Add(new ColorOptionViewModel("Dark Green", Colors.DarkGreen));
            colors.Add(new ColorOptionViewModel("Dark Khaki", Colors.DarkKhaki));
            colors.Add(new ColorOptionViewModel("Dark Magenta", Colors.DarkMagenta));
            colors.Add(new ColorOptionViewModel("Dark OliveGreen", Colors.DarkOliveGreen));
            colors.Add(new ColorOptionViewModel("Dark Orange", Colors.DarkOrange));
            colors.Add(new ColorOptionViewModel("Dark Orchid", Colors.DarkOrchid));
            colors.Add(new ColorOptionViewModel("Dark Red", Colors.DarkRed));
            colors.Add(new ColorOptionViewModel("Dark Salmon", Colors.DarkSalmon));
            colors.Add(new ColorOptionViewModel("Dark SeaGreen", Colors.DarkSeaGreen));
            colors.Add(new ColorOptionViewModel("Dark SlateBlue", Colors.DarkSlateBlue));
            colors.Add(new ColorOptionViewModel("Dark SlateGray", Colors.DarkSlateGray));
            colors.Add(new ColorOptionViewModel("Dark Turquoise", Colors.DarkTurquoise));
            colors.Add(new ColorOptionViewModel("Dark Violet", Colors.DarkViolet));
            colors.Add(new ColorOptionViewModel("Deep Pink", Colors.DeepPink));
            colors.Add(new ColorOptionViewModel("Deep SkyBlue", Colors.DeepSkyBlue));
            colors.Add(new ColorOptionViewModel("Dim Gray", Colors.DimGray));
            colors.Add(new ColorOptionViewModel("Dodger Blue", Colors.DodgerBlue));
            colors.Add(new ColorOptionViewModel("Firebrick", Colors.Firebrick));
            colors.Add(new ColorOptionViewModel("Floral White", Colors.FloralWhite));
            colors.Add(new ColorOptionViewModel("Forest Green", Colors.ForestGreen));
            colors.Add(new ColorOptionViewModel("Fuchsia", Colors.Fuchsia));
            colors.Add(new ColorOptionViewModel("Gainsboro", Colors.Gainsboro));
            colors.Add(new ColorOptionViewModel("Ghost White", Colors.GhostWhite));
            colors.Add(new ColorOptionViewModel("Gold", Colors.Gold));
            colors.Add(new ColorOptionViewModel("Goldenrod", Colors.Goldenrod));
            colors.Add(new ColorOptionViewModel("Gray", Colors.Gray));
            colors.Add(new ColorOptionViewModel("Green", Colors.Green));
            colors.Add(new ColorOptionViewModel("Green Yellow", Colors.GreenYellow));
            colors.Add(new ColorOptionViewModel("Honeydew", Colors.Honeydew));
            colors.Add(new ColorOptionViewModel("Hot Pink", Colors.HotPink));
            colors.Add(new ColorOptionViewModel("Indian Red", Colors.IndianRed));
            colors.Add(new ColorOptionViewModel("Indigo", Colors.Indigo));
            colors.Add(new ColorOptionViewModel("Ivory", Colors.Ivory));
            colors.Add(new ColorOptionViewModel("Khaki", Colors.Khaki));
            colors.Add(new ColorOptionViewModel("Lavender", Colors.Lavender));
            colors.Add(new ColorOptionViewModel("Lavender Blush", Colors.LavenderBlush));
            colors.Add(new ColorOptionViewModel("Lawn Green", Colors.LawnGreen));
            colors.Add(new ColorOptionViewModel("Lemon Chiffon", Colors.LemonChiffon));
            colors.Add(new ColorOptionViewModel("Light Blue", Colors.LightBlue));
            colors.Add(new ColorOptionViewModel("Light Coral", Colors.LightCoral));
            colors.Add(new ColorOptionViewModel("Light Cyan", Colors.LightCyan));
            colors.Add(new ColorOptionViewModel("Light Goldenrod Yellow", Colors.LightGoldenrodYellow));
            colors.Add(new ColorOptionViewModel("Light Gray", Colors.LightGray));
            colors.Add(new ColorOptionViewModel("Light Green", Colors.LightGreen));
            colors.Add(new ColorOptionViewModel("Light Pink", Colors.LightPink));
            colors.Add(new ColorOptionViewModel("Light Salmon", Colors.LightSalmon));
            colors.Add(new ColorOptionViewModel("Light Sea Green", Colors.LightSeaGreen));
            colors.Add(new ColorOptionViewModel("Light Sky Blue", Colors.LightSkyBlue));
            colors.Add(new ColorOptionViewModel("Light Slate Gray", Colors.LightSlateGray));
            colors.Add(new ColorOptionViewModel("Light Steel Blue", Colors.LightSteelBlue));
            colors.Add(new ColorOptionViewModel("Light Yellow", Colors.LightYellow));
            colors.Add(new ColorOptionViewModel("Lime", Colors.Lime));
            colors.Add(new ColorOptionViewModel("Lime Green", Colors.LimeGreen));
            colors.Add(new ColorOptionViewModel("Linen", Colors.Linen));
            colors.Add(new ColorOptionViewModel("Magenta", Colors.Magenta));
            colors.Add(new ColorOptionViewModel("Maroon", Colors.Maroon));
            colors.Add(new ColorOptionViewModel("Medium Aquamarine", Colors.MediumAquamarine));
            colors.Add(new ColorOptionViewModel("Medium Blue", Colors.MediumBlue));
            colors.Add(new ColorOptionViewModel("Medium Orchid", Colors.MediumOrchid));
            colors.Add(new ColorOptionViewModel("Medium Purple", Colors.MediumPurple));
            colors.Add(new ColorOptionViewModel("Medium Sea Green", Colors.MediumSeaGreen));
            colors.Add(new ColorOptionViewModel("Medium Slate Blue", Colors.MediumSlateBlue));
            colors.Add(new ColorOptionViewModel("Medium Spring Green", Colors.MediumSpringGreen));
            colors.Add(new ColorOptionViewModel("Medium Turquoise", Colors.MediumTurquoise));
            colors.Add(new ColorOptionViewModel("Medium VioletRed", Colors.MediumVioletRed));
            colors.Add(new ColorOptionViewModel("Midnight Blue", Colors.MidnightBlue));
            colors.Add(new ColorOptionViewModel("Mint Cream", Colors.MintCream));
            colors.Add(new ColorOptionViewModel("Misty Rose", Colors.MistyRose));
            colors.Add(new ColorOptionViewModel("Moccasin", Colors.Moccasin));
            colors.Add(new ColorOptionViewModel("NavajoWhite", Colors.NavajoWhite));
            colors.Add(new ColorOptionViewModel("Navy", Colors.Navy));
            colors.Add(new ColorOptionViewModel("Old Lace", Colors.OldLace));
            colors.Add(new ColorOptionViewModel("Olive", Colors.Olive));
            colors.Add(new ColorOptionViewModel("Olive Drab", Colors.OliveDrab));
            colors.Add(new ColorOptionViewModel("Orange", Colors.Orange));
            colors.Add(new ColorOptionViewModel("Orange Red", Colors.OrangeRed));
            colors.Add(new ColorOptionViewModel("Orchid", Colors.Orchid));
            colors.Add(new ColorOptionViewModel("Pale Goldenrod", Colors.PaleGoldenrod));
            colors.Add(new ColorOptionViewModel("Pale Green", Colors.PaleGreen));
            colors.Add(new ColorOptionViewModel("Pale Turquoise", Colors.PaleTurquoise));
            colors.Add(new ColorOptionViewModel("Pale VioletRed", Colors.PaleVioletRed));
            colors.Add(new ColorOptionViewModel("Papaya Whip", Colors.PapayaWhip));
            colors.Add(new ColorOptionViewModel("Peach Puff", Colors.PeachPuff));
            colors.Add(new ColorOptionViewModel("Peru", Colors.Peru));
            colors.Add(new ColorOptionViewModel("Pink", Colors.Pink));
            colors.Add(new ColorOptionViewModel("Plum", Colors.Plum));
            colors.Add(new ColorOptionViewModel("Powder Blue", Colors.PowderBlue));
            colors.Add(new ColorOptionViewModel("Purple", Colors.Purple));
            colors.Add(new ColorOptionViewModel("Red", Colors.Red));
            colors.Add(new ColorOptionViewModel("Rosy Brown", Colors.RosyBrown));
            colors.Add(new ColorOptionViewModel("Royal Blue", Colors.RoyalBlue));
            colors.Add(new ColorOptionViewModel("Saddle Brown", Colors.SaddleBrown));
            colors.Add(new ColorOptionViewModel("Salmon", Colors.Salmon));
            colors.Add(new ColorOptionViewModel("Sandy Brown", Colors.SandyBrown));
            colors.Add(new ColorOptionViewModel("Sea Green", Colors.SeaGreen));
            colors.Add(new ColorOptionViewModel("Sea Shell", Colors.SeaShell));
            colors.Add(new ColorOptionViewModel("Sienna", Colors.Sienna));
            colors.Add(new ColorOptionViewModel("Silver", Colors.Silver));
            colors.Add(new ColorOptionViewModel("Sky Blue", Colors.SkyBlue));
            colors.Add(new ColorOptionViewModel("Slate Blue", Colors.SlateBlue));
            colors.Add(new ColorOptionViewModel("Slate Gray", Colors.SlateGray));
            colors.Add(new ColorOptionViewModel("Snow", Colors.Snow));
            colors.Add(new ColorOptionViewModel("Spring Green", Colors.SpringGreen));
            colors.Add(new ColorOptionViewModel("Steel Blue", Colors.SteelBlue));
            colors.Add(new ColorOptionViewModel("Tan", Colors.Tan));
            colors.Add(new ColorOptionViewModel("Teal", Colors.Teal));
            colors.Add(new ColorOptionViewModel("Thistle", Colors.Thistle));
            colors.Add(new ColorOptionViewModel("Tomato", Colors.Tomato));
            colors.Add(new ColorOptionViewModel("Transparent", Colors.Transparent));
            colors.Add(new ColorOptionViewModel("Turquoise", Colors.Turquoise));
            colors.Add(new ColorOptionViewModel("Violet", Colors.Violet));
            colors.Add(new ColorOptionViewModel("Wheat", Colors.Wheat));
            colors.Add(new ColorOptionViewModel("White", Colors.White));
            colors.Add(new ColorOptionViewModel("White Smoke", Colors.WhiteSmoke));
            colors.Add(new ColorOptionViewModel("Yellow", Colors.Yellow));
            colors.Add(new ColorOptionViewModel("Yellow Green", Colors.YellowGreen));
        }

        public static void LoadDefaultParentingColorMapping(this ICollection<EventTypeViewModel> eventTypes)
        {
            eventTypes.Clear();
            foreach (var e in DefaultEventTypeList)
            {
                eventTypes.Add(e.Clone());
            }
        }
    }
}
