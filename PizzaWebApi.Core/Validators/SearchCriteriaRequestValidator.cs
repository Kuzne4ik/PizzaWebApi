using FluentValidation;
using PizzaWebApi.Core.Requests;

namespace PizzaWebApi.Core.Validators
{
    public class SearchCriteriaRequestValidator : AbstractValidator<SearchCriteriaRequest>
    {
        public SearchCriteriaRequestValidator()
        {
            RuleFor(c => c.Page).GreaterThan(-1).NotEmpty();
            RuleFor(c => c.PageSize).GreaterThan(-1).NotEmpty();
        }
    }
}
