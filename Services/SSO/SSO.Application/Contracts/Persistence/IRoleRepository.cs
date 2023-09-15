using SSO.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SSO.Application.Contracts.Persistence
{
    public interface IRoleRepository
    {
        Task<List<Role>> GetAsync(CancellationToken cancellationToken);
        Task<Role> GetAsync(int id, CancellationToken cancellationToken);
        Task<List<Role>> GetByRoleIdsAsync(List<int> ids, CancellationToken cancellationToken);
        Task<Role> GetWithGroupsAsync(int id, CancellationToken cancellationToken);
        Task<Role> GetWithUsersAsync(int id, CancellationToken cancellationToken);
        Task<bool> IsUniqueDisplayTitleAsync(int id, string displayTitle, CancellationToken cancellationToken);
        Task UpdateAsync(Role role, CancellationToken cancellationToken);
    }
}
