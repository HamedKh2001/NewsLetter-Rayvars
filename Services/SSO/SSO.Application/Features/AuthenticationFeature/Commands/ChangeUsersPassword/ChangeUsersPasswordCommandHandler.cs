using MediatR;
using SharedKernel.Contracts.Infrastructure;
using SharedKernel.Exceptions;
using SSO.Application.Contracts.Persistence;
using SSO.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SSO.Application.Features.AuthenticationFeature.Commands.ChangeUsersPassword
{
    public class ChangeUsersPasswordCommandHandler : IRequestHandler<ChangeUsersPasswordCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEncryptionService _encryptionService;

        public ChangeUsersPasswordCommandHandler(IUserRepository userRepository, IEncryptionService encryptionService)
        {
            _userRepository = userRepository;
            _encryptionService = encryptionService;
        }

        public async Task<Unit> Handle(ChangeUsersPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetAsync(request.UserId, cancellationToken);
            if (user is null)
                throw new NotFoundException(nameof(User), request.UserId);

            user.Password = _encryptionService.HashPassword(request.UserNewPassword);
            await _userRepository.UpdateAsync(user, cancellationToken);
            return Unit.Value;
        }
    }
}
