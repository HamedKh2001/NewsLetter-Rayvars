using MediatR;
using SSO.Application.Features.GroupFeature.Queries.GetGroup;

namespace SSO.Application.Features.GroupFeature.Commands.CreateGroup
{
    public class CreateGroupCommand : IRequest<GroupDto>
    {
        public string Caption { get; set; }
        public bool IsPermissionBase { get; set; }
    }
}
