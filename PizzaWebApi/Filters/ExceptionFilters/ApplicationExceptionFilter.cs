using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PizzaWebApi.Web.Filters.ExceptionFilters
{
    /// <summary>
    /// Catch ApplicationException and create HTTP 500 InternalFailure response with error message
    /// </summary>
    public class ApplicationExceptionFilter : IExceptionFilter
    {
        /// <summary>
        /// Ловить исключения типа ApplicationException 
        /// </summary>
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is ApplicationException ex)
            {
                var error = new ProblemDetails
                {
                    Title = "An error occurred",
                    Detail = ex.Message,
                    Status = 500,
                    Type = "https://httpstatuses.com/500"
                };
                context.Result = new ObjectResult(error)
                {
                    StatusCode = 500
                };
                context.ExceptionHandled = true;
            }
        }
    }
}
