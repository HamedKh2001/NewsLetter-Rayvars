using MediatR;
using System.Collections.Generic;

namespace SSO.Application.Features.RoleFeature.Queries.GetRoleGroups
{
    public class GetRoleGroupsQuery : IRequest<List<GetRoleGroupsDto>>
    {
        public long RoleId { get; set; }
    }
}
