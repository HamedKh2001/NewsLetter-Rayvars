using SharedKernel.Common;
using SSO.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SSO.Application.Contracts.Persistence
{
    public interface IGroupRepository
    {
        Task<List<Group>> GetAsync(CancellationToken cancellationToken);
        Task<Group> GetWithRolesAsync(int id, CancellationToken cancellationToken);
        Task<PaginatedResult<Group>> GetAsync(string caption, int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<Group> GetWithUsersAsync(int id, CancellationToken cancellationToken);
        Task<Group> GetAsync(int id, CancellationToken cancellationToken);
        Task<List<Group>> GetByIdsAsync(List<int> ids, CancellationToken cancellationToken);
        Task<Group> CreateAsync(Group group, CancellationToken cancellationToken);
        Task UpdateAsync(Group group, CancellationToken cancellationToken);
        Task DeleteAsync(Group group, CancellationToken cancellationToken);
        Task<bool> IsUniqueCaptionAsync(string caption, CancellationToken cancellationToken);
        Task<bool> IsUniqueCaptionAsync(int id, string caption, CancellationToken cancellationToken);
    }
}
