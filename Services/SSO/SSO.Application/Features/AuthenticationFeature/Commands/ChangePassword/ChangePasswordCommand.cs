using MediatR;
using Newtonsoft.Json;

namespace SSO.Application.Features.AuthenticationFeature.Commands.ChangePassword
{
    public class ChangePasswordCommand : IRequest
    {
        public long UserId { get; set; }
        [JsonIgnore]
        public string CurrentPassword { get; set; }
        [JsonIgnore]
        public string NewPassword { get; set; }
    }
}
