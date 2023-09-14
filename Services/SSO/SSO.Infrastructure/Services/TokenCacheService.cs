using Microsoft.Extensions.Logging;
using SharedKernel.Contracts.Infrastructure;
using SSO.Application.Contracts.Infrastructure;
using System;
using System.Threading.Tasks;

namespace SSO.Infrastructure.Services
{
    public class TokenCacheService : ITokenCacheService
    {
        private readonly IDistributedCacheWrapper _tokenCache;
        private readonly ILogger<TokenCacheService> _logger;

        public TokenCacheService(IDistributedCacheWrapper tokenCache, ILogger<TokenCacheService> logger)
        {
            _tokenCache = tokenCache;
            _logger = logger;
        }

        public async Task AddOrUpdateAsync(long userId, string token)
        {
            try
            {
                await _tokenCache.SetStringAsync(userId.ToString(), token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error on set User's token in tokenDb. userId: {userId}");
            }
        }

        public async Task RemoveAsync(long userId)
        {
            try
            {
                await _tokenCache.RemoveAsync(userId.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error on removing User's token from tokenDb. userId: {userId}");
            }
        }
    }
}
