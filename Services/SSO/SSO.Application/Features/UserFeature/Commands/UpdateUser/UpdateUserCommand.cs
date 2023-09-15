using MediatR;
using SSO.Domain.Enums;
using System;

namespace SSO.Application.Features.UserFeature.Commands.UpdateUser
{
    public class UpdateUserCommand : IRequest
    {
        public int Id { get; set; }
        public NationalType NationalType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public GenderType Gender { get; set; }
    }
}
