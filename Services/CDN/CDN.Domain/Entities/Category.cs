using SharedKernel.Common;

namespace CDN.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Title { get; set; }
        public string Path { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }


        public virtual ICollection<NewsLetter> NewsLetters { get; set; }
    }
}
