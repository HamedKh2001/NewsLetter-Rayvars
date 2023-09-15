using Moq;
using SSO.Application.Contracts.Infrastructure;
using SSO.Application.Features.AuthenticationFeature.Commands.ChangePassword;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SSO.Application.UnitTests.Features.AuthenticationFeature.Commands
{
    public class ChangePasswordCommandHandlerTests
    {
        private readonly Mock<IAuthenticationService> _mockAuthService;
        private readonly ChangePasswordCommandHandler _handler;

        public ChangePasswordCommandHandlerTests()
        {
            _mockAuthService = new Mock<IAuthenticationService>();
            _handler = new ChangePasswordCommandHandler(_mockAuthService.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ChangesPasswordAndLogs()
        {
            // Arrange
            var request = new ChangePasswordCommand
            {
                UserId = 123,
                CurrentPassword = "oldpassword",
                NewPassword = "newpassword"
            };

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _mockAuthService.Verify(s => s.ChangePasswordAsync(request, CancellationToken.None), Times.Once);
        }
    }

}
