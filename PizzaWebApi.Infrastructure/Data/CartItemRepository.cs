using Microsoft.EntityFrameworkCore;
using PizzaWebApi.Core.Models;
using PizzaWebApi.Core.Interfaces;

namespace PizzaWebApi.Infrastructure.Data
{
    public class CartItemRepository : RepositoryBase<CartItem>, ICartItemRepository
    {
        public CartItemRepository(AppDbContext repositoryContext) : base(repositoryContext)
        {
            RepositoryContext = repositoryContext;
        }

        public CartItem? GetCartItem(int cartId, int productId) => RepositoryContext.CartItems.SingleOrDefault(t => t.CartId == cartId && t.ProductId == productId);

        public async Task<bool> DeleteAsync(int cartId, int productId)
        {
            var cartItem = RepositoryContext.CartItems.SingleOrDefault(t => t.CartId == cartId && t.ProductId == productId);
            if (cartItem != null)
            {
                // Remove Item.
                RepositoryContext.CartItems.Remove(cartItem);
                var res = await RepositoryContext.SaveChangesAsync();
                return res > 0;
            }
            return await Task.FromResult(false);
        }

        public async Task<decimal> GetSum(int cartId)
        {
            return await RepositoryContext.CartItems.Where(t => t.CartId == cartId).SumAsync(t => t.Quantity * t.Product.Price);
        }

        public async Task<IEnumerable<CartItem>> GetCartItemsAsync(int cartId)
        {
            return await RepositoryContext.CartItems
                .Include(t => t.Product)
                .Where(t => t.CartId == cartId)
                .ToListAsync();
        }
    }
}
