// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

using ParentingTrackerApp.ViewModels;
using System;
using System.ComponentModel;
using System.Threading;

namespace ParentingTrackerApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : INotifyPropertyChanged
    {
        private Timer _timer;

        private DateTime _time;
        
        public MainPage()
        {
            InitializeComponent();

            DataContext = CentralViewModel;

            _timer = new Timer(TimerOnTick, null, 0, 1000);
        }

        public DateTime Time
        {
            get { return _time; } set
            {
                if (_time != value)
                {
                    _time = value;
                    RaisePropertyChanged("Time");
                }
            }
        }

        public CentralViewModel CentralViewModel { get; } = new CentralViewModel();

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
    }
}
