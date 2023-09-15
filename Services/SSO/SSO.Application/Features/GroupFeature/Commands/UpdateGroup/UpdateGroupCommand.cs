using MediatR;

namespace SSO.Application.Features.GroupFeature.Commands.UpdateGroup
{
    public class UpdateGroupCommand : IRequest
    {
        public int Id { get; set; }
        public string Caption { get; set; }
        public bool IsPermissionBase { get; set; }
    }
}
