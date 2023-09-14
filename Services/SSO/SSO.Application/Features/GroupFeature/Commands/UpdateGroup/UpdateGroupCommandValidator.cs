using FluentValidation;
using SSO.Application.Contracts.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace SSO.Application.Features.GroupFeature.Commands.UpdateGroup
{
    public class UpdateGroupCommandValidator : AbstractValidator<UpdateGroupCommand>
    {
        private readonly IGroupRepository _groupRepository;

        public UpdateGroupCommandValidator(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;

            RuleFor(p => p.Caption)
              .NotEmpty().WithMessage("{Caption} is required.")
              .NotNull()
              .MaximumLength(200).WithMessage("{Caption} must not exceed 200 characters.");

            RuleFor(p => p).MustAsync(BeUniqueCaption).WithMessage("The specified caption already exists.");
        }

        public async Task<bool> BeUniqueCaption(UpdateGroupCommand command, CancellationToken cancellationToken)
        {
            return await _groupRepository.IsUniqueCaptionAsync(command.Id, command.Caption, cancellationToken);
        }
    }
}
