using AutoMapper;
using MediatR;
using SharedKernel.Contracts.Infrastructure;
using SSO.Application.Contracts.Persistence;
using SSO.Application.Features.UserFeature.Queries.GetUser;
using SSO.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SSO.Application.Features.UserFeature.Commands.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEncryptionService _encryptionService;
        private readonly IDateTimeService _dateTimeService;
        private readonly IMapper _mapper;

        public CreateUserCommandHandler(IUserRepository userRepository, IEncryptionService encryptionService, IDateTimeService dateTimeService, IMapper mapper)
        {
            _userRepository = userRepository;
            _encryptionService = encryptionService;
            _dateTimeService = dateTimeService;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = _mapper.Map<User>(request);
            user.Password = _encryptionService.HashPassword(request.Password);
            user.CreatedDate = _dateTimeService.Now;
            user.IsActive = true;

            var result = await _userRepository.CreateAsync(user, cancellationToken);

            return _mapper.Map<UserDto>(result);
        }
    }
}
