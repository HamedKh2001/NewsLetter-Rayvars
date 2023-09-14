using FluentValidation;

namespace SSO.Application.Features.RoleFeature.Queries.GetRoleUsers
{
    public class GetRoleUsersQueryValidator : AbstractValidator<GetRoleUsersQuery>
    {
        public GetRoleUsersQueryValidator()
        {
            RuleFor(p => p.RoleId).GreaterThan(0).WithMessage("The {roleId} is required.");
        }
    }
}
