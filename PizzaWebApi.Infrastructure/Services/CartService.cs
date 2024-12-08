using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PizzaWebApi.Core.Interfaces;
using PizzaWebApi.Core.Models;
using PizzaWebApi.Core.ApiModels;
using PizzaWebApi.Core.Requests;
using PizzaWebApi.Core.Response;

namespace PizzaWebApi.Infrastructure.Services
{
    public class CartService : ICartService
    {
        private ICartRepository _cartRepository;
        ICartItemRepository _cartItemRepository;
        IPromoCodeService _promoCodeService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CartService(ICartRepository cartRepository, 
                            ICartItemRepository cartItemRepository, 
                            IPromoCodeService promoCodeService,
                            IMapper mapper, 
                            ILogger<CartService> logger)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _promoCodeService = promoCodeService;

            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Получить все Cart по критерию поиска
        /// </summary>
        /// <param name="pageCriteriaRequest"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        /// /// ToDo: Этот метод для адммна
        public async Task<SearchResult<CartDTO>> FindAllAsync(PageCriteriaRequest pageCriteriaRequest)
        {
            _logger.LogInformation($"{nameof(FindAllAsync)} run");

            try
            {
                var allQuery = _cartRepository.ListQuery();
                var query = allQuery.Skip(pageCriteriaRequest.Skip).Take(pageCriteriaRequest.PageSize);

                var count = await allQuery.CountAsync();
                var items = await query.ProjectToType<CartDTO>().ToListAsync();

                return new SearchResult<CartDTO>(items, count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(FindAllAsync)} exception");
                throw new ApplicationException("Get Carts failed");
            }
        }

        /// <summary>
        /// Get Cart by Id
        /// </summary>
        /// <param name="id">Cart ID</param>
        /// ToDo: Этот метод для адммна
        public async Task<CartDTO> GetByIdAsync(int id)
        {
            _logger.LogInformation($"{nameof(GetByIdAsync)} run");
            try
            {
                return await _cartRepository.FindByConditionQuery(t => t.Id == id).ProjectToType<CartDTO>().SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(GetByIdAsync)} exception");
                throw new ApplicationException("Get Cart failed");
            }
        }

