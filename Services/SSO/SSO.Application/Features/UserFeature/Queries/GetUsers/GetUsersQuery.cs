using MediatR;
using SharedKernel.Common;
using SSO.Application.Features.UserFeature.Queries.GetUser;

namespace SSO.Application.Features.UserFeature.Queries.GetUsers
{
    public class GetUsersQuery : PaginationQuery, IRequest<PaginatedList<UserDto>>
    {
        public string SearchParam { get; set; }
    }
}
