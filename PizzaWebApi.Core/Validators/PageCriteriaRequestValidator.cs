using FluentValidation;
using PizzaWebApi.Core.Requests;

namespace PizzaWebApi.Core.Validators
{
    public class PageCriteriaRequestValidator : AbstractValidator<PageCriteriaRequest>
    {
        public PageCriteriaRequestValidator()
        {
            RuleFor(c => c.Page).GreaterThan(-1).NotEmpty();
            RuleFor(c => c.PageSize).GreaterThan(-1).NotEmpty();
        }
    }
}
