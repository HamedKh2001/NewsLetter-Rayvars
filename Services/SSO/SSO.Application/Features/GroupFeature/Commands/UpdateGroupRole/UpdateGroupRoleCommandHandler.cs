using MediatR;
using SharedKernel.Exceptions;
using SharedKernel.Extensions;
using SSO.Application.Contracts.Persistence;
using SSO.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SSO.Application.Features.GroupFeature.Commands.UpdateGroupRole
{
    public class UpdateGroupRoleCommandHandler : IRequestHandler<UpdateGroupRoleCommand>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IRoleRepository _roleRepository;

        public UpdateGroupRoleCommandHandler(IGroupRepository groupRepository, IRoleRepository roleRepository)
        {
            _groupRepository = groupRepository;
            _roleRepository = roleRepository;
        }

        public async Task<Unit> Handle(UpdateGroupRoleCommand request, CancellationToken cancellationToken)
        {
            var group = await _groupRepository.GetWithRolesAsync(request.GroupId, cancellationToken);
            if (group == null)
                throw new NotFoundException(nameof(Group), request.GroupId);

            var (deleteRoles, addRoleIds) = ConsistencyGroupRole(group.Roles, request.RoleIds);

            if (addRoleIds.Count > 0)
            {
                var addRoles = await _roleRepository.GetByRoleIdsAsync(addRoleIds, cancellationToken);
                if (addRoleIds.Count != addRoles.Count)
                    throw new BadRequestException("Some RoleIds are Invalid.");
                group.Roles.AddRange(addRoles);
            }

            if (deleteRoles.Count > 0)
            {
                group.Roles.RemoveRange(deleteRoles);
            }

            await _groupRepository.UpdateAsync(group, cancellationToken);

            return Unit.Value;
        }

        private static (List<Role> deleteRoles, List<long> addRoleIds) ConsistencyGroupRole(ICollection<Role> roles, List<long> roleIds)
        {
            var deleteRoles = roles.Where(rr => roleIds.All(r => r != rr.Id)).Distinct().ToList();
            var addRoleIds = roleIds.Where(rr => roles.All(r => r.Id != rr)).Distinct().ToList();

            return (deleteRoles, addRoleIds);
        }
    }
}
