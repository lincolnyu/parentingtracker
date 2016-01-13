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

namespace ParentingTrackerApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : INotifyPropertyChanged
    {
        private readonly Timer _timer;
        private DateTime _time;

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

        public CentralViewModel Central { get { return CentralViewModel; } }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void TimerOnTick(object state)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
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

        private void PivotOnSelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs args)
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
            var df = AnalyticsInfo.VersionInfo.DeviceFamily;
            switch (df)
            {
                case "Windows.Desktop":
                    SetAdsSize(300, 250);
                    break;
                case "Windows.Mobile":
                    SetAdsSize(480, 80);
                    break;
            }
        }

        private void SetAdsSize(double width, double height)
        {
            MyAds.Width = width;
            AdsRow.Height = new GridLength(height);
            MyAds.Height = height;
        }
    }
}
