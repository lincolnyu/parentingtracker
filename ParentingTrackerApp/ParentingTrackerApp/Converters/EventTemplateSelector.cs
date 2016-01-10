using ParentingTrackerApp.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ParentingTrackerApp.Converters
{
    public class EventTemplateSelector : DataTemplateSelector
    {
        public DataTemplate RunningEventDataTemplate { get; set; }

        public DataTemplate LoggedEventDataTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var element = container as FrameworkElement;

            var e = item as EventViewModel;
            if (element != null && e != null)
            {
                return (e.Status == EventViewModel.Statuses.Running) ?
                    RunningEventDataTemplate : LoggedEventDataTemplate;
            }

            return null;

        }
    }
}
