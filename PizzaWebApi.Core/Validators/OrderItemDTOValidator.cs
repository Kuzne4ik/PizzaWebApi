using FluentValidation;
using PizzaWebApi.Core.ApiModels;

namespace PizzaWebApi.Core.Validators
{
    public class OrderItemDTOValidator : AbstractValidator<OrderItemDTO>
    {
        public OrderItemDTOValidator()
        {
            RuleFor(c => c.OrderId).GreaterThan(0);
            RuleFor(c => c.ProductId).GreaterThan(0);
            RuleFor(c => c.Quantity).GreaterThan(0);
            RuleFor(c => c.Price).GreaterThan(0);
        }
    }
}
