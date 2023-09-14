using AutoMapper;
using MediatR;
using SharedKernel.Exceptions;
using SSO.Application.Contracts.Persistence;
using SSO.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SSO.Application.Features.RoleFeature.Queries.GetRoleUsers
{
    public class GetRoleUsersQueryHandler : IRequestHandler<GetRoleUsersQuery, List<GetRoleUsersDto>>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public GetRoleUsersQueryHandler(IRoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<List<GetRoleUsersDto>> Handle(GetRoleUsersQuery request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetWithUsersAsync(request.RoleId, cancellationToken);
            if (role is null)
                throw new NotFoundException(nameof(Role), request.RoleId);

            return _mapper.Map<List<GetRoleUsersDto>>(role.Groups); ;
        }
    }
}
