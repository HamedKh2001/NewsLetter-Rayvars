using MediatR;
using SharedKernel.Common;
using SSO.Application.Features.UserFeature.Queries.GetUser;

namespace SSO.Application.Features.GroupFeature.Queries.GetGroupUsers
{
    public class GetGroupUsersQuery : PaginationQuery, IRequest<PaginatedList<UserDto>>
    {
        public long GroupId { get; set; }
        public string SearchParam { get; set; }
    }
}
