using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PizzaWebApi.Web.Filters.ExceptionFilters
{
    /// <summary>
    /// Catch KeyNotFoundException and create HTTP 400 Bad Request response with error message
    /// </summary>
    public class KeyNotFoundExceptionFilter : IExceptionFilter
    {
        /// <summary>
        /// Ловить исключения типа KeyNotFoundException 
        /// </summary>
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is KeyNotFoundException ex)
            {
                var error = new ProblemDetails
                {
                    Title = "An error occurred",
                    Detail = ex.Message,
                    Status = 400,
                    Type = "https://httpstatuses.com/400"
                };
                context.Result = new ObjectResult(error)
                {
                    StatusCode = 400
                };
                context.ExceptionHandled = true;
            }
        }
    }
}
