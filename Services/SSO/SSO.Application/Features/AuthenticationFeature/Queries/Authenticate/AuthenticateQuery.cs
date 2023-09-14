using MediatR;
using Newtonsoft.Json;

namespace SSO.Application.Features.AuthenticationFeature.Queries.Authenticate
{
    public class AuthenticateQuery : IRequest<AuthenticateDto>
    {
        public string UserName { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
    }
}
