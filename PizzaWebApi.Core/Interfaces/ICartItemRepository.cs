using PizzaWebApi.Core.Models;
using PizzaWebApi.SharedKernel.Interfaces;

namespace PizzaWebApi.Core.Interfaces
{
    public interface ICartItemRepository : IRepositoryBase<CartItem>
    {
        CartItem? GetCartItem(int cartId, int productId);
        Task<bool> DeleteAsync(int cartId, int productId);
        Task<decimal> GetSum(int cartId);
        Task<IList<CartItem>> GetCartItemsAsync(int cartId);
    }
}
