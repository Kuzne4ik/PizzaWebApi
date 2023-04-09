using PizzaWebApi.Core.Models;

namespace PizzaWebApi.Core.Events
{
    public class OrderStateChagedEvent
    {
        public OrderStateChagedEvent(int orderId, OrderState newOrderState)
        {
            OrderId = orderId;
            NewOrderState = newOrderState;
        }

        public int OrderId { get; set; }
        public OrderState NewOrderState { get; set; }
    }
}
