using PizzaWebApi.Core.Models;
using PizzaWebApi.SharedKernel.Interfaces;

namespace PizzaWebApi.Core.Interfaces
{
    public interface ICartRepository : IRepositoryBase<Cart>
    {
        Task<bool> ClearCart(int cartId);
        int? GetCartIdByUserId(int userId);
    }
}
