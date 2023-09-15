using CDN.Application.Contracts.Infrastructure;
using CDN.Application.Features.FileFeature.Queries.DownloadFile;
using CDN.Domain.Entities;
using Moq;
using SharedKernel.Exceptions;
using Xunit;

namespace CDN.Application.UnitTests.Features.FileFeature.Queries.DownloadFile
{
    public class DownloadFileQueryHandlerTests
    {
        private readonly Mock<INewsLetterCacheService> _newsLetterCacheServiceMock;

        private readonly DownloadNewsLetterQueryHandler _handler;

        public DownloadFileQueryHandlerTests()
        {
            _newsLetterCacheServiceMock = new Mock<INewsLetterCacheService>();

            _handler = new DownloadNewsLetterQueryHandler(_newsLetterCacheServiceMock.Object);
        }

        [Fact]
        public async Task Handle_WhenFileNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var request = new DownloadNewsLetterQuery(1);
            _newsLetterCacheServiceMock.Setup(x => x.Get(request.Id)).Returns((NewsLetter)null);

            // Act
            var act = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(act);
        }

        [Fact]
        public async Task Handle_WhenFileInactive_ShouldReturnNotFound()
        {
            // Arrange
            var request = new DownloadNewsLetterQuery(1);
            var file = new NewsLetter { Id = request.Id, Category = new Category { IsActive = false } };
            _newsLetterCacheServiceMock.Setup(x => x.Get(request.Id)).Returns(file);

            // Act
            var act = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(act);
        }
    }
}