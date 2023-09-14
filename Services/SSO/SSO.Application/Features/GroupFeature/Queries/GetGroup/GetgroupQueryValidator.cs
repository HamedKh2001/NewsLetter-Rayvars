using FluentValidation;

namespace SSO.Application.Features.GroupFeature.Queries.GetGroup
{
    public class GetgroupQueryValidator : AbstractValidator<GetGroupQuery>
    {
        public GetgroupQueryValidator()
        {
            RuleFor(p => p.Id).GreaterThan(0).WithMessage("The {id} is required.");
        }
    }
}
