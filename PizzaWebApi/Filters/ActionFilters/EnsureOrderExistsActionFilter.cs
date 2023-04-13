using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using PizzaWebApi.Core.Interfaces;
using PizzaWebApi.Core.Models;

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

        public EnsureOrderExistsActionFilter(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Call before Action execute
        /// </summary>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ActionArguments.ContainsKey("orderId"))
                throw new ArgumentException("Query param orderId is not exists");
            var orderId = (int)context.ActionArguments["orderId"];
            if (orderId < 0 || !await _orderService.OrderIsExistById(orderId))
            {
                var error = new ProblemDetails
                {
                    Title = "An error occurred",
                    Detail = $"Could not find a Order with ID: {orderId}",
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
