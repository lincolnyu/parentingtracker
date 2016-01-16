using ParentingTrackerApp.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ParentingTrackerApp.Export;
using Windows.UI.Xaml.Input;
using Windows.UI.Popups;
using System;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ParentingTrackerApp.Views
{
    public sealed partial class ExportView : UserControl
    {
        const string InfoMobile = "The following external file will be updated with entries in this app without losing any existing data. For this mobile phone app, specifiy the name of the file on the Document folder of your OneDrive which can be shared across devices but may require authentication to access.";

        public ExportView()
        {
            InitializeComponent();
            DataContextChanged += ViewOnDataContextChanged;
        }

        public FilePicker FilePicker { get; private set; }

        public OneDriveMobile OneDriveMobile { get; private set; }

        private void ViewOnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            UpdateForDataContextChange();
        }

        private void UpdateForDataContextChange()
        {
            if (DataContext != null)
            {
                switch (MainPage.DeviceFamily)
                {
                    case MainPage.DeviceFamilies.WindowsDesktop:
                        FilePicker = new FilePicker((CentralViewModel)DataContext);
                        break;
                    case MainPage.DeviceFamilies.WindowsMobile:
                        OneDriveMobile = new OneDriveMobile((CentralViewModel)DataContext);
                        UpdateUiForMobile();
                        break;
                }
            }
        }

        private void UpdateUiForMobile()
        {
            InfoText.Text = InfoMobile;
            HeaderButtonCol.Width = new GridLength(80);
            Header.Width = 80;
            Header.Text = "File Name:";
            Select.Visibility = Visibility.Collapsed;
            SelectButtonCol.Width = new GridLength(0);
        }

        private async void SelectFileOnClick(object sender, RoutedEventArgs args)
        {
            if (FilePicker != null)
            {
                await FilePicker.PickFile();
            }
            else if (OneDriveMobile != null)
            {
                await OneDriveMobile.Connect();
            }
        }
       
        private async void ExportOnClick(object sender, RoutedEventArgs args)
        {
            if (FilePicker != null)
            {
                await FilePicker.Merge();
            }
            else if (OneDriveMobile != null)
            {
                await OneDriveMobile.Merge();
            }
        }

        private async void ViewOnClick(object sender, RoutedEventArgs args)
        {
            if (FilePicker != null)
            {
                await FilePicker.View(Nav);
            }
            else if (OneDriveMobile != null)
            {
                await OneDriveMobile.View(Nav);
            }
        }

        private async void ClearOnClick(object sender, RoutedEventArgs args)
        {
            var dlg = new MessageDialog("Are you sure you want to clear the external file?" );
            dlg.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(YesToClearHandler)));
            dlg.Commands.Add(new UICommand("No", new UICommandInvokedHandler(NoToClearHandler)));
            var command = await dlg.ShowAsync();
            if ((int)command.Id != 1)
            {
                return;
            }
            if (FilePicker != null)
            {
                await FilePicker.Clear();
            }
            else if (OneDriveMobile != null)
            {
                await OneDriveMobile.Clear();
            }
        }

        private void NoToClearHandler(IUICommand command)
        {
            command.Id = 0;
        }

        private void YesToClearHandler(IUICommand command)
        {
            command.Id = 1;
        }

        private void UserControlOnSizeChanged(object sender, SizeChangedEventArgs args)
        {
            var upperHeight = InfoText.ActualHeight + Header.ActualHeight + InputGrid.ActualHeight + MergeButton.ActualHeight;
            UpperRow.Height = new GridLength(upperHeight);
            LowerRow.Height = new GridLength(MainGrid.ActualHeight - upperHeight);
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
