using SharedKernel.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CDN.Domain.Entities
{
    public class NewsLetter : BaseEntity
    {
        public int CategoryId { get; set; }
        public string FileName { get; set; }
        public string TagName { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }


        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    }
}
