using SSO.Application.Features.GroupFeature.Commands.CreateGroup;
using SSO.Application.Features.GroupFeature.Commands.DeleteGroup;
using SSO.Application.Features.GroupFeature.Commands.UpdateGroup;
using SSO.Application.Features.GroupFeature.Commands.UpdateGroupRole;
using SSO.Application.Features.GroupFeature.Commands.UpdateGroupUsers;
using SSO.Application.Features.GroupFeature.Queries.GetGroup;
using SSO.Application.Features.GroupFeature.Queries.GetGroupRoles;
using SSO.Application.Features.GroupFeature.Queries.GetGroups;
using SSO.Application.Features.GroupFeature.Queries.GetGroupUsers;
using SSO.Application.Features.UserFeature.Queries.GetUser;
using SSO.WebAPI.Controllers.V1;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SharedKernel.Common;
using Xunit;

namespace SSO.WebAPI.UnitTests.Controllers.V1
{
    public class GroupControllerTests
    {
        private readonly Mock<ISender> _mediatorMock;
        private readonly GroupController _groupController;

        public GroupControllerTests()
        {
            _mediatorMock = new Mock<ISender>();
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof(ISender))).Returns(_mediatorMock.Object);

            _groupController = new GroupController()
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
        public async Task Get_ReturnsPaginatedList_WhenCalledWithGetGroupsQuery()
        {
            // Arrange
            var query = new GetGroupsQuery();
            var expectedGroups = new List<GroupDto> { new GroupDto { Id = 1, Caption = "Group 1" }, new GroupDto { Id = 2, Caption = "Group 2" } };
            var expectedResult = new PaginatedList<GroupDto>()
            {
                Items = expectedGroups,
                Pagination = new PaginationModel
                {
                    CurrentPage = 1,
                    PageSize = 10,
                }
            };
            _mediatorMock.Setup(m => m.Send(query, default)).ReturnsAsync(expectedResult);

            // Act
            var result = await _groupController.Get(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().NotBeNull();
            okResult.Value.Should().BeAssignableTo<PaginatedList<GroupDto>>();
            var groups = okResult.Value as PaginatedList<GroupDto>;
            groups.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task Get_ReturnsGroupDto_WhenCalledWithGetGroupQuery()
        {
            // Arrange
            var groupId = 1;
            var query = new GetGroupQuery { Id = groupId };
            var expectedGroup = new GroupDto { Id = groupId, Caption = "Group 1" };
            _mediatorMock.Setup(m => m.Send(query, default)).ReturnsAsync(expectedGroup);

            // Act
            var result = await _groupController.Get(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().NotBeNull();
            okResult.Value.Should().BeAssignableTo<GroupDto>();
            var group = okResult.Value as GroupDto;
            group.Should().BeEquivalentTo(expectedGroup);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenCalledWithCreateGroupCommand()
        {
            // Arrange
            var command = new CreateGroupCommand { Caption = "Group 1" };
            var expectedGroup = new GroupDto { Id = 1, Caption = "Group 1" };
            _mediatorMock.Setup(m => m.Send(command, default)).ReturnsAsync(expectedGroup);

            // Act
            var result = await _groupController.Create(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdAtActionResult = result as CreatedAtActionResult;
            createdAtActionResult.ActionName.Should().Be(nameof(_groupController.Get));
            createdAtActionResult.RouteValues.Should().NotBeNull();
            createdAtActionResult.RouteValues.Should().ContainKey("groupId");
            createdAtActionResult.RouteValues["groupId"].Should().Be(expectedGroup.Id);
            createdAtActionResult.Value.Should().NotBeNull();
            createdAtActionResult.Value.Should().BeAssignableTo<GroupDto>();
            var group = createdAtActionResult.Value as GroupDto;
            group.Should().BeEquivalentTo(expectedGroup);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenIdDoesNotMatch()
        {
            // Arrange
            var id = 1;
            var command = new UpdateGroupCommand { Id = 2 };

            // Act
            var result = await _groupController.Update(id, command, CancellationToken.None);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
            _mediatorMock.Verify(m => m.Send(command, default), Times.Never);
        }

        [Fact]
        public async Task Update_CallsMediator_WhenIdMatches()
        {
            // Arrange
            var id = 1;
            var command = new UpdateGroupCommand { Id = id };

            // Act
            var result = await _groupController.Update(id, command, CancellationToken.None);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mediatorMock.Verify(m => m.Send(command, default), Times.Once);
        }

        [Fact]
        public async Task UpdateGroupRole_CallsMediator()
        {
            // Arrange
            var command = new UpdateGroupRoleCommand();

            // Act
            var result = await _groupController.UpdateGroupRole(command, CancellationToken.None);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mediatorMock.Verify(m => m.Send(command, default), Times.Once);
        }

        [Fact]
        public async Task Delete_CallsMediator()
        {
            // Arrange
            var command = new DeleteGroupCommand();

            // Act
            var result = await _groupController.Delete(command, CancellationToken.None);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mediatorMock.Verify(m => m.Send(command, default), Times.Once);
        }

        [Fact]
        public async Task GetGroupRoles_ReturnsOkResult()
        {
            // Arrange
            var query = new GetGroupRolesQuery();

            _mediatorMock
                .Setup(m => m.Send(query, default))
                .ReturnsAsync(new GroupRolesDto());

            // Act
            var result = await _groupController.GetGroupRoles(query, CancellationToken.None);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetGroupUsers_ReturnsOkResult()
        {
            // Arrange
            var query = new GetGroupUsersQuery();

            _mediatorMock
                .Setup(m => m.Send(query, default))
                .ReturnsAsync(new PaginatedList<UserDto>());

            // Act
            var result = await _groupController.GetGroupUsers(query, CancellationToken.None);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task UpdateGroupUsers_ReturnsBadRequest_WhenIdDoesNotMatch()
        {
            // Arrange
            var id = 1;
            var command = new UpdateGroupUsersCommand { GroupId = 2 };

            // Act
            var result = await _groupController.UpdateGroupUsers(id, command, CancellationToken.None);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
            _mediatorMock.Verify(m => m.Send(command, default), Times.Never);
        }
    }
}
