using MediatR;

namespace SSO.Application.Features.AuthenticationFeature.Queries.LogoutUser
{
    public class LogoutUserQuery : IRequest
    {
        public long UserId { get; set; }
        public string AccessToken { get; set; }
    }
}
