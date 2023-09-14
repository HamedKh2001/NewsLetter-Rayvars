using AutoMapper;
using MediatR;
using SharedKernel.Common;
using SharedKernel.Exceptions;
using SSO.Application.Contracts.Persistence;
using SSO.Application.Features.UserFeature.Queries.GetUser;
using SSO.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SSO.Application.Features.GroupFeature.Queries.GetGroupUsers
{
    public class GetGroupUsersQueryHandler : IRequestHandler<GetGroupUsersQuery, PaginatedList<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetGroupUsersQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedList<UserDto>> Handle(GetGroupUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetByGroupIdAsync(request.GroupId, request.SearchParam, request.PageNumber, request.PageSize, cancellationToken);
            if (users is null)
                throw new NotFoundException(nameof(Group), request.GroupId);

            return _mapper.Map<PaginatedList<UserDto>>(users);
        }
    }
}
