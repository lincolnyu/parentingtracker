// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

using ParentingTrackerApp.ViewModels;
using System;
using System.ComponentModel;
using System.Threading;
using Windows.UI.Xaml.Navigation;

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
    }
}
