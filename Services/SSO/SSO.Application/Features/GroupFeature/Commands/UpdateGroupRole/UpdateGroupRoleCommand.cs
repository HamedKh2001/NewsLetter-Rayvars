using MediatR;
using System.Collections.Generic;

namespace SSO.Application.Features.GroupFeature.Commands.UpdateGroupRole
{
    public class UpdateGroupRoleCommand : IRequest
    {
        public long GroupId { get; set; }
        public List<long> RoleIds { get; set; }
    }
}
