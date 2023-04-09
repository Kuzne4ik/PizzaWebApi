using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PizzaWebApi.Web.ExceptionFilters
{
    /// <summary>
    /// Catch ApplicationException and create HTTP 500 InternalFailure response with error message
    /// </summary>
    public class ApplicationExceptionFilter : IActionFilter, IOrderedFilter
    {
        /// <summary>
        /// Переопределение порядка исполнения
        /// -1 исполняется первым, далее по нарастающей
        /// </summary>
        public int Order => int.MaxValue;

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
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
