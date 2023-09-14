using FluentValidation;
using System.Threading.Tasks;
using System.Threading;
using SSO.Application.Contracts.Persistence;

namespace SSO.Application.Features.GroupFeature.Commands.CreateGroup
{
    public class CreateGroupCommandValidator : AbstractValidator<CreateGroupCommand>
    {
        private readonly IGroupRepository _groupRepository;

        public CreateGroupCommandValidator(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;

            RuleFor(p => p.Caption)
              .NotNull()
              .NotEmpty().WithMessage("{Caption} is required.")
              .MaximumLength(200).WithMessage("{Caption} must not exceed 50 characters.")
              .MustAsync(BeUniqueCaption).WithMessage("The specified caption already exists.");
        }

        public async Task<bool> BeUniqueCaption(string caption, CancellationToken cancellationToken)
        {
            return await _groupRepository.IsUniqueCaptionAsync(caption, cancellationToken);
        }
    }
}
