using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PizzaWebApi.Web.ExceptionFilters
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
                context.Result = new ObjectResult(ex.Message)
                {
                    StatusCode = 400
                };
                context.ExceptionHandled = true;
            }
        }
    }
}
