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

        public EnsureCartExistsActionFilter(ICartService cartService)
        {
            _cartService = cartService;
        }

        /// <summary>
        /// Call before Action execute
        /// </summary>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ActionArguments.ContainsKey("cartId"))
                throw new ArgumentException("Query param cartId is not exists");
            var cartId = (int)context.ActionArguments["cartId"];
            if (!await _cartService.CartIsExistById(cartId))
            {
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
