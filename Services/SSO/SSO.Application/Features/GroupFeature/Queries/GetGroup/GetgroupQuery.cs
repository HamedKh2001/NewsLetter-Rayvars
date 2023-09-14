using MediatR;

namespace SSO.Application.Features.GroupFeature.Queries.GetGroup
{
    public class GetGroupQuery : IRequest<GroupDto>
    {
        public long Id { get; set; }
    }
}
