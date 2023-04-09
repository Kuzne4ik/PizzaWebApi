using FluentValidation;
using PizzaWebApi.Core.ApiModels;

namespace PizzaWebApi.Core.Validators
{
    public class CategoryDTOValidator : AbstractValidator<CategoryDTO>
    {
        public CategoryDTOValidator()
        {
            RuleFor(c => c.Name).NotEmpty().MaximumLength(250);
            RuleFor(c => c.Title).NotEmpty().MaximumLength(50);
        }
    }
}
