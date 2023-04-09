using FluentValidation;
using PizzaWebApi.Core.ApiModels;

namespace PizzaWebApi.Core.Validators
{
    public class ProductDTOValidator : AbstractValidator<ProductDTO>
    {
        public ProductDTOValidator()
        {
            RuleFor(c => c.Name).NotEmpty().MaximumLength(250);
            RuleFor(c => c.Title).NotEmpty().MaximumLength(50);
            RuleFor(c => c.CategoryId).GreaterThan(0);
        }
    }
}
