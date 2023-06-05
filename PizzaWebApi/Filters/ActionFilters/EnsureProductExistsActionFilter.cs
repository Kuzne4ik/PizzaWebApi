using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using PizzaWebApi.Core.Interfaces;
using PizzaWebApi.Core.Models;

namespace PizzaWebApi.Web.Filters.ActionFilters
{
    /// <summary>
    /// Check is existst Product action filter
    /// Use with EnsureProductExistsAttribute only! Do not use globaly!
    /// </summary>
    /// <remarks>
    /// Внимание! Нельзя объявлять глобально, так как требуется точечное использование в определенных
    /// методах Action с помощью аттрибута
    /// Работает в связке с EnsureProductExistsAttribute
    /// </remarks>
    public class EnsureProductExistsActionFilter : IAsyncActionFilter
    {
        private readonly IProductService _productService;
        private readonly ILogger<EnsureProductExistsActionFilter> _logger;

        public EnsureProductExistsActionFilter(IProductService productService, ILogger<EnsureProductExistsActionFilter> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        /// <summary>
        /// Call before Action execute
        /// </summary>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ActionArguments.ContainsKey("productId") || context.ActionArguments["productId"] == null)
                throw new ArgumentException("Query param productId is not exists");
            var productId = (int)context.ActionArguments["productId"]!;
            if (productId < 0 || !await _productService.ProductIsExistById(productId))
            {
                _logger.LogWarning("Product with ID = {0} is not exists", productId);
                var error = new ProblemDetails
                {
                    Title = "An error occurred",
                    Detail = $"Could not find a Product with ID: {productId}",
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
