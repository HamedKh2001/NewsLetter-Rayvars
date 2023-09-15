using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SharedKernel.Common;
using SSO.Application.Features.UserFeature.Commands.CreateUser;
using SSO.Application.Features.UserFeature.Commands.DeleteUser;
using SSO.Application.Features.UserFeature.Commands.UpdateUser;
using SSO.Application.Features.UserFeature.Queries.GetUser;
using SSO.Application.Features.UserFeature.Queries.GetUsers;
using SSO.WebAPI.Controllers.V1;
using Xunit;

namespace SSO.WebAPI.UnitTests.Controllers.V1
{
    public class UserControllerTests
    {
        private readonly Mock<ISender> _mediatorMock;
        private readonly UserController _userController;

        public UserControllerTests()
        {
            _mediatorMock = new Mock<ISender>();
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof(ISender))).Returns(_mediatorMock.Object);

            _userController = new UserController()
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
            var query = new GetUsersQuery();
            var expectedUsers = new List<UserDto> { new UserDto { Id = 1, UserName = "testuser" } };
            _mediatorMock.Setup(m => m.Send(query, default)).ReturnsAsync(new PaginatedList<UserDto>()
            {
                Items = expectedUsers,
                Pagination = new PaginationModel
                {
                    PageSize = query.PageSize,
                }
            });

            // Act
            var result = await _userController.Get(query, CancellationToken.None);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result.As<OkObjectResult>();
            okResult.Value.Should().BeOfType<PaginatedList<UserDto>>();
            var users = okResult.Value.As<PaginatedList<UserDto>>().Items;
            users.Should().BeEquivalentTo(expectedUsers);
        }

        [Fact]
        public async Task Get_ReturnsOkResult_ForSingleUser()
        {
            // Arrange
            var query = new GetUserQuery()
            {
                Id = 1
            };
            var expectedUser = new UserDto { Id = 1, UserName = "testuser" };
            _mediatorMock.Setup(m => m.Send(query, default)).ReturnsAsync(expectedUser);

            // Act
            var result = await _userController.Get(query, CancellationToken.None);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result.As<OkObjectResult>();
            okResult.Value.Should().BeOfType<UserDto>();
            var user = okResult.Value.As<UserDto>();
            user.Should().BeEquivalentTo(expectedUser);
        }


        [Fact]
        public async Task Create_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var command = new CreateUserCommand { UserName = "testuser", Password = "testpass" };
            var expectedUser = new UserDto { Id = 1, UserName = "testuser" };
            _mediatorMock.Setup(m => m.Send(command, default)).ReturnsAsync(expectedUser);

            // Act
            var result = await _userController.Create(command, CancellationToken.None);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result.As<CreatedAtActionResult>();
            createdResult.ActionName.Should().Be(nameof(UserController.Get));
            createdResult.RouteValues.Should().ContainKey("userId").And.ContainValue(expectedUser.Id);
            createdResult.Value.Should().BeOfType<UserDto>();
            var user = createdResult.Value.As<UserDto>();
            user.Should().BeEquivalentTo(expectedUser);
        }

        [Fact]
        public async Task Update_Should_Return_NoContent_When_Successful()
        {
            // Arrange
            var command = new UpdateUserCommand { Id = 1, FirstName = "John", LastName = "Doe" };

            // Act
            var result = await _userController.Update(1, command, CancellationToken.None) as NoContentResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status204NoContent);
        }

        [Fact]
        public async Task Update_Should_Return_BadRequest_When_Id_Does_Not_Match_Command_Id()
        {
            // Arrange
            var command = new UpdateUserCommand { Id = 1, FirstName = "John", LastName = "Doe" };

            // Act
            var result = await _userController.Update(2, command, CancellationToken.None) as BadRequestResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task Delete_Should_Return_NoContent_When_Successful()
        {
            // Arrange
            var command = new DeleteUserCommand { Id = 1 };

            // Act
            var result = await _userController.Delete(command, CancellationToken.None) as NoContentResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status204NoContent);
        }
    }
}
