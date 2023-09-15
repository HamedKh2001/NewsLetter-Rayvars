using CDN.Application.Contracts.Infrastructure;
using CDN.Application.Contracts.Persistence;
using CDN.Application.Features.FileFeature.Commands.UploadFile;
using CDN.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using SharedKernel.Contracts.Infrastructure;
using Xunit;

namespace CDN.Application.UnitTests.Features.FileFeature.Commands.UploadFile
{
    public class UploadFileCommandHandlerTests
    {
        private readonly Mock<INewsLetterRepository> _newsLetterRepository;
        private readonly Mock<ICategoryCacheService> _categoryCacheServiceMock;
        private readonly Mock<IDateTimeService> _dateTimeServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IFormFile> _formFileMock;
        private readonly Mock<ILogger<UploadNewsLetterCommandHandler>> _loggerMock;
        private readonly UploadNewsLetterCommandHandler _handler;

        public UploadFileCommandHandlerTests()
        {
            _newsLetterRepository = new Mock<INewsLetterRepository>();
            _categoryCacheServiceMock = new Mock<ICategoryCacheService>();
            _dateTimeServiceMock = new Mock<IDateTimeService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<UploadNewsLetterCommandHandler>>();
            _formFileMock = new Mock<IFormFile>();

            _handler = new UploadNewsLetterCommandHandler(
                _newsLetterRepository.Object,
                _categoryCacheServiceMock.Object,
                _dateTimeServiceMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Handle_WithValidRequest_CreatesFileEntityAndReturnsDownloadUrl()
        {
            // Arrange
            var secretKey = Guid.NewGuid();
            var request = new UploadNewsLettereCommand(
                1,
                _formFileMock.Object,
                //new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("test")), 0, 10, "data", "test.txt"),
                "tag1-tag2",
                "test");

            var category = new Category
            {
                Id = 1,
                Path = "C:\\",
                Title = "test",
            };

            var file = new NewsLetter
            {
                Id = 1,
                CategoryId = category.Id,
                FileName = $"{request.FileName}{Path.GetExtension(request.FormFile.FileName)}",
                FileSize = request.FormFile.Length,
                FileType = "txt",
                TagName = request.TagName,
                CreatedDate = DateTime.Now,
                IsDeleted = false,
            };

            _categoryCacheServiceMock.Setup(x => x.Get(request.CategoryId)).Returns(category);
            _newsLetterRepository.Setup(x => x.CreateAsync(It.IsAny<NewsLetter>(), CancellationToken.None)).ReturnsAsync(file);
            _formFileMock.Setup(x => x.ContentType);
            _formFileMock.Setup(_ => _.FileName).Returns("test.jpg");
            _formFileMock.Setup(_ => _.Length).Returns(10);

            _newsLetterRepository.Setup(x => x.CreateAsync(It.IsAny<NewsLetter>(), CancellationToken.None)).ReturnsAsync(file);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull().And.BeOfType<string>();
            //result.Should().Be($"File/Download/{file.Id}/{file.Key}");

            _categoryCacheServiceMock.Verify(x => x.Get(request.CategoryId), Times.Once);

            _newsLetterRepository.Verify(x => x.CreateAsync(It.IsAny<NewsLetter>(), CancellationToken.None), Times.Once);

            _unitOfWorkMock.Verify(x => x.BeginTransaction(), Times.Once);
            _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);
            _unitOfWorkMock.Verify(x => x.Rollback(), Times.Never);
        }
    }

}
