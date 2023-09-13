using AutoMapper;
using CDN.Application.Contracts.Persistence;
using CDN.Application.Features.CategoryFeature.Queries.GetCategory;
using MediatR;
using SharedKernel.Common;
using System.Threading;
using System.Threading.Tasks;

namespace CDN.Application.Features.CategoryFeature.Queries.GetCategories
{
    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, PaginatedList<CategoryDto>>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public GetCategoriesQueryHandler(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedList<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _categoryRepository.GetAsync(request.IsActive,
                                                                request.PageNumber,
                                                                request.PageSize,
                                                                cancellationToken);
            return _mapper.Map<PaginatedList<CategoryDto>>(categories);
        }
    }
}
