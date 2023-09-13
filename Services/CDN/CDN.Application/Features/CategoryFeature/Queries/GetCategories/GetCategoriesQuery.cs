using CDN.Application.Features.CategoryFeature.Queries.GetCategory;
using MediatR;
using SharedKernel.Common;

namespace CDN.Application.Features.CategoryFeature.Queries.GetCategories
{
    public class GetCategoriesQuery : PaginationQuery, IRequest<PaginatedList<CategoryDto>>
    {
        public bool? IsActive { get; set; }
    }
}
