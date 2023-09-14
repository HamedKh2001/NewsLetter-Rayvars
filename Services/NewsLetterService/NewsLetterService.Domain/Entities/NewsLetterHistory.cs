using SharedKernel.Common;
using static NewsLetterService.Domain.Enums.Act;

namespace NewsLetterService.Domain.Entities
{
    public class NewsLetterHistory : BaseEntity
    {
        public int PersonnelId { get; set; }
        public int NewsLetterId { get; set; }
        public ActType Act { get; set; }
        public DateTime DateOfAct { get; set; }

        public virtual Personnel Personnel { get; set; }
    }
}
