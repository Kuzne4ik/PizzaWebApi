using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PizzaWebApi.Core.Exceptions;

namespace PizzaWebApi.Web.ExceptionFilters
{
    /// <summary>
    /// Catch OrderStateConflictException and create HTTP 409 InternalFailure response with error message
    /// </summary>
    public class OrderStateConflictExceptionFilter : IActionFilter, IOrderedFilter
    {
        /// <summary>
        /// Переопределение порядка исполнения
        /// -1 исполняется первым, далее по нарастающей
        /// </summary>
        public int Order => int.MaxValue;

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
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
