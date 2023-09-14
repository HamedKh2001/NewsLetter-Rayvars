using AutoMapper;
using MediatR;
using SharedKernel.Exceptions;
using SSO.Application.Contracts.Persistence;
using SSO.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SSO.Application.Features.GroupFeature.Commands.UpdateGroup
{
    public class UpdateGroupCommandHandler : IRequestHandler<UpdateGroupCommand>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IMapper _mapper;

        public UpdateGroupCommandHandler(IGroupRepository groupRepository, IMapper mapper)
        {
            _groupRepository = groupRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
        {
            var groupToUpdate = await _groupRepository.GetAsync(request.Id, cancellationToken);
            if (groupToUpdate is null)
                throw new NotFoundException(nameof(Group), request.Id);

            groupToUpdate = _mapper.Map<Group>(request);
            await _groupRepository.UpdateAsync(groupToUpdate, cancellationToken);

            return Unit.Value;
        }
    }
}
