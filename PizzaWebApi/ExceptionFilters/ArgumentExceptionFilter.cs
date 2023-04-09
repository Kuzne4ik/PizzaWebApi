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
        /// Переопределение порядка исполнения
        /// -1 исполняется первым, далее по нарастающей
        /// </summary>
        public int Order => int.MaxValue;

        public void OnException(ExceptionContext context)
        {
            if (context.Exception is ArgumentException ex)
            {
                // Так тоже можно BadRequestObjectResult(ex.Message) 
                context.Result = new ObjectResult(ex.Message)
                {
                    StatusCode = 400
                };
                context.ExceptionHandled = true;
            }
        }
    }
}
