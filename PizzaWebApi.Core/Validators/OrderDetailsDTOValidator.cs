using FluentValidation;
using PizzaWebApi.Core.ApiModels;

namespace PizzaWebApi.Core.Validators
{
    public class OrderDetailsDTOValidator : AbstractValidator<OrderDetailsDTO>
    {
        public OrderDetailsDTOValidator()
        {
            RuleFor(c => c.UserId).GreaterThan(0);
            RuleFor(c => c.FirstName).MaximumLength(250);
            RuleFor(c => c.LastName).MaximumLength(250);
            //withount +  [0-9]+
            RuleFor(c => c.Phone).NotEmpty().Length(11).Must(x => ulong.TryParse(x, out var val)).WithMessage("Invalid Phone number");
            RuleFor(c => c.Email).NotEmpty().EmailAddress().WithMessage("Invalid E-mail");
        }
    }
}
