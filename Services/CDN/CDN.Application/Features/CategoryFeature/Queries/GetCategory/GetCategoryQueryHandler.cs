using AutoMapper;
using CDN.Application.Contracts.Persistence;
using CDN.Domain.Entities;
using MediatR;
using SharedKernel.Exceptions;
using System.Threading;
using System.Threading.Tasks;

namespace CDN.Application.Features.CategoryFeature.Queries.GetCategory
{
    public class GetCategoryQueryHandler : IRequestHandler<GetCategoryQuery, CategoryDto>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public GetCategoryQueryHandler(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<CategoryDto> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetAsync(request.Id, cancellationToken);
            if (category is null)
                throw new NotFoundException(nameof(Category), request.Id);

            return _mapper.Map<CategoryDto>(category);
        }
    }
}
