using MediatR;
using SharedKernel.Common;
using SSO.Application.Features.GroupFeature.Queries.GetGroup;

namespace SSO.Application.Features.GroupFeature.Queries.GetGroups
{
    public class GetGroupsQuery : PaginationQuery, IRequest<PaginatedList<GroupDto>>
    {
        public string Caption { get; set; }
    }
}
