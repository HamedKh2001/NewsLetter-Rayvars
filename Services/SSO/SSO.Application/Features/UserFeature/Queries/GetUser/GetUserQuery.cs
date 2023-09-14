using MediatR;

namespace SSO.Application.Features.UserFeature.Queries.GetUser
{
    public class GetUserQuery : IRequest<UserDto>
    {
        public long Id { get; set; }
    }
}
