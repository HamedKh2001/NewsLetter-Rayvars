using SharedKernel.Common;
using System.Collections.Generic;

namespace SSO.Domain.Entities
{
    public class Role : BaseEntity
    {
        public string Title { get; set; }
        public string DisplayTitle { get; set; }
        public string Discriminator { get; set; }

        public virtual ICollection<Group> Groups { get; set; } = new List<Group>();
    }
}
