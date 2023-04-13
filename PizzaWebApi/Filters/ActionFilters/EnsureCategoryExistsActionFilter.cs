using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using PizzaWebApi.Core.Interfaces;

namespace PizzaWebApi.Web.Filters.ActionFilters
{
    /// <summary>
    /// Check is existst Category action filter
    /// Use with EnsureCategoryExistsAttribute only! Do not use globaly!
    /// </summary>
    /// <remarks>
    /// Внимание! Нельзя объявлять глобально, так как требуется точечное использование в определенных
    /// методах Action с помощью аттрибута
    /// Работает в связке с EnsureCategoryExistsAttribute
    /// </remarks>
    public class EnsureCategoryExistsActionFilter : IAsyncActionFilter
    {
        private readonly ICategoryService _categoryService;


        public EnsureCategoryExistsActionFilter(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Call before Action execute
        /// </summary>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ActionArguments.ContainsKey("categoryId"))
                throw new ArgumentException("Query param CategoryId is not exists");
            var categoryId = (int)context.ActionArguments["categoryId"];
            if (categoryId < 0 || !await _categoryService.CategoryIsExistById(categoryId))
            {
                var error = new ProblemDetails
                {
                    Title = "An error occurred",
                    Detail = $"Could not find a Category with ID: {categoryId}",
                    Status = 404,
                    Type = "https://httpstatuses.com/404"
                };
                context.Result = new ObjectResult(error)
                {
                    StatusCode = 404
                };
                return;
            }
            await next(); //need to pass the execution to next
        }
    }
}
