using AutoMapper;
using CDN.Application.Contracts.Persistence;
using CDN.Application.Features.CategoryFeature.Commands.CreateCategory;
using CDN.Application.Features.CategoryFeature.Queries.GetCategory;
using CDN.Application.UnitTests.Common;
using CDN.Domain.Entities;
using FluentAssertions;
using Moq;
using SharedKernel.Contracts.Infrastructure;
using Xunit;

namespace CDN.Application.UnitTests.Features.CategoryFeature.Commands.CreateCategory
{
    public class CreateCategoryCommandHandlerTests
    {
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
        private readonly IMapper _mapper;
        private readonly Mock<IDateTimeService> _dateTimeServiceMock;
        private readonly CreateCategoryCommandHandler _handler;

        public CreateCategoryCommandHandlerTests()
        {
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _mapper = _mapper.GetMapper();

            _dateTimeServiceMock = new Mock<IDateTimeService>();

            _handler = new CreateCategoryCommandHandler(
                _categoryRepositoryMock.Object,
                _mapper,
                _dateTimeServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsCategoryDto()
        {
            // Arrange
            var request = new CreateCategoryCommand
            {
                Title = "Title",
            };
            var expectedCreateDate = DateTime.UtcNow;
            var expectedCategory = new Category()
            {
                Id = 1,
                IsActive = true,
                Title = "Title",
                CreatedDate = expectedCreateDate
            };
            var expectedCategoryDto = new CategoryDto()
            {
                Id = 1,
                IsActive = true,
                Title = "Title",
                CreatedDate = expectedCreateDate
            };

            _categoryRepositoryMock
                .Setup(x => x.CreateAsync(It.IsAny<Category>(), CancellationToken.None))
                .ReturnsAsync(expectedCategory);

            _dateTimeServiceMock
                .Setup(x => x.Now)
                .Returns(expectedCreateDate);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeOfType<CategoryDto>();
            result.Should().BeEquivalentTo(expectedCategoryDto);

            _categoryRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Category>(), CancellationToken.None), Times.Once);
            _dateTimeServiceMock.Verify(x => x.Now, Times.Once);
        }
    }
}
