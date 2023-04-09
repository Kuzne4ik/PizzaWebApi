using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PizzaWebApi.Core.Exceptions;

namespace PizzaWebApi.Web.ExceptionFilters
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
                context.Result = new ObjectResult(ex.Message)
                {
                    StatusCode = 409
                };
                context.ExceptionHandled = true;
            }
        }
    }
}
