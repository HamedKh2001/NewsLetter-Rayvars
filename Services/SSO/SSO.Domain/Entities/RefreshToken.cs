using SharedKernel.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSO.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public int UserId { get; set; }
        public string Token { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpirationDate { get; set; }


        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
