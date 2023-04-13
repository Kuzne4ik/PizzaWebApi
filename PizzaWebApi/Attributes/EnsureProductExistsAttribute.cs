using Microsoft.AspNetCore.Mvc;
using PizzaWebApi.Web.Filters.ActionFilters;

namespace PizzaWebApi.Web.Attributes
{
    /// <summary>
    /// Check is exists Product by id
    /// Use with EnsureProductExistsActionFilter only. Constructor make factory for EnsureProductExistsActionFilter DI factory access.
    /// </summary>
    /// <remarks>
    /// EnsureProductExistsAttribute это связка для EnsureProductExistsActionFilter,
    /// чтобы использовать конейнер зависимостей корректным образом
    /// Это наследник TypeFilterAttribute этот класс-атрибут используется специально для получения зависимостей
    /// из контейнера внедрения зависимостей.
    /// Атрибуты C# не позволяют передавать зависимости в свои конструкторы
    /// Они создаются как объекты-одиночки (singltone), поэтому существует только один
    /// экземпляр для жизненного цикла вашего приложения.
    /// В итоге получается вызов объектов с жизненным циклом transient и scoped в объекте singltone, что нарушает их жизненный цикл.
    /// Чтобы не применять антипатерн внедрения зависимостей, применяется 
    /// Пример антипатерна: var service = (IProductRepository) context.HttpContext.RequestServices.GetService(typeof(IProductRepository));
    /// TypeFilterAttribute или ServiceFilterAttribute имеет конструктор куда передается тип фильтра как аргумент, за счет чего фильтр получает корректный доступ к
    /// к контейнеру внедрения зависимостей (фабрика).
    /// Когда вызывается действие, декорированное атрибутом EnsureRecipe-ExistsAttribute,
    /// фреймворк вызывает метод CreateInstance() для атри-
    /// бута.Он создает новый экземпляр EnsureRecipeExistsFilter и использует
    /// контейнер внедрения зависимостей для заполнения зависимостей
    /// </remarks>
    public class EnsureProductExistsAttribute : ServiceFilterAttribute
    {
        /// <summary>
        /// TypeFilterAttribute через базовый конструктор передает тип фильтра как аргумент для
        /// организации доступа к DI контейнеру
        /// </summary>
        public EnsureProductExistsAttribute() : base(typeof(EnsureProductExistsActionFilter)) { }
    }
}
