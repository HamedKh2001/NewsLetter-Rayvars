using FluentValidation;
using SharedKernel.Contracts.Infrastructure;
using SSO.Application.Contracts.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace SSO.Application.Features.AuthenticationFeature.Commands.ChangeUsersPassword
{
    public class ChangeUsersPasswordCommandValidator : AbstractValidator<ChangeUsersPasswordCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEncryptionService _encryptionService;

        public ChangeUsersPasswordCommandValidator(IUserRepository userRepository, IEncryptionService encryptionService)
        {
            _userRepository = userRepository;
            _encryptionService = encryptionService;

            RuleFor(p => p.AdminUserId).GreaterThan(0).WithMessage("The {adminUserId} is required.");
            RuleFor(p => p).MustAsync(BeValidAdminUser).WithMessage("The admin information is invalid.");

            RuleFor(p => p.UserId).GreaterThan(0).WithMessage("The {userId} is required.");
        }

        private async Task<bool> BeValidAdminUser(ChangeUsersPasswordCommand command, CancellationToken cancellationToken)
        {
            var encPass = _encryptionService.HashPassword(command.AdminPassword);
            return await _userRepository.GetUserByPasswordAsync(command.AdminUserId, encPass, cancellationToken) != null;
        }
    }
}
