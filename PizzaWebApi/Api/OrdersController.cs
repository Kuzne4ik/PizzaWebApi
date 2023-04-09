#nullable disable
using Microsoft.AspNetCore.Mvc;
using PizzaWebApi.Core.Interfaces;
using PizzaWebApi.Core.ApiModels;
using PizzaWebApi.Core.Requests;
using PizzaWebApi.Core.Response;

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
        /// <param name="id">Product ID</param>
        [HttpGet("{id}", Name = "GetOrders")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<OrderDTO> GetAsync(int id)
        {
            return _orderService.GetByIdAsync(id);
        }

        /// <summary>
        /// Creates a Order
        /// </summary>
        /// <returns>A newly created Order ID</returns>
        /// <response code="201">Returns the newly created item ID</response>
        [HttpPost("cart-id={cartId}", Name = "Checkout")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task<int> Checkout(int cartId, [FromBody] OrderDetailsDTO orderDetailsDTO)
        {
            return _orderService.CheckoutAsync(cartId, orderDetailsDTO);
        }

        /// <summary>
        /// Set Order State to Complete
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <returns></returns>
        [HttpPut("{id}/complete", Name = "SetOrderToComplete")]
        public Task<bool> SetOrderComplete(int id)
        {
            return _orderService.SetOrderCompletedAsync(id);
        }
    }
}
