using FluentValidation;
using SSO.Application.Contracts.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace SSO.Application.Features.RoleFeature.Commands.UpdateRole
{
    public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
    {
        private readonly IRoleRepository _roleRepository;

        public UpdateRoleCommandValidator(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;

            RuleFor(p => p.Id)
            .GreaterThan(0).WithMessage("{Id} is required.");

            RuleFor(p => p.DisplayTitle)
              .NotEmpty().WithMessage("{DisplayTitle} is required.")
              .NotNull()
              .MaximumLength(100).WithMessage("{DisplayTitle} must not exceed 100 characters.");

            RuleFor(p => p).MustAsync(BeUniqueDisplayTitle).WithMessage("The specified displayTitle already exists.");
        }
        public async Task<bool> BeUniqueDisplayTitle(UpdateRoleCommand command, CancellationToken cancellationToken)
        {
            return await _roleRepository.IsUniqueDisplayTitleAsync(command.Id, command.DisplayTitle, cancellationToken);
        }

    }
}
