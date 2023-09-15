using AutoMapper;
using CDN.Application.Contracts.Persistence;
using CDN.Application.Features.CategoryFeature.Queries.GetCategories;
using CDN.Application.Features.CategoryFeature.Queries.GetCategory;
using CDN.Application.UnitTests.Common;
using CDN.Domain.Entities;
using FluentAssertions;
using Moq;
using SharedKernel.Common;
using Xunit;

namespace CDN.Application.UnitTests.Features.CategoryFeature.Queries.GetCategory
{
    public class GetCategoriesQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetCategoriesQueryHandlerTests()
        {
            _mapper = _mapper.GetMapper();
        }


        [Fact]
        public async Task Handle_WithValidRequest_ReturnsPaginatedListOfCategoryDto()
        {
            // Arrange
            var dt = DateTime.Now;
            var categories = new List<Category>
        {
            new Category { Id = 1, Title = "Category 1", IsActive = true, CreatedDate = dt.AddDays(-1) },
            new Category { Id = 2, Title = "Category 2", IsActive = true, CreatedDate = dt.AddDays(-2) },
            new Category { Id = 3, Title = "Category 3", IsActive = true, CreatedDate = dt.AddDays(-3) },
            new Category { Id = 4, Title = "Category 4", IsActive = true, CreatedDate = dt.AddDays(-4) },
            new Category { Id = 5, Title = "Category 5", IsActive = true, CreatedDate = dt.AddDays(-5) }
        };
            var mockRepo = new Mock<ICategoryRepository>();
            mockRepo.Setup(x => x.GetAsync(true, 1, 3, CancellationToken.None)).ReturnsAsync(new PaginatedResult<Category>(categories.Skip(2).ToList(), 5, 1, 3));
            var query = new GetCategoriesQuery { IsActive = true, PageNumber = 1, PageSize = 3 };
            var handler = new GetCategoriesQueryHandler(mockRepo.Object, _mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().NotBeNullOrEmpty();
            result.Items.Should().HaveCount(3);
            result.Items.Should().BeEquivalentTo(new List<CategoryDto>
        {
            new CategoryDto { Id = 3, Title = "Category 3", IsActive = true, CreatedDate = dt.AddDays(-3) },
            new CategoryDto { Id = 4, Title = "Category 4", IsActive = true, CreatedDate = dt.AddDays(-4) },
            new CategoryDto { Id = 5, Title = "Category 5", IsActive = true, CreatedDate = dt.AddDays(-5) }
        }, options => options.Excluding(x => x.Path));
            result.Pagination.Should().NotBeNull();
            result.Pagination.CurrentPage.Should().Be(1);
            result.Pagination.TotalPages.Should().Be(2);
            result.Pagination.PageSize.Should().Be(3);
            result.Pagination.TotalCount.Should().Be(5);
            result.Pagination.HasPrevious.Should().BeFalse();
            result.Pagination.HasNext.Should().BeTrue();
        }
    }

}
