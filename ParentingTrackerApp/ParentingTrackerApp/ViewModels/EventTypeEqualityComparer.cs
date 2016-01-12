using System;
using System.Collections.Generic;

namespace ParentingTrackerApp.ViewModels
{
    class EventTypeEqualityComparer : IEqualityComparer<EventTypeViewModel>
    {
        public static EventTypeEqualityComparer Instance { get; } = new EventTypeEqualityComparer();

        public bool Equals(EventTypeViewModel x, EventTypeViewModel y)
        {
            return x.Name == y.Name;
        }

        public int GetHashCode(EventTypeViewModel obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}
