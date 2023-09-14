using MediatR;
using Newtonsoft.Json;

namespace SSO.Application.Features.AuthenticationFeature.Commands.ChangeUsersPassword
{
    public class ChangeUsersPasswordCommand : IRequest
    {
        public long AdminUserId { get; set; }
        public long UserId { get; set; }
        [JsonIgnore]
        public string AdminPassword { get; set; }
        [JsonIgnore]
        public string UserNewPassword { get; set; }
    }
}
