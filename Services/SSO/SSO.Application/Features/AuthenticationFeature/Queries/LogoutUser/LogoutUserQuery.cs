using MediatR;

namespace SSO.Application.Features.AuthenticationFeature.Queries.LogoutUser
{
    public class LogoutUserQuery : IRequest
    {
        public int UserId { get; set; }
        public string AccessToken { get; set; }
    }
}
