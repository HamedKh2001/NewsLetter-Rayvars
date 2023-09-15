using Moq;
using SharedKernel.Exceptions;
using SSO.Application.Contracts.Persistence;
using SSO.Application.Features.GroupFeature.Commands.UpdateGroupRole;
using SSO.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SSO.Application.UnitTests.Features.GroupFeature.Commands
{
    public class UpdateGroupRoleCommandHandlerTests
    {
        private readonly Mock<IGroupRepository> _groupRepositoryMock = new Mock<IGroupRepository>();
        private readonly Mock<IRoleRepository> _roleRepositoryMock = new Mock<IRoleRepository>();
        private readonly UpdateGroupRoleCommandHandler _handler;

        public UpdateGroupRoleCommandHandlerTests()
        {
            _handler = new UpdateGroupRoleCommandHandler(_groupRepositoryMock.Object, _roleRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_GroupNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var request = new UpdateGroupRoleCommand { GroupId = 1, RoleIds = new List<int> { 2, 3 } };
            _groupRepositoryMock.Setup(repo => repo.GetWithRolesAsync(request.GroupId, CancellationToken.None)).ReturnsAsync((Group)null);

            // Act and Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_InvalidRoleIds_ThrowsBadRequestException()
        {
            // Arrange
            var request = new UpdateGroupRoleCommand { GroupId = 1, RoleIds = new List<int> { 2, 3 } };
            var group = new Group { Id = 1 };
            _groupRepositoryMock.Setup(repo => repo.GetWithRolesAsync(request.GroupId, CancellationToken.None)).ReturnsAsync(group);
            _roleRepositoryMock.Setup(repo => repo.GetByRoleIdsAsync(request.RoleIds, CancellationToken.None)).ReturnsAsync(new List<Role> { new Role { Id = 2 } });

            // Act and Assert
            await Assert.ThrowsAsync<BadRequestException>(() => _handler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_AddAndDeleteRoles_UpdateGroupAndLogsEvent()
        {
            // Arrange
            var request = new UpdateGroupRoleCommand { GroupId = 1, RoleIds = new List<int> { 2, 3 } };
            var group = new Group { Id = 1, Roles = new List<Role> { new Role { Id = 1 }, new Role { Id = 2 } } };
            var addRoles = new List<Role> { new Role { Id = 3 } };
            _groupRepositoryMock.Setup(repo => repo.GetWithRolesAsync(request.GroupId, CancellationToken.None)).ReturnsAsync(group);
            _groupRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Group>(), CancellationToken.None));
            _roleRepositoryMock.Setup(repo => repo.GetByRoleIdsAsync(It.IsAny<List<int>>(), CancellationToken.None)).ReturnsAsync(addRoles);

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.DoesNotContain(group.Roles, r => r.Id == 1);
            _groupRepositoryMock.Verify(repo => repo.UpdateAsync(group, CancellationToken.None), Times.Once);
        }
    }

}
