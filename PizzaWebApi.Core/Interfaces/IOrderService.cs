using PizzaWebApi.Core.ApiModels;
using PizzaWebApi.Core.Requests;
using PizzaWebApi.Core.Response;

namespace PizzaWebApi.Core.Interfaces
{
    public interface IOrderService
    {
        Task<SearchResult<OrderDTO>> FindAllAsync(PageCriteriaRequest pageCriteriaRequest);

        Task<OrderDTO> GetByIdAsync(int id);

        Task<int> CheckoutAsync(int cartId, OrderDetailsDTO orderDetails);

        Task<bool> SetOrderCompletedAsync(int orderId);
    }
}
