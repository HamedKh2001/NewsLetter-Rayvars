using AutoMapper;
using CDN.Application.Contracts.Infrastructure;
using CDN.Application.Contracts.Persistence;
using CDN.Domain.Entities;
using MediatR;
using SharedKernel.Exceptions;
using System.Threading;
using System.Threading.Tasks;

namespace CDN.Application.Features.CategoryFeature.Commands.UpdateCategory
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICategoryCacheService _categoryCacheService;
        private readonly IMapper _mapper;

        public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, ICategoryCacheService categoryCacheService, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _categoryCacheService = categoryCacheService;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var categoryToUpdate = await _categoryRepository.GetAsync(request.Id, cancellationToken);
            if (categoryToUpdate is null)
                throw new NotFoundException(nameof(Category), request.Id);

            _mapper.Map(request, categoryToUpdate);

            await _categoryRepository.UpdateAsync(categoryToUpdate, cancellationToken);
            _categoryCacheService.Remove(categoryToUpdate.Id);

            return Unit.Value;
        }
    }
}
