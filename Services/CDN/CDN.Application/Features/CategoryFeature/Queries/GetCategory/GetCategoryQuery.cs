using MediatR;

namespace CDN.Application.Features.CategoryFeature.Queries.GetCategory
{
    public class GetCategoryQuery : IRequest<CategoryDto>
    {
        public long Id { get; set; }
    }
}
