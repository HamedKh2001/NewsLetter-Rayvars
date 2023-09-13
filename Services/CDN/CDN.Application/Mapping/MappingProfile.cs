using AutoMapper;
using CDN.Application.Features.CategoryFeature.Commands.CreateCategory;
using CDN.Application.Features.CategoryFeature.Commands.UpdateCategory;
using CDN.Application.Features.CategoryFeature.Queries.GetCategory;
using CDN.Domain.Entities;
using SharedKernel.Common;

namespace CDN.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap(typeof(PaginatedResult<>), typeof(PaginatedList<>));

            CreateMap<Category, CategoryDto>();
            CreateMap<CreateCategoryCommand, Category>();
            CreateMap<UpdateCategoryCommand, Category>();
        }
    }
}
