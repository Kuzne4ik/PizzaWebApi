#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaWebApi.Core.Interfaces;
using PizzaWebApi.Core.ApiModels;
using PizzaWebApi.Core.Requests;
using PizzaWebApi.Core.Response;
using Microsoft.AspNetCore.Authorization;
using PizzaWebApi.Core.Models;

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
        [HttpGet("{id}", Name = "GetCartById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CartDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<CartDTO> GetAsync(int id)
        {
            return _cartService.GetByIdAsync(id);
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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CartDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task<CartDTO> GetCartByUserId(int userId)
        {
            return _cartService.GetCartByUserId(userId);
        }


        /// <summary>
        /// Add CartItem to Cart
        /// </summary>
        /// <param name="cartid">Cart ID</param>
        /// <param name="productId">Product ID</param>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="DbUpdateException"></exception>
        //[Authorize]
        [HttpPost("{cartid}/items", Name = "AddCart")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CartItemDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task<CartItemDTO> AddItemToCart(int cartid, int productId)
        {
            return _cartService.AddItemToCartAsync(cartid, productId);
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
        [HttpPut("{cartId}/items", Name = "UpdateCart")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task<bool> UpdateItem(int cartId, int productId, int qty)
        {
            return _cartService.UpdateItemAsync(cartId, productId, qty);
        }

        /// <summary>
        /// Clear the Cart
        /// </summary>
        /// <param name="cartId">Cart ID</param>
        /// <returns></returns>
        //[Authorize]
        [HttpDelete("{cartId}/items", Name = "ClearCart")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task<bool> ClearCart(int cartId)
        {
            return _cartService.ClearCart(cartId);
        }

        /// <summary>
        /// Delete Cart Item
        /// </summary>
        /// <param name="cartId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        //[Authorize]
        [HttpDelete(Name = "DeleteCartItem")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Task<bool> RemoveItemFromCartAsync(int cartId, int productId)
        {
            return _cartService.RemoveItemFromCartAsync(cartId, productId);
        }

        /// <summary>
        /// Set ProomoCode for the Cart
        /// </summary>
        /// <param name="cartId">Cart ID</param>
        /// <param name="promocode">Code value</param>
        /// <returns></returns>
        //[Authorize]
        [HttpPut("{cartId}/promocode/{promocode}", Name = "SetCartPromocode")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task<bool> SetPrormoCode(int cartId, string promocode)
        {
            return _cartService.SetPromocode(cartId, promocode);
        }

        /// <summary>
        /// Get the Cart Total
        /// </summary>
        /// <param name="cartId">Cart ID</param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("{cartId}/total", Name = "GetCartTotal")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task<decimal> GetTotal(int cartId)
        {
            return _cartService.GetTotal(cartId);
        }
    }
}
