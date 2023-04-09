using FluentValidation;
using PizzaWebApi.Core.Requests;

namespace PizzaWebApi.Core.Validators
{
    public class TokenRefreshRequestValidator : AbstractValidator<TokenRefreshRequest>
    {
        public TokenRefreshRequestValidator()
        {
            RuleFor(c => c.Token).NotEmpty();
            RuleFor(c => c.RefreshToken).NotEmpty();
        }
    }
}
