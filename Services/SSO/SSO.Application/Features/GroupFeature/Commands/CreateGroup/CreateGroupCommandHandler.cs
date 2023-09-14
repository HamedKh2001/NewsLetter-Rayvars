using AutoMapper;
using MediatR;
using SSO.Application.Contracts.Persistence;
using SSO.Application.Features.GroupFeature.Queries.GetGroup;
using SSO.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SSO.Application.Features.GroupFeature.Commands.CreateGroup
{
    public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, GroupDto>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IMapper _mapper;

        public CreateGroupCommandHandler(IGroupRepository groupRepository, IMapper mapper)
        {
            _groupRepository = groupRepository;
            _mapper = mapper;
        }

        public async Task<GroupDto> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
        {
            var group = new Group { Caption = request.Caption };
            var result = await _groupRepository.CreateAsync(group, cancellationToken);
            return _mapper.Map<GroupDto>(result);
        }
    }
}
