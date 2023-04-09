using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PizzaWebApi.Web.ExceptionFilters
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
                context.Result = new ObjectResult(ex.Message)
                {
                    StatusCode = 500
                };
                context.ExceptionHandled = true;
            }
        }
    }
}
