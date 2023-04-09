using Microsoft.Extensions.Logging;
using PizzaWebApi.Core.Events;
using PizzaWebApi.Core.Interfaces;

namespace PizzaWebApi.Core.EventHandler
{
    public class OrderStateChangedHandler : IEventListener<OrderStateChagedEvent>
    {
        private readonly ILogger<OrderStateChangedHandler> logger;

        public OrderStateChangedHandler(ILogger<OrderStateChangedHandler> logger)
        {
            this.logger = logger;
        }

        public void Handle(OrderStateChagedEvent orderStateChangedEvent)
        {
            logger.LogDebug($"OrderStateChangedEvent handled for Order {orderStateChangedEvent.OrderId} new State {orderStateChangedEvent.NewOrderState}");

            Console.WriteLine($"OrderStateChangedEvent handled for Order {orderStateChangedEvent.OrderId} new State {orderStateChangedEvent.NewOrderState}");
        }
    }
}
