using AutoMapper;
using CDN.Application.Contracts.Infrastructure;
using CDN.Application.Contracts.Persistence;
using CDN.Application.Features.CategoryFeature.Commands.UpdateCategory;
using CDN.Application.UnitTests.Common;
using CDN.Domain.Entities;
using FluentAssertions;
using MediatR;
using Moq;
using SharedKernel.Exceptions;
using Xunit;

namespace CDN.Application.UnitTests.Features.CategoryFeature.Commands.UpdateCategory
{
    public class UpdateCategoryCommandHandlerTests
    {
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
        private readonly Mock<ICategoryCacheService> _categoryCacheServiceMock;
        private readonly IMapper _mapper;
        private readonly UpdateCategoryCommandHandler _handler;

        public UpdateCategoryCommandHandlerTests()
        {
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _categoryCacheServiceMock = new Mock<ICategoryCacheService>();
            _mapper = _mapper.GetMapper();
            _handler = new UpdateCategoryCommandHandler(
                _categoryRepositoryMock.Object,
                _categoryCacheServiceMock.Object,
                _mapper);
        }

        [Fact]
        public async Task Handle_WhenCategoryToUpdateIsNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var request = new UpdateCategoryCommand { Id = 1 };
            _categoryRepositoryMock.Setup(x => x.GetAsync(request.Id, CancellationToken.None)).ReturnsAsync((Category)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_WhenCategoryToUpdateExists_ShouldUpdateCategoryAndRemoveFromCache()
        {
            // Arrange
            var request = new UpdateCategoryCommand { Id = 1, Title = "New Category Name" };
            var categoryToUpdate = new Category { Id = 1, Title = "Old Category Name" };
            var updatedCategory = new Category { Id = 1, Title = "New Category Name" };
            _categoryRepositoryMock.Setup(x => x.GetAsync(request.Id, CancellationToken.None)).ReturnsAsync(categoryToUpdate);
            _categoryRepositoryMock.Setup(x => x.UpdateAsync(updatedCategory, CancellationToken.None));

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().Be(Unit.Value);
            _categoryRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Category>(c => c.Title == updatedCategory.Title && c.Id == updatedCategory.Id), CancellationToken.None), Times.Once);
            _categoryCacheServiceMock.Verify(x => x.Remove(categoryToUpdate.Id), Times.Once);
        }
    }

}
