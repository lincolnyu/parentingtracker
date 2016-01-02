using ParentingTrackerApp.ViewModels;
using System.Collections.Generic;
using Windows.UI;

namespace ParentingTrackerApp.Helpers
{
    public static class DefaultGlobals
    {
        public static void LoadColorOptions(this ICollection<ColorOptionViewModel> colors)
        {
            colors.Clear();
            colors.Add(new ColorOptionViewModel("Red", Colors.Red));
            colors.Add(new ColorOptionViewModel("Green", Colors.Green));
            colors.Add(new ColorOptionViewModel("Blue", Colors.Blue));
            colors.Add(new ColorOptionViewModel("Pink", Colors.Pink));
            colors.Add(new ColorOptionViewModel("Light Pink", Colors.LightPink));
            colors.Add(new ColorOptionViewModel("Deep Pink", Colors.DeepPink));
            colors.Add(new ColorOptionViewModel("Wheat", Colors.Wheat));
            colors.Add(new ColorOptionViewModel("Brown", Colors.Brown));
            colors.Add(new ColorOptionViewModel("Dark Blue", Colors.DarkBlue));
            colors.Add(new ColorOptionViewModel("Light Blue", Colors.LightBlue));
            colors.Add(new ColorOptionViewModel("Gray", Colors.Gray));
        }

        public static void LoadDefaultParentingColorMapping(this ICollection<EventTypeViewModel> types)
        {
            types.Clear();
            types.Add(new EventTypeViewModel("Breast feeding, Left", Colors.Pink));
            types.Add(new EventTypeViewModel("Breast feeding, Right", Colors.Wheat));
            types.Add(new EventTypeViewModel("Nappy changing", Colors.Brown));
            types.Add(new EventTypeViewModel("Bathing", Colors.Blue));
            types.Add(new EventTypeViewModel("Sleeping", Colors.DarkBlue));
            types.Add(new EventTypeViewModel("Measuring", Colors.Gray));
            types.Add(new EventTypeViewModel("Others", Colors.Green));
        }
    }
}
