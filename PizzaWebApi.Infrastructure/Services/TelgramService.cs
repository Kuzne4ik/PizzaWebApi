using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PizzaWebApi.Core.Interfaces;
using PizzaWebApi.SharedKernel.Interfaces;
using System.Text;

namespace PizzaWebApi.Infrastructure.Services
{
    public class TelegramService : ITelegramService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger _logger;
        
        public TelegramService(IOrderRepository orderRepository, ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        /// <summary>
        /// SendOrders24ReportAsync
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public Task SendOrders24ReportAsync()
        {
            _logger.LogInformation($"{nameof(SendOrders24ReportAsync)} run");

            try
            {
                var ordersQuery = _orderRepository.ListQuery().Where(t => t.Created > DateTime.UtcNow.AddHours(-24))
                    .Include(t => t.OrderItems)
                    .ThenInclude(t => t.Product);

                var sb = new StringBuilder();

                sb.AppendLine("Orders 24 hours report");
                foreach (var order in ordersQuery)
                {
                    sb.AppendLine($"Order # {order.Id}");
                    sb.AppendLine($"Created: {order.Created}");
                    sb.AppendLine($"User ID: {order.UserId} Phone: {order.Phone}");
                    sb.AppendLine($"#\t Name \t\t\t Amnt ");
                    for (int i = 0; i < order.OrderItems.Count; i++)
                    {
                        var item = order.OrderItems[i];
                        sb.AppendLine($"{i + 1}.\t {item.Product.Name} \t\t {item.Quantity}");
                    }
                }

                var msg = sb.ToString();

                Console.WriteLine(msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(SendOrders24ReportAsync)} exception");
            }
            return Task.CompletedTask;
        }

    }
}
