using Moq;
using SharedKernel.Contracts.Infrastructure;
using SharedKernel.Exceptions;
using SSO.Application.Contracts.Persistence;
using SSO.Application.Features.AuthenticationFeature.Commands.ChangeUsersPassword;
using SSO.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SSO.Application.UnitTests.Features.AuthenticationFeature.Commands
{
    public class ChangeUsersPasswordCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IEncryptionService> _encryptionServiceMock;
        private readonly ChangeUsersPasswordCommandHandler _handler;

        public ChangeUsersPasswordCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _encryptionServiceMock = new Mock<IEncryptionService>();
            _handler = new ChangeUsersPasswordCommandHandler(_userRepositoryMock.Object, _encryptionServiceMock.Object);
        }

        [Fact]
        public async Task Handle_WhenUserExists_ShouldUpdatePasswordAndLogSuccess()
        {
            // Arrange
            var userId = 1;
            var adminUserId = 1;
            var user = new User { Id = userId, Password = "old_password" };
            var newEncryptedPassword = "new_encrypted_password";
            var command = new ChangeUsersPasswordCommand { UserId = userId, UserNewPassword = "new_password", AdminUserId = adminUserId };
            _userRepositoryMock.Setup(x => x.GetAsync(userId, CancellationToken.None)).ReturnsAsync(user);
            _encryptionServiceMock.Setup(x => x.HashPassword(command.UserNewPassword)).Returns(newEncryptedPassword);

            // Act
            await _handler.Handle(command, default);

            // Assert
            _userRepositoryMock.Verify(x => x.UpdateAsync(user, CancellationToken.None), Times.Once);
            _encryptionServiceMock.Verify(x => x.HashPassword(command.UserNewPassword), Times.Once);
            Assert.Equal(newEncryptedPassword, user.Password);
        }

        [Fact]
        public async Task Handle_WhenUserDoesNotExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var userId = 1;
            var adminUserId = 1;
            var command = new ChangeUsersPasswordCommand { UserId = userId, UserNewPassword = "new_password", AdminUserId = adminUserId };
            _userRepositoryMock.Setup(x => x.GetAsync(userId, CancellationToken.None)).ReturnsAsync(null as User);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, default);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(act);
        }
    }

}
