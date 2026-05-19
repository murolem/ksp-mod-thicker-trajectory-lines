using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ThickerTrajectoryLines
{
    // Source - https://stackoverflow.com/a/65674963
    // Posted by Chris
    // Retrieved 2026-05-19, License - CC BY-SA 4.0

    /// <summary>
    /// Wrapper around event types that you get via reflection.
    /// Allows to add an event handler to any event.
    /// </summary>
    public class ModelEvent
    {
        private object source;
        private EventInfo sourceEvent;
        private Delegate eventHandler;
        public event EventHandler OnEvent;

        public ModelEvent(object source, EventInfo sourceEvent)
        {
            this.source = source;
            this.sourceEvent = sourceEvent;
            ParameterExpression[] sourceEventHandlerParameters =
                sourceEvent
                    .EventHandlerType
                    .GetMethod("Invoke")
                    .GetParameters()
                    .Select(parameter => Expression.Parameter(parameter.ParameterType))
                    .ToArray();
            MethodCallExpression fireOnEvent = Expression.Call(
                Expression.Constant(new Action(FireOnEvent)),
                "Invoke",
                Type.EmptyTypes);
            this.eventHandler = Expression.Lambda(
                    sourceEvent.EventHandlerType,
                    fireOnEvent,
                    sourceEventHandlerParameters)
                .Compile();
            this.sourceEvent.AddEventHandler(this.source, this.eventHandler);
        }

        private void FireOnEvent()
        {
            this.OnEvent?.Invoke(this, EventArgs.Empty);
        }
    }

}