using MediatR;
using System.Collections.Generic;

namespace SSO.Application.Features.RoleFeature.Queries.GetRoles
{
    public class GetRolesQuery : IRequest<List<RoleDto>>
    {
    }
}
