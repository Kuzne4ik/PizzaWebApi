using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PizzaWebApi.Web.ExceptionFilters
{
    /// <summary>
    /// Catch ArgumentException and create HTTP 400 Bad Request response with error message
    /// </summary>
    public class ArgumentExceptionFilter : IExceptionFilter
    {
        /// <summary>
        /// Ловить исключения типа ArgumentException 
        /// </summary>
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is ArgumentException ex)
            {
                var error = new ProblemDetails
                {
                    Title = "An error occurred",
                    Detail = ex.Message,
                    Status = 400,
                    Type = "https://httpstatuses.com/400"
                };
                // Так тоже можно BadRequestObjectResult(ex.Message) 
                context.Result = new ObjectResult(error)
                {
                    StatusCode = 400
                };
                context.ExceptionHandled = true;
            }
        }
    }
}
