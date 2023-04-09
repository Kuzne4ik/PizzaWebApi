using FluentValidation;
using PizzaWebApi.Core.Requests.Mediators;

namespace PizzaWebApi.Core.Validators
{
    public class ProductsSearchCriteriaRequestValidator : AbstractValidator<ProductsSearchCriteriaRequest>
    {
        public ProductsSearchCriteriaRequestValidator()
        {
            RuleFor(c => c.Page).GreaterThan(-1).NotEmpty();
            RuleFor(c => c.PageSize).GreaterThan(-1).NotEmpty();
        }
    }
}
