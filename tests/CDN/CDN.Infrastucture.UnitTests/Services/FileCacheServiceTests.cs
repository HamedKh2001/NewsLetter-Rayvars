using CDN.Application.Contracts.Persistence;
using CDN.Domain.Entities;
using CDN.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using SharedKernel.Contracts.Infrastructure;
using Xunit;

namespace CDN.Infrastucture.UnitTests.Services
{
    public class NewsLetterCacheServiceTests
    {
        private readonly Mock<INewsLetterRepository> _mockNewsLetterRepository;
        private readonly Mock<IDistributedCacheWrapper> _mockRedisCache;
        private readonly Mock<ILogger<CategoryCacheService>> _mockLogger;
        private readonly NewsLetterCacheService _newsLetterCacheService;

        public NewsLetterCacheServiceTests()
        {
            _mockNewsLetterRepository = new Mock<INewsLetterRepository>();
            _mockRedisCache = new Mock<IDistributedCacheWrapper>();
            _mockLogger = new Mock<ILogger<CategoryCacheService>>();

            _newsLetterCacheService = new NewsLetterCacheService(_mockNewsLetterRepository.Object, _mockRedisCache.Object, _mockLogger.Object);
        }

        [Fact]
        public void Get_NewsLetterExistsInCache_ShouldReturnNewsLetterFromCache()
        {
            // Arrange
            var newsLetterId = 1;
            var newsLetter = new NewsLetter { Id = newsLetterId };
            var serializedNewsLetter = JsonConvert.SerializeObject(newsLetter, Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects
                    });

            _mockRedisCache.Setup(x => x.GetString($"NewsLetter_{newsLetterId}")).Returns(serializedNewsLetter);

            // Act
            var result = _newsLetterCacheService.Get(newsLetterId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(newsLetterId);
            _mockRedisCache.Verify(x => x.GetString($"NewsLetter_{newsLetterId}"), Times.Once);
            _mockNewsLetterRepository.Verify(x => x.GetWithCategory(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void Get_NewsLetterExistsInDatabase_ShouldReturnNewsLetterFromDatabaseAndAddToCache()
        {
            // Arrange
            var newsLetterId = 1;
            var newsLetter = new NewsLetter { Id = newsLetterId };
            _mockNewsLetterRepository.Setup(x => x.GetWithCategory(newsLetterId)).Returns(newsLetter);

            // Act
            var result = _newsLetterCacheService.Get(newsLetterId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(newsLetterId);
            _mockRedisCache.Verify(x => x.GetString($"NewsLetter_{newsLetterId}"), Times.Once);
            _mockNewsLetterRepository.Verify(x => x.GetWithCategory(newsLetterId), Times.Once);
            _mockRedisCache.Verify(x => x.SetString(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
        }

        [Fact]
        public void Get_NewsLetterDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var newsLetterId = 1;
            _mockNewsLetterRepository.Setup(x => x.GetWithCategory(newsLetterId)).Returns<NewsLetter>(null);

            // Act
            var result = _newsLetterCacheService.Get(newsLetterId);

            // Assert
            result.Should().BeNull();
            _mockRedisCache.Verify(x => x.GetString($"NewsLetter_{newsLetterId}"), Times.Once);
            _mockNewsLetterRepository.Verify(x => x.GetWithCategory(newsLetterId), Times.Once);
            _mockRedisCache.Verify(x => x.SetString(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Never);
        }

        [Fact]
        public void Remove_ShouldCallRemoveOnRedisCache()
        {
            // Arrange
            var newsLetterId = 1;

            // Act
            _newsLetterCacheService.Remove(newsLetterId);

            // Assert
            _mockRedisCache.Verify(x => x.Remove($"NewsLetter_{newsLetterId}"), Times.Once);
        }
    }
}
