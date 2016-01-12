using System.Threading;

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
    }
}
