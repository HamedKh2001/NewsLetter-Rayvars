using MediatR;

namespace CDN.Application.Features.CategoryFeature.Queries.GetCategory
{
    public class GetCategoryQuery : IRequest<CategoryDto>
    {
        public int Id { get; set; }
    }
}
