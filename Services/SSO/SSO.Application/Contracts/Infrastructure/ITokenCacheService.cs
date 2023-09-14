using System.Threading.Tasks;

namespace SSO.Application.Contracts.Infrastructure
{
    public interface ITokenCacheService
    {
        Task AddOrUpdateAsync(long userId, string token);
        Task RemoveAsync(long userId);
    }
}
