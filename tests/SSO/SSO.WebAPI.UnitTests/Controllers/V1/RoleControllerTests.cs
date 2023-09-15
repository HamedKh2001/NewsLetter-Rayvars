using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SSO.Application.Features.RoleFeature.Commands.UpdateRole;
using SSO.Application.Features.RoleFeature.Queries.GetRoleGroups;
using SSO.Application.Features.RoleFeature.Queries.GetRoles;
using SSO.Application.Features.RoleFeature.Queries.GetRoleUsers;
using SSO.WebAPI.Controllers.V1;
using Xunit;

namespace SSO.WebAPI.UnitTests._roleControllers.V1
{
    public class RoleControllerTests
    {
        private readonly Mock<ISender> _mediatorMock;
        private readonly RoleController _roleController;

        public RoleControllerTests()
        {
            _mediatorMock = new Mock<ISender>();
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof(ISender))).Returns(_mediatorMock.Object);

            _roleController = new RoleController()
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
        public async Task Get_Should_Return_All_Roles()
        {
            // Arrange
            var expectedRoles = new List<RoleDto>
            {
                new RoleDto { Id = 1, Title = "Role 1" },
                new RoleDto { Id = 2, Title = "Role 2" },
                new RoleDto { Id = 3, Title = "Role 3" }
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetRolesQuery>(), default))
                .ReturnsAsync(expectedRoles);

            // Act
            var result = await _roleController.Get(CancellationToken.None);

            // Assert
            result.Value.Should().BeEquivalentTo(expectedRoles);
        }

        [Fact]
        public async Task Update_Should_Return_NoContent_When_Update_Succeeds()
        {
            // Arrange
            var command = new UpdateRoleCommand { Id = 1, DisplayTitle = "Updated Role Name" };
            _mediatorMock.Setup(m => m.Send(command, default))
                .ReturnsAsync(Unit.Value)
                .Verifiable();

            // Act
            var result = await _roleController.Update(command.Id, command, CancellationToken.None);

            // Assert
            _mediatorMock.Verify();
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Update_Should_Return_BadRequest_When_Id_Does_Not_Match_Command_Id()
        {
            // Arrange
            var command = new UpdateRoleCommand { Id = 1, DisplayTitle = "Updated Role Name" };

            // Act
            var result = await _roleController.Update(2, command, CancellationToken.None);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task GetRoleGroups_Should_Return_All_Groups_For_Given_Role()
        {
            // Arrange
            var roleId = 1;
            var expectedGroups = new List<GetRoleGroupsDto>
            {
                new GetRoleGroupsDto { GroupId = 1, GroupCaption = "Group 1" },
                new GetRoleGroupsDto { GroupId = 2, GroupCaption = "Group 2" }
            };

            _mediatorMock.Setup(m => m.Send(It.Is<GetRoleGroupsQuery>(q => q.RoleId == roleId), default))
                .ReturnsAsync(expectedGroups);

            // Act
            var result = await _roleController.GetRoleGroups(new GetRoleGroupsQuery { RoleId = roleId }, CancellationToken.None);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().NotBeNull();
            okResult.Value.Should().BeAssignableTo<List<GetRoleGroupsDto>>();
            var group = okResult.Value as List<GetRoleGroupsDto>;
            group.Should().BeEquivalentTo(expectedGroups);
        }

        [Fact]
        public async Task GetRoleUsers_ReturnsOkObjectResult_WithRoleUsers()
        {
            // Arrange
            var roleId = 1;
            var users = new List<GetRoleUsersDto>
        {
            new GetRoleUsersDto { GroupId = 1, GroupCaption = "user1" },
            new GetRoleUsersDto { GroupId = 2, GroupCaption = "user2" }
        };
            _mediatorMock.Setup(m => m.Send(It.Is<GetRoleUsersQuery>(q => q.RoleId == roleId), default))
                .ReturnsAsync(users);

            // Act
            var result = await _roleController.GetRoleUsers(new GetRoleUsersQuery { RoleId = roleId }, CancellationToken.None);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().NotBeNull();
            okResult.Value.Should().BeAssignableTo<List<GetRoleUsersDto>>();
            var group = okResult.Value as List<GetRoleUsersDto>;
            group.Should().BeEquivalentTo(users);
        }
    }
}
