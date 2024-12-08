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

            // Может быть Empty или Null. Но если есть символ, их должно быть более 3
            When(c => !string.IsNullOrEmpty(c.Keyword),
                () =>
                {
                    RuleFor(t => t.Keyword)
                        .MinimumLength(3);
                }
            );
        }
    }
}
