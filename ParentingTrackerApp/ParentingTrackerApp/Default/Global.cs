using ParentingTrackerApp.ViewModels;
using System.Collections.Generic;
using Windows.UI;

namespace ParentingTrackerApp.Default
{
    public static class Global
    {
        public static void LoadColorOptions(this ICollection<ColorOptionViewModel> colors)
        {
            colors.Clear();
            colors.Add(new ColorOptionViewModel("Red", Colors.Red));
            colors.Add(new ColorOptionViewModel("Green", Colors.Green));
            colors.Add(new ColorOptionViewModel("Blue", Colors.Blue));
            colors.Add(new ColorOptionViewModel("Pink", Colors.Pink));
            colors.Add(new ColorOptionViewModel("Wheat", Colors.Wheat));
            colors.Add(new ColorOptionViewModel("Brown", Colors.Brown));
        }

        public static void LoadDefaultBreastFeedingEventTypes(this ICollection<EventTypeViewModel> types)
        {
            types.Clear();
            types.Add(new EventTypeViewModel("Breast feeding, Left", Colors.Pink));
            types.Add(new EventTypeViewModel("Breast feeding, Right", Colors.Wheat));
            types.Add(new EventTypeViewModel("Nappy changing", Colors.Brown));
        }
    }
}
