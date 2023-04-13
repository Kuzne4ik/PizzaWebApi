using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PizzaWebApi.Core.Exceptions;

namespace PizzaWebApi.Web.Filters.ExceptionFilters
{
    /// <summary>
    /// Catch OrderStateConflictException and create HTTP 409 InternalFailure response with error message
    /// </summary>
    public class OrderStateConflictExceptionFilter : IExceptionFilter
    {
        /// <summary>
        /// Ловить исключения типа OrderStateConflictException 
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is OrderStateConflictException ex)
            {
                var error = new ProblemDetails
                {
                    Title = "An error occurred",
                    Detail = ex.Message,
                    Status = 409,
                    Type = "https://httpstatuses.com/409"
                };
                context.Result = new ObjectResult(error)
                {
                    StatusCode = 409
                };
                context.ExceptionHandled = true;
            }
        }
    }
}
