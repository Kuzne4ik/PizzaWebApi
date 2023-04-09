namespace PizzaWebApi.Core.Interfaces
{
    /// <summary>
    /// Event listener marker interface (for IoC convenience).
    /// </summary>
    public interface IEventListener
    { }

    /// <summary>
    /// Event listener interface
    /// </summary>
    /// <typeparam name="T">The type of the event data</typeparam>
    public interface IEventListener<in T> : IEventListener where T : class
    {
        /// <summary>
        /// Handle the published event with data.
        /// </summary>
        /// <param name="act">The event data</param>
        void Handle(T act);
    }
}
