using CDN.Application.Features.CategoryFeature.Queries.GetCategory;
using MediatR;

namespace CDN.Application.Features.CategoryFeature.Commands.CreateCategory
{
    public class CreateCategoryCommand : IRequest<CategoryDto>
    {
        public string Title { get; set; }
        public string Path { get; set; }
    }
}
