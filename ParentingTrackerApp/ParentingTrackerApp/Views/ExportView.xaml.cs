using ParentingTrackerApp.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ParentingTrackerApp.Export;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ParentingTrackerApp.Views
{
    public sealed partial class ExportView : UserControl
    {
        const string InfoMobile = "The following external file will be updated with entries in this app without losing any existing data. For this mobile phone app, the file which you can specify a name for is to be sitting on the Document folder of your OneDrive which can be shared across devices.";

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
                        break;
                }
            }

            UpdateUiForMobile();
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

        private void UserControlOnSizeChanged(object sender, SizeChangedEventArgs args)
        {
            var upperHeight = InfoText.ActualHeight + Header.ActualHeight + InputGrid.ActualHeight + MergeButton.ActualHeight;
            UpperRow.Height = new GridLength(upperHeight);
            LowerRow.Height = new GridLength(MainGrid.ActualHeight - upperHeight);
        }
    }
}
