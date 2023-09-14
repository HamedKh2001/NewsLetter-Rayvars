using FluentValidation;

namespace SSO.Application.Features.RoleFeature.Queries.GetRoleGroups
{
    public class GetRoleGroupsQueryValidator : AbstractValidator<GetRoleGroupsQuery>
    {
        public GetRoleGroupsQueryValidator()
        {
            RuleFor(p => p.RoleId).GreaterThan(0).WithMessage("The {roleId} is required.");
        }
    }
}
