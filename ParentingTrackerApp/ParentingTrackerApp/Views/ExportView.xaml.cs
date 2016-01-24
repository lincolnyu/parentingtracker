using ParentingTrackerApp.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ParentingTrackerApp.Export;
using Windows.UI.Xaml.Input;
using Windows.UI.Popups;
using System;
using System.Threading.Tasks;
using System.ComponentModel;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ParentingTrackerApp.Views
{
    public sealed partial class ExportView : UserControl
    {
        private const string InfoMobile = "The following external file will be updated with entries in this app without losing any existing data. For this mobile phone app, specifiy the name of the file on the Document folder of your OneDrive which can be shared across devices but may require authentication to access.";

        private bool _isViewing;

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
                var c = (CentralViewModel)DataContext;
                c.PropertyChanged += ViewModelOnPropertyChanged;
                switch (MainPage.DeviceFamily)
                {
                    case MainPage.DeviceFamilies.WindowsDesktop:
                        UpdateUiAsPerModeSelection();
                        break;
                    case MainPage.DeviceFamilies.WindowsMobile:
                        c.ExportUsingOneDriveSdk = true;// NOTE forced to be using OneDrive and this will be saved
                        OneDriveMobile = new OneDriveMobile((CentralViewModel)DataContext);
                        UpdateUiForMobile();
                        break;
                }
            }
        }

        private async void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "ExportUsingOneDriveSdk"
                || args.PropertyName == "ExportFileText")
            {
                UpdateUiAsPerModeSelection();
                await Refresh();
            }
        }

        private void UpdateUiAsPerModeSelection()
        {
            var c = (CentralViewModel)DataContext;
            if (c.ExportUsingOneDriveSdk)
            {
                HeaderButtonCol.Width = new GridLength(70);
                Header.Width = 70;
                Header.Text = "File name:";
                Select.Visibility = Visibility.Collapsed;
                SelectButtonCol.Width = new GridLength(0);
                FilePicker = null;
                OneDriveMobile = new OneDriveMobile(c);
            }
            else
            {
                HeaderButtonCol.Width = new GridLength(70);
                Header.Width = 70;
                Header.Text = "File path:";
                Select.Visibility = Visibility.Visible;
                SelectButtonCol.Width = new GridLength(30);
                OneDriveMobile = null;
                FilePicker = new FilePicker(c);
            }
        }

        private void UpdateUiForMobile()
        {
            InfoText.Text = InfoMobile;
            HeaderButtonCol.Width = new GridLength(70);
            Header.Width = 70;
            Header.Text = "File Name:";
            Select.Visibility = Visibility.Collapsed;
            SelectButtonCol.Width = new GridLength(0);
            OneDrive.Visibility = Visibility.Collapsed;
            OneDriveButtonCol.Width = new GridLength(0);
        }

        private async void SelectFileOnClick(object sender, RoutedEventArgs args)
        {
            bool result = false;
            if (FilePicker != null)
            {
                result = await FilePicker.PickFile();
            }
            else if (OneDriveMobile != null)
            {
                result = await OneDriveMobile.Connect();
            }
            await Refresh(result && _isViewing);
        }
       
        private async void ExportOnClick(object sender, RoutedEventArgs args)
        {
            bool result = false;
            if (FilePicker != null)
            {
                result = await FilePicker.Merge();
            }
            else if (OneDriveMobile != null)
            {
                result = await OneDriveMobile.Merge();
            }
            await Refresh(result && _isViewing);
        }

        private async void ViewOnClick(object sender, RoutedEventArgs args)
        {
            await Refresh(!_isViewing);
        }

        private async Task Refresh(bool view)
        {
            _isViewing = view;
            await Refresh();
        }

        private async Task Refresh()
        {
            if (_isViewing)
            {
                ViewButton.Content = "Hide";
                var shown = await ShowPage();
                if (!shown)
                {
                    _isViewing = false;
                }
            }
            if (!_isViewing)
            {
                Nav.NavigateToString("");
                ViewButton.Content = "View";
            }
        }
        

        private async Task<bool> ShowPage()
        {
            bool result = false;
            if (FilePicker != null)
            {
                result = await FilePicker.View(Nav);
            }
            else if (OneDriveMobile != null)
            {
                result = await OneDriveMobile.View(Nav);
            }
            return result;
        }

        private async void ClearOnClick(object sender, RoutedEventArgs args)
        {
            var dlg = new MessageDialog("Are you sure you want to clear the external file?" );
            dlg.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(MainPage.YesCommandHandler)));
            dlg.Commands.Add(new UICommand("No", new UICommandInvokedHandler(MainPage.NoCommandHandler)));
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
            await Refresh();
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
