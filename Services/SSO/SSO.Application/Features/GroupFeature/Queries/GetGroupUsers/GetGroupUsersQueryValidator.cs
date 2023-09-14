using FluentValidation;

namespace SSO.Application.Features.GroupFeature.Queries.GetGroupUsers
{
    public class GetGroupUsersQueryValidator : AbstractValidator<GetGroupUsersQuery>
    {
        public GetGroupUsersQueryValidator()
        {
            RuleFor(p => p.GroupId).GreaterThan(0).WithMessage("The {groupId} is required.");
        }
    }
}
