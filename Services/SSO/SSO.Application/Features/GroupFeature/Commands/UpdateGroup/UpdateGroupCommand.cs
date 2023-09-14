using MediatR;

namespace SSO.Application.Features.GroupFeature.Commands.UpdateGroup
{
    public class UpdateGroupCommand : IRequest
    {
        public long Id { get; set; }
        public string Caption { get; set; }
        public bool IsPermissionBase { get; set; }
    }
}
