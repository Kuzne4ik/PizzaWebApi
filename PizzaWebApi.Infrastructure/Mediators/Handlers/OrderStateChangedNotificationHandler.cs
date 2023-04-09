using MediatR;
using Microsoft.Extensions.Logging;
using PizzaWebApi.Infrastructure.Mediators.Notifications;
using PizzaWebApi.Core.Interfaces;

namespace PizzaWebApi.Infrastructure.Mediators.Handlers
{
    public class OrderStateChangedNotificationHandler : INotificationHandler<OrderStateChangedNotification>
    {
        private IProductRepository _productRepository;
        private ILogger<GetAllProductsHandler> _logger;

        public OrderStateChangedNotificationHandler(
            IProductRepository productRepository,
            ILogger<GetAllProductsHandler> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public Task Handle(OrderStateChangedNotification notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(OrderStateChangedNotificationHandler)} handle {nameof(GetAllProductsHandler)} Order {notification.OrderId} new State {notification.NewOrderState}");

            Console.WriteLine($"{nameof(OrderStateChangedNotificationHandler)} Order {notification.OrderId} new State {notification.NewOrderState}");

            return Task.CompletedTask;
        }
    }
}
