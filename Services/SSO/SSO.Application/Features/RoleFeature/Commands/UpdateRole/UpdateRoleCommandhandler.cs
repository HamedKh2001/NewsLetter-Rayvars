using AutoMapper;
using MediatR;
using SharedKernel.Exceptions;
using SSO.Application.Contracts.Persistence;
using SSO.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SSO.Application.Features.RoleFeature.Commands.UpdateRole
{
    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public UpdateRoleCommandHandler(IRoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var roleToUpdate = await _roleRepository.GetAsync(request.Id, cancellationToken);
            if (roleToUpdate is null)
                throw new NotFoundException(nameof(Role), request.Id);

            roleToUpdate = _mapper.Map<Role>(request);
            await _roleRepository.UpdateAsync(roleToUpdate, cancellationToken);


            return Unit.Value;
        }
    }
}
