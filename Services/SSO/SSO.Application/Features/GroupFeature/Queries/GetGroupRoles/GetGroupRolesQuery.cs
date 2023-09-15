using MediatR;

namespace SSO.Application.Features.GroupFeature.Queries.GetGroupRoles
{
    public class GetGroupRolesQuery : IRequest<GroupRolesDto>
    {
        public int GroupId { get; set; }
    }
}
