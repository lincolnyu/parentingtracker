using System.Collections.Generic;
using System.ComponentModel;
using Windows.UI;

namespace ParentingTrackerApp.ViewModels
{
    public class EventTypeViewModel : BaseViewModel
    {
        private string _name;
        private ColorOptionViewModel _selectedColor;

        public EventTypeViewModel()
        {
            Name = "New event type";
            SelectedColor = AvailableColors[0];
        }

        public EventTypeViewModel(string name, Color color)
        {
            Name = name;
            Color = color;
        }

        /// <summary>
        ///  Event name
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChangedEvent("Name");
                }
            }
        }

        /// <summary>
        ///  Event color
        /// </summary>
        public Color Color
        {
            get { return SelectedColor.Color ; }
            set
            {
                foreach (var colorOption in AvailableColors)
                {
                    if (colorOption.Color == value)
                    {
                        SelectedColor = colorOption;
                        break;
                    }
                }
            }
        }

        /// <summary>
        ///  View model for the selected color
        /// </summary>
        public ColorOptionViewModel SelectedColor
        {
            get { return _selectedColor; }
            set
            {
                if (_selectedColor != value)
                {
                    _selectedColor = value;
                    RaisePropertyChangedEvent("SelectedColor");
                    RaisePropertyChangedEvent("Color");
                }
            }
        }

        public IList<ColorOptionViewModel> AvailableColors
        {
            get { return ColorOptionViewModel.ColorOptions; }
        }
    }
}
