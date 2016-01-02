// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

using ParentingTrackerApp.ViewModels;
using System;
using System.ComponentModel;
using System.Threading;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;
using Windows.System.Profile;

namespace ParentingTrackerApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : INotifyPropertyChanged
    {
        private const string DesktopAdsId = "6280a407-b64e-431b-b032-c97dac77cae7";
        private const string MobileAdsId = "535a18c3-9e0d-4ba4-a359-f1928cf778a0";

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
                () => { Time = DateTime.Now; });
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
            var df = AnalyticsInfo.VersionInfo.DeviceFamily;
            switch (df)
            {
                case "Desktop":
                    SetAdsId(DesktopAdsId);
                    MyAds.Width = 728;
                    MyAds.Height = 90;
                    break;
                case "Mobile":
                    if (args.NewSize.Width < 320 && args.NewSize.Width >= 300)
                    {
                        SetAdsId(MobileAdsId);
                        MyAds.Width = 300;
                        MyAds.Height = 50;
                    }
                    else if (args.NewSize.Width < 480)
                    {
                        SetAdsId(MobileAdsId);
                        MyAds.Width = 320;
                        MyAds.Height = 50;
                    }
                    else if (args.NewSize.Width < 640)
                    {
                        SetAdsId(MobileAdsId);
                        MyAds.Width = 480;
                        MyAds.Height = 80;
                    }
                    else if (args.NewSize.Width < 728)
                    {
                        SetAdsId(MobileAdsId);
                        MyAds.Width = 640;
                        MyAds.Height = 100;
                    }
                    else
                    {
                        // could be Windows Tablet
                        SetAdsId(DesktopAdsId);
                        MyAds.Width = 728;
                        MyAds.Height = 90;
                    }
                    break;
            }
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
