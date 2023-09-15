using CDN.Application.Contracts.Persistence;
using CDN.Domain.Entities;
using CDN.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using SharedKernel.Contracts.Infrastructure;
using System.Reflection;
using Xunit;

namespace CDN.Infrastucture.UnitTests.Services
{
    public class CategoryCacheServiceTests
    {
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly Mock<IDistributedCacheWrapper> _mockRedisCache;
        private readonly Mock<ILogger<CategoryCacheService>> _mockLogger;
        private readonly CategoryCacheService _categoryCacheService;
        private readonly string PREFIX;


        public CategoryCacheServiceTests()
        {

            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockRedisCache = new Mock<IDistributedCacheWrapper>();
            _mockLogger = new Mock<ILogger<CategoryCacheService>>();

            _categoryCacheService = new CategoryCacheService(
                _mockCategoryRepository.Object,
                _mockRedisCache.Object,
                _mockLogger.Object);

            PREFIX = typeof(CategoryCacheService)
                .GetField("PREFIX", BindingFlags.NonPublic | BindingFlags.Static)
                .GetValue(null) as string;
        }

        [Fact]
        public void Get_ReturnsCategoryFromCache()
        {
            // Arrange
            var categoryId = 1;
            var category = new Category { Id = categoryId, Title = "Test Category" };
            var json = JsonConvert.SerializeObject(category);
            _mockRedisCache.Setup(x => x.GetString($"{PREFIX}{categoryId}"))
                .Returns(json);

            // Act
            var result = _categoryCacheService.Get(categoryId);

            // Assert
            _mockRedisCache.Verify(x => x.GetString($"{PREFIX}{categoryId}"), Times.Once);
            _mockCategoryRepository.Verify(x => x.Get(It.IsAny<int>()), Times.Never);
            result.Should().BeEquivalentTo(category);
        }

        [Fact]
        public void Get_ReturnsCategoryFromDatabase()
        {
            // Arrange
            var categoryId = 1;
            var category = new Category { Id = categoryId, Title = "Test Category" };
            _mockCategoryRepository.Setup(x => x.Get(categoryId)).Returns(category);
            _mockRedisCache.Setup(x => x.GetString($"{PREFIX}{categoryId}"))
                .Returns<string>(null);

            // Act
            var result = _categoryCacheService.Get(categoryId);

            // Assert
            _mockRedisCache.Verify(x => x.GetString($"{PREFIX}{categoryId}"), Times.Once);
            _mockCategoryRepository.Verify(x => x.Get(categoryId), Times.Once);
            _mockRedisCache.Verify(x => x.SetString($"{PREFIX}{categoryId}", It.IsAny<string>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
            result.Should().BeEquivalentTo(category);
        }

        [Fact]
        public void Get_ReturnsNull_WhenCategoryNotFound()
        {
            // Arrange
            var categoryId = 1;
            _mockRedisCache.Setup(x => x.GetString($"{PREFIX}{categoryId}"))
                .Returns<string>(null);
            _mockCategoryRepository.Setup(x => x.Get(categoryId)).Returns<Category>(null);

            // Act
            var result = _categoryCacheService.Get(categoryId);

            // Assert
            _mockRedisCache.Verify(x => x.GetString($"{PREFIX}{categoryId}"), Times.Once);
            _mockCategoryRepository.Verify(x => x.Get(categoryId), Times.Once);
            _mockRedisCache.Verify(x => x.SetString($"{PREFIX}{categoryId}", It.IsAny<string>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Never);
            result.Should().BeNull();
        }

        [Fact]
        public void Remove_RemovesCategoryFromCache()
        {
            // Arrange
            var categoryId = 1;

            // Act
            _categoryCacheService.Remove(categoryId);

            // Assert
            _mockRedisCache.Verify(x => x.Remove($"{PREFIX}{categoryId}"), Times.Once);
        }
    }

}
