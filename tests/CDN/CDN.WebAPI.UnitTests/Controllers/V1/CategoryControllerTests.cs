using CDN.API.Controllers.V1;
using CDN.Application.Features.CategoryFeature.Commands.CreateCategory;
using CDN.Application.Features.CategoryFeature.Commands.UpdateCategory;
using CDN.Application.Features.CategoryFeature.Queries.GetCategories;
using CDN.Application.Features.CategoryFeature.Queries.GetCategory;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using SharedKernel.Common;
using Xunit;

namespace CDN.WebAPI.UnitTests.Controllers.V1
{
    public class CategoryControllerTests
    {
        private readonly Mock<ISender> _mediatorMock;
        private readonly CategoryController _categoryController;

        public CategoryControllerTests()
        {
            _mediatorMock = new Mock<ISender>();
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof(ISender))).Returns(_mediatorMock.Object);

            _categoryController = new CategoryController()
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
        public async Task Get_ReturnsOkResult()
        {
            // Arrange
            var query = new GetCategoriesQuery();
            var expectedCategories = new List<CategoryDto>
        {
            new CategoryDto { Id = 1, Title = "Category 1" },
            new CategoryDto { Id = 2, Title = "Category 2" }
        };
            _mediatorMock.Setup(m => m.Send(It.Is<GetCategoriesQuery>(x => x.IsActive == query.IsActive && x.PageSize == query.PageSize && x.PageNumber == query.PageNumber), CancellationToken.None))
                .ReturnsAsync(new PaginatedList<CategoryDto>()
                {
                    Items = expectedCategories,
                    Pagination = new PaginationModel { PageSize = query.PageSize }
                });

            // Act
            var result = await _categoryController.Get(query, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualCategories = Assert.IsAssignableFrom<PaginatedList<CategoryDto>>(okResult.Value);
            Assert.Equal(expectedCategories, actualCategories.Items);
        }

        [Fact]
        public async Task Get_ReturnsOkResultWithCategoryDto()
        {
            // Arrange
            var query = new GetCategoryQuery()
            {
                Id = 1
            };
            var expectedCategory = new CategoryDto { Id = 1, Title = "Category 1" };
            _mediatorMock.Setup(m => m.Send(query, CancellationToken.None)).ReturnsAsync(expectedCategory);

            // Act
            var result = await _categoryController.Get(query, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualCategory = Assert.IsType<CategoryDto>(okResult.Value);
            Assert.Equal(expectedCategory, actualCategory);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var command = new CreateCategoryCommand { Title = "New Category" };
            var expectedCategory = new CategoryDto { Id = 1, Title = "New Category" };
            _mediatorMock.Setup(m => m.Send(command, CancellationToken.None)).ReturnsAsync(expectedCategory);

            // Act
            var result = await _categoryController.Create(command, CancellationToken.None);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("Get", createdAtActionResult.ActionName);
            var actualRouteValues = Assert.IsType<RouteValueDictionary>(createdAtActionResult.RouteValues);
            Assert.Equal(expectedCategory.Id, actualRouteValues["employeeId"]);
            var actualCategory = Assert.IsType<CategoryDto>(createdAtActionResult.Value);
            Assert.Equal(expectedCategory, actualCategory);
        }

        [Fact]
        public async Task Update_ReturnsNoContentResult()
        {
            // Arrange
            var command = new UpdateCategoryCommand { Id = 1, Title = "Updated Category" };

            // Act
            var result = await _categoryController.Update(1, command, CancellationToken.None);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }
    }

}
