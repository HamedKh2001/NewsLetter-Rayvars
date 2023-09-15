using MediatR;
using System.Collections.Generic;

namespace SSO.Application.Features.RoleFeature.Queries.GetRoleUsers
{
    public class GetRoleUsersQuery : IRequest<List<GetRoleUsersDto>>
    {
        public int RoleId { get; set; }
    }
}
