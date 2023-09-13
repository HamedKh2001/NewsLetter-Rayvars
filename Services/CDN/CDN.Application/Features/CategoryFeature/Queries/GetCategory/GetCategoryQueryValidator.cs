using FluentValidation;

namespace CDN.Application.Features.CategoryFeature.Queries.GetCategory
{
    public class GetCategoryQueryValidator : AbstractValidator<GetCategoryQuery>
    {
        public GetCategoryQueryValidator()
        {
            RuleFor(p => p.Id).GreaterThan(0).WithMessage("The {id} is required.");
        }
    }
}
