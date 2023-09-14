using SharedKernel.Common;
using System.Collections.Generic;

namespace SSO.Domain.Entities
{
    public class Group : BaseEntity
    {
        public string Caption { get; set; }
        public bool IsPermissionBase { get; set; }

        public virtual ICollection<User> Users { get; set; } = new List<User>();
        public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
    }
}
