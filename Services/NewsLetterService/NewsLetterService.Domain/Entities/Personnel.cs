using SharedKernel.Common;

namespace NewsLetterService.Domain.Entities
{
    public class Personnel : BaseEntity
    {
        public string Name { get; set; }
        public string NationalCode { get; set; }

        public virtual ICollection<NewsLetterHistory> NewsLetterHistories { get; set; }
    }
}
