using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using PizzaWebApi.Core.Interfaces;

namespace PizzaWebApi.Web.Filters.ActionFilters
{
    /// <summary>
    /// Check is existst Order action filter
    /// Use with EnsureOrderExistsAttribute only! Do not use globaly!
    /// </summary>
    /// <remarks>
    /// Внимание! Нельзя объявлять глобально, так как требуется точечное использование в определенных
    /// методах Action с помощью аттрибута
    /// Работает в связке с EnsureOrderExistsAttribute
    /// </remarks>
    public class EnsureOrderExistsActionFilter : IAsyncActionFilter
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<EnsureOrderExistsActionFilter> _logger;

        /// <summary>
        /// Фильтр для аттрибута Action проверка существования Order по ID
        /// </summary>
        /// <param name="orderService"></param>
        /// <param name="logger"></param>
        public EnsureOrderExistsActionFilter(IOrderService orderService, ILogger<EnsureOrderExistsActionFilter> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        /// <summary>
        /// Call before Action execute
        /// </summary>
        public async Task OnActionExecutionAsync(ActionExecutingContext cxt, ActionExecutionDelegate next)
        {
            if (!cxt.ActionArguments.ContainsKey("orderId") || cxt.ActionArguments["orderId"] == null)
                throw new ArgumentException("Query param orderId is not exists");
            var orderId = (int)cxt.ActionArguments["orderId"]!;
            if (orderId < 0 || !await _orderService.OrderIsExistById(orderId))
            {
                _logger.LogWarning("Order with ID = {0} is not exists", orderId);

                var error = new ProblemDetails
                {
                    Title = "An error occurred",
                    Detail = $"Could not find a Order with ID: {orderId}",
                    Status = 404,
                    Type = "https://httpstatuses.com/404"
                };
                cxt.Result = new ObjectResult(error)
                {
                    StatusCode = 404
                };
                return;
            }
            await next(); //need to pass the execution to next
        }
    }
}
