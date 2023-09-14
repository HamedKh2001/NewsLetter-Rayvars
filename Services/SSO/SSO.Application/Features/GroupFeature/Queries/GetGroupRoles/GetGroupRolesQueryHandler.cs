using AutoMapper;
using MediatR;
using SharedKernel.Exceptions;
using SSO.Application.Contracts.Persistence;
using SSO.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SSO.Application.Features.GroupFeature.Queries.GetGroupRoles
{
    public class GetGroupRolesQueryHandler : IRequestHandler<GetGroupRolesQuery, GroupRolesDto>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IMapper _mapper;

        public GetGroupRolesQueryHandler(IGroupRepository groupRepository, IMapper mapper)
        {
            _mapper = mapper;
            _groupRepository = groupRepository;
        }

        public async Task<GroupRolesDto> Handle(GetGroupRolesQuery request, CancellationToken cancellationToken)
        {
            var groupRoles = await _groupRepository.GetWithRolesAsync(request.GroupId, cancellationToken);
            if (groupRoles is null)
                throw new NotFoundException(nameof(Group), request.GroupId);

            return _mapper.Map<GroupRolesDto>(groupRoles);
        }
    }
}
