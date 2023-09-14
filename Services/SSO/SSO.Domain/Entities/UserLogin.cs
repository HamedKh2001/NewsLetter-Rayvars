using SharedKernel.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSO.Domain.Entities
{
    public class UserLogin : BaseEntity
    {
        public int UserId { get; set; }
        public string IpAddress { get; set; }
        public string ExtraInfo { get; set; }
        public DateTime CreatedDate { get; set; }


        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
