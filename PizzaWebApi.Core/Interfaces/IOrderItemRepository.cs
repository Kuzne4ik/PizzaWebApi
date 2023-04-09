using PizzaWebApi.Core.Models;
using PizzaWebApi.SharedKernel.Interfaces;

namespace PizzaWebApi.Infrastructure.Services
{
    public interface IOrderItemRepository : IRepositoryBase<OrderItem>
    {
        Task<decimal> GetSum(int orderId);
        Task<IEnumerable<OrderItem>> GetOrderItemsAsync(int orderId);
    }
}