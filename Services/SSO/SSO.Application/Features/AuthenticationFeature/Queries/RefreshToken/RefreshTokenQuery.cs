using MediatR;
using Newtonsoft.Json;
using SSO.Application.Features.AuthenticationFeature.Queries.Authenticate;

namespace SSO.Application.Features.AuthenticationFeature.Queries.RefreshToken
{
    public class RefreshTokenQuery : IRequest<AuthenticateDto>
    {
        [JsonIgnore]
        public string AccessToken { get; set; }
        [JsonIgnore]
        public string RefreshToken { get; set; }
    }
}
