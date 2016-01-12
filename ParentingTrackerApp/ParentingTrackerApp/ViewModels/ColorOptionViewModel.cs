using ParentingTrackerApp.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public static ObservableCollection<ColorOptionViewModel> ColorOptions { get; } 
            = new ObservableCollection<ColorOptionViewModel>();

        public string Name { get; }

        public Color Color { get; }
    }
}
