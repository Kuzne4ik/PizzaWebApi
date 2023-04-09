using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PizzaWebApi.Web.ExceptionFilters
{
    /// <summary>
    /// Catch KeyNotFoundException and create HTTP 400 Bad Request response with error message
    /// </summary>
    public class KeyNotFoundExceptionFilter : IActionFilter, IOrderedFilter
    {
        /// <summary>
        /// Переопределение порядка исполнения
        /// -1 исполняется первым, далее по нарастающей
        /// </summary>
        public int Order => int.MaxValue;

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
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
