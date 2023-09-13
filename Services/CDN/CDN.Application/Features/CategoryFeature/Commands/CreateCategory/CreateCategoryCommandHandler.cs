using AutoMapper;
using CDN.Application.Contracts.Persistence;
using CDN.Application.Features.CategoryFeature.Queries.GetCategory;
using CDN.Domain.Entities;
using MediatR;
using SharedKernel.Contracts.Infrastructure;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CDN.Application.Features.CategoryFeature.Commands.CreateCategory
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly IDateTimeService _dateTimeService;

        public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IMapper mapper, IDateTimeService dateTimeService)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _dateTimeService = dateTimeService;
        }

        public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var newCategory = _mapper.Map<Category>(request);
            newCategory.IsActive = true;
            newCategory.CreatedDate = _dateTimeService.Now;

            var result = await _categoryRepository.CreateAsync(newCategory, cancellationToken);

            return _mapper.Map<CategoryDto>(result);
        }
    }
}
