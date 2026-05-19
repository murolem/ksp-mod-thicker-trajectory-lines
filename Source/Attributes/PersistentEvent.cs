using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ThickerTrajectoryLines.Attributes
{
    /// <summary>
    /// Made for marking event counterparts of [Persistent] properties.
    ///
    /// Used to then track those bitches down for hot reloading purposes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Event)]
    public class PersistentEvent : Attribute
    {
        public static EventInfo[] GetEventsFromClass(Type type, BindingFlags bindingFlags)
        {
            return type.GetEvents(bindingFlags)
                .Where(eventInfo => eventInfo.GetCustomAttribute<PersistentEvent>() != null)
                .ToArray();
        }
    }
}