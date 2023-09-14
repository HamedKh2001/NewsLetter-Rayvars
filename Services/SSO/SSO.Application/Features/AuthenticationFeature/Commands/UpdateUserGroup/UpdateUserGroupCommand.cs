using MediatR;
using System.Collections.Generic;

namespace SSO.Application.Features.AuthenticationFeature.Commands.UpdateUserGroup
{
    public class UpdateUserGroupCommand : IRequest
    {
        public long UserId { get; set; }
        public List<long> GroupIds { get; set; }
    }
}
