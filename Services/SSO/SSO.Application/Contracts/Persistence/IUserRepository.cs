using SharedKernel.Common;
using SSO.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SSO.Application.Contracts.Persistence
{
    public interface IUserRepository
    {
        Task<User> CreateAsync(User user, CancellationToken cancellationToken);
        Task DeleteAsync(User user, CancellationToken cancellationToken);
        Task<PaginatedResult<User>> GetAsync(string searchParam, int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<PaginatedResult<User>> GetAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<User> GetAsync(int id, CancellationToken cancellationToken);
        Task UpdateAsync(User user, CancellationToken cancellationToken);
        Task<User> GetUserWithRolesAsync(string userName, string encPass, CancellationToken cancellationToken);
        Task<User> GetUserWithRolesAsync(int id, CancellationToken cancellationToken);
        Task<User> GetUserByPasswordAsync(int userId, string encPass, CancellationToken cancellationToken);
        Task<User> GetWithRoleAndRefreshTokensAsync(int userId, CancellationToken cancellationToken);
        Task<List<User>> GetByUserIdsAsync(List<int> ids, CancellationToken cancellationToken);
        Task<User> GetUserWithGroupsAsync(int id, CancellationToken cancellationToken);
        Task<bool> IsUniqueUserNameAsync(string userName, CancellationToken cancellationToken);
        Task<User> GetByUserNameAsync(string userName, CancellationToken cancellationToken);
        Task<bool> IsUniqueMobileAsync(string mobile, CancellationToken cancellationToken);
        Task<bool> IsUniqueMobileAsync(string mobile, int id, CancellationToken cancellationToken);
        Task<PaginatedResult<User>> GetByGroupIdAsync(int groupId, string searchParam, int pageNumber, int pageSize, CancellationToken cancellationToken);
    }
}
