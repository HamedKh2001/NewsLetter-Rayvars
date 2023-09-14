using Newtonsoft.Json;

namespace SSO.Application.Features.AuthenticationFeature.Queries.Authenticate
{
    public class AuthenticateDto
    {
        [JsonIgnore]
        public string Token { get; set; }
        [JsonIgnore]
        public string RefreshToken { get; set; }
    }
}
