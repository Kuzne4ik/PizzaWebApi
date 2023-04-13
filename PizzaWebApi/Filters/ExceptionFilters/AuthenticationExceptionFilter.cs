using System.Security.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PizzaWebApi.Web.Filters.ExceptionFilters
{
    /// <summary>
    /// Catch AuthenticationException and create HTTP 403 Forbidden response with error message
    /// </summary>
    public class AuthenticationExceptionFilter : IExceptionFilter
    {
        /// <summary>
        /// Ловить исключения типа AuthenticationException 
        /// </summary>
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is AuthenticationException ex)
            {
                var error = new ProblemDetails
                {
                    Title = "An error occurred",
                    Detail = ex.Message,
                    Status = 403,
                    Type = "https://httpstatuses.com/403"
                };
                context.Result = new ObjectResult(error)
                {
                    StatusCode = 403
                };
                context.ExceptionHandled = true;
            }
        }
    }
}
