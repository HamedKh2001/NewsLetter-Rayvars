using MediatR;
using Newtonsoft.Json;
using SSO.Application.Features.UserFeature.Queries.GetUser;
using SSO.Domain.Enums;

namespace SSO.Application.Features.UserFeature.Commands.CreateUser
{
    public class CreateUserCommand : IRequest<UserDto>
    {
        public string UserName { get; set; }
        public NationalType NationalType { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public GenderType Gender { get; set; }
    }
}
