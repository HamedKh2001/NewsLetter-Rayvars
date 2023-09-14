using MediatR;

namespace SSO.Application.Features.GroupFeature.Queries.GetGroupRoles
{
    public class GetGroupRolesQuery : IRequest<GroupRolesDto>
    {
        public long GroupId { get; set; }
    }
}
