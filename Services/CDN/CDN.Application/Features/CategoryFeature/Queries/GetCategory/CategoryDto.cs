using System;

namespace CDN.Application.Features.CategoryFeature.Queries.GetCategory
{
    public class CategoryDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
