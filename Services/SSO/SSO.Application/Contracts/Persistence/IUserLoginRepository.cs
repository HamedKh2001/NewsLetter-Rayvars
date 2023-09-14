using SharedKernel.Common;
using SSO.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SSO.Application.Contracts.Persistence
{
    public interface IUserLoginRepository
    {
        Task<UserLogin> CreateAsync(UserLogin userLogin, CancellationToken cancellationToken);
        Task<PaginatedResult<UserLogin>> GetAsync(int userId, int pageNumber, int pageSize, CancellationToken cancellationToken);
    }
}
