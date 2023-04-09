using PizzaWebApi.Core.Models;
using PizzaWebApi.Core.Interfaces;

namespace PizzaWebApi.Infrastructure.Data
{
    public class CartRepository : RepositoryBase<Cart>, ICartRepository
    {
        public CartRepository(AppDbContext repositoryContext) : base(repositoryContext)
        {
            RepositoryContext = repositoryContext;
        }

        public async Task<bool> ClearCart(int cartId)
        {
            var cartItems = RepositoryContext.CartItems.Where(t => t.CartId == cartId);
            foreach (var cartItem in cartItems)
            {
                RepositoryContext.CartItems.Remove(cartItem);
            }
            var res = await RepositoryContext.SaveChangesAsync();
            if(res > 0)
                return true;
            return false;
        }

        public int? GetCartIdByUserId(int userId)
        {
            return RepositoryContext.Carts.Where(t => t.UserId == userId).Select(t => t.Id).SingleOrDefault();
        }
    }
}
