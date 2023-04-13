#nullable disable
using Microsoft.AspNetCore.Mvc;
using PizzaWebApi.Core.Interfaces;
using PizzaWebApi.Core.ApiModels;
using PizzaWebApi.Core.Requests;
using PizzaWebApi.Core.Response;
using PizzaWebApi.Web.Attributes;

namespace PizzaWebApi.Web.Api
{
    public class OrdersController : BaseApiController
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService categoriesService)
        {
            _orderService = categoriesService;
        }

        /// <summary>
        /// Get All Orders
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SearchResult<OrderDTO>))]
        [HttpPost("search", Name = "SearchOrders")]
        public Task<SearchResult<OrderDTO>> GetAllAsync([FromBody] PageCriteriaRequest pageCriteriaRequest)
        {
            return _orderService.FindAllAsync(pageCriteriaRequest);
        }

        /// <summary>
        /// Get Order by Id
        /// </summary>
        /// <param name="orderId">Product ID</param>
        [HttpGet("{orderId}", Name = "GetOrder"), EnsureOrderExists]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<OrderDTO> GetAsync(int orderId)
        {
            return _orderService.GetByIdAsync(orderId);
        }

        /// <summary>
        /// Creates an Order
        /// </summary>
        /// <returns>A newly created Order ID</returns>
        /// <response code="200">Returns the newly created Order ID</response>
        [HttpPost("cart-id={cartId}", Name = "Checkout"), EnsureCartExists]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<int> Checkout(int cartId, [FromBody] OrderDetailsDTO orderDetailsDTO)
        {
            return _orderService.CheckoutAsync(cartId, orderDetailsDTO);
        }

        /// <summary>
        /// Set Order State to Complete
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <returns></returns>
        [HttpPut("{orderId}/complete", Name = "SetOrderToComplete"), EnsureOrderExists]
        public Task<bool> SetOrderComplete(int orderId)
        {
            return _orderService.SetOrderCompletedAsync(orderId);
        }
    }
}
