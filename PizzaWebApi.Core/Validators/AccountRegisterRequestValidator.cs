using FluentValidation;
using PizzaWebApi.Core.Requests;

namespace PizzaWebApi.Core.Validators
{
    public class AccountRegisterRequestValidator : AbstractValidator<AccountRegisterRequest>
    {
        public AccountRegisterRequestValidator()
        {
            RuleFor(c => c.UserName).NotEmpty();
            RuleFor(c => c.Password).NotEmpty();
            RuleFor(c => c.Name).NotEmpty();
            RuleFor(c => c.Email).NotEmpty().EmailAddress();
        }
    }
}
