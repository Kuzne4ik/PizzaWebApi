#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaWebApi.Core.Interfaces;
using PizzaWebApi.Core.ApiModels;
using PizzaWebApi.Core.Requests;
using PizzaWebApi.Core.Response;
using Microsoft.AspNetCore.Authorization;
using PizzaWebApi.Core.Models;
using PizzaWebApi.Web.Attributes;

namespace PizzaWebApi.Web.Api
{
    public class CartsController : BaseApiController
    {
        private readonly ICartService _cartService;

        public CartsController(ICartService CartsService)
        {
            _cartService = CartsService;
        }

        /// <summary>
        /// Get Carts per page
        /// </summary>
        [Authorize(Roles = UserRoles.Admin)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SearchResult<CartDTO>))]
        [HttpPost("search", Name = "SearchCarts")]
        public Task<SearchResult<CartDTO>> GetAllAsync([FromBody] PageCriteriaRequest pageCriteriaRequest)
        {
            return _cartService.FindAllAsync(pageCriteriaRequest);
        }

        /// <summary>
        /// Get Cart by ID
        /// </summary>
        /// <param name="id">Cart ID</param>
        //[Authorize]
        [HttpGet("{cartId}", Name = "GetCartById"), EnsureCartExists]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CartDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<CartDTO> GetAsync(int cartId)
        {
            return _cartService.GetByIdAsync(cartId);
        }

        /// <summary>
        /// Get Cart by UserId
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>A newly created Cart</returns>
        /// <response code="201">Returns the newly created item</response>
        /// <exception cref="DbUpdateException"></exception>
        //[Authorize]
        [HttpGet("user-id={userId}", Name = "GetCartByUserId")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CartDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task<CartDTO> GetCartByUserId(int userId)
        {
            return _cartService.GetCartByUserId(userId);
        }


        /// <summary>
        /// Add CartItem to Cart
        /// </summary>
        /// <param name="cartId">Cart ID</param>
        /// <param name="productId">Product ID</param>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="DbUpdateException"></exception>
        //[Authorize]
        [HttpPost("{cartId}/items", Name = "AddCart"), EnsureCartExists]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CartItemDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<CartItemDTO> AddItemToCart(int cartId, int productId)
        {
            return _cartService.AddItemToCartAsync(cartId, productId);
        }

        /// <summary>
        /// Update CartItem
        /// </summary>
        /// <param name="cartId">Cart ID</param>
        /// <param name="productId">Product ID</param>
        /// <param name="qty">CartItem quantity</param>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="DbUpdateException"></exception>
        //[Authorize]
        [HttpPut("{cartId}/items", Name = "UpdateCart"), EnsureCartExists]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<bool> UpdateItem(int cartId, int productId, int qty)
        {
            return _cartService.UpdateItemAsync(cartId, productId, qty);
        }

        /// <summary>
        /// Clear the Cart
        /// </summary>
        /// <param name="cartId">Cart ID</param>
        /// <returns>Is success</returns>
        //[Authorize]
        [HttpDelete("{cartId}/items", Name = "ClearCart"), EnsureCartExists]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<bool> ClearCart(int cartId)
        {
            return _cartService.ClearCart(cartId);
        }

        /// <summary>
        /// Delete Cart Item
        /// </summary>
        /// <param name="cartId"></param>
        /// <param name="productId"></param>
        /// <returns>Is success</returns>
        //[Authorize]
        [HttpDelete(Name = "DeleteCartItem"), EnsureCartExists]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<bool> RemoveItemFromCartAsync(int cartId, int productId)
        {
            return _cartService.RemoveItemFromCartAsync(cartId, productId);
        }

        /// <summary>
        /// Set ProomoCode for the Cart
        /// </summary>
        /// <param name="cartId">Cart ID</param>
        /// <param name="promocode">Promocode value</param>
        //[Authorize]
        [HttpPut("{cartId}/promocode/{promocode}", Name = "SetCartPromocode"), EnsureCartExists]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<bool> SetPromoCode(int cartId, string promocode)
        {
            return _cartService.SetPromocode(cartId, promocode);
        }

        /// <summary>
        /// Get the Cart Total
        /// </summary>
        /// <param name="cartId">Cart ID</param>
        //[Authorize]
        [HttpGet("{cartId}/total", Name = "GetCartTotal"), EnsureCartExists]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(decimal))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<decimal> GetTotal(int cartId)
        {
            return _cartService.GetTotal(cartId);
        }
    }
}
