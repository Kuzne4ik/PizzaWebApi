using FluentValidation;
using PizzaWebApi.Core.Requests;

namespace PizzaWebApi.Core.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(c => c.UserNameOrEmail).NotEmpty();
            RuleFor(c => c.Password).NotEmpty();
        }
    }
}
