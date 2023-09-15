using AutoMapper;
using MediatR;
using Moq;
using SharedKernel.Exceptions;
using SSO.Application.Contracts.Persistence;
using SSO.Application.Features.RoleFeature.Commands.UpdateRole;
using SSO.Application.UnitTests.Common;
using SSO.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SSO.Application.UnitTests.Features.RoleFeature.Commands
{
    public class UpdateRoleCommandHandlerTests
    {
        private readonly Mock<IRoleRepository> _mockRoleRepository;
        private readonly IMapper _mapper;
        private readonly UpdateRoleCommandHandler _handler;

        public UpdateRoleCommandHandlerTests()
        {
            _mockRoleRepository = new Mock<IRoleRepository>();
            _mapper = _mapper.GetMapper();
            _handler = new UpdateRoleCommandHandler(
                _mockRoleRepository.Object,
                _mapper);
        }

        [Fact]
        public async Task Handle_WhenRoleExists_ShouldUpdateRoleAndReturnUnit()
        {
            // Arrange
            var roleId = 1;
            var request = new UpdateRoleCommand { Id = roleId, DisplayTitle = "New Role Name" };
            var existingRole = new Role { Id = roleId, Title = "Old Role Name" };
            _mockRoleRepository.Setup(x => x.GetAsync(roleId, CancellationToken.None)).ReturnsAsync(existingRole);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            _mockRoleRepository.Verify(x => x.UpdateAsync(It.IsAny<Role>(), CancellationToken.None), Times.Once);
            Assert.Equal(Unit.Value, result);
        }

        [Fact]
        public async Task Handle_WhenRoleDoesNotExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var roleId = 1;
            var request = new UpdateRoleCommand { Id = roleId, DisplayTitle = "New Role Name" };
            _mockRoleRepository.Setup(x => x.GetAsync(roleId, CancellationToken.None)).ReturnsAsync((Role)null);

            // Act and Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(request, CancellationToken.None));
            _mockRoleRepository.Verify(x => x.UpdateAsync(It.IsAny<Role>(), CancellationToken.None), Times.Never);
        }
    }
}