        public async Task<CartDTO> GetCartByUserId(int userId)
        {
            _logger.LogInformation($"{nameof(GetCartByUserId)} run");

            if (!await UserIsExistById(userId))
            {
                throw new KeyNotFoundException($"The User {userId} not found");
            }

            if (!await CartIsExistByUserId(userId))
            {
                var cart = new Cart()
                {
                    UserId = userId,
                    CreatedBy = userId
                };
                await _cartRepository.AddAsync(cart);
            }
            int? cartId = -1;
            try 
            {
                cartId = _cartRepository.GetCartIdByUserId(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(GetCartByUserId)} exception");
                throw new ApplicationException("Get Cart failed");
            }
            if (cartId == null)
                throw new KeyNotFoundException($"The Cart by User {userId} not found");

            try
            {
                return await _cartRepository.FindByConditionQuery(t => t.Id == cartId.Value).ProjectToType<CartDTO>().SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(GetCartByUserId)} exception");
                throw new ApplicationException("Get Cart failed");
            }
        }

        /// <summary>
        /// Add Item to Shoping Cart
        /// </summary>
        /// <param name="cartId">Cart ID</param>
        /// <param name="productId">Product ID</param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="ApplicationException"></exception>
        public async Task<CartItemDTO> AddItemToCartAsync(int cartId, int productId)
        {
            _logger.LogInformation($"{nameof(AddItemToCartAsync)} run");
            if (cartId < 1)
            {
                throw new ArgumentException($"Argument Cart ID {productId} is wrong");
            }
            if (productId < 1)
            {
                throw new ArgumentException($"Argument Product ID {productId} is wrong");
            }
            var cartItemIsExists = await CartItemIsExists(cartId, productId);

            try
            {
                CartItem cartItem;

                if (cartItemIsExists)
                {
                    _logger.LogDebug($"CartItem Exist add one");

                    cartItem = _cartItemRepository.GetCartItem(cartId, productId);
                    // item does exist in the cart, then add one
                    cartItem.Quantity++;

                    _ = await _cartItemRepository.UpdateAsync(cartItem);
                }
                else
                {
                    _logger.LogDebug($"Create new CartItem Product {productId} for Cart {cartId}");

                    // Create a new cart item if no cart item exists.                 
                    cartItem = new CartItem
                    {
                        ProductId = productId,
                        CartId = cartId,
                        Quantity = 1
                    };

                    _ = await _cartItemRepository.AddAsync(cartItem);
                }

                await SetUpdateInfoForCart(cartId);

                return await _cartItemRepository.FindByConditionQuery(t => t.Id == cartItem.Id).ProjectToType<CartItemDTO>().SingleAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(AddItemToCartAsync)} exception");
                throw new ApplicationException("Add Item to Cart failed");
            }
        }

        /// <summary>
        /// Remove Cart Item from Shoping Cart 
        /// </summary>
        /// <param name="cartId">Shoping Cart ID</param>
        /// <param name="productId">Product ID</param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task<bool> RemoveItemFromCartAsync(int cartId, int productId)
        {
            _logger.LogInformation($"{nameof(RemoveItemFromCartAsync)} run");
            if (!await CartItemIsExists(cartId, productId))
            {
                throw new KeyNotFoundException($"The CartItem from Cart {cartId} and Product {productId} not found");
            }

            try
            {
                await SetUpdateInfoForCart(cartId);

                return await _cartItemRepository.DeleteAsync(cartId, productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(RemoveItemFromCartAsync)} exception");
                throw new ApplicationException("Update Cart failed");
            }
        }

        /// <summary>
        /// Update CartItem Quantity
        /// </summary>
        /// <param name="cartId"></param>
        /// <param name="productId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="ApplicationException"></exception>
        public async Task<bool> UpdateItemAsync(int cartId, int productId, int quantity)
        {
            _logger.LogInformation($"{nameof(UpdateItemAsync)} run");
            if (!await CartItemIsExists(cartId, productId))
            {
                throw new KeyNotFoundException($"The CartItem from Cart {cartId} and Product {productId} not found");
            }
            if(quantity < 1)
            {
                throw new ArgumentException($"Argument {nameof(quantity)} must be greater then 0");
            }

            try
            {
                await SetUpdateInfoForCart(cartId);

                var cartItem = _cartItemRepository.GetCartItem(cartId, productId);
                cartItem.Quantity = quantity;

                return await _cartItemRepository.UpdateAsync(cartItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(UpdateItemAsync)} exception");
                throw new ApplicationException("Update Cart failed");
            }
        }

        /// <summary>
        /// Clear Cart
        /// </summary>
        /// <param name="cartId">Cart ID</param>
        /// <returns>Sucess</returns>
        public async Task<bool> ClearCart(int cartId)
        {
            _logger.LogInformation($"{nameof(ClearCart)} run");
            try
            {
                await SetUpdateInfoForCart(cartId);

                return await _cartRepository.ClearCart(cartId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ClearCart)} exception");
                throw new ApplicationException("Delete Cart failed");
            }
        }

        /// <summary>
        /// Set Promocode for Cart
        /// </summary>
        /// <param name="cartId"></param>
        /// <param name="promoCode"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task<bool> SetPromocode(int cartId, string promoCode)
        {
            _logger.LogInformation($"{nameof(GetTotal)} run");
            try
            {
                var cart = await _cartRepository.FindByIdAsync(cartId);
                cart.PromoCode = promoCode;
                cart.UpdatedBy = cart.UserId;
                cart.Updated = DateTime.UtcNow;

                _ = await _cartRepository.UpdateAsync(cart);
                return await _cartRepository.UpdateAsync(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(SetPromocode)} exception");
                throw new ApplicationException("Update Cart failed");
            }
        }

        /// <summary>
        /// Get Total
        /// </summary>
        /// <param name="cartId">Cart ID</param>
        /// <returns></returns>
        public async Task<decimal> GetTotal(int cartId)
        {
            _logger.LogInformation($"{nameof(GetTotal)} run");
            try
            {
                var promocode = await _cartRepository.FindByConditionQuery(t => t.Id == cartId).Select(t => t.PromoCode).SingleOrDefaultAsync();
                var sum = await _cartItemRepository.GetSum(cartId);
                
                if (!string.IsNullOrEmpty(promocode))
                {
                    var discount = _promoCodeService.CarculateDiscount(promocode, sum);
                    return sum - discount;
                }
                return sum;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ClearCart)} exception");
                throw new ApplicationException("Grt Total failed");
            }
        }

        public async Task<bool> CartIsExistById(int id)
        {
            _logger.LogInformation($"{nameof(CartIsExistById)} run");
            try
            {
                return await _cartRepository.AnyAsync(t => t.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(CartIsExistById)} exception");
                throw new ApplicationException("Any failed");
            }
        }

        private Task<bool> UserIsExistById(int userId)
        {
            _logger.LogInformation($"{nameof(UserIsExistById)} run");
            try
            {
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(UserIsExistById)} exception");
                throw new ApplicationException("Any failed");
            }
        }

        
        private async Task<bool> CartIsExistByUserId(int userId)
        {
            _logger.LogInformation($"{nameof(CartIsExistByUserId)} run");
            try
            {
                return await _cartRepository.AnyAsync(t => t.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(CartIsExistByUserId)} exception");
                throw new ApplicationException("Any failed");
            }
        }

        private async Task<bool> CartItemIsExists(int cartId, int productId)
        {
            _logger.LogInformation($"{nameof(CartItemIsExists)} run");
            try
            {
                return await _cartItemRepository.AnyAsync(x => x.CartId == cartId && x.ProductId == productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(CartItemIsExists)} exception");
                throw new ApplicationException("Any failed");
            }
        }

        private async Task SetUpdateInfoForCart(int cartId, int userId = 1)
        {
            var cart = await _cartRepository.FindByIdAsync(cartId);

            cart.UpdatedBy = userId;
            cart.Updated = DateTime.UtcNow;
            _ = await _cartRepository.UpdateAsync(cart);
        }
    }
}
