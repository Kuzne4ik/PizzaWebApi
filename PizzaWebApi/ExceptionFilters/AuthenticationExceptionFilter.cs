using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Authentication;

namespace PizzaWebApi.Web.ExceptionFilters
{
    /// <summary>
    /// Catch AuthenticationException and create HTTP 403 Forbidden response with error message
    /// </summary>
    public class AuthenticationExceptionFilter : IActionFilter, IOrderedFilter
    {
        /// <summary>
        /// Переопределение порядка исполнения
        /// -1 исполняется первым, далее по нарастающей
        /// </summary>
        public int Order => int.MaxValue;

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is AuthenticationException ex)
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
