using PizzaWebApi.Core.ApiModels;
using PizzaWebApi.Core.Requests;
using PizzaWebApi.Core.Response;

namespace PizzaWebApi.Core.Interfaces
{
    public interface ICartService
    {
        Task<SearchResult<CartDTO>> FindAllAsync(PageCriteriaRequest pageCriteriaRequest);

        Task<CartDTO> GetByIdAsync(int id);

        Task<CartDTO> GetCartByUserId(int userId);

        Task<bool> RemoveItemFromCartAsync(int cartId, int productId);

        Task<bool> UpdateItemAsync(int cartId, int productId, int quantity);

        Task<CartItemDTO> AddItemToCartAsync(int cartId, int productId);


        Task<bool> ClearCart(int id);

        Task<bool> SetPromocode(int cartId, string promoCode);

        Task<decimal> GetTotal(int cartId);


    }
}
