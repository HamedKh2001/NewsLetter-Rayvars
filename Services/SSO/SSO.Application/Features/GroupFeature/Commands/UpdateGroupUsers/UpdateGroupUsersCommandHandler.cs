using AutoMapper;
using MediatR;
using SharedKernel.Exceptions;
using SharedKernel.Extensions;
using SSO.Application.Contracts.Persistence;
using SSO.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SSO.Application.Features.GroupFeature.Commands.UpdateGroupUsers
{
    public class UpdateGroupUsersCommandHandler : IRequestHandler<UpdateGroupUsersCommand>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UpdateGroupUsersCommandHandler(IGroupRepository groupRepository, IUserRepository userRepository, IMapper mapper)
        {
            _groupRepository = groupRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateGroupUsersCommand request, CancellationToken cancellationToken)
        {
            var group = await _groupRepository.GetWithUsersAsync(request.GroupId, cancellationToken);
            if (group is null)
                throw new NotFoundException(nameof(Group), request.GroupId);

            var (deleteUsers, addUserIds) = ConsistencyGroupUser(group.Users, request.UserIds);

            if (addUserIds.Count > 0)
            {
                var addUsers = await _userRepository.GetByUserIdsAsync(addUserIds, cancellationToken);
                if (addUserIds.Count != addUsers.Count)
                    throw new BadRequestException("Some UserIds are Invalid.");
                group.Users.AddRange(addUsers);
            }

            if (deleteUsers.Count > 0)
            {
                group.Users.RemoveRange(deleteUsers);
            }

            if (addUserIds.Count > 0 || deleteUsers.Count > 0)
            {
                await _groupRepository.UpdateAsync(group, cancellationToken);
            }

            return Unit.Value;
        }


        private static (List<User> deleteUsers, List<long> addUserIds) ConsistencyGroupUser(ICollection<User> users, List<long> userIds)
        {
            var deleteUsers = users.Where(uu => userIds.All(u => u != uu.Id)).Distinct().ToList();
            var addUserIds = userIds.Where(uu => users.All(u => u.Id != uu)).Distinct().ToList();

            return (deleteUsers, addUserIds);
        }
    }
}
