namespace PizzaWebApi.Core.Interfaces
{
    /// <summary>
    /// Publishes events to listeners.
    /// </summary>
    public interface IEventPublisher
    {
        /// <summary>
        /// Publish an event
        /// </summary>
        /// <typeparam name="T">The type of the event data</typeparam>
        /// <param name="act">The event data</param>
        void Publish<T>(T act) where T : class;
    }
}
