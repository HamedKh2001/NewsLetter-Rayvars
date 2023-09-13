using CDN.Domain.Entities;
using SharedKernel.Common;
using System.Threading;
using System.Threading.Tasks;

namespace CDN.Application.Contracts.Persistence
{
    public interface ICategoryRepository
    {
        Task<Category> GetAsync(long id, CancellationToken cancellationToken = default);
        Category Get(long id);
        Task<PaginatedResult<Category>> GetAsync(bool? isActive, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task<Category> CreateAsync(Category category, CancellationToken cancellationToken = default);
        Task UpdateAsync(Category category, CancellationToken cancellationToken = default);
        Task<bool> IsUniqueTitleAsync(string title, CancellationToken cancellationToken = default);
        Task<bool> IsUniqueTitleAsync(string title, long id, CancellationToken cancellationToken = default);
    }
}
