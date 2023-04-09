using MediatR;
using PizzaWebApi.Core.Models;

namespace PizzaWebApi.Infrastructure.Mediators.Notifications
{
    public class OrderStateChangedNotification : INotification
    {
        public OrderStateChangedNotification(int orderId, OrderState newOrderState)
        {
            OrderId = orderId;
            NewOrderState = newOrderState;
        }

        public int OrderId { get; }
        public OrderState NewOrderState { get; }
    }
}
