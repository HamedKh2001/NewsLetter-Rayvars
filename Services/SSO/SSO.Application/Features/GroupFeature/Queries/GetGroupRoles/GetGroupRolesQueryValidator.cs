using FluentValidation;

namespace SSO.Application.Features.GroupFeature.Queries.GetGroupRoles
{
    public class GetGroupRolesQueryValidator : AbstractValidator<GetGroupRolesQuery>
    {
        public GetGroupRolesQueryValidator()
        {
            RuleFor(p => p.GroupId).GreaterThan(0).WithMessage("The {groupId} is required.");
        }
    }
}
