// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

using ParentingTrackerApp.ViewModels;
using System;
using System.ComponentModel;
using System.Threading;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;
using Windows.System.Profile;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Xaml.Input;
using ParentingTrackerApp.Views;

namespace ParentingTrackerApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : INotifyPropertyChanged
    {
        public enum DeviceFamilies
        {
            WindowsDesktop, // may also include tablets
            WindowsMobile
        }

        private static SolidColorBrush RedButtonBrush = new SolidColorBrush(Colors.Red);

        private readonly Timer _timer;
        private DateTime _time;
        private static Brush _prevButtonBrush;

        static MainPage()
        {
            var df = AnalyticsInfo.VersionInfo.DeviceFamily;
            switch (df)
            {
                case "Windows.Desktop":
                    DeviceFamily = DeviceFamilies.WindowsDesktop;
                    break;
                case "Windows.Mobile":
                    DeviceFamily = DeviceFamilies.WindowsMobile;
                    break;
            }
        }

        public MainPage()
        {
            InitializeComponent();

            DataContext = this;

            _time = DateTime.Now;
            _timer = new Timer(TimerOnTick, null, 0, 1000);
        }

        public DateTime Time
        {
            get { return _time; }
            set
            {
                if (_time != value)
                {
                    _time = value;
                    RaisePropertyChanged("Time");
                }
            }
        }

        public static CentralViewModel CentralViewModel { get; } = new CentralViewModel();

        public static DeviceFamilies DeviceFamily { get; }

        public CentralViewModel Central { get { return CentralViewModel; } }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void TimerOnTick(object state)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    Time = DateTime.Now;
                    Central.Refresh();
                });
        }

        private void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            CentralViewModel.Dispatcher = Dispatcher;

            await CentralViewModel.Load();
        }

        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            CentralViewModel.Dispatcher = null;

            await CentralViewModel.Save();
        }

        private void PivotOnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            var currentView = SystemNavigationManager.GetForCurrentView();
            if (MainPivot.SelectedIndex == 0)
            {
                currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                currentView.BackRequested += CurrentViewOnBackRequested;
            }
            else
            {
                currentView.BackRequested -= CurrentViewOnBackRequested;
                currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
        }

        private void CurrentViewOnBackRequested(object sender, BackRequestedEventArgs args)
        {
            var tv = ((PivotItem)MainPivot.SelectedItem).Content as TrackingView;
            if (tv != null && tv.Restore())
            {
                return;
            }
            if (CentralViewModel.IsEditing)
            {
                CentralViewModel.CloseEditor();
                args.Handled = true;
            }
        }

        /// <summary>
        ///  Sets ads size
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <remarks>
        /// https://msdn.microsoft.com/en-us/library/windows/apps/mt219682.aspx
        /// http://stackoverflow.com/questions/30055312/is-there-a-correct-recommended-way-of-detecting-my-uwp-app-im-running-on-a-phon
        /// </remarks>
        private void MainPageOnSizeChanged(object sender, SizeChangedEventArgs args)
        {
            UpdateClockRowDimension();
            AdjustAds(args.NewSize);
        }

        private void UpdateClockRowDimension()
        {
            ClockRow.Height = new GridLength(Clock.ActualHeight);
        }

        // TODO use single ads source
        private void AdjustAds(Size size)
        {
            switch (DeviceFamily)
            {
                case DeviceFamilies.WindowsDesktop:
                    SetAdsSize(300, 250);
                    PositionAds(size);
                    break;
                case DeviceFamilies.WindowsMobile:
                    SetAdsSize(480, 80);
                    break;
            }
        }

        private void PositionAds(Size size)
        {
            if (size.Width > 800)
            {
                AdsRow.Height = new GridLength(0);
                AdsCol.Width = new GridLength(300);
                MyAds.SetValue(Grid.RowProperty, 0);
                MyAds.SetValue(Grid.RowSpanProperty, 2);
                MyAds.SetValue(Grid.ColumnProperty, 1);
                MyAds.SetValue(Grid.ColumnSpanProperty, 1);
            }
            else
            {
                AdsRow.Height = new GridLength(250);
                AdsCol.Width = new GridLength(0);
                MyAds.SetValue(Grid.RowProperty, 2);
                MyAds.SetValue(Grid.RowSpanProperty, 1);
                MyAds.SetValue(Grid.ColumnProperty, 0);
                MyAds.SetValue(Grid.ColumnSpanProperty, 2);
            }
        }

        private void SetAdsSize(double width, double height)
        {
            MyAds.Width = width;
            AdsRow.Height = new GridLength(height);
            MyAds.Height = height;
        }

        public static void RedHighlightButtonOnPointerEntered(object sender)
        {
            var prevBrush = ((Button)sender).Background;
            if (prevBrush != RedButtonBrush)
            {
                _prevButtonBrush = prevBrush;
            }
            ((Button)sender).Background = new SolidColorBrush(Colors.Red);
        }

        public static void RedHighlightButtonOnPointerExited(object sender)
        {
            ((Button)sender).Background = _prevButtonBrush;
        }
    }
}
