using FluentValidation;
using PizzaWebApi.Core.ApiModels;

namespace PizzaWebApi.Core.Validators
{
    public class CartDTOValidator : AbstractValidator<CartDTO>
    {
        public CartDTOValidator()
        {
            RuleFor(c => c.UserId).GreaterThan(0);
        }
    }
}
