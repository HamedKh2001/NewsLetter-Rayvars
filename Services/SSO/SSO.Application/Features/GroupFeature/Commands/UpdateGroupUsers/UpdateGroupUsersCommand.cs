using MediatR;
using System.Collections.Generic;

namespace SSO.Application.Features.GroupFeature.Commands.UpdateGroupUsers
{
    public class UpdateGroupUsersCommand : IRequest
    {
        public int GroupId { get; set; }
        public List<int> UserIds { get; set; }
    }
}
