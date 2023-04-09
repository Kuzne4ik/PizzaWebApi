using Microsoft.EntityFrameworkCore;
using PizzaWebApi.Core.Models;
using PizzaWebApi.Infrastructure.Services;

namespace PizzaWebApi.Infrastructure.Data
{
    public class OrderItemRepository : RepositoryBase<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(AppDbContext repositoryContext) : base(repositoryContext)
        {
            RepositoryContext = repositoryContext;
        }

        public async Task<decimal> GetSum(int orderId)
        {
            return await RepositoryContext.OrderItems.Where(t => t.OrderId == orderId).SumAsync(t => t.Quantity * t.Product.Price);
        }

        public async Task<IEnumerable<OrderItem>> GetOrderItemsAsync(int orderId) => await RepositoryContext.OrderItems.Where(t => t.OrderId == orderId).ToListAsync();
    }
}
