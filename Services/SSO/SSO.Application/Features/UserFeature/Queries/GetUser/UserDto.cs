using System;

namespace SSO.Application.Features.UserFeature.Queries.GetUser
{
    public class UserDto
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte NationalType { get; set; }
        public bool IsActive { get; set; }
        public string Mobile { get; set; }
        public byte Gender { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
