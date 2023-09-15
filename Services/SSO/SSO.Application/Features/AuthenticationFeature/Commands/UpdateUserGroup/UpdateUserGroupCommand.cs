using MediatR;
using System.Collections.Generic;

namespace SSO.Application.Features.AuthenticationFeature.Commands.UpdateUserGroup
{
    public class UpdateUserGroupCommand : IRequest
    {
        public int UserId { get; set; }
        public List<int> GroupIds { get; set; }
    }
}
