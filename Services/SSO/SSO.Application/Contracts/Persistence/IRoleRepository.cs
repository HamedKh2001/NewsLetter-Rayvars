using SSO.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SSO.Application.Contracts.Persistence
{
    public interface IRoleRepository
    {
        Task<List<Role>> GetAsync(CancellationToken cancellationToken);
        Task<Role> GetAsync(long id, CancellationToken cancellationToken);
        Task<List<Role>> GetByRoleIdsAsync(List<long> ids, CancellationToken cancellationToken);
        Task<Role> GetWithGroupsAsync(long id, CancellationToken cancellationToken);
        Task<Role> GetWithUsersAsync(long id, CancellationToken cancellationToken);
        Task<bool> IsUniqueDisplayTitleAsync(long id, string displayTitle, CancellationToken cancellationToken);
        Task UpdateAsync(Role role, CancellationToken cancellationToken);
    }
}
