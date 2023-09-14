﻿using AutoMapper;
using MediatR;
using SharedKernel.Common;
using SSO.Application.Contracts.Persistence;
using SSO.Application.Features.UserFeature.Queries.GetUser;
using System.Threading;
using System.Threading.Tasks;

namespace SSO.Application.Features.UserFeature.Queries.GetUsers
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PaginatedList<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUsersQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedList<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var pagedusers = await _userRepository.GetAsync(request.SearchParam, request.PageNumber, request.PageSize, cancellationToken);
            return _mapper.Map<PaginatedList<UserDto>>(pagedusers);
        }
    }
}
