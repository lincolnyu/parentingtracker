using ParentingTrackerApp.Helpers;
using ParentingTrackerApp.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Linq;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Controls.Primitives;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ParentingTrackerApp.Views
{
    public sealed partial class CustomizingView : UserControl
    {
        public CustomizingView()
        {
            InitializeComponent();
        }

        private void AddOnClicked(object sender, RoutedEventArgs e)
        {
            var tvm = (CentralViewModel)DataContext;
            var etvm = new EventTypeViewModel();
            tvm.EventTypes.Add(etvm);

            // TODO may actually want to put this in the view model
            // NOTE from time to time XAML based tech requires this kind of silly hacks to work.
            DelayHelper.Delay(etvm, Kick, 100, Dispatcher);
        }

        private void Kick(object state)
        {
            var etvm = (EventTypeViewModel)state;
            etvm.SelectedColor = etvm.AvailableColors.FirstOrDefault();
            EventTypesList.SelectedItem = etvm;
            EventTypesList.ScrollIntoView(etvm);
        }

        private void DelOnClicked(object sender, RoutedEventArgs e)
        {
            var tvm = (CentralViewModel)DataContext;
            var del = (EventTypeViewModel)((FrameworkElement)sender).DataContext;
            tvm.EventTypes.Remove(del);
        }

        private async void ResetOnClicked(object sender, RoutedEventArgs e)
        {
            var res = await MainPage.PromptUserToConfirm("Events with custom types will lose their types. Are you sure to continue?");
            if (!res)
            {
                return;
            }
            var tvm = (CentralViewModel)DataContext;
            tvm.ResetEventTypesToDefault();
        }

        private void MainGridOnSizeChanged(object sender, SizeChangedEventArgs args)
        {
            ButtonRow.Height = new GridLength(AddButton.ActualHeight);
        }

        private void ColorPickerButtonOnClick(object sender, RoutedEventArgs e)
        {
            var senderElement = (FrameworkElement)sender;
            var flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
            flyoutBase.ShowAt(senderElement);
        }

        private void ItemGridOnSizeChanged(object sender, SizeChangedEventArgs args)
        {
            var grid = (Grid)sender;
            var txt = (TextBox)grid.FindName("NameText");
            var mcol = grid.ColumnDefinitions[0];
            txt.MaxWidth = mcol.ActualWidth - 50;
        }

        private void RedHighlightButtonOnPointerEntered(object sender, PointerRoutedEventArgs args)
        {
            MainPage.RedHighlightButtonOnPointerEntered(sender);
        }

        private void RedHighlightButtonOnPointerExited(object sender, PointerRoutedEventArgs args)
        {
            MainPage.RedHighlightButtonOnPointerExited(sender);
        }
    }
}
