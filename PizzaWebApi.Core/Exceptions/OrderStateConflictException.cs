namespace PizzaWebApi.Core.Exceptions
{
    /// <summary>
    /// Order State conflict situation
    /// </summary>
    public class OrderStateConflictException : Exception
    {
        public OrderStateConflictException()
        {
        }

        public OrderStateConflictException(string message)
            : base(message)
        {
        }

        public OrderStateConflictException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
