using PizzaWebApi.Core.Interfaces;

namespace PizzaWebApi.Core.Events
{
    /// <summary>
    /// Base class
    /// </summary>
    public class EventPublisher : IEventPublisher
    {
        private readonly IEnumerable<IEventListener> _listeners;

        public EventPublisher(IEnumerable<IEventListener> listeners)
        {
            _listeners = listeners;
        }

        public void Publish<T>(T act) where T : class
        {
            var handlersForPayload = _listeners.OfType<IEventListener<T>>();
            foreach (var handler in handlersForPayload)
            {
                handler.Handle(act);
            }
        }
    }
}
