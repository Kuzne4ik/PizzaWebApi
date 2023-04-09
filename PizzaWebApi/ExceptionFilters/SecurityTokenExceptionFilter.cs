using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

namespace PizzaWebApi.Web.ExceptionFilters
{
    /// <summary>
    /// Catch SecurityTokenException and create HTTP 403 Forbidden Bad Request response with error message
    /// </summary>
    public class SecurityTokenExceptionFilter : IActionFilter, IOrderedFilter
    {
        /// <summary>
        /// Переопределение порядка исполнения
        /// -1 исполняется первым, далее по нарастающей
        /// </summary>
        public int Order => int.MaxValue;

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is SecurityTokenException ex)
            {
                context.Result = new ObjectResult(ex.Message)
                {
                    StatusCode = 403
                };
                context.ExceptionHandled = true;
            }
        }
    }
}
