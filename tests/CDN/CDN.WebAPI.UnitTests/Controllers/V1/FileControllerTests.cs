using CDN.API.Controllers.V1;
using CDN.Application.Features.FileFeature.Commands.UploadFile;
using CDN.Application.Features.FileFeature.Queries.DownloadFile;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CDN.WebAPI.UnitTests.Controllers.V1
{
    public class NewsLetterControllerTests
    {
        private readonly NewsLetterController _controller;
        private readonly Mock<ISender> _mediatorMock;

        public NewsLetterControllerTests()
        {
            _mediatorMock = new Mock<ISender>();
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof(ISender))).Returns(_mediatorMock.Object);

            _controller = new NewsLetterController()
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        RequestServices = serviceProviderMock.Object,
                    }
                }
            };
        }

        [Fact]
        public async Task Upload_ReturnsOkResult_WhenCommandIsValid()
        {
            // Arrange
            var categoryId = 1;
            var formFile = new Mock<IFormFile>().Object;
            var tagName = "tag";
            var newsLetterName = "newsLetter.txt";
            var url = "http://example.com";
            var command = new UploadNewsLettereCommand(categoryId, formFile, tagName, newsLetterName);
            _mediatorMock.Setup(m => m.Send(It.Is<UploadNewsLettereCommand>(x => x.CategoryId == command.CategoryId), default)).ReturnsAsync(url);

            // Act
            var result = await _controller.Upload(categoryId, formFile, tagName, newsLetterName, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(url, okResult.Value);
        }

        [Fact]
        public async Task Download_ReturnsNewsLetter_WhenQueryIsValid()
        {
            // Arrange
            var id = 1;
            var key = Guid.NewGuid();
            var newsLetterName = "newsLetter.txt";
            var contentType = "text/plain";
            var newsLetterBytes = new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F };
            var query = new DownloadNewsLetterQuery(id);
            var newsLetter = new DownloadNewsLetterDto()
            {
                ContentType = contentType,
                Memory = new MemoryStream(newsLetterBytes),
                FileName = newsLetterName,
            };
            _mediatorMock.Setup(m => m.Send(It.Is<DownloadNewsLetterQuery>(q => q.Id == query.Id), default)).ReturnsAsync(newsLetter);

            // Act
            var result = await _controller.Download(CancellationToken.None, id);

            // Assert
            result.Should().BeOfType<FileStreamResult>();
            var newsLetterResult = Assert.IsType<FileStreamResult>(result);
            Assert.Equal(newsLetterBytes, GetByteArr(newsLetterResult.FileStream));
            Assert.Equal(contentType, newsLetterResult.ContentType);
            Assert.Equal(newsLetterName, newsLetterResult.FileDownloadName);
        }


        private byte[] GetByteArr(Stream stream)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }

}
