using Microsoft.AspNetCore.Mvc;
using PizzaWebApi.Web.Filters.ActionFilters;

namespace PizzaWebApi.Web.Attributes
{
    /// <summary>
    /// Check is exists Category by id
    /// Use with EnsureCategoryExistsActionFilter only. Constructor make factory for EnsureCategoryExistsActionFilter DI factory access.
    /// </summary>
    /// <remarks>
    /// EnsureCategoryExistsAttribute это связка для EnsureCategoryExistsActionFilter,
    /// чтобы использовать конейнер зависимостей корректным образом
    /// Это наследник TypeFilterAttribute этот класс-атрибут используется специально для получения зависимостей
    /// из контейнера внедрения зависимостей.
    /// Атрибуты C# не позволяют передавать зависимости в свои конструкторы
    /// Они создаются как объекты-одиночки (singltone), поэтому существует только один
    /// экземпляр для жизненного цикла вашего приложения.
    /// В итоге получается вызов объектов с жизненным циклом transient и scoped в объекте singltone, что нарушает их жизненный цикл.
    /// Чтобы не применять антипатерн внедрения зависимостей, применяется 
    /// Пример антипатерна: var service = (ICategoryRepository) context.HttpContext.RequestServices.GetService(typeof(ICategoryRepository));
    /// TypeFilterAttribute или ServiceFilterAttribute имеет конструктор куда передается тип фильтра как аргумент, за счет чего фильтр получает корректный доступ к
    /// к контейнеру внедрения зависимостей (фабрика).
    /// Когда вызывается действие, декорированное атрибутом EnsureRecipe-ExistsAttribute,
    /// фреймворк вызывает метод CreateInstance() для атри-
    /// бута.Он создает новый экземпляр EnsureRecipeExistsFilter и использует
    /// контейнер внедрения зависимостей для заполнения зависимостей
    /// </remarks>
    public class EnsureCategoryExistsAttribute : ServiceFilterAttribute
    {
        /// <summary>
        /// TypeFilterAttribute через базовый конструктор передает тип фильтра как аргумент для
        /// организации доступа к DI контейнеру
        /// </summary>
        public EnsureCategoryExistsAttribute() : base(typeof(EnsureCategoryExistsActionFilter)) { }
    }
}
