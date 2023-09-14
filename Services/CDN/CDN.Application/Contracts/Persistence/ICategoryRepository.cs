using CDN.Domain.Entities;
using SharedKernel.Common;

namespace CDN.Application.Contracts.Persistence
{
    public interface ICategoryRepository
    {
        Task<Category> GetAsync(int id, CancellationToken cancellationToken = default);
        Category Get(int id);
        Task<PaginatedResult<Category>> GetAsync(bool? isActive, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task<Category> CreateAsync(Category category, CancellationToken cancellationToken = default);
        Task UpdateAsync(Category category, CancellationToken cancellationToken = default);
        Task<bool> IsUniqueTitleAsync(string title, CancellationToken cancellationToken = default);
        Task<bool> IsUniqueTitleAsync(string title, int id, CancellationToken cancellationToken = default);
    }
}
