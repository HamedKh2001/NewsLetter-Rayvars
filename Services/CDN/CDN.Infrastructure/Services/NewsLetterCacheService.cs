using CDN.Application.Contracts.Infrastructure;
using CDN.Application.Contracts.Persistence;
using CDN.Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SharedKernel.Contracts.Infrastructure;

namespace CDN.Infrastructure.Services
{
    public class NewsLetterCacheService : INewsLetterCacheService
    {
        private readonly INewsLetterRepository _newsLetterRepository;
        private readonly IDistributedCacheWrapper _redisCache;
        private readonly ILogger<CategoryCacheService> _logger;
        private const string PREFIX = "NewsLetter_";

        public NewsLetterCacheService(INewsLetterRepository newsLetterRepository, IDistributedCacheWrapper redisCache, ILogger<CategoryCacheService> logger)
        {
            _newsLetterRepository = newsLetterRepository;
            _redisCache = redisCache;
            _logger = logger;
        }

        public NewsLetter Get(long newsLetterId)
        {
            var redisResult = GetNewsLetterRedis(newsLetterId);
            if (redisResult != null)
            {
                _logger.LogInformation($"Get newsLetterId From redis => {newsLetterId}");
                return redisResult;
            }

            var contextResult = GetNewsLetterContext(newsLetterId);
            if (contextResult != null)
            {
                _logger.LogInformation($"Get newsLetterId From Database => {newsLetterId}");

                Add(contextResult);
                return contextResult;
            }

            _logger.LogWarning($"There is no newsLetterId => {newsLetterId}");
            return null;
        }

        public void Remove(long newsLetterId)
        {
            _redisCache.Remove($"{PREFIX}{newsLetterId}");
        }

        #region Privates        

        private void Add(NewsLetter newsLetter)
        {
            var obj = JsonConvert.SerializeObject(newsLetter, Formatting.Indented,
                new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects
                });

            _redisCache.SetString($"{PREFIX}{newsLetter.Id}", obj, new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(5)
            });
        }

        private NewsLetter GetNewsLetterRedis(long newsLetterId)
        {
            try
            {
                var newsLetter = _redisCache.GetString($"{PREFIX}{newsLetterId}");
                return JsonConvert.DeserializeObject<NewsLetter>(newsLetter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetNewsLetterRedis => {newsLetterId}");
                return null;
            }
        }

        private NewsLetter GetNewsLetterContext(long newsLetterId)
        {
            try
            {
                var newsLetter = _newsLetterRepository.GetWithCategory(newsLetterId);
                if (newsLetter != null)
                    return newsLetter;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetNewsLetterContext => {newsLetterId}");
            }

            return null;
        }

        #endregion
    }
}
