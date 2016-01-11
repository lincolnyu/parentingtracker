// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

using ParentingTrackerApp.ViewModels;
using System;
using System.ComponentModel;
using System.Threading;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;
using Windows.System.Profile;
using Windows.Foundation;

namespace ParentingTrackerApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : INotifyPropertyChanged
    {
        private const string DesktopAdsId = "AdMediator-Id-6280A407-B64E-431B-B032-C97daC77CAE7";
        private const string MobileAdsId = "AdMediator-Id-535A18C3-9E0D-4BA4-A359-F1928CF778A0";

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

            await CentralViewModel.Load();
        }

        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            await CentralViewModel.Save();
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
                    SetAdsId(DesktopAdsId);
                    SetAdsSize(728, 90);
                    break;
                case "Windows.Mobile":
                    if (size.Width < 320 && size.Width >= 300)
                    {
                        SetAdsId(MobileAdsId);
                        SetAdsSize(300, 50);
                    }
                    else if (size.Width < 480)
                    {
                        SetAdsId(MobileAdsId);
                        SetAdsSize(320, 50);
                    }
                    else if (size.Width < 640)
                    {
                        SetAdsId(MobileAdsId);
                        SetAdsSize(480, 80);
                    }
                    else if (size.Width < 728)
                    {
                        SetAdsId(MobileAdsId);
                        SetAdsSize(640, 100);
                    }
                    else
                    {
                        // could be Windows Tablet
                        SetAdsId(DesktopAdsId);
                        SetAdsSize(728, 90);
                    }
                    break;
            }
        }

        private void SetAdsSize(double width, double height)
        {
            MyAds.Width = width;
            AdsRow.Height = new GridLength(height);
            MyAds.Height = height;
        }
        
        private void SetAdsId(string id)
        {
            if (MyAds.Id != id)
            {
                MyAds.Id = id;
            }
        }
    }
}
