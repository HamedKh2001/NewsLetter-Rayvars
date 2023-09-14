using FluentValidation;

namespace SSO.Application.Features.GroupFeature.Commands.UpdateGroupUsers
{
    public class UpdateGroupUsersCommandValidator : AbstractValidator<UpdateGroupUsersCommand>
    {
        public UpdateGroupUsersCommandValidator()
        {
            RuleFor(p => p.GroupId).GreaterThan(0).WithMessage("The {groupId} is required.");
            RuleFor(x => x.UserIds).Must(x => x.Count > 0).WithMessage("There is not any {UserId}");
            RuleForEach(x => x.UserIds).GreaterThan(0).WithMessage("The userId {CollectionIndex} is required.");
        }
    }
}
