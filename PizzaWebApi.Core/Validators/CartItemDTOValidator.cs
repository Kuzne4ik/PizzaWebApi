using FluentValidation;
using PizzaWebApi.Core.ApiModels;

namespace PizzaWebApi.Core.Validators
{
    public class CartItemDTOValidator : AbstractValidator<CartItemDTO>
    {
        public CartItemDTOValidator()
        {
            RuleFor(c => c.CartId).GreaterThan(0);

            RuleFor(c => c.Quantity).GreaterThan(0);

            RuleFor(c => c.ProductId).GreaterThan(0);
        }
    }
}
