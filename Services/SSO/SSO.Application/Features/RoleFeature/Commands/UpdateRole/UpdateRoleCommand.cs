using MediatR;

namespace SSO.Application.Features.RoleFeature.Commands.UpdateRole
{
    public class UpdateRoleCommand : IRequest
    {
        public int Id { get; set; }
        public string DisplayTitle { get; set; }
    }
}
