using ParentingTrackerApp.Default;
using System.Collections.Generic;
using Windows.UI;

namespace ParentingTrackerApp.ViewModels
{
    public class ColorOptionViewModel : BaseViewModel
    {
        static ColorOptionViewModel()
        {
            ColorOptions.LoadColorOptions();
        }

        public ColorOptionViewModel(string name, Color color)
        {
            Name = name;
            Color = color;
        }

        public static List<ColorOptionViewModel> ColorOptions { get; } = new List<ColorOptionViewModel>();

        public string Name { get; }

        public Color Color { get; }
    }
}
