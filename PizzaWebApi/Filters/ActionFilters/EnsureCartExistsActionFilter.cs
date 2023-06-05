using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using PizzaWebApi.Core.Interfaces;

namespace PizzaWebApi.Web.Filters.ActionFilters
{
    /// <summary>
    /// Check is existst Cart action filter
    /// Use with EnsureCartExistsAttribute only! Do not use globaly!
    /// </summary>
    /// <remarks>
    /// Внимание! Нельзя объявлять глобально, так как требуется точечное использование в определенных
    /// методах Action с помощью аттрибута
    /// Работает в связке с EnsureCartExistsAttribute
    /// </remarks>
    public class EnsureCartExistsActionFilter : IAsyncActionFilter
    {
        private readonly ICartService _cartService;
        private readonly ILogger<EnsureCartExistsActionFilter> _logger;

        public EnsureCartExistsActionFilter(ICartService cartService, ILogger<EnsureCartExistsActionFilter> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        /// <summary>
        /// Call before Action execute
        /// </summary>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ActionArguments.ContainsKey("cartId") || context.ActionArguments["cartId"] == null)
                throw new ArgumentException("Query param cartId is not exists");
            var cartId = (int)context.ActionArguments["cartId"]!;
            if (cartId < 0 || !await _cartService.CartIsExistById(cartId))
            {
                _logger.LogWarning("Cart with ID = {0} is not exists", cartId);
                var error = new ProblemDetails
                {
                    Title = "An error occurred",
                    Detail = $"Could not find a cart with ID: {cartId}",
                    Status = 404,
                    Type = "https://httpstatuses.com/404"
                };
                context.Result = new ObjectResult(error)
                {
                    StatusCode = 404
                };
                return;
            }
            await next(); //need to pass the execution to next
        }
    }
}
