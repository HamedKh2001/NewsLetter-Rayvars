using CDN.Application.Contracts.Infrastructure;
using CDN.Application.Contracts.Persistence;
using CDN.Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SharedKernel.Contracts.Infrastructure;
using System;

namespace CDN.Infrastructure.Services
{
    public class CategoryCacheService : ICategoryCacheService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IDistributedCacheWrapper _redisCache;
        private readonly ILogger<CategoryCacheService> _logger;
        private const string PREFIX = "Category_";

        public CategoryCacheService(ICategoryRepository categoryRepository, IDistributedCacheWrapper redisCache, ILogger<CategoryCacheService> logger)
        {
            _categoryRepository = categoryRepository;
            _redisCache = redisCache;
            _logger = logger;
        }

        public Category Get(int categoryId)
        {
            var redisResult = GetCategoryRedis(categoryId);
            if (redisResult != null)
            {
                _logger.LogInformation($"Get categoryId From redis => {categoryId}");
                return redisResult;
            }

            var contextResult = GetCategoryContext(categoryId);
            if (contextResult != null)
            {
                _logger.LogInformation($"Get categoryId From Database => {categoryId}");

                Add(contextResult);
                return contextResult;
            }

            _logger.LogWarning($"There is no categoryId => {categoryId}");
            return null;
        }

        public void Remove(int categoryId)
        {
            _redisCache.Remove($"{PREFIX}{categoryId}");
        }

        #region Privates        

        private void Add(Category category)
        {
            var obj = JsonConvert.SerializeObject(category);
            _redisCache.SetString($"{PREFIX}{category.Id}", obj, new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(30)
            });
        }

        private Category GetCategoryRedis(int categoryId)
        {
            try
            {
                var category = _redisCache.GetString($"{PREFIX}{categoryId}");
                return JsonConvert.DeserializeObject<Category>(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetCategoryRedis => {categoryId}");
                return null;
            }
        }

        private Category GetCategoryContext(int categoryId)
        {
            try
            {
                var category = _categoryRepository.Get(categoryId);
                if (category != null)
                    return category;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetCategoryContext => {categoryId}");
            }

            return null;
        }

        #endregion
    }
}
