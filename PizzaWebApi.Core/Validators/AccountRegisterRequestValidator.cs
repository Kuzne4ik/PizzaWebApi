using FluentValidation;
using PizzaWebApi.Core.Requests;

namespace PizzaWebApi.Core.Validators
{
    public class AccountRegisterRequestValidator : AbstractValidator<AccountRegisterRequest>
    {
        public AccountRegisterRequestValidator()
        {
            RuleFor(c => c.UserName).NotEmpty().MinimumLength(3).MaximumLength(256);
            RuleFor(c => c.Password).NotEmpty().MinimumLength(6);
            RuleFor(c => c.Name).NotEmpty().MinimumLength(3).MaximumLength(256);
            RuleFor(c => c.Email).NotEmpty().EmailAddress();
        }
    }
}
