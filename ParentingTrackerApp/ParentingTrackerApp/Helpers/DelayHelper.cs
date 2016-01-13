using System;
using System.Threading;
using Windows.UI.Core;

namespace ParentingTrackerApp.Helpers
{
    public static class DelayHelper
    {
        public delegate void DelayedTaskHandler(object state);

        public static void Delay(object state, DelayedTaskHandler handler, int dueTime)
        {
            Timer timer = null;
            timer = new Timer(x =>
            {
                handler(x);
                timer.Dispose();
            }, state, dueTime, System.Threading.Timeout.Infinite);
        }

        public static void Delay(object state, DelayedTaskHandler handler, int dueTime, 
            CoreDispatcher dispatcher, CoreDispatcherPriority priority = CoreDispatcherPriority.Normal)
        {
            Timer timer = null;
            timer = new Timer(async x=>
             {
                 timer.Dispose();
                 await dispatcher.RunAsync(priority, () => handler(x));
             }, state, dueTime, Timeout.Infinite);
        }
    }
}
